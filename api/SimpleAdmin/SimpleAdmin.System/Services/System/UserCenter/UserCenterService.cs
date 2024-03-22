// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using System.Text.RegularExpressions;

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

    public UserCenterService(ISysUserService userService, IRelationService relationService, IResourceService resourceService,
        IMenuService menuService, IConfigService configService, ISysOrgService sysOrgService,
        IMessageService messageService)
    {
        _userService = userService;
        _relationService = relationService;
        _resourceService = resourceService;
        _menuService = menuService;
        _configService = configService;
        _sysOrgService = sysOrgService;
        _messageService = messageService;
    }

    #region 查询

    /// <inheritdoc />
    public async Task<List<SysResource>> GetLoginMenu(BaseIdInput input)
    {
        var result = new List<SysResource>();
        //获取用户信息
        var userInfo = await _userService.GetUserByAccount(UserManager.UserAccount, UserManager.TenantId);
        if (userInfo != null)
        {
            //定义菜单ID列表
            var menuIdList = new HashSet<long>();//获取所有的菜单和模块以及单页面列表，并按分类和排序码排序
            var allModuleAndMenuAndSpaList = await _resourceService.GetMenuAndSpaListByModuleId(input.Id);
            //通过 App.GetOptions<TOptions> 获取选项
            var settings = App.GetOptions<SystemSettingsOptions>();
            //如果设置了超级管理员可以看到全部
            if (settings.SuperAdminViewAllData && UserManager.SuperAdmin)
            {
                menuIdList = allModuleAndMenuAndSpaList.Select(it => it.Id).ToHashSet();
            }
            else
            {
                //获取用户所拥有的资源集合
                var resourceList =
                    await _relationService.GetRelationListByObjectIdAndCategory(userInfo.Id, CateGoryConst.RELATION_SYS_USER_HAS_RESOURCE);
                if (resourceList.Count == 0)//如果没有就获取角色的
                    //获取角色所拥有的资源集合
                    resourceList = await _relationService.GetRelationListByObjectIdListAndCategory(userInfo.RoleIdList,
                        CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);
                //获取菜单Id集合
                menuIdList.AddRange(resourceList.Select(r => r.TargetId.ToLong()).ToList());
            }
            var allMenuList = new List<SysResource>();//菜单列表
            var allSpaList = new List<SysResource>();//单页列表
            //遍历菜单集合
            allModuleAndMenuAndSpaList.ForEach(it =>
            {
                switch (it.Category)
                {
                    case CateGoryConst.RESOURCE_MENU://菜单
                        allMenuList.Add(it);//添加到菜单列表
                        break;

                    case CateGoryConst.RESOURCE_SPA://单页
                        allSpaList.Add(it);//添加到单页列表
                        break;
                }
            });
            //获取我的菜单列表,只显示启用的
            var myMenus = allMenuList.Where(it => menuIdList.Contains(it.Id) && it.Status == CommonStatusConst.ENABLE).ToList();
            // 对获取到的角色对应的菜单列表进行处理，获取父列表
            var parentList = GetMyParentMenus(allMenuList, myMenus);
            myMenus.AddRange(parentList);//合并列表
            // 遍历单页列表
            allSpaList.ForEach(it =>
            {
                it.ParentId = SimpleAdminConst.ZERO;
            });
            myMenus.AddRange(allSpaList);//单页添加到菜单
            //构建meta
            ConstructMeta(myMenus);
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
        //如果没数据去系统配置里取默认的工作台
        var devConfig = await _configService.GetByConfigKey(CateGoryConst.CONFIG_SYS_BASE, SysConfigConst.SYS_DEFAULT_WORKBENCH_DATA);
        if (devConfig != null)
        {
            return devConfig.ConfigValue.ToLower();//返回工作台信息
        }
        return "";
    }

    /// <inheritdoc />
    public async Task<List<LoginOrgTreeOutput>> LoginOrgTree()
    {
        var orgList = await _sysOrgService.GetListAsync();//获取全部机构
        var parentOrgList = _sysOrgService.GetOrgParents(orgList, UserManager.OrgId);//获取父节点列表
        var topOrg = parentOrgList.Where(it => it.ParentId == SimpleAdminConst.ZERO).FirstOrDefault();//获取顶级节点
        if (topOrg != null)
        {
            var orgChildList = await _sysOrgService.GetChildListById(topOrg.Id);//获取下级
            var orgTree = ConstrucOrgTrees(orgChildList, 0, UserManager.OrgId);//获取组织架构
            return orgTree;
        }
        return new List<LoginOrgTreeOutput>();
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<SysMessage>> LoginMessagePage(MessagePageInput input)
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

    /// <inheritdoc />
    public async Task<List<SysResource>> ShortcutTree()
    {
        var sysResourceList = await _resourceService.GetAllModuleAndMenuAndSpaList();//获取模块和菜单和单页列表
        var userInfo = await _userService.GetUserById(UserManager.UserId);
        var hasResourcesList = new HashSet<long>();//拥有的资源列表
        var hasModuleList = new HashSet<long>();//拥有的资源列表
        var userResourceList =
            (await _relationService.GetRelationListByObjectIdAndCategory(UserManager.UserId, CateGoryConst.RELATION_SYS_USER_HAS_RESOURCE))
            .Select(it => it.TargetId.ToLong()).ToList();//获取用户资源id列表
        var userModuleList =
            (await _relationService.GetRelationListByObjectIdAndCategory(UserManager.UserId, CateGoryConst.RELATION_SYS_USER_HAS_MODULE))
            .Select(it => it.TargetId.ToLong()).ToList();//获取用户模块id列表
        if (userResourceList.Count > 0)//如果用户资源列表大于0就以用户为主
        {
            hasResourcesList.AddRange(userResourceList);
            hasResourcesList.AddRange(userModuleList);
        }
        else
        {
            var roleIds = userInfo.RoleIdList;//获取角色ID列表
            var roleResourceList =
                (await _relationService.GetRelationListByObjectIdListAndCategory(roleIds, CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE))
                .Select(it => it.TargetId.ToLong()).ToList();//获取角色资源id列表
            var roleModuleList =
                (await _relationService.GetRelationListByObjectIdListAndCategory(roleIds, CateGoryConst.RELATION_SYS_ROLE_HAS_MODULE))
                .Select(it => it.TargetId.ToLong()).ToList();//获取角色模块id列表
            hasResourcesList.AddRange(roleResourceList);
            hasResourcesList.AddRange(roleModuleList);
        }

        // 获取目录
        var catalogList = sysResourceList.Where(it => it.MenuType == SysResourceConst.CATALOG).ToList();
        //过滤出拥有的资源列表
        sysResourceList =
            sysResourceList.Where(it =>
                    hasResourcesList.Contains(it.Id) || it.Category == CateGoryConst.RESOURCE_SPA)
                .ToList();//获取拥有的资源列表
        var parentIds = sysResourceList.Select(it => it.ParentId).Distinct().ToList();//获取父ID列表
        var parentList = catalogList.Where(it => parentIds.Contains(it.Id)).ToList();//获取所拥有的父级目录
        sysResourceList.AddRange(parentList);//添加到列表
        return await _menuService.ShortcutTree(sysResourceList);
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
                throw Oops.Bah("手机号码格式错误");
            input.Phone = CryptogramUtil.Sm4Encrypt(input.Phone);
            var any = await IsAnyAsync(it => it.Phone == input.Phone && it.Id != UserManager.UserId);//判断是否有重复的
            if (any)
                throw Oops.Bah("系统已存在该手机号");
        }
        if (!string.IsNullOrEmpty(input.Email))
        {
            var match = input.Email.MatchEmail();
            if (!match.isMatch)
                throw Oops.Bah("邮箱格式错误");
        }

        //更新指定字段
        var result = await UpdateSetColumnsTrueAsync(it => new SysUser
        {
            Name = input.Name,
            Email = input.Email,
            Phone = input.Phone,
            Nickname = input.Nickname,
            Gender = input.Gender,
            Birthday = input.Birthday
        }, it => it.Id == UserManager.UserId);
        if (result)
            _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
    }

    /// <inheritdoc />
    public async Task UpdateSignature(UpdateSignatureInput input)
    {
        var signatureArray = input.Signature.Split(",");//分割
        // var base64String = signatureArray[1];//根据逗号分割取到base64字符串
        // var image = base64String.GetSkBitmapFromBase64();//转成图片
        // var resizeImage = image.ResizeImage(100, 50);//重新裁剪
        // var newBase64String = resizeImage.ImgToBase64String();//重新转为base64
        // var newSignature = signatureArray[0] + "," + newBase64String;//赋值新的签名

        //更新签名
        var result = await UpdateSetColumnsTrueAsync(it => new SysUser
        {
            Signature = input.Signature
        }, it => it.Id == UserManager.UserId);
        if (result)
            _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
    }

    /// <inheritdoc />
    public async Task UpdateWorkbench(UpdateWorkbenchInput input)
    {
        //关系表保存个人工作台
        await _relationService.SaveRelation(CateGoryConst.RELATION_SYS_USER_WORKBENCH_DATA, UserManager.UserId, null, input.WorkbenchData,
            true);
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
        var loginPolicy = await _configService.GetConfigsByCategory(CateGoryConst.CONFIG_PWD_POLICY);//获取密码策略
        var containNumber = loginPolicy.First(it => it.ConfigKey == SysConfigConst.PWD_CONTAIN_NUM).ConfigValue.ToBoolean();//是否包含数字
        var containLower = loginPolicy.First(it => it.ConfigKey == SysConfigConst.PWD_CONTAIN_LOWER).ConfigValue.ToBoolean();//是否包含小写
        var containUpper = loginPolicy.First(it => it.ConfigKey == SysConfigConst.PWD_CONTAIN_UPPER).ConfigValue.ToBoolean();//是否包含大写
        var containChar = loginPolicy.First(it => it.ConfigKey == SysConfigConst.PWD_CONTAIN_CHARACTER).ConfigValue.ToBoolean();//是否包含特殊字符
        var minLength = loginPolicy.First(it => it.ConfigKey == SysConfigConst.PWD_MIN_LENGTH).ConfigValue.ToInt();//最小长度
        if (minLength > newPassword.Length)
            throw Oops.Bah($"密码长度不能小于{minLength}");
        if (containNumber && !Regex.IsMatch(newPassword, "[0-9]"))
            throw Oops.Bah("密码必须包含数字");
        if (containLower && !Regex.IsMatch(newPassword, "[a-z]"))
            throw Oops.Bah("密码必须包含小写字母");
        if (containUpper && !Regex.IsMatch(newPassword, "[A-Z]"))
            throw Oops.Bah("密码必须包含大写字母");
        if (containChar && !Regex.IsMatch(newPassword, "[~!@#$%^&*()_+`\\-={}|\\[\\]:\";'<>?,./]"))
            throw Oops.Bah("密码必须包含特殊字符");
        // var similarity = PwdUtil.Similarity(password, newPassword);
        // if (similarity > 80)
        //     throw Oops.Bah($"新密码请勿与旧密码过于相似");
        newPassword = CryptogramUtil.Sm4Encrypt(newPassword);//SM4加密
        userInfo.Password = newPassword;
        await UpdateSetColumnsTrueAsync(it => new SysUser() { Password = newPassword }, it => it.Id == userInfo.Id);//更新密码
        _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
    }

    /// <inheritdoc />
    public async Task<string> UpdateAvatar(BaseFileInput input)
    {
        var userInfo = await _userService.GetUserById(UserManager.UserId);

        var file = input.File;
        using var fileStream = file.OpenReadStream();//获取文件流
        var bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);
        fileStream.Close();
        var base64String = Convert.ToBase64String(bytes);//转base64
        var avatar = base64String.ToImageBase64();//转图片
        await UpdateSetColumnsTrueAsync(it => new SysUser() { Avatar = avatar }, it => it.Id == userInfo.Id);// 更新头像
        _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
        return avatar;
    }

    /// <inheritdoc />
    public async Task SetDefaultModule(SetDefaultModuleInput input)
    {
        //获取用户信息
        var userInfo = await _userService.GetUserById(UserManager.UserId);
        //如果是默认模块
        if (input.IfDefault)
            userInfo.DefaultModule = input.Id;
        else
            userInfo.DefaultModule = null;
        await Context.Updateable(userInfo).UpdateColumns(it => new { it.DefaultModule }).ExecuteCommandAsync();//修改默认模块
        _userService.DeleteUserFromRedis(UserManager.UserId);//redis删除用户数据
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
            var parents = _resourceService.GetResourceParent(allMenuList, it.ParentId.Value);//获取父级
            parents.ForEach(parent =>
            {
                // 如果父级菜单存在，并且父级菜单状态为启用,并不在父级列表中，则添加到父级列表
                if (parent != null && parent.Status == CommonStatusConst.ENABLE && !parentList.Contains(parent)
                    && !myMenus.Contains(parent))
                {
                    parentList.Add(parent);//添加到父列表
                }
            });
        });
        return parentList;
    }

    /// <summary>
    /// 构建Meta
    /// </summary>
    /// <param name="myMenus">我的菜单集合</param>
    private void ConstructMeta(List<SysResource> myMenus)
    {
        myMenus.ForEach(it =>
        {
            //定义meta
            var meta = new Meta
            {
                Icon = it.Icon,
                Title = it.Title,
                IsAffix = it.IsAffix,
                IsHide = it.IsHide,
                IsKeepAlive = it.IsKeepAlive,
                IsFull = it.IsFull,
                ActiveMenu = it.ActiveMenu,
                IsLink = it.Category == SysResourceConst.LINK ? it.Path : ""
            };
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
        var orgParents = orgList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (orgParents.Count > 0)//如果数量大于0
        {
            var data = new List<LoginOrgTreeOutput>();
            foreach (var item in orgParents)//遍历字典
            {
                var loginOrg = new LoginOrgTreeOutput
                {
                    Children = ConstrucOrgTrees(orgList, item.Id, orgId),//添加子节点
                    Id = item.Id,
                    Label = item.Name,
                    Pid = item.ParentId,
                    Style = orgId != item.Id ? null : new LoginOrgTreeOutput.MyStyle()
                };
                data.Add(loginOrg);//添加到列表
            }
            return data;//返回结果
        }
        return new List<LoginOrgTreeOutput>();
    }

    #endregion 方法
}
