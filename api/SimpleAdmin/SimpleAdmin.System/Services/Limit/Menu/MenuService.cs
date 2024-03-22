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
/// <inheritdoc cref="IMenuService"/>
/// </summary>
public class MenuService : DbRepository<SysResource>, IMenuService
{
    private readonly ILogger<MenuService> _logger;
    private readonly IResourceService _resourceService;
    private readonly IRelationService _relationService;

    public MenuService(ILogger<MenuService> logger, IResourceService resourceService, IRelationService relationService)
    {
        _logger = logger;
        _resourceService = resourceService;
        _relationService = relationService;
    }

    /// <inheritdoc/>
    public List<SysResource> ConstructMenuTrees(List<SysResource> resourceList, long? parentId = 0)
    {
        //找下级资源ID列表
        var resources = resourceList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (resources.Count > 0)//如果数量大于0
        {
            var data = new List<SysResource>();
            foreach (var item in resources)//遍历资源
            {
                item.Children = ConstructMenuTrees(resourceList, item.Id);//添加子节点
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<SysResource>();
    }

    /// <inheritdoc />
    public async Task<List<SysResource>> Tree(MenuTreeInput input, bool showDisabled = true)
    {
        //获取所有菜单
        var sysResources = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);
        sysResources = sysResources.WhereIF(input.Module != null, it => it.Module.Value == input.Module.Value)//根据模块查找
            .WhereIF(!showDisabled, it => it.Status == CommonStatusConst.ENABLE)//是否显示禁用的
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title == input.SearchKey)//根据关键字查找
            .ToList();
        //构建菜单树
        var tree = ConstructMenuTrees(sysResources);
        return tree;
    }

    /// <inheritdoc />
    public async Task<List<SysResource>> ShortcutTree(List<SysResource> sysResources = null)
    {
        if (sysResources == null)
            //获取所有菜单
            sysResources = (await _resourceService.GetAllModuleAndMenuAndSpaList())
                .Where(it => it.Status == CommonStatusConst.ENABLE).ToList();
        // //获取所有单页
        // var sysSpas = (await _resourceService.GetConfigsByCategory(CateGoryConst.RESOURCE_SPA))
        //     .Where(it => it.Status == CommonStatusConst.ENABLE).ToList();
        sysResources.ForEach(it =>
        {
            if (it.MenuType == SysResourceConst.CATALOG)
                it.ParentId = it.Module.Value;//目录的父级ID设置为模块ID
        });

        //构建菜单树
        var tree = ConstructMenuTrees(sysResources, null);
        //将单页的排前面根据排序码排序
        tree = tree.OrderByDescending(it => it.Category == CateGoryConst.RESOURCE_SPA).ThenBy(it => it.SortCode).ToList();
        return tree;
    }

