namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="ISpaService"/>
/// </summary>
public class SpaService : DbRepository<SysResource>, ISpaService
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IResourceService _resourceService;

    public SpaService(ISimpleCacheService simpleCacheService, IResourceService resourceService)
    {
        _simpleCacheService = simpleCacheService;
        this._resourceService = resourceService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysResource>> Page(SpaPageInput input)
    {

        var query = Context.Queryable<SysResource>()
                         .Where(it => it.Category == CateGoryConst.Resource_SPA)//单页
                         .WhereIF(!string.IsNullOrEmpty(input.MenuType), it => it.MenuType == input.MenuType)//根据菜单类型查询
                         .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title.Contains(input.SearchKey) || it.Path.Contains(input.SearchKey))//根据关键字查询
                         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
                         .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(SpaAddInput input)
    {
        CheckInput(input);//检查参数
        input.Code = RandomHelper.CreateRandomString(10);//code取随机值
        var sysResource = input.Adapt<SysResource>();//实体转换
        if (await InsertAsync(sysResource))//插入数据
            await _resourceService.RefreshCache(CateGoryConst.Resource_SPA);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(SpaEditInput input)
    {
        CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换
        if (await UpdateAsync(sysResource))//更新数据
            await _resourceService.RefreshCache(CateGoryConst.Resource_SPA);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            //获取所有
            var resourceList = await _resourceService.GetListByCategory(CateGoryConst.Resource_SPA);
            //找到要删除的
            var sysresources = resourceList.Where(it => ids.Contains(it.Id)).ToList();
            //查找内置单页面
            var system = sysresources.Where(it => it.Code == ResourceConst.System).FirstOrDefault();
            if (system != null)
                throw Oops.Bah($"不可删除系统内置单页面:{system.Title}");
            //删除菜单
            await DeleteAsync(sysresources);
            await _resourceService.RefreshCache(CateGoryConst.Resource_SPA);//刷新缓存
        }
    }

    #region 方法


    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysResource"></param>
    private void CheckInput(SysResource sysResource)
    {
        //判断菜单类型
        if (sysResource.MenuType == ResourceConst.MENU)//如果是菜单
        {
            if (string.IsNullOrEmpty(sysResource.Name))
            {
                throw Oops.Bah($"单页名称不能为空");
            }
            if (string.IsNullOrEmpty(sysResource.Component))
            {
                throw Oops.Bah($"组件地址不能为空");
            }
        }
        else if (sysResource.MenuType == ResourceConst.IFRAME || sysResource.MenuType == ResourceConst.LINK)//如果是内链或者外链
        {
            sysResource.Name = RandomHelper.CreateNum(10);//设置name为随机数
            sysResource.Component = null;
        }
        else
        {
            throw Oops.Bah($"单页类型错误:{sysResource.MenuType}");//都不是
        }
        //设置为单页
        sysResource.Category = CateGoryConst.Resource_SPA;
    }
    #endregion
}
