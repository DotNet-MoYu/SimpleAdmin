// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using MoYu.EventBus;

namespace SimpleAdmin.Plugin.Mobile;

/// <summary>
/// <inheritdoc cref="IMobileMenuService"/>
/// </summary>
public class MobileMenuService : DbRepository<MobileResource>, IMobileMenuService
{
    private readonly ILogger<MobileMenuService> _logger;
    private readonly IMobileResourceService _resourceService;
    private readonly IRelationService _relationService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ISysRoleService _roleService;

    public MobileMenuService(ILogger<MobileMenuService> logger, IMobileResourceService resourceService, IRelationService relationService,
        IEventPublisher eventPublisher, ISysRoleService roleService)
    {
        _logger = logger;
        _resourceService = resourceService;
        _relationService = relationService;
        _eventPublisher = eventPublisher;
        _roleService = roleService;
    }

    /// <inheritdoc/>
    public List<MobileResource> ConstructMenuTrees(List<MobileResource> resourceList, long parentId = 0)
    {
        //找下级资源ID列表
        var resources = resourceList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (resources.Count > 0)//如果数量大于0
        {
            var data = new List<MobileResource>();
            foreach (var item in resources)//遍历资源
            {
                item.Children = ConstructMenuTrees(resourceList, item.Id);//添加子节点
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<MobileResource>();
    }

    /// <inheritdoc />
    public async Task<List<MobileResource>> Tree(MenuTreeInput input)
    {
        //获取所有菜单
        var mobileResources = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);
        mobileResources = mobileResources.WhereIF(input.Module != null, it => it.Module.Value == input.Module.Value)//根据模块查找
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title == input.SearchKey)//根据关键字查找
            .ToList();
        //构建菜单树
        var tree = ConstructMenuTrees(mobileResources);
        return tree;
    }

