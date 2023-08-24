using Masuit.Tools.Models;

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

    /// <inheritdoc />
    public override async Task<List<SysPosition>> GetListAsync()
    {
        //先从Redis拿
        var sysPositions =
            _simpleCacheService.Get<List<SysPosition>>(SystemConst.Cache_SysPosition);
        if (sysPositions == null)
        {
            //redis没有就去数据库拿
            sysPositions = await base.GetListAsync();
            if (sysPositions.Count > 0)
            {
                //插入Redis
                _simpleCacheService.Set(SystemConst.Cache_SysPosition, sysPositions);
            }
        }
        return sysPositions;
    }

    /// <inheritdoc />
    public async Task<List<SysPosition>> GetPositionListByIdList(IdListInput input)
    {
        var positions = await GetListAsync();
        var positionList =
            positions.Where(it => input.IdList.Contains(it.Id)).ToList();// 获取指定ID的岗位列表
        return positionList;
    }


    /// <inheritdoc/>
    public async Task<LinqPagedList<SysPosition>> PositionSelector(PositionSelectorInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级机构
        var positions = await GetListAsync();
        var result = positions.WhereIF(input.OrgId > 0, it => orgIds.Contains(it.OrgId))//父级
            .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.OrgId))//在指定机构列表查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey),
                it => it.Name.Contains(input.SearchKey))//根据关键字查询
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
        var query = Context.Queryable<SysPosition>()
            .WhereIF(input.OrgId > 0, it => orgIds.Contains(it.OrgId))//根据组织ID查询
            .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.OrgId))//在指定机构列表查询
            .WhereIF(!string.IsNullOrEmpty(input.Category),
                it => it.Category == input.Category)//根据分类
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey),
                it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField),
                $"{input.SortField} {input.SortOrder}")
            .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(PositionAddInput input, string name = SimpleAdminConst.SysPos)
    {
        await CheckInput(input, name);//检查参数
        var sysPosition = input.Adapt<SysPosition>();//实体转换
        sysPosition.Code = RandomHelper.CreateRandomString(10);//赋值Code
        if (await InsertAsync(sysPosition))//插入数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(PositionEditInput input, string name = SimpleAdminConst.SysPos)
    {
        await CheckInput(input, name);//检查参数
        var sysPosition = input.Adapt<SysPosition>();//实体转换
        if (await UpdateAsync(sysPosition))//更新数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input, string name = SimpleAdminConst.SysPos)
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
            var positionJsons = await Context.Queryable<SysUser>()
                .Where(it => !SqlFunc.IsNullOrEmpty(it.PositionJson)).Select(it => it.PositionJson)
                .ToListAsync();
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
        _simpleCacheService.Remove(SystemConst.Cache_SysPosition);//删除Key
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
        var positionCategorys = new List<string>()
        {
            CateGoryConst.Position_HIGH, CateGoryConst.Position_LOW, CateGoryConst.Position_MIDDLE
        };
        if (!positionCategorys.Contains(sysPosition.Category))
            throw Oops.Bah($"{name}所属分类错误:{sysPosition.Category}");
        var sysPositions = await GetListAsync();//获取全部
        if (sysPositions.Any(it =>
                it.OrgId == sysPosition.OrgId && it.Name == sysPosition.Name
                && it.Id != sysPosition.Id))//判断同级是否有名称重复的
            throw Oops.Bah($"存在重复的{name}:{sysPosition.Name}");
        if (sysPosition.Id > 0)//如果ID大于0表示编辑
        {
            var postion =
                sysPositions.Where(it => it.Id == sysPosition.Id).FirstOrDefault();//获取当前职位
            if (postion != null)
            {
                if (postion.OrgId != sysPosition.OrgId)//如果orgId不一样表示换机构了
                {
                    if (await Context.Queryable<SysUser>().Where(it =>
                            it.PositionId == sysPosition.Id || SqlFunc.JsonLike(it.PositionJson,
                                sysPosition.Id.ToString())).AnyAsync())//如果职位下有用户
                        throw Oops.Bah($"该{name}下已存在用户,请先删除{name}下的用户");
                }
            }
            else
                throw Oops.Bah($"{name}不存在");
        }
    }

    #endregion 方法
}