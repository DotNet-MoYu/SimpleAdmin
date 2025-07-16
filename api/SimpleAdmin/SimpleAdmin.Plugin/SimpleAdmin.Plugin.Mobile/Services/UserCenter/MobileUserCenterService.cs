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

/// <inheritdoc cref="IMobileUserCenterService"/>
public class MobileUserCenterService : IMobileUserCenterService
{
    private readonly IMobileMenuService _menuService;
    private readonly IAuthService _authService;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IRelationService _relationService;
    private readonly IMobileResourceService _resourceService;
    private readonly ISysUserService _userService;

    public MobileUserCenterService(IRelationService relationService, IMobileResourceService resourceService, ISysUserService userService,
        IMobileMenuService menuService, IAuthService authService, ISimpleCacheService simpleCacheService)
    {
        _relationService = relationService;
        _resourceService = resourceService;
        _userService = userService;
        _menuService = menuService;
        _authService = authService;
        _simpleCacheService = simpleCacheService;
    }

    /// <inheritdoc />
    public async Task<List<MobileResource>> GetOwnMobileMenu()
    {
        var result = new List<MobileResource>();
        //获取用户信息
        var userInfo = await _userService.GetUserByAccount(UserManager.UserAccount);
        if (userInfo != null)
        {
            //获取用户所拥有的资源集合
            var resourceList =
                await _relationService.GetRelationListByObjectIdAndCategory(userInfo.Id, MobileConst.RELATION_SYS_USER_HAS_MOBILE_RESOURCE);
            if (resourceList.Count == 0)//如果没有就获取角色的
                //获取角色所拥有的资源集合
                resourceList = await _relationService.GetRelationListByObjectIdListAndCategory(userInfo.RoleIdList,
                    MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE);
            //定义菜单ID列表
            var menuIdList = new HashSet<long>();
            //获取菜单集合
            menuIdList.AddRange(resourceList.Select(r => r.TargetId.ToLong()).ToList());
            //获取所有的菜单和模块以及单页面列表，并按分类和排序码排序
            var allModuleAndMenuAndSpaList = await _resourceService.GetaModuleAndMenuAndSpaList();
            var allModuleList = new List<MobileResource>();//模块列表
            var allMenuList = new List<MobileResource>();//菜单列表
            //遍历菜单集合
            allModuleAndMenuAndSpaList.ForEach(it =>
            {
                switch (it.Category)
                {
                    case CateGoryConst.RESOURCE_MODULE://模块
                        allModuleList.Add(it);//添加到模块列表
                        break;
                    case CateGoryConst.RESOURCE_MENU://菜单
                        allMenuList.Add(it);//添加到菜单列表
                        break;
                }
            });
            //获取我的菜单列表
            var myMenus = allMenuList.Where(it => menuIdList.Contains(it.Id)).ToList();
            // 对获取到的角色对应的菜单列表进行处理，获取父列表
            var parentList = GetMyParentMenus(allMenuList, myMenus);
            myMenus.AddRange(parentList);//合并列表
            //获取我的模块信息Id列表
            var moduleIds = myMenus.Select(it => it.Module.Value).Distinct().ToList();
            //获取我的模块集合
            var myModules = GetMyModules(allModuleList, moduleIds, 0);
            myMenus.AddRange(myModules);//模块添加到菜单列表
            //构建meta
            ConstructMeta(myMenus);
            //构建菜单树
            result = _menuService.ConstructMenuTrees(myMenus);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<MobileLoginUserOutput> GetLoginUser()
    {
        var user = await _userService.GetUserById(UserManager.UserId);
        if (user != null)
            user.Avatar = await _userService.GetUserAvatar(user.Id);//获取头像
        var loginUser = user.Adapt<MobileLoginUserOutput>();
        loginUser.MobileButtonCodeList = await GetButtonCodeList(UserManager.UserId);//获取移动端按钮码集合
        return loginUser;
    }



    #region 方法

    public async Task<List<string>> GetButtonCodeList(long userId)
    {
        var buttonCodeList = new List<string>();//按钮ID集合
        //获取用户移动资源集合
        var resourceList = await _relationService.GetRelationListByObjectIdAndCategory(userId, MobileConst.RELATION_SYS_USER_HAS_MOBILE_RESOURCE);
        var buttonIdList = new List<long>();//按钮ID集合
        if (resourceList.Count == 0)//如果有表示用户单独授权了不走用户角色
        {
            //获取用户角色关系集合
            var roleList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.RELATION_SYS_USER_HAS_ROLE);
            var roleIdList = roleList.Select(x => x.TargetId.ToLong()).ToList();//角色ID列表
            if (roleIdList.Count > 0)//如果该用户有角色
            {
                resourceList = await _relationService.GetRelationListByObjectIdListAndCategory(roleIdList,
                    MobileConst.RELATION_SYS_ROLE_HAS_MOBILE_RESOURCE);//获取移动端资源集合
            }
        }
        resourceList.ForEach(it =>
        {
            if (!string.IsNullOrEmpty(it.ExtJson))
                buttonIdList.AddRange(it.ExtJson.ToJsonEntity<RelationRoleResource>().ButtonInfo);//如果有按钮权限，将按钮ID放到buttonIdList
        });
        if (buttonIdList.Count > 0)
        {
            buttonCodeList = await _resourceService.GetCodeByIds(buttonIdList, CateGoryConst.RESOURCE_BUTTON);
        }
        return buttonCodeList;
    }


    /// <summary>
    /// 获取父菜单集合
    /// </summary>
    /// <param name="allMenuList">所有菜单列表</param>
    /// <param name="myMenus">我的菜单列表</param>
    /// <returns></returns>
    private List<MobileResource> GetMyParentMenus(List<MobileResource> allMenuList, List<MobileResource> myMenus)
    {
        var parentList = new List<MobileResource>();
        myMenus.ForEach(it =>
        {
            //找到父ID对应的菜单
            var parent = allMenuList.Where(r => r.Id == it.ParentId.Value).FirstOrDefault();
            if (parent != null && !parentList.Contains(parent) && !myMenus.Contains(parent))//如果不为空且两个列表里没有
            {
                parentList.Add(parent);//添加到父列表
            }
        });
        return parentList;
    }

    /// <summary>
    /// 获取我的模块集合
    /// </summary>
    /// <param name="allModuleList"></param>
    /// <param name="moduleIds"></param>
    /// <param name="spaCount"></param>
    /// <returns></returns>
    private List<MobileResource> GetMyModules(List<MobileResource> allModuleList, List<long> moduleIds, int spaCount)
    {
        //获取我的模块信息
        var myModules = allModuleList.Where(it => moduleIds.Contains(it.Id)).ToList();
        // 如果一个模块都没拥有
        if (myModules.Count == 0)
        {
            // 如果系统中无模块（极端情况）
            if (allModuleList.Count == 0)
            {
                if (spaCount == 0)// 如果系统中无单页面，则返回空列表
                {
                    return new List<MobileResource>();
                }
                else
                {
                    // 否则构造一个模块，并添加到拥有模块
                    var mobileResource = new MobileResource();
                    mobileResource.Id = CommonUtils.GetSingleId();
                    mobileResource.Path = "/" + RandomHelper.CreateRandomString(10);
                    mobileResource.Category = CateGoryConst.RESOURCE_MODULE;
                    allModuleList.Add(mobileResource);
                    myModules.Add(mobileResource);
                }
            }
            else
            {
                // 否则将系统中第一个模块作为拥有的模块
                myModules.Add(allModuleList[0]);
            }
        }
        return myModules;
    }

    /// <summary>
    /// 构建Meta
    /// </summary>
    /// <param name="myMenus">我的菜单集合</param>
    private void ConstructMeta(List<MobileResource> myMenus)
    {
        // 遍历myMenus列表
        myMenus.ForEach(it =>
        {
            // 将模块的父id设置为0，设置随机path
            if (it.Category == CateGoryConst.RESOURCE_MODULE)
            {
                it.ParentId = 0;
                it.Path = "/" + RandomHelper.CreateRandomString(10);
            }
            // 将根菜单的父id设置为模块的id
            if (it.Category == CateGoryConst.RESOURCE_MENU)
            {
                if (it.ParentId == SimpleAdminConst.ZERO)
                {
                    it.ParentId = it.Module;
                }
            }
            //定义meta
            var meta = new MobleMeta { Icon = it.Icon, Title = it.Title, Type = it.Category.ToLower() };
            // 如果是菜单，则设置type菜单类型为小写
            if (it.Category == CateGoryConst.RESOURCE_MENU)
            {
                if (it.MenuType != SysResourceConst.CATALOG)//菜单类型不是目录
                {
                    meta.Type = it.MenuType.ToLower();
                }
            }
            it.Meta = meta;
        });
    }

    #endregion
}
