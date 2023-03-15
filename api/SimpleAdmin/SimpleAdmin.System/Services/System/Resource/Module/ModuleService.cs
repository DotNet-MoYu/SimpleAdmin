
using DnsClient.Internal;


namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IModuleService"/>
/// </summary>
public class ModuleService : DbRepository<SysResource>, IModuleService
{
    private readonly ILogger<ModuleService> _logger;
    private readonly ISimpleRedis _simpleRedis;
    private readonly IResourceService _resourceService;
    private readonly IRelationService _relationService;

    public ModuleService(ILogger<ModuleService> logger, ISimpleRedis simpleRedis,
                         IResourceService resourceService,
                         IRelationService relationService)
    {
        this._logger = logger;
        this._simpleRedis = simpleRedis;
        this._resourceService = resourceService;
        this._relationService = relationService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysResource>> Page(ModulePageInput input)
    {

        var query = Context.Queryable<SysResource>()
                         .Where(it => it.Category == CateGoryConst.Resource_MODULE)//模块
                         .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title.Contains(input.SearchKey))//根据关键字查询
                         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
                         .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(ModuleAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换
        if (await InsertAsync(sysResource))//插入数据
            await _resourceService.RefreshCache(CateGoryConst.Resource_MODULE);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(ModuleEditInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换
        if (await UpdateAsync(sysResource))//更新数据
            await _resourceService.RefreshCache(CateGoryConst.Resource_MODULE);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            //获取所有
            var resourceList = await _resourceService.GetListAsync();
            //找到要删除的模块
            var sysresources = resourceList.Where(it => ids.Contains(it.Id)).ToList();
            //查找内置模块
            var system = sysresources.Where(it => it.Code == ResourceConst.System).FirstOrDefault();
            if (system != null)
                throw Oops.Bah($"不可删除系统内置模块:{system.Title}");
            //获取模块下的所有菜单Id列表
            var menuIds = resourceList.Where(it => ids.Contains(it.Module.ToLong()) && it.ParentId.ToLong() == SimpleAdminConst.Zero).Select(it => it.Id).ToList();
            //需要删除的资源ID列表
            var resourceIds = new List<long>();
            //遍历列表
            menuIds.ForEach(it =>
            {
                //获取菜单所有子节点
                var child = _resourceService.GetChildListById(resourceList, it, false);
                //将子节点ID添加到删除ID列表
                resourceIds.AddRange(child.Select(it => it.Id).ToList());
                resourceIds.Add(it);//添加到删除ID列表
            });
            ids.AddRange(resourceIds);
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {

                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除菜单和按钮
                await Context.Deleteable<SysRelation>()//关系表删除对应SYS_ROLE_HAS_RESOURCE
                .Where(it => it.Category == CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE && resourceIds.Contains(SqlFunc.ToInt64(it.TargetId))).ExecuteCommandAsync();

            });
            if (result.IsSuccess)//如果成功了
            {
                await _resourceService.RefreshCache();//资源表刷新缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);//关系表刷新缓存
            }
            else
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0002);
            }
        }
    }



    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysResource"></param>
    private async Task CheckInput(SysResource sysResource)
    {
        var sysResourceList = await _resourceService.GetListByCategory(CateGoryConst.Resource_MODULE);
        //判断是否从存在重复模块
        var hasSameName = sysResourceList.Any(it => it.Title == sysResource.Title && it.Id != sysResource.Id);
        if (hasSameName)
        {
            throw Oops.Bah($"存在重复的模块:{sysResource.Title}");
        }
        //设置为模块
        sysResource.Category = CateGoryConst.Resource_MODULE;
    }
    #endregion

}