    /// <inheritdoc />
    public async Task Add(MobileMenuAddInput input)
    {
        await CheckInput(input);//检查参数
        var mobileResource = input.Adapt<MobileResource>();//实体转换

        if (await InsertAsync(mobileResource))//插入数据
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_MENU);//刷新菜单缓存
    }

    /// <inheritdoc />
    public async Task Edit(MobileMenuEditInput input)
    {
        await CheckInput(input);//检查参数
        var mobileResource = input.Adapt<MobileResource>();//实体转换
        if (await UpdateAsync(mobileResource))//更新数据
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_MENU);//刷新菜单缓存
    }

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        if (ids.Count > 0)
        {
            //获取所有菜单和按钮
            var resourceList = await _resourceService.GetListAsync(new List<string> { CateGoryConst.RESOURCE_MENU, CateGoryConst.RESOURCE_BUTTON });
            //找到要删除的菜单
            var mobileResources = resourceList.Where(it => ids.Contains(it.Id)).ToList();
            //查找内置菜单
            var system = mobileResources.Where(it => it.Code == SysResourceConst.SYSTEM).FirstOrDefault();
            if (system != null)
                throw Oops.Bah($"不可删除系统菜单:{system.Title}");
            //需要删除的资源ID列表
            var resourceIds = new List<long>();
            //遍历菜单列表
            mobileResources.ForEach(it =>
            {
                //获取菜单所有子节点
                var child = _resourceService.GetChildListById(resourceList, it.Id, false);
                //将子节点ID添加到删除资源ID列表
                resourceIds.AddRange(child.Select(it => it.Id).ToList());
                resourceIds.Add(it.Id);//添加到删除资源ID列表
            });
            ids.AddRange(resourceIds);//添加到删除ID列表
            //事务
            var result = await Tenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除菜单和按钮
                await Context.Deleteable<SysRelation>()//关系表删除对应SYS_ROLE_HAS_RESOURCE
                    .Where(it => it.Category == MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE
                        && resourceIds.Contains(SqlFunc.ToInt64(it.TargetId))).ExecuteCommandAsync();
            });
            if (result.IsSuccess)//如果成功了
            {
                await _resourceService.RefreshCache(CateGoryConst.RESOURCE_MENU);//资源表菜单刷新缓存
                await _resourceService.RefreshCache(CateGoryConst.RESOURCE_BUTTON);//资源表按钮刷新缓存
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
        var mobileResources = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);
        var resource = mobileResources.Where(it => it.Id == input.Id).FirstOrDefault();
        return resource;
    }

    /// <inheritdoc />
    public async Task ChangeModule(MenuChangeModuleInput input)
    {
        var mobileResource = await Detail(new BaseIdInput { Id = input.Id });
        if (mobileResource != null)
        {
            if (mobileResource.Module == input.Module)//如果模块ID没变直接返回
                return;
            if (mobileResource.ParentId != 0)
                throw Oops.Bah($"非顶级菜单不可修改所属模块");
            //获取所有菜单和模块
            var resourceList = await _resourceService.GetListAsync(new List<string> { CateGoryConst.RESOURCE_MENU, CateGoryConst.RESOURCE_MODULE });
            if (!resourceList.Any(it => it.Category == CateGoryConst.RESOURCE_MODULE && it.Id == input.Module.Value))
                throw Oops.Bah($"不存在的模块");
            //获取所有菜单
            var menuList = resourceList.Where(it => it.Category == CateGoryConst.RESOURCE_MENU).ToList();
            //获取需要改变模块菜单的所有子菜单
            var childList = _resourceService.GetChildListById(menuList, input.Id);
            childList.ForEach(it => it.Module = input.Module);
            //更新数据
            await UpdateRangeAsync(childList);
            //刷新菜单缓存
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_MENU);
        }
    }

    /// <inheritdoc />
    public async Task GrantRoleMobileResource(GrantResourceInput input)
    {
        var menuIds = input.GrantInfoList.Select(it => it.MenuId).ToList();//菜单ID
        var extJsons = input.GrantInfoList.Select(it => it.ToJson()).ToList();//拓展信息
        var relationRoles = new List<SysRelation>();//要添加的角色资源和授权关系表
        var sysRole = (await _roleService.GetListAsync()).Where(it => it.Id == input.Id).FirstOrDefault();//获取角色
        if (sysRole != null)
        {
            //遍历菜单列表
            for (var i = 0; i < menuIds.Count; i++)
            {
                //将角色资源添加到列表
                relationRoles.Add(new SysRelation
                {
                    ObjectId = sysRole.Id,
                    TargetId = menuIds[i].ToString(),
                    Category = MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE,
                    ExtJson = extJsons?[i]
                });
            }
            //事务
            var result = await Tenant.UseTranAsync(async () =>
            {
                var relatioRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
                //如果不是代码生成,就删除老的
                if (!input.IsCodeGen)
                    await relatioRep.DeleteAsync(it =>
                        it.ObjectId == sysRole.Id && (it.Category == MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE
                        || it.Category == MobileConst.RELATION_SYS_USER_HAS_MOBILE_RESOURCE));
                await relatioRep.InsertRangeAsync(relationRoles);//添加新的
            });
            if (result.IsSuccess)//如果成功了
            {
                await _relationService.RefreshCache(MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE);//刷新关系缓存
                await _relationService.RefreshCache(MobileConst.RELATION_SYS_USER_HAS_MOBILE_RESOURCE);//刷新关系缓存
                await _eventPublisher.PublishAsync(EventSubscriberConst.CLEAR_USER_CACHE, new List<long> { input.Id });//发送事件清除角色下用户缓存
            }
            else
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0003);
            }
        }
    }

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="mobileResource"></param>
    private async Task CheckInput(MobileResource mobileResource)
    {
        mobileResource.Category = mobileResource.MenuType;//设置为菜单
        //获取所有菜单列表
        var menList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);
        //判断是否有同级且同名的菜单
        if (menList.Any(it => it.ParentId == mobileResource.ParentId && it.Title == mobileResource.Title && it.Id != mobileResource.Id))
            throw Oops.Bah($"存在重复的菜单名称:{mobileResource.Title}");
        if (mobileResource.ParentId != 0)
        {
            //获取父级,判断父级ID正不正确
            var parent = menList.Where(it => it.Id == mobileResource.ParentId).FirstOrDefault();
            if (parent != null)
            {
                if (parent.Module != mobileResource.Module)//如果父级的模块和当前模块不一样
                    throw Oops.Bah($"模块与上级菜单不一致");
                if (parent.Id == mobileResource.Id)
                    throw Oops.Bah($"上级菜单不能选择自己");
            }
            else
            {
                throw Oops.Bah($"上级菜单不存在:{mobileResource.ParentId}");
            }
        }
    }

    #endregion 方法
}
