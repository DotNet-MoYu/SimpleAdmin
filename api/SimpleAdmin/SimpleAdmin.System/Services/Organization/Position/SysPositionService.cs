// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="ISysPositionService"/>
/// </summary>
public class SysPositionService : DbRepository<SysPosition>, ISysPositionService
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly ISysOrgService _sysOrgService;
    private readonly IDictService _dictService;

    public SysPositionService(ISimpleCacheService simpleCacheService, ISysOrgService sysOrgService, IDictService dictService)
    {
        _simpleCacheService = simpleCacheService;
        _sysOrgService = sysOrgService;
        _dictService = dictService;
    }

    #region 查询

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
    public async Task<List<PositionSelectorOutput>> Selector(PositionSelectorInput input)
    {
        var sysOrgList = await _sysOrgService.GetListAsync(false);//获取所有组织
        var sysPositions = await GetListAsync();//获取所有职位
        if (input.OrgIds != null)//根据数据范围查
        {
            sysOrgList = sysOrgList.Where(it => input.OrgIds.Contains(it.Id)).ToList();//在指定组织列表查询
            sysPositions = sysPositions.Where(it => input.OrgIds.Contains(it.OrgId)).ToList();//在指定职位列表查询
        }
        var result = await ConstructPositionSelector(sysOrgList, sysPositions);//构造树
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
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级组织
        var query = Context.Queryable<SysPosition>().WhereIF(input.OrgId > 0, it => orgIds.Contains(it.OrgId))//根据组织ID查询
            .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.OrgId))//在指定组织列表查询
            .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类
            .WhereIF(!string.IsNullOrEmpty(input.Status), it => it.Status == input.Status)//根据状态
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
            .OrderBy(it => it.SortCode)//排序
            .OrderBy(it => it.CreateTime);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }



    /// <inheritdoc/>
    public async Task<List<PositionTreeOutput>> Tree(PositionTreeInput input)
    {
        var result = new List<PositionTreeOutput>();//返回结果
        var sysOrgList = await _sysOrgService.GetListAsync(false);//获取所有组织
        var sysPositions = await GetListAsync();//获取所有职位
        if (input.OrgIds != null)//根据数据范围查
        {
            sysOrgList = sysOrgList.Where(it => input.OrgIds.Contains(it.Id)).ToList();//在指定组织列表查询
            sysPositions = sysPositions.Where(it => input.OrgIds.Contains(it.OrgId)).ToList();//在指定职位列表查询
        }
        var posCategory = await _dictService.GetChildrenByDictValue(SysDictConst.POSITION_CATEGORY);//获取职位分类
        var topOrgList = sysOrgList.Where(it => it.ParentId == 0).ToList();//获取顶级组织
        //遍历顶级组织
        foreach (var org in topOrgList)
        {
            var childIds = await _sysOrgService.GetOrgChildIds(org.Id, true, sysOrgList);//获取组织下的所有子级ID
            var orgPositions = sysPositions.Where(it => childIds.Contains(it.OrgId)).ToList();//获取组织下的职位
            if (orgPositions.Count == 0) continue;
            var positionTreeOutput = new PositionTreeOutput
            {
                Id = org.Id,
                Name = org.Name,
                IsPosition = false
            };//实例化组织树
            //获取组织下的职位职位分类
            foreach (var category in posCategory)
            {
                var id = CommonUtils.GetSingleId();//生成唯一ID临时用,因为前端需要ID
                var categoryTreeOutput = new PositionTreeOutput
                {
                    Id = id,
                    Name = category.DictLabel,
                    IsPosition = false
                };//实例化职位分类树
                var positions = orgPositions.Where(it => it.Category == category.DictValue).ToList();//获取职位分类下的职位
                //遍历职位，实例化职位树
                positions.ForEach(it =>
                {
                    categoryTreeOutput.Children.Add(new PositionTreeOutput()
                    {
                        Id = it.Id,
                        Name = it.Name,
                        IsPosition = true
                    });//添加职位
                });
                positionTreeOutput.Children.Add(categoryTreeOutput);
            }
            result.Add(positionTreeOutput);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<SysPosition> Detail(BaseIdInput input)
    {
        return await GetSysPositionById(input.Id);
    }

    #endregion

    #region 编辑

    /// <inheritdoc />
    public async Task Edit(PositionEditInput input, string name = SystemConst.SYS_POS)
    {
        await CheckInput(input, name);//检查参数
        var sysPosition = input.Adapt<SysPosition>();//实体转换
        if (await UpdateAsync(sysPosition))//更新数据
            await RefreshCache();//刷新缓存
    }



    /// <inheritdoc />
    public async Task RefreshCache()
    {
        _simpleCacheService.Remove(SystemConst.CACHE_SYS_POSITION);//删除缓存
        await GetListAsync();//重新写入缓存
    }

    #endregion

    #region 新增

    /// <inheritdoc />
    public async Task Add(PositionAddInput input, string name = SystemConst.SYS_POS)
    {
        await CheckInput(input, name);//检查参数
        var sysPosition = input.Adapt<SysPosition>();//实体转换
        if (await InsertAsync(sysPosition))//插入数据
            await RefreshCache();//刷新缓存
    }

    #endregion

    #region 删除

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
            //删除职位
            if (await DeleteByIdsAsync(ids.Cast<object>().ToArray()))
                await RefreshCache();//刷新缓存
        }
    }

    #endregion

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
            if (position == null)
                throw Oops.Bah($"{name}不存在");
        }
        //如果code没填
        if (string.IsNullOrEmpty(sysPosition.Code))
        {
            sysPosition.Code = RandomHelper.CreateRandomString(10);//赋值Code
        }
        else
        {
            //判断是否有相同的Code
            if (sysPositions.Any(it => it.Code == sysPosition.Code && it.Id != sysPosition.Id))
                throw Oops.Bah($"存在重复的编码:{sysPosition.Code}");
        }
    }

    /// <summary>
    /// 构建职位选择器
    /// </summary>
    /// <param name="orgList">组织列表</param>
    /// <param name="sysPositions">职位列表</param>
    /// <param name="parentId">父Id</param>
    /// <returns></returns>
    public async Task<List<PositionSelectorOutput>> ConstructPositionSelector(List<SysOrg> orgList, List<SysPosition> sysPositions,
        long parentId = SimpleAdminConst.ZERO)
    {
        //找下级组织列表
        var orgInfos = orgList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        var data = new List<PositionSelectorOutput>();
        if (orgInfos.Count > 0)//如果数量大于0
        {
            foreach (var item in orgInfos)//遍历组织
            {
                var childIds = await _sysOrgService.GetOrgChildIds(item.Id, true, orgList);//获取组织下的所有子级ID
                var orgPositions = sysPositions.Where(it => childIds.Contains(it.OrgId)).ToList();//获取组织下的职位
                if (orgPositions.Count > 0)//如果组织和组织下级有职位
                {
                    var positionSelectorOutput = new PositionSelectorOutput
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Children = await ConstructPositionSelector(orgList, sysPositions, item.Id)//递归
                    };//实例化职位树
                    var positions = orgPositions.Where(it => it.OrgId == item.Id).ToList();//获取组织下的职位
                    if (positions.Count > 0)//如果数量大于0
                    {
                        foreach (var position in positions)
                        {
                            positionSelectorOutput.Children.Add(new PositionSelectorOutput
                            {
                                Id = position.Id,
                                Name = position.Name
                            });//添加职位
                        }
                    }
                    data.Add(positionSelectorOutput);//添加到列表
                }
            }
            return data;//返回结果
        }
        return new List<PositionSelectorOutput>();
    }

    #endregion 方法
}
