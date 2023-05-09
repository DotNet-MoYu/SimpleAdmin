namespace SimpleAdmin.System;

/// <inheritdoc cref="IUserCenterService"/>
public class UserCenterService : DbRepository<SysUser>, IUserCenterService
{
    private readonly ISysUserService _userService;
    private readonly IRelationService _relationService;
    private readonly IResourceService _resourceService;
    private readonly IMenuService _menuService;
    private readonly IConfigService _configService;
    private readonly ISysOrgService _sysOrgService;
    private readonly IMessageService _messageService;

    public UserCenterService(ISysUserService userService,
                             IRelationService relationService,
                             IResourceService resourceService,
                             IMenuService menuService,
                             IConfigService configService,
                             ISysOrgService sysOrgService,
                             IMessageService messageService)
    {
        _userService = userService;
        _relationService = relationService;
        _resourceService = resourceService;
        this._menuService = menuService;
        _configService = configService;
        this._sysOrgService = sysOrgService;
        this._messageService = messageService;
    }

    #region 查询

    /// <inheritdoc />
    public async Task<List<SysResource>> GetOwnMenu()
    {
        var result = new List<SysResource>();
        //获取用户信息
        var userInfo = await _userService.GetUserByAccount(UserManager.UserAccount);
        if (userInfo != null)
        {
            //获取用户所拥有的资源集合
            var resourceList = await _relationService.GetRelationListByObjectIdAndCategory(userInfo.Id, CateGoryConst.Relation_SYS_USER_HAS_RESOURCE);
            if (resourceList.Count == 0)//如果没有就获取角色的
                //获取角色所拥有的资源集合
                resourceList = await _relationService.GetRelationListByObjectIdListAndCategory(userInfo.RoleIdList, CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);
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
            var devConfig = await _configService.GetByConfigKey(CateGoryConst.Config_SYS_BASE, DevConfigConst.SYS_DEFAULT_WORKBENCH_DATA);
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
    public async Task<List<LoginOrgTreeOutput>> LoginOrgTree()
    {
        var orgList = await _sysOrgService.GetListAsync();//获取全部机构
        var parentOrgs = _sysOrgService.GetOrgParents(orgList, UserManager.OrgId);//获取父节点列表
        var topOrg = parentOrgs.Where(it => it.ParentId == SimpleAdminConst.Zero).FirstOrDefault();//获取顶级节点
        if (topOrg != null)
        {
            var orgs = await _sysOrgService.GetChildListById(topOrg.Id);//获取下级
            var orgTree = ConstrucOrgTrees(orgs, 0, UserManager.OrgId);//获取组织架构
            return orgTree;
        }
        return new List<LoginOrgTreeOutput>();
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<DevMessage>> LoginMessagePage(MessagePageInput input)
    {
        var messages = await _messageService.MyMessagePage(input, UserManager.UserId);//分页查询
        return messages;
    }

    /// <inheritdoc />
    public async Task<MessageDetailOutPut> LoginMessageDetail(BaseIdInput input)
    {
        return await _messageService.Detail(input, true);//返回详情，不带用户列表
    }

    /// <inheritdoc />
    public async Task<int> UnReadCount()
    {
        return await _messageService.UnReadCount(UserManager.UserId);
    }

    #endregion 查询

    #region 编辑

    /// <inheritdoc />
    public async Task UpdateUserInfo(UpdateInfoInput input)
    {
        //如果手机号不是空
        if (!string.IsNullOrEmpty(input.Phone))
        {
            if (!input.Phone.MatchPhoneNumber())//判断是否是手机号格式
                throw Oops.Bah($"手机号码格式错误");
            input.Phone = CryptogramUtil.Sm4Encrypt(input.Phone);
            var any = await IsAnyAsync(it => it.Phone == input.Phone && it.Id != UserManager.UserId);//判断是否有重复的
            if (any)
                throw Oops.Bah($"系统已存在该手机号");
        }
        if (!string.IsNullOrEmpty(input.Email))
        {
            var match = input.Email.MatchEmail();
            if (!match.isMatch)
                throw Oops.Bah($"邮箱格式错误");
        }

        //更新指定字段
        var result = await UpdateAsync(it => new SysUser
        {
            Name = input.Name,
            Email = input.Email,
            Phone = input.Phone,
            Nickname = input.Nickname,
            Gender = input.Gender,
            Birthday = input.Birthday,
        }, it => it.Id == UserManager.UserId);
        if (result)
            _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
    }

    /// <inheritdoc />
    public async Task UpdateSignature(UpdateSignatureInput input)
    {
        var user = await _userService.GetUserById(UserManager.UserId);//获取信息
        var signatureArray = input.Signature.Split(",");//分割
        var base64String = signatureArray[1];//根据逗号分割取到base64字符串
        var image = base64String.GetSKBitmapFromBase64();//转成图片
        var resizeImage = image.ResizeImage(100, 50);//重新裁剪
        var newBase64String = resizeImage.ImgToBase64String();//重新转为base64
        var newSignature = signatureArray[0] + "," + newBase64String;//赋值新的签名

        //更新签名
        var result = await UpdateAsync(it => new SysUser
        {
            Signature = newSignature
        }, it => it.Id == UserManager.UserId);
        if (result)
            _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
    }

    /// <inheritdoc />
    public async Task UpdateWorkbench(UpdateWorkbenchInput input)
    {
        //关系表保存个人工作台
        await _relationService.SaveRelation(CateGoryConst.Relation_SYS_USER_WORKBENCH_DATA, UserManager.UserId, null, input.WorkbenchData, true);
    }

    /// <inheritdoc />
    public async Task DeleteMyMessage(BaseIdInput input)
    {
        await _messageService.DeleteMyMessage(input, UserManager.UserId);
    }

    /// <inheritdoc />
    public async Task UpdatePassword(UpdatePasswordInput input)
    {
        //获取用户信息
        var userInfo = await _userService.GetUserById(UserManager.UserId);
        var password = CryptogramUtil.Sm2Decrypt(input.Password);//SM2解密
        if (userInfo.Password != password) throw Oops.Bah("原密码错误");
        var newPassword = CryptogramUtil.Sm2Decrypt(input.NewPassword);//sm2解密
        newPassword = CryptogramUtil.Sm4Encrypt(newPassword);//SM4加密
        userInfo.Password = newPassword;
        await Context.Updateable(userInfo).UpdateColumns(it => new { it.Password }).ExecuteCommandAsync();//修改密码
        _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
    }

    /// <inheritdoc />
    public async Task<string> UpdateAvatar(BaseFileInput input)
    {
        var userInfo = await _userService.GetUserById(UserManager.UserId);

        var file = input.File;
        using var fileStream = file.OpenReadStream();//获取文件流
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);
        fileStream.Close();
        var base64String = Convert.ToBase64String(bytes);//转base64
        var avatar = base64String.ToImageBase64();//转图片
        userInfo.Avatar = avatar;
        await Context.Updateable(userInfo).UpdateColumns(it => new { it.Avatar }).ExecuteCommandAsync();//修改密码
        _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
        return avatar;
    }

    #endregion 编辑

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
                    sysResource.Id = CommonUtils.GetSingleId();
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
                if (it.ParentId == SimpleAdminConst.Zero)
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

    /// <summary>
    /// 构建机构树
    /// </summary>
    /// <param name="orgList">机构列表</param>
    /// <param name="parentId">父ID</param>
    /// <param name="orgId">用户ID</param>
    /// <returns></returns>
    public List<LoginOrgTreeOutput> ConstrucOrgTrees(List<SysOrg> orgList, long parentId, long orgId)
    {
        //找下级字典ID列表
        var orgs = orgList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (orgs.Count > 0)//如果数量大于0
        {
            var data = new List<LoginOrgTreeOutput>();
            foreach (var item in orgs)//遍历字典
            {
                var loginOrg = new LoginOrgTreeOutput
                {
                    Children = ConstrucOrgTrees(orgList, item.Id, orgId),//添加子节点
                    Id = item.Id,
                    Label = item.Name,
                    Pid = item.ParentId,
                    Style = orgId != item.Id ? null : new LoginOrgTreeOutput.MyStyle { }
                };
                data.Add(loginOrg);//添加到列表
            }
            return data;//返回结果
        }
        return new List<LoginOrgTreeOutput>();
    }

    #endregion 方法
}