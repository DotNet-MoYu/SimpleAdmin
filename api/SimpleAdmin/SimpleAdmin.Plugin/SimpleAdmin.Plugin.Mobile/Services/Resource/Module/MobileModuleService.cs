// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.Mobile;

/// <summary>
/// <inheritdoc cref="IMobileModuleService"/>
/// </summary>
public class MobileModuleService : DbRepository<MobileResource>, IMobileModuleService
{
    private readonly ILogger<MobileModuleService> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IMobileResourceService _mobileResourceService;
    private readonly IRelationService _relationService;

    public MobileModuleService(ILogger<MobileModuleService> logger, ISimpleCacheService simpleCacheService,
        IMobileResourceService mobileResourceService,
        IRelationService relationService)
    {
        _logger = logger;
        _simpleCacheService = simpleCacheService;
        _mobileResourceService = mobileResourceService;
        _relationService = relationService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<MobileResource>> Page(ModulePageInput input)
    {
        var query = Context.Queryable<MobileResource>()
            .Where(it => it.Category == CateGoryConst.RESOURCE_MODULE)//模块
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
            .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(MobileModuleAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<MobileResource>();//实体转换
        if (await InsertAsync(sysResource))//插入数据
            await _mobileResourceService.RefreshCache(CateGoryConst.RESOURCE_MODULE);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(MobileModuleEditInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<MobileResource>();//实体转换
        if (await UpdateAsync(sysResource))//更新数据
            await _mobileResourceService.RefreshCache(CateGoryConst.RESOURCE_MODULE);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        if (ids.Count > 0)
        {
            //获取所有
            var resourceList = await _mobileResourceService.GetListAsync();
            //找到要删除的模块
            var sysresources = resourceList.Where(it => ids.Contains(it.Id)).ToList();
            //查找内置模块
            var system = sysresources.Where(it => it.Code == SysResourceConst.SYSTEM).FirstOrDefault();
            if (system != null)
                throw Oops.Bah($"不可删除系统内置模块:{system.Title}");
            //获取模块下的所有菜单Id列表
            var menuIds = resourceList.Where(it => ids.Contains(it.Module.ToLong()) && it.ParentId.ToLong() == SimpleAdminConst.ZERO)
                .Select(it => it.Id).ToList();
            //需要删除的资源ID列表
            var resourceIds = new List<long>();
            //遍历列表
            menuIds.ForEach(it =>
            {
                //获取菜单所有子节点
                var child = _mobileResourceService.GetChildListById(resourceList, it, false);
                //将子节点ID添加到删除ID列表
                resourceIds.AddRange(child.Select(it => it.Id).ToList());
                resourceIds.Add(it);//添加到删除ID列表
            });
            ids.AddRange(resourceIds);
            //事务
            var result = await Tenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除菜单和按钮
                await Context.Deleteable<SysRelation>()//关系表删除对应SYS_ROLE_HAS_MOBILE_RESOURCE
                    .Where(it => it.Category == MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE
                        && resourceIds.Contains(SqlFunc.ToInt64(it.TargetId))).ExecuteCommandAsync();
            });
            if (result.IsSuccess)//如果成功了
            {
                await _mobileResourceService.RefreshCache();//资源表刷新缓存
                await _relationService.RefreshCache(MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE);//关系表刷新缓存
            }
            else
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0002);
            }
        }
    }

    /// <inheritdoc />
    public async Task<MobileResource> Detail(BaseIdInput input)
    {
        var mobileResource = await GetFirstAsync(it => it.Id == input.Id);
        return mobileResource;
    }

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysResource"></param>
    private async Task CheckInput(MobileResource sysResource)
    {
        var sysResourceList = await _mobileResourceService.GetListByCategory(CateGoryConst.RESOURCE_MODULE);
        //判断是否从存在重复模块
        var hasSameName = sysResourceList.Any(it => it.Title == sysResource.Title && it.Id != sysResource.Id);
        if (hasSameName)
        {
            throw Oops.Bah($"存在重复的模块:{sysResource.Title}");
        }
        //设置为模块
        sysResource.Category = CateGoryConst.RESOURCE_MODULE;
    }

    #endregion 方法
}
