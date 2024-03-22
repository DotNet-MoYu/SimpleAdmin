// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using Microsoft.Extensions.Logging;
using NewLife.Serialization;

namespace SimpleAdmin.Application;

/// <inheritdoc cref="IRoleService"/>
public class RoleService : IRoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly ISysUserService _sysUserService;
    private readonly ISysRoleService _sysRoleService;
    private readonly IResourceService _resourceService;
    private readonly IRelationService _relationService;

    public RoleService(ILogger<RoleService> logger, ISysUserService sysUserService, ISysRoleService sysRoleService,
        IResourceService resourceService, IRelationService relationService)
    {
        _logger = logger;
        _sysUserService = sysUserService;
        _sysRoleService = sysRoleService;
        _resourceService = resourceService;
        _relationService = relationService;
    }

    #region 查询

    /// <inheritdoc/>
    public async Task<RoleOwnResourceOutput> OwnResource(BaseIdInput input, string category)
    {
        return await _sysRoleService.OwnResource(input, category);
    }

    /// <inheritdoc/>
    public async Task<List<UserSelectorOutPut>> OwnUser(BaseIdInput input)
    {
        return await _sysRoleService.OwnUser(input);
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysRole>> Page(RolePageInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        input.OrgIds = dataScope;
        input.Category = CateGoryConst.ROLE_ORG;//只查询机构角色
        //分页查询
        var pageInfo = await _sysRoleService.Page(input);
        return pageInfo;
    }


    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<RoleSelectorOutPut>> RoleSelector(RoleSelectorInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        input.OrgIds = dataScope;
        return await _sysRoleService.RoleSelector(input);
    }

    /// <inheritdoc/>
    public async Task<List<RoleTreeOutput>> Tree(RoleTreeInput input)
    {
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        input.OrgIds = dataScope;
        return await _sysRoleService.Tree(input);
    }

    /// <inheritdoc/>
    public async Task<SysRole> Detail(BaseIdInput input)
    {
        var role = await _sysRoleService.Detail(input);
        var errorMessage = "您没有权限查看该角色";
        await _sysUserService.CheckApiDataScope(role.OrgId, role.CreateUserId, errorMessage);
        return role;
    }

    /// <inheritdoc/>
    public async Task<List<ResTreeSelector>> ResourceTreeSelector()
    {
        var resourceTreeSelectors = new List<ResTreeSelector>();//定义结果
        var user = await _sysUserService.GetUserById(UserManager.UserId);//获取用户
        //获取角色资源关系
        var relations =
            await _relationService.GetRelationListByObjectIdListAndCategory(user.RoleIdList, CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);
        var menuIds = new HashSet<long>();//菜单ID集合
        var buttonIds = new HashSet<long>();//按钮ID集合
        relations.ForEach(it =>
        {
            var resources = it.ExtJson.ToJsonEntity<RelationRoleResource>();//转换为对象获取资源
            menuIds.Add(resources.MenuId);//添加菜单ID
            resources.ButtonInfo.ForEach(b => buttonIds.Add(b));//添加按钮ID
        });
        var menuList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);//获取所有菜单
        var buttonList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_BUTTON);//获取所有按钮
        var moduleList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MODULE);//获取所有模块
        var ownMenuList = menuList.Where(it => menuIds.Contains(it.Id)).ToList();//已拥有的菜单
        var parentMenuList = menuList.Where(it => ownMenuList.Select(m => m.ParentId).Contains(it.Id)).ToList();//获取所有父级菜单 
        ownMenuList.AddRange(parentMenuList);//将父级菜单加入到拥有菜单列表
        var ownButtonList = buttonList.Where(it => buttonIds.Contains(it.Id)).ToList();//已拥有的按钮
        var moduleIds = ownMenuList.Select(it => it.Module.GetValueOrDefault()).Distinct().ToList();//模块ID集合
        var ownModuleList = moduleList.Where(it => moduleIds.Contains(it.Id)).ToList();//已拥有的模块
        //遍历模块
        foreach (var module in ownModuleList)
        {
            //将实体转换为ResourceTreeSelectorOutPut获取基本信息
            var resourceTreeSelector = module.Adapt<ResTreeSelector>();
            resourceTreeSelector.Menu = await GetRoleGrantResourceMenus(module.Id, ownMenuList, ownButtonList);//获取授权菜单
            resourceTreeSelectors.Add(resourceTreeSelector);
        }
        return resourceTreeSelectors;
    }

    #endregion

    #region 增加

    /// <inheritdoc/>
    public async Task Add(RoleAddInput input)
    {
        await CheckInput(input);//检查参数
        await _sysRoleService.Add(input);
    }

    #endregion

    #region 编辑

    /// <inheritdoc/>
    public async Task Edit(RoleEditInput input)
    {
        await CheckInput(input);//检查参数
        await _sysRoleService.Edit(input);
    }

    /// <inheritdoc/>
    public async Task GrantResource(GrantResourceInput input)
    {
        await _sysRoleService.GrantResource(input);
    }

    /// <inheritdoc/>
    public async Task GrantUser(GrantUserInput input)
    {
        await _sysRoleService.GrantUser(input);
    }

    #endregion

    #region 删除

    /// <inheritdoc/>
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        //获取要删除的机构列表
        var roleList = (await _sysRoleService.GetListAsync()).Where(it => ids.Contains(it.Id)).ToList();
        if (roleList.Any(it => it.Category == CateGoryConst.ROLE_GLOBAL))//如果有全局角色
            throw Oops.Bah("不能删除全局角色");
        //检查数据范围
        var orgIds = roleList.Select(it => it.OrgId.GetValueOrDefault()).ToList();
        var createUserIds = roleList.Select(it => it.CreateUserId.GetValueOrDefault()).ToList();
        await _sysUserService.CheckApiDataScope(orgIds, createUserIds, "您没有权限删除这些角色");
        await _sysRoleService.Delete(input);
    }

    #endregion

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysRole"></param>
    private async Task CheckInput(SysRole sysRole)
    {
        sysRole.Category = CateGoryConst.ROLE_ORG;
        if (sysRole.DefaultDataScope.ScopeCategory == CateGoryConst.SCOPE_ALL)
            throw Oops.Bah("不能添加全局数据范围的角色");
        await _sysUserService.CheckApiDataScope(sysRole.OrgId, sysRole.CreateUserId.GetValueOrDefault(), "您没有权限添加该角色");
    }

    /// <summary>
    /// 获取授权菜单
    /// </summary>
    /// <param name="moduleId">模块Id</param>
    /// <param name="ownMenuList">拥有菜单列表</param>
    /// <param name="ownButtonList">拥有权限按钮列表</param>
    /// <returns></returns>
    public async Task<List<ResTreeSelector.RoleGrantResourceMenu>> GetRoleGrantResourceMenus(long moduleId, List<SysResource> ownMenuList,
        List<SysResource> ownButtonList)
    {
        var roleGrantResourceMenus = new List<ResTreeSelector.RoleGrantResourceMenu>();//定义结果
        var parentMenuList = ownMenuList.Where(it => it.ParentId == SimpleAdminConst.ZERO).ToList();//获取一级目录
        //遍历一级目录
        foreach (var parent in parentMenuList)
        {
            //如果是目录则去遍历下级
            if (parent.MenuType == SysResourceConst.CATALOG)
            {
                //获取所有下级菜单
                var menuList = _resourceService.GetChildListById(ownMenuList, parent.Id, false);
                if (menuList.Count > 0)//如果有菜单
                {
                    //遍历下级菜单
                    foreach (var menu in menuList)
                    {
                        //如果菜单类型是菜单
                        if (menu.MenuType is SysResourceConst.MENU or SysResourceConst.SUBSET)
                        {
                            //获取菜单下按钮集合并转换成对应实体
                            var buttonList = ownButtonList.Where(it => it.ParentId == menu.Id).ToList();
                            var buttons = buttonList.Adapt<List<ResTreeSelector.RoleGrantResourceButton>>();
                            roleGrantResourceMenus.Add(new ResTreeSelector.RoleGrantResourceMenu
                            {
                                Id = menu.Id,
                                ParentId = parent.Id,
                                ParentName = parent.Title,
                                Module = moduleId,
                                Title = _resourceService.GetRoleGrantResourceMenuTitle(menuList, menu),//菜单名称需要特殊处理因为有二级菜单
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
                    var buttonList = ownButtonList.Where(it => it.ParentId == parent.Id).ToList();
                    roleGrantResourcesButtons.Button = buttonList.Adapt<List<ResTreeSelector.RoleGrantResourceButton>>();
                }
                roleGrantResourceMenus.Add(roleGrantResourcesButtons);
            }
        }
        return roleGrantResourceMenus;
    }

    #endregion
}