    /// <inheritdoc />
    public async Task Add(MenuAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换

        if (await InsertAsync(sysResource))//插入数据
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_MENU);//刷新菜单缓存
    }

    /// <inheritdoc />
    public async Task Edit(MenuEditInput input)
    {
        var resource = await CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换
        var updatePath = resource.Path != input.Path;//是否更新路径
        var permissions = new List<SysRelation>();
        if (updatePath)
        {
            //获取所有角色和用户的权限关系
            var rolePermissions = await _relationService.GetRelationByCategory(CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION);
            var userPermissions = await _relationService.GetRelationByCategory(CateGoryConst.RELATION_SYS_USER_HAS_PERMISSION);
            //找到所有匹配的权限
            rolePermissions = rolePermissions.Where(it => it.TargetId.Contains(resource.Path)).ToList();
            userPermissions = userPermissions.Where(it => it.TargetId.Contains(resource.Path)).ToList();
            //更新路径
            rolePermissions.ForEach(it => it.TargetId = it.TargetId.Replace(resource.Path, input.Path));
            userPermissions.ForEach(it => it.TargetId = it.TargetId.Replace(resource.Path, input.Path));
            //添加到权限列表
            permissions.AddRange(rolePermissions);
            permissions.AddRange(userPermissions);
        }
        //事务
        var result = await Tenant.UseTranAsync(async () =>
        {
            await UpdateAsync(sysResource);//更新数据
            if (permissions.Count > 0)//如果权限列表大于0就更新    
            {
                await Context.Updateable(permissions)
                    .ExecuteCommandAsync();//更新关系表
            }
        });
        if (result.IsSuccess)//如果成功了
        {
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_MENU);//刷新菜单缓存
            //刷新关系表缓存
            if (updatePath)
            {
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION);
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_USER_HAS_PERMISSION);
            }
        }
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
            var sysResources = resourceList.Where(it => ids.Contains(it.Id)).ToList();
            //查找内置菜单
            var system = sysResources.Where(it => it.Code == SysResourceConst.SYSTEM).FirstOrDefault();
            if (system != null)
                throw Oops.Bah($"不可删除系统菜单:{system.Title}");
            //需要删除的资源ID列表
            var resourceIds = new List<long>();
            //遍历菜单列表
            sysResources.ForEach(it =>
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
                    .Where(it => it.Category == CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE && resourceIds.Contains(SqlFunc.ToInt64(it.TargetId)))
                    .ExecuteCommandAsync();
            });
            if (result.IsSuccess)//如果成功了
            {
                await _resourceService.RefreshCache(CateGoryConst.RESOURCE_MENU);//资源表菜单刷新缓存
                await _resourceService.RefreshCache(CateGoryConst.RESOURCE_BUTTON);//资源表按钮刷新缓存
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);//关系表刷新缓存
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
    public async Task<SysResource> Detail(BaseIdInput input)
    {
        var sysResources = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);
        var resource = sysResources.Where(it => it.Id == input.Id).FirstOrDefault();
        return resource;
    }

    /// <inheritdoc />
    public async Task ChangeModule(MenuChangeModuleInput input)
    {
        var sysResource = await Detail(new BaseIdInput { Id = input.Id });
        if (sysResource != null)
        {
            if (sysResource.Module == input.Module)//如果模块ID没变直接返回
                return;
            if (sysResource.ParentId != 0)
                throw Oops.Bah("非顶级菜单不可修改所属模块");
            //获取所有菜单和模块
            var resourceList = await _resourceService.GetListAsync(new List<string> { CateGoryConst.RESOURCE_MENU, CateGoryConst.RESOURCE_MODULE });
            if (!resourceList.Any(it => it.Category == CateGoryConst.RESOURCE_MODULE && it.Id == input.Module.Value))
                throw Oops.Bah("不存在的模块");
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

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysResource"></param>
    private async Task<SysResource> CheckInput(SysResource sysResource)
    {
        //获取所有菜单列表
        var menList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);
        //判断是否有同级且同名的菜单
        if (menList.Any(it => it.ParentId == sysResource.ParentId && it.Title == sysResource.Title && it.Id != sysResource.Id))
            throw Oops.Bah($"存在重复的菜单名称:{sysResource.Title}");
        if (sysResource.ParentId != 0)
        {
            //获取父级,判断父级ID正不正确
            var parent = menList.Where(it => it.Id == sysResource.ParentId).FirstOrDefault();
            if (parent != null)
            {
                if (parent.Module != sysResource.Module)//如果父级的模块和当前模块不一样
                    throw Oops.Bah("模块与上级菜单不一致");
                if (parent.Id == sysResource.Id)
                    throw Oops.Bah("上级菜单不能选择自己");
            }
            else
            {
                throw Oops.Bah($"上级菜单不存在:{sysResource.ParentId}");
            }
        }
        //如果ID大于0表示编辑
        if (sysResource.Id > 0)
        {
            var resource = menList.Where(it => it.Id == sysResource.Id).FirstOrDefault();
            if (resource == null)
                throw Oops.Bah($"菜单不存在:{sysResource.Id}");
            return resource;
        }
        return null;
    }

    #endregion 方法
}
