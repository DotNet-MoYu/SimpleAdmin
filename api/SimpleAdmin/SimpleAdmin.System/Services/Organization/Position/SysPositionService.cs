// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="ISysPositionService"/>
/// </summary>
public class SysPositionService : DbRepository<SysPosition>, ISysPositionService
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly ISysOrgService _sysOrgService;

    public SysPositionService(ISimpleCacheService simpleCacheService, ISysOrgService sysOrgService)
    {
        _simpleCacheService = simpleCacheService;
        _sysOrgService = sysOrgService;
    }

    /// <summary>
    /// 获取全部
    /// </summary>
    /// <returns></returns>
    public override async Task<List<SysPosition>> GetListAsync()
    {
        //先从Redis拿
        var sysPositions = _simpleCacheService.Get<List<SysPosition>>(SystemConst.CACHE_SYS_POSITION);
        if (sysPositions == null)
        {
            //redis没有就去数据库拿
            sysPositions = await base.GetListAsync();
            if (sysPositions.Count > 0)
            {
                //插入Redis
                _simpleCacheService.Set(SystemConst.CACHE_SYS_POSITION, sysPositions);
            }
        }
        return sysPositions;
    }

    /// <inheritdoc />
    public async Task<List<SysPosition>> GetPositionListByIdList(IdListInput input)
    {
        var positions = await GetListAsync();
        var positionList = positions.Where(it => input.IdList.Contains(it.Id)).ToList();// 获取指定ID的岗位列表
        return positionList;
    }

    /// <inheritdoc/>
    public async Task<LinqPagedList<SysPosition>> PositionSelector(PositionSelectorInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级机构
        var positions = await GetListAsync();
        var result = positions.WhereIF(input.OrgId > 0, it => orgIds.Contains(it.OrgId))//父级
            .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.OrgId))//在指定机构列表查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .ToList().LinqPagedList(input.PageNum, input.PageSize);
        return result;
    }

    /// <inheritdoc />
    public async Task<SysPosition> GetSysPositionById(long id)
    {
        var sysPositions = await GetListAsync();
        var result = sysPositions.Where(it => it.Id == id).FirstOrDefault();
        return result;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysPosition>> Page(PositionPageInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级机构
        var query = Context.Queryable<SysPosition>().WhereIF(input.OrgId > 0, it => orgIds.Contains(it.OrgId))//根据组织ID查询
            .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.OrgId))//在指定机构列表查询
            .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}").OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(PositionAddInput input, string name = SystemConst.SYS_POS)
    {
        await CheckInput(input, name);//检查参数
        var sysPosition = input.Adapt<SysPosition>();//实体转换
        sysPosition.Code = RandomHelper.CreateRandomString(10);//赋值Code
        if (await InsertAsync(sysPosition))//插入数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(PositionEditInput input, string name = SystemConst.SYS_POS)
    {
        await CheckInput(input, name);//检查参数
        var sysPosition = input.Adapt<SysPosition>();//实体转换
        if (await UpdateAsync(sysPosition))//更新数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input, string name = SystemConst.SYS_POS)
    {
        //获取所有ID
        var ids = input.Ids;
        if (ids.Count > 0)
        {
            //如果组织下有用户则不能删除
            if (await Context.Queryable<SysUser>().AnyAsync(it => ids.Contains(it.PositionId)))
            {
                throw Oops.Bah($"请先删除{name}下的用户");
            }
            //获取用户表有兼任组织的信息 oracle要改成Context.Queryable<SysUser>().Where(it => SqlFunc.Length(it.PositionJson) > 0).Select(it => it.PositionJson).ToListAsync();
            var positionJsons = await Context.Queryable<SysUser>().Where(it => !SqlFunc.IsNullOrEmpty(it.PositionJson)).Select(it => it.PositionJson).ToListAsync();
            if (positionJsons.Count > 0)
            {
                positionJsons.ForEach(it =>
                {
                    //获取机构列表
                    var positionIds = it.Select(it => it.PositionId).ToList();
                    //获取交集
                    var samePositionIds = ids.Intersect(positionIds).ToList();
                    if (samePositionIds.Count > 0)
                    {
                        throw Oops.Bah($"请先删除{name}下的兼任用户");
                    }
                });
            }
            //删除职位
            if (await DeleteByIdsAsync(ids.Cast<object>().ToArray()))
                await RefreshCache();//刷新缓存
        }
    }

    /// <inheritdoc />
    public async Task RefreshCache()
    {
        _simpleCacheService.Remove(SystemConst.CACHE_SYS_POSITION);//删除Key
        await GetListAsync();//重新写入缓存
    }

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysPosition"></param>
    /// <param name="name">名称</param>
    private async Task CheckInput(SysPosition sysPosition, string name)
    {
        //所有分类放一个列表
        var posCategoryList = new List<string>
        {
            CateGoryConst.POSITION_HIGH, CateGoryConst.POSITION_LOW, CateGoryConst.POSITION_MIDDLE
        };
        if (!posCategoryList.Contains(sysPosition.Category))
            throw Oops.Bah($"{name}所属分类错误:{sysPosition.Category}");
        var sysPositions = await GetListAsync();//获取全部
        if (sysPositions.Any(it => it.OrgId == sysPosition.OrgId && it.Name == sysPosition.Name && it.Id != sysPosition.Id))//判断同级是否有名称重复的
            throw Oops.Bah($"存在重复的{name}:{sysPosition.Name}");
        if (sysPosition.Id > 0)//如果ID大于0表示编辑
        {
            var position = sysPositions.Where(it => it.Id == sysPosition.Id).FirstOrDefault();//获取当前职位
            if (position != null)
            {
                if (position.OrgId != sysPosition.OrgId)//如果orgId不一样表示换机构了
                {
                    if (await Context.Queryable<SysUser>().Where(it => it.PositionId == sysPosition.Id || SqlFunc.JsonLike(it.PositionJson, sysPosition.Id.ToString())).AnyAsync())//如果职位下有用户
                        throw Oops.Bah($"该{name}下已存在用户,请先删除{name}下的用户");
                }
            }
            else
                throw Oops.Bah($"{name}不存在");
        }
    }

    #endregion 方法
}
