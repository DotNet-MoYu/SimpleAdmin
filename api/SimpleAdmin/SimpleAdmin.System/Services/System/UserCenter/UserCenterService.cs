using Masuit.Tools;

namespace SimpleAdmin.System;

/// <inheritdoc cref="IUserCenterService"/>
public class UserCenterService : DbRepository<SysUser>, IUserCenterService
{
    private readonly ISysUserService _userService;
    private readonly IRelationService _relationService;
    private readonly IResourceService _resourceService;
    private readonly IMenuService _menuService;
    private readonly IConfigService _configService;

    public UserCenterService(ISysUserService userService,
                             IRelationService relationService,
                             IResourceService resourceService,
                             IMenuService menuService,
                             IConfigService configService)
    {
        _userService = userService;
        _relationService = relationService;
        _resourceService = resourceService;
        this._menuService = menuService;
        _configService = configService;
    }

    /// <inheritdoc />
    public async Task<List<SysResource>> GetOwnMenu()
    {
        var result = new List<SysResource>();
        //获取用户信息
        var userInfo = await _userService.GetUserByAccount(UserManager.UserAccount);
        if (userInfo != null)
        {
            //获取角色所拥有的资源集合
            var resourceList = await _relationService.GetRelationListByObjectIdListAndCategory(userInfo.RoleIdList, CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);
            //定义菜单ID列表
            HashSet<long> menuIdList = new HashSet<long>();

            //获取菜单集合
            menuIdList.AddRange(resourceList.Select(r => r.TargetId.ToLong()).ToList());

            //获取所有的菜单和模块以及单页面列表，并按分类和排序码排序
            var allModuleAndMenuAndSpaList = await _resourceService.GetaModuleAndMenuAndSpaList();
            List<SysResource> allModuleList = new List<SysResource>();//模块列表
            List<SysResource> allMenuList = new List<SysResource>();//菜单列表
            List<SysResource> allSpaList = new List<SysResource>();//单页列表
            //遍历菜单集合
            allModuleAndMenuAndSpaList.ForEach(it =>
            {
                switch (it.Category)
                {
                    case CateGoryConst.Resource_MODULE://模块
                        it.Name = it.Title;//设置Name等于title
                        allModuleList.Add(it);//添加到模块列表
                        break;
                    case CateGoryConst.Resource_MENU://菜单
                        allMenuList.Add(it);//添加到菜单列表
                        break;
                    case CateGoryConst.Resource_SPA://单页
                        allSpaList.Add(it);//添加到单页列表
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
            var myModules = GetMyModules(allModuleList, moduleIds, allSpaList.Count);
            myMenus.AddRange(myModules);//模块添加到菜单列表
            // 遍历单页列表
            allSpaList.ForEach(it =>
             {
                 // 将第一个模块作为所有单页面的所属模块，并添加
                 var firstModuleId = myModules[0].Id;
                 it.ParentId = firstModuleId;
                 it.Module = firstModuleId;
             });

            myMenus.AddRange(allSpaList);//但也添加到菜单

            //构建meta
            ConstructMeta(myMenus, allSpaList[0].Id);
            //构建菜单树
            result = _menuService.ConstructMenuTrees(myMenus);

        }
        return result;
    }

    /// <inheritdoc />
    public async Task<string> GetLoginWorkbench()
    {
        //获取个人工作台信息
        var sysRelation = await _relationService.GetWorkbench(UserManager.UserId);
        if (sysRelation != null)
        {
            //如果有数据直接返回个人工作台
            return sysRelation.ExtJson.ToLower();
        }
        else
        {
            //如果没数据去系统配置里取默认的工作台
            var devConfig = await _configService.GetByConfigKey(CateGoryConst.Config_SYS_BASE, DevConfigConst.SYS_DEFAULT_WORKBENCH_DATA_KEY);
            if (devConfig != null)
            {
                return devConfig.ConfigValue.ToLower();//返回工作台信息
            }
            else
            {
                return "";
            }

        }
    }

    /// <inheritdoc />
    public async Task UpdateUserInfo(UserUpdateInfoInput input)
    {
        //更新指定字段
        await UpdateAsync(it => new SysUser
        {
            Name = input.Name,
            Email = input.Email,
            Phone = input.Phone,
            Nickname = input.Nickname,
            Gender = input.Gender,
            Birthday = input.Birthday,
            Signature = input.Signature
        }, it => it.Id == UserManager.UserId);
    }


    #region 方法
    /// <summary>
    /// 获取父菜单集合
    /// </summary>
    /// <param name="allMenuList">所有菜单列表</param>
    /// <param name="myMenus">我的菜单列表</param>
    /// <returns></returns>
    private List<SysResource> GetMyParentMenus(List<SysResource> allMenuList, List<SysResource> myMenus)
    {
        var parentList = new List<SysResource>();
        myMenus.ForEach(it =>
        {
            //找到父ID对应的菜单
            var parent = allMenuList.Where(r => r.Id == it.ParentId.Value).FirstOrDefault();
            if (parent != null && !parentList.Contains(parent))//如果不为空且夫列表里没有
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
    private List<SysResource> GetMyModules(List<SysResource> allModuleList, List<long> moduleIds, int spaCount)
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
                    return new List<SysResource>();
                }
                else
                {
                    // 否则构造一个模块，并添加到拥有模块
                    SysResource sysResource = new SysResource();
                    sysResource.Id = YitIdHelper.NextId();
                    sysResource.Path = "/" + RandomHelper.CreateRandomString(10);
                    sysResource.Category = CateGoryConst.Resource_MODULE;
                    allModuleList.Add(sysResource);
                    myModules.Add(sysResource);

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
    /// <param name="firstSpaId">第一个单页面ID</param>
    private void ConstructMeta(List<SysResource> myMenus, long firstSpaId)
    {
        myMenus.ForEach(it =>
        {
            // 将模块的父id设置为0，设置随机path
            if (it.Category == CateGoryConst.Resource_MODULE)
            {
                it.ParentId = 0;
                it.Path = "/" + RandomHelper.CreateRandomString(10);
            }
            // 将根菜单的父id设置为模块的id
            if (it.Category == CateGoryConst.Resource_MENU)
            {
                if (it.ParentId == 0)
                {
                    it.ParentId = it.Module;
                }

            }
            //定义meta
            Meta meta = new Meta { Icon = it.Icon, Title = it.Title, Type = it.Category.ToLower() };
            // 如果是菜单，则设置type菜单类型为小写
            if (it.Category == CateGoryConst.Resource_MENU)
            {
                if (it.MenuType != ResourceConst.CATALOG)//菜单类型不是目录
                {
                    meta.Type = it.MenuType.ToLower();
                }

            }
            // 如果是单页面
            if (it.Category == CateGoryConst.Resource_SPA)
            {
                meta.Type = ResourceConst.MENU.ToLower();//类型等于菜单
                if (it.Id == firstSpaId)
                {
                    // 如果是首页（第一个单页面）则设置affix
                    meta.Affix = true;
                }
                else
                {
                    // 否则隐藏该单页面
                    meta.hidden = true;
                }
            }
            it.Meta = meta;
        });
    }

    #endregion

}
