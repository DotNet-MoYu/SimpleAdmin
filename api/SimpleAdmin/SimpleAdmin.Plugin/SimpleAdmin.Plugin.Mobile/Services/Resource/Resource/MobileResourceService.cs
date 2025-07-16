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

/// <inheritdoc cref="IMobileResourceService"/>
public class MobileResourceService : DbRepository<MobileResource>, IMobileResourceService
{
    private readonly ISimpleCacheService _simpleCacheService;

    public MobileResourceService(ISimpleCacheService simpleCacheService)
    {
        _simpleCacheService = simpleCacheService;
    }

    /// <inheritdoc />
    public async Task<List<string>> GetCodeByIds(List<long> ids, string category)
    {
        //根据分类获取所有
        var mobileResources = await GetListByCategory(category);
        //条件查询
        var result = mobileResources.Where(it => ids.Contains(it.Id)).Select(it => it.Code).ToList();
        return result;
    }

    /// <inheritdoc/>
    public async Task<List<MobileResource>> GetListAsync(List<string>? categoryList = null)
    {
        //定义结果
        var mobileResources = new List<MobileResource>();

        //定义资源分类列表,如果是空的则获取全部资源
        categoryList = categoryList != null
            ? categoryList
            : new List<string>
                { CateGoryConst.RESOURCE_MENU, CateGoryConst.RESOURCE_BUTTON, CateGoryConst.RESOURCE_SPA, CateGoryConst.RESOURCE_MODULE };
        //遍历列表
        foreach (var category in categoryList)
        {
            //根据分类获取到资源列表
            var data = await GetListByCategory(category);
            //添加到结果集
            mobileResources.AddRange(data);
        }
        return mobileResources;
    }

    /// <inheritdoc/>
    public async Task<List<MobileResource>> GetaModuleAndMenuAndSpaList()
    {
        //获取所有的菜单和模块以及单页面列表，
        var mobileResources = await GetListAsync(new List<string>
            { CateGoryConst.RESOURCE_MODULE, CateGoryConst.RESOURCE_MENU, CateGoryConst.RESOURCE_SPA });
        if (mobileResources != null)
        {
            //并按分类和排序码排序
            mobileResources = mobileResources.OrderBy(it => it.Category).ThenBy(it => it.SortCode).ToList();
        }
        return mobileResources;
    }

    /// <inheritdoc/>
    public async Task RefreshCache(string category = null)
    {
        //如果分类是空的
        if (category == null)
        {
            //删除全部key
            _simpleCacheService.DelByPattern(MobileConst.CACHE_MOBILE_RESOURCE);
            await GetListAsync();
        }
        else
        {
            //否则只删除一个Key
            _simpleCacheService.Remove(MobileConst.CACHE_MOBILE_RESOURCE + category);
            await GetListByCategory(category);
        }
    }

    /// <inheritdoc />
    public async Task<List<MobileResource>> GetChildListById(long resId, bool isContainOneself = true)
    {
        //获取所有机构
        var mobileResources = await GetListAsync();
        //查找下级
        var childLsit = GetResourceChilden(mobileResources, resId);
        if (isContainOneself)//如果包含自己
        {
            //获取自己的机构信息
            var self = mobileResources.Where(it => it.Id == resId).FirstOrDefault();
            if (self != null) childLsit.Insert(0, self);//如果机构不为空就插到第一个
        }
        return childLsit;
    }

    /// <inheritdoc />
    public List<MobileResource> GetChildListById(List<MobileResource> mobileResources, long resId, bool isContainOneself = true)
    {
        //查找下级
        var childLsit = GetResourceChilden(mobileResources, resId);
        if (isContainOneself)//如果包含自己
        {
            //获取自己的机构信息
            var self = mobileResources.Where(it => it.Id == resId).FirstOrDefault();
            if (self != null) childLsit.Insert(0, self);//如果机构不为空就插到第一个
        }
        return childLsit;
    }

    /// <inheritdoc />
    public async Task<List<MobileResource>> GetListByCategory(string category)
    {
        //先从Redis拿
        var mobileResources = _simpleCacheService.Get<List<MobileResource>>(MobileConst.CACHE_MOBILE_RESOURCE + category);
        if (mobileResources == null)
        {
            //redis没有就去数据库拿
            mobileResources = await base.GetListAsync(it => it.Category == category);
            if (mobileResources.Count > 0)
            {
                //插入Redis
                _simpleCacheService.Set(MobileConst.CACHE_MOBILE_RESOURCE + category, mobileResources);
            }
        }
        return mobileResources;
    }

    /// <inheritdoc />
    public async Task<List<ResTreeSelector>> ResourceTreeSelector()
    {
        var resourceTreeSelectors = new List<ResTreeSelector>();//定义结果
        //获取模块列表
        var moduleList = await GetListByCategory(CateGoryConst.RESOURCE_MODULE);
        //遍历模块
        foreach (var module in moduleList)
        {
            //将实体转换为ResourceTreeSelectorOutPut获取基本信息
            var resourceTreeSelector = module.Adapt<ResTreeSelector>();
            resourceTreeSelector.Menu = await GetRoleGrantResourceMenus(module.Id);
            resourceTreeSelectors.Add(resourceTreeSelector);
        }
        return resourceTreeSelectors;
    }

    /// <inheritdoc />
    public List<PermissionTreeSelector> PermissionTreeSelector(List<string> routes)
    {
        var permissions = new List<PermissionTreeSelector>();//权限列表

        // 获取所有需要数据权限的控制器
        var controllerTypes =
            App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(RolePermissionAttribute), false));
        foreach (var controller in controllerTypes)
        {
            //获取数据权限特性
            var routeAttributes = controller.GetCustomAttributes<RouteAttribute>().ToList();
            if (routeAttributes == null)
                continue;
            var route = routeAttributes[0];//取第一个值
            var routeName = GetRouteName(controller.Name, route.Template);//赋值路由名称
            //如果路由包含在路由列表中
            if (routes.Contains(routeName))
            {
                //获取所有方法
                var menthods = controller.GetMethods();
                //遍历方法
                foreach (var menthod in menthods)
                {
                    //获取忽略数据权限特性
                    var ignoreRolePermission = menthod.GetCustomAttribute<IgnoreRolePermissionAttribute>();
                    if (ignoreRolePermission == null)//如果是空的代表需要数据权限
                    {
                        //获取接口描述
                        var displayName = menthod.GetCustomAttribute<DisplayNameAttribute>();
                        if (displayName != null)
                        {
                            //默认路由名称
                            var apiRoute = StringHelper.FirstCharToLower(menthod.Name);
                            //获取get特性
                            var requestGet = menthod.GetCustomAttribute<HttpGetAttribute>();
                            if (requestGet != null)//如果是get方法
                                apiRoute = requestGet.Template;
                            else
                            {
                                //获取post特性
                                var requestPost = menthod.GetCustomAttribute<HttpPostAttribute>();
                                if (requestPost != null)//如果是post方法
                                    apiRoute = requestPost.Template;
                            }
                            apiRoute = route.Template + $"/{apiRoute}";
                            var apiName = displayName.DisplayName;//如果描述不为空则接口名称用描述的名称
                            //合并
                            var permissionName = apiRoute + $"[{apiName}]";
                            //添加到权限列表
                            permissions.Add(new PermissionTreeSelector
                            {
                                ApiName = apiName,
                                ApiRoute = apiRoute,
                                PermissionName = permissionName
                            });
                        }
                    }
                }
            }
        }
        return permissions;
    }

    /// <inheritdoc />
    public async Task<List<MobileResource>> GetMenuByMenuIds(List<long> menuIds)
    {
        //获取所有菜单
        var menuList = await GetListByCategory(CateGoryConst.RESOURCE_MENU);
        //获取菜单信息
        var menus = menuList.Where(it => menuIds.Contains(it.Id)).ToList();
        return menus;
    }

    #region 方法

    /// <summary>
    /// 获取路由地址名称
    /// </summary>
    /// <param name="controllerName">控制器地址</param>
    /// <param name="template">路由名称</param>
    /// <returns></returns>
    private string GetRouteName(string controllerName, string template)
    {
        if (!template.StartsWith("/"))
            template = "/" + template;//如果路由名称不是/开头则加上/防止控制器没写
        if (template.Contains("[controller]"))
        {
            controllerName = controllerName.Replace("Controller", "");//去掉Controller
            controllerName = StringHelper.FirstCharToLower(controllerName);//转首字母小写写
            template = template.Replace("[controller]", controllerName);//替换[controller]
        }

        return template;
    }

    /// <summary>
    /// 获取资源所有下级
    /// </summary>
    /// <param name="resourceList">资源列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns></returns>
    public List<MobileResource> GetResourceChilden(List<MobileResource> resourceList, long parentId)
    {
        //找下级资源ID列表
        var resources = resourceList.Where(it => it.ParentId == parentId).ToList();
        if (resources.Count > 0)//如果数量大于0
        {
            var data = new List<MobileResource>();
            foreach (var item in resources)//遍历资源
            {
                var res = GetResourceChilden(resourceList, item.Id);
                data.AddRange(res);//添加子节点;
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<MobileResource>();
    }

    /// <summary>
    /// 获取上级
    /// </summary>
    /// <param name="resourceList"></param>
    /// <param name="parentId"></param>
    /// <returns></returns>
    public List<MobileResource> GetResourceParent(List<MobileResource> resourceList, long parentId)
    {
        //找上级资源ID列表
        var resources = resourceList.Where(it => it.Id == parentId).FirstOrDefault();
        if (resources != null)//如果数量大于0
        {
            var data = new List<MobileResource>();
            var parents = GetResourceParent(resourceList, resources.ParentId.Value);
            data.AddRange(parents);//添加子节点;
            data.Add(resources);//添加到列表
            return data;//返回结果
        }
        return new List<MobileResource>();
    }

    /// <summary>
    /// 获取授权菜单
    /// </summary>
    /// <param name="moduleId">模块ID</param>
    /// <returns></returns>
    public async Task<List<ResTreeSelector.RoleGrantResourceMenu>> GetRoleGrantResourceMenus(long moduleId)
    {
        var roleGrantResourceMenus = new List<ResTreeSelector.RoleGrantResourceMenu>();//定义结果
        var allMenuList = (await GetListByCategory(CateGoryConst.RESOURCE_MENU)).Where(it => it.Module == moduleId).ToList();//获取所有菜单列表
        var allButtonList = await GetListByCategory(CateGoryConst.RESOURCE_BUTTON);//获取所有按钮列表
        var parentMenuList = allMenuList.Where(it => it.ParentId == SimpleAdminConst.ZERO).ToList();//获取一级目录

        //遍历一级目录
        foreach (var parent in parentMenuList)
        {
            //如果是目录则去遍历下级
            if (parent.MenuType == SysResourceConst.CATALOG)
            {
                //获取所有下级菜单
                var menuList = GetChildListById(allMenuList, parent.Id, false);
                if (menuList.Count > 0)//如果有菜单
                {
                    //遍历下级菜单
                    foreach (var menu in menuList)
                    {
                        //如果菜单类型是菜单
                        if (menu.MenuType == SysResourceConst.MENU)
                        {
                            //获取菜单下按钮集合并转换成对应实体
                            var buttonList = allButtonList.Where(it => it.ParentId == menu.Id).ToList();
                            var buttons = buttonList.Adapt<List<ResTreeSelector.RoleGrantResourceButton>>();
                            roleGrantResourceMenus.Add(new ResTreeSelector.RoleGrantResourceMenu
                            {
                                Id = menu.Id,
                                ParentId = parent.Id,
                                ParentName = parent.Title,
                                Module = moduleId,
                                Title = GetRoleGrantResourceMenuTitle(menuList, menu),//菜单名称需要特殊处理因为有二级菜单
                                Button = buttons
                            });
                        }
                        else if (menu.MenuType == SysResourceConst.LINK || menu.MenuType == SysResourceConst.IFRAME)//如果是内链或者外链
                        {
                            //直接加到资源列表
                            roleGrantResourceMenus.Add(new ResTreeSelector.RoleGrantResourceMenu
                            {
                                Id = menu.Id,
                                ParentId = parent.Id,
                                ParentName = parent.Title,
                                Module = moduleId,
                                Title = menu.Title
                            });
                        }
                    }
                }
                else
                {
                    //否则就将自己加到一级目录里面
                    roleGrantResourceMenus.Add(new ResTreeSelector.RoleGrantResourceMenu
                    {
                        Id = parent.Id,
                        ParentId = parent.Id,
                        ParentName = parent.Title,
                        Module = moduleId
                    });
                }
            }
            else
            {
                //就将自己加到一级目录里面
                var roleGrantResourcesButtons = new ResTreeSelector.RoleGrantResourceMenu
                {
                    Id = parent.Id,
                    ParentId = parent.Id,
                    ParentName = parent.Title,
                    Module = moduleId,
                    Title = parent.Title
                };
                //如果菜单类型是菜单
                if (parent.MenuType == SysResourceConst.MENU)
                {
                    //获取菜单下按钮集合并转换成对应实体
                    var buttonList = allButtonList.Where(it => it.ParentId == parent.Id).ToList();
                    roleGrantResourcesButtons.Button = buttonList.Adapt<List<ResTreeSelector.RoleGrantResourceButton>>();
                }
                roleGrantResourceMenus.Add(roleGrantResourcesButtons);
            }
        }
        return roleGrantResourceMenus;
    }

    /// <summary>
    /// 获取授权菜单类菜单名称
    /// </summary>
    /// <param name="menuList">菜单列表</param>
    /// <param name="menu">当前菜单</param>
    /// <returns></returns>
    private string GetRoleGrantResourceMenuTitle(List<MobileResource> menuList, MobileResource menu)
    {
        //查找菜单上级
        var parentList = GetResourceParent(menuList, menu.ParentId.Value);
        //如果有父级菜单
        if (parentList.Count > 0)
        {
            var titles = parentList.Select(it => it.Title).ToList();//提取出父级的name
            var title = string.Join("- ", titles) + $"-{menu.Title}";//根据-分割,转换成字符串并在最后加上菜单的title
            return title;
        }
        else
        {
            return menu.Title;//原路返回
        }
    }

    #endregion 方法
}
