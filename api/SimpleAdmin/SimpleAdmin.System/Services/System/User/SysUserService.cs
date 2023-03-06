using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using Masuit.Tools;
using Org.BouncyCastle.Asn1;

namespace SimpleAdmin.System;


/// <summary>
/// <inheritdoc cref="ISysUserService"/>
/// </summary>
public class SysUserService : DbRepository<SysUser>, ISysUserService
{
    private readonly ILogger<ILogger> _logger;
    private readonly ISimpleRedis _simpleRedis;
    private readonly IRelationService _relationService;
    private readonly IResourceService _resourceService;
    private readonly ISysOrgService _sysOrgService;
    private readonly IRoleService _roleService;
    private readonly IImportExportService _importExportService;
    private readonly ISysPositionService _sysPositionService;
    private readonly IDictService _dictService;
    private readonly IConfigService _configService;

    public SysUserService(ILogger<ILogger> logger, ISimpleRedis simpleRedis,
                       IRelationService relationService,
                       IResourceService resourceService,
                       ISysOrgService orgService,
                       IRoleService roleService,
                       IImportExportService importExportService,
                       ISysPositionService sysPositionService,
                       IDictService dictService,
                       IConfigService configService)
    {
        this._logger = logger;
        _simpleRedis = simpleRedis;
        _relationService = relationService;
        _resourceService = resourceService;
        _sysOrgService = orgService;
        _roleService = roleService;
        this._importExportService = importExportService;
        this._sysPositionService = sysPositionService;
        this._dictService = dictService;
        this._configService = configService;
    }

    #region 查询

    /// <inheritdoc/>
    public async Task<SysUser> GetUserByAccount(string account)
    {
        var userId = await GetIdByAccount(account);//获取用户ID
        if (userId > 0)
        {
            var sysUser = await GetUserById(userId);//获取用户信息
            if (sysUser.Account == account)//这里做了比较用来限制大小写
                return sysUser;
            else
                return null;
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<SysUser> GetUserByPhone(string phone)
    {
        var userId = await GetIdByPhone(phone);//获取用户ID
        if (userId > 0)
        {
            return await GetUserById(userId);//获取用户信息
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<long> GetIdByPhone(string phone)
    {
        //先从Redis拿
        var userId = _simpleRedis.HashGet<long>(RedisConst.Redis_SysUserPhone, new string[] { phone })[0];
        if (userId == 0)
        {
            phone = CryptogramUtil.Sm4Encrypt(phone);//SM4加密一下
            //单查获取用户手机号对应的账号
            userId = await GetFirstAsync(it => it.Phone == phone, it => it.Id);
            if (userId > 0)
            {
                //插入Redis
                _simpleRedis.HashAdd(RedisConst.Redis_SysUserPhone, phone, userId);
            }
        }
        return userId;

    }

    /// <inheritdoc/>
    public async Task<SysUser> GetUserById(long Id)
    {
        //先从Redis拿
        var sysUser = _simpleRedis.HashGet<SysUser>(RedisConst.Redis_SysUser, new string[] { Id.ToString() })[0];
        if (sysUser == null)
        {
            sysUser = await Context.Queryable<SysUser>()
            .LeftJoin<SysOrg>((u, o) => u.OrgId == o.Id)//连表
            .LeftJoin<SysPosition>((u, o, p) => u.PositionId == p.Id)//连表
            .Where(u => u.Id == Id)
            .Select((u, o, p) => new SysUser { Id = u.Id.SelectAll(), OrgName = o.Name, PositionName = p.Name })
            .FirstAsync();
            if (sysUser != null)
            {
                //获取按钮码
                var buttonCodeList = await GetButtonCodeList(sysUser.Id);
                //获取数据权限
                var dataScopeList = await GetPermissionListByUserId(sysUser.Id, sysUser.OrgId);
                //获取权限码
                var permissionCodeList = dataScopeList.Select(it => it.ApiUrl).ToList();
                //获取角色码
                var roleCodeList = await _roleService.GetRoleListByUserId(sysUser.Id);
                //权限码赋值
                sysUser.ButtonCodeList = buttonCodeList;
                sysUser.RoleCodeList = roleCodeList.Select(it => it.Code).ToList();
                sysUser.RoleIdList = roleCodeList.Select(it => it.Id).ToList();
                sysUser.PermissionCodeList = permissionCodeList;
                sysUser.DataScopeList = dataScopeList;
                //插入Redis
                _simpleRedis.HashAdd(RedisConst.Redis_SysUser, sysUser.Id.ToString(), sysUser);
            }
        }
        return sysUser;
    }


    /// <inheritdoc/>
    public async Task<long> GetIdByAccount(string account)
    {
        //先从Redis拿
        var userId = _simpleRedis.HashGet<long>(RedisConst.Redis_SysUserAccount, new string[] { account })[0];
        if (userId == 0)
        {

            //单查获取用户账号对应ID
            userId = await GetFirstAsync(it => it.Account == account, it => it.Id);
            if (userId != 0)
            {
                //插入Redis
                _simpleRedis.HashAdd(RedisConst.Redis_SysUserAccount, account, userId);
            }
        }
        return userId;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetButtonCodeList(long userId)
    {
        List<string> buttonCodeList = new();//按钮ID集合
        //获取关系集合
        var roleList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.Relation_SYS_USER_HAS_ROLE);
        var roleIdList = roleList.Select(x => x.TargetId.ToLong()).ToList();//角色ID列表
        if (roleIdList.Count > 0)//如果该用户有角色
        {
            List<long>? buttonIdList = new List<long>();//按钮ID集合
            var resourceList = await _relationService.GetRelationListByObjectIdListAndCategory(roleIdList, CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);//获取资源集合
            resourceList.ForEach(it =>
            {
                if (!string.IsNullOrEmpty(it.ExtJson)) buttonIdList.AddRange(it.ExtJson.ToJsonEntity<RelationRoleResuorce>().ButtonInfo);//如果有按钮权限，将按钮ID放到buttonIdList
            });
            if (buttonIdList.Count > 0)
            {
                buttonCodeList = await _resourceService.GetCodeByIds(buttonIdList, CateGoryConst.Resource_BUTTON);
            }
        }
        return buttonCodeList;
    }

    /// <inheritdoc/>
    public async Task<List<DataScope>> GetPermissionListByUserId(long userId, long orgId)
    {
        var permissions = new List<DataScope>();//权限集合
        var roleIdList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.Relation_SYS_USER_HAS_ROLE);//根据用户ID获取角色ID
        if (roleIdList.Count > 0)//如果角色ID不为空
        {
            //获取角色权限信息
            var sysRelations = await _relationService.GetRelationListByObjectIdListAndCategory(roleIdList.Select(it => it.TargetId.ToLong()).ToList(), CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION);
            var relationGroup = sysRelations.GroupBy(it => it.TargetId).ToList();//根据目标ID,也就是接口名分组，因为存在一个用户多个角色
            var orgs = await _sysOrgService.GetListAsync();//获取所有机构
            var scopeAllList = orgs.Select(it => it.Id).ToList();//获取所有机构ID
            //遍历分组
            foreach (var it in relationGroup)
            {
                HashSet<long> scopeSet = new HashSet<long>();//定义不可重复列表
                var relationList = it.ToList();//关系列表
                foreach (var relation in relationList)
                {
                    var rolePermission = relation.ExtJson.ToJsonEntity<RelationRolePermission>();
                    var scopeCategory = rolePermission.ScopeCategory;//根据数据权限分类
                    if (scopeCategory != CateGoryConst.SCOPE_SELF)//如果不是仅自己
                    {
                        if (scopeCategory == CateGoryConst.SCOPE_ALL)//全部数据范围
                        {
                            scopeSet.AddRange(scopeAllList);//添加到范围列表
                        }
                        else if (scopeCategory == CateGoryConst.SCOPE_ORG)//只有自己机构
                        {
                            scopeSet.Add(orgId);//添加到范围列表
                        }
                        else if (scopeCategory == CateGoryConst.SCOPE_ORG_CHILD)//机构及以下机构
                        {
                            var scopeOrgChildList = (await _sysOrgService.GetChildListById(orgId)).Select(it => it.Id).ToList();//获取所属机构的下级机构Id列表
                            scopeSet.AddRange(scopeOrgChildList);//添加到范围列表
                        }
                        else
                        {
                            scopeSet.AddRange(rolePermission.ScopeDefineOrgIdList);//添加自定义范围的机构ID
                        }
                    }
                }
                permissions.Add(new DataScope { ApiUrl = it.Key, DataScopes = scopeSet.ToList() });//将改URL的权限集合加入权限集合列表
            }
        }
        return permissions;
    }


    /// <inheritdoc/>
    public async Task<List<long>> GetLoginUserApiDataScope()
    {
        var orgList = new List<long>();
        var userInfo = await GetUserById(UserManager.UserId);//获取用户信息
        // 路由名称
        var routeName = App.HttpContext.Request.Path.Value;
        //获取当前url的数据范围
        var dataScope = userInfo.DataScopeList.Where(it => it.ApiUrl == routeName).FirstOrDefault();
        if (dataScope != null)
        {
            orgList.AddRange(dataScope.DataScopes);//添加到机构列表
        }
        return orgList;
    }

    /// <inheritdoc/>
    public async Task<List<UserSelectorOutPut>> UserSelector(UserSelectorInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级机构
        var result = await Context.Queryable<SysUser>()
                         .WhereIF(input.OrgId > 0, it => orgIds.Contains(it.OrgId))//指定机构
                         .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.OrgId))//在指定机构列表查询
                         .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey) || it.Account.Contains(input.SearchKey))//根据关键字查询
                         .Select<UserSelectorOutPut>()//映射成SysUserSelectorOutPut
                         .ToListAsync();
        return result;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysUser>> Page(UserPageInput input)
    {
        var query = await GetQuery(input);//获取查询条件
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task<List<SysUser>> List(UserPageInput input)
    {
        var query = await GetQuery(input);//获取查询条件
        var list = await query.ToListAsync();
        return list;
    }

    /// <inheritdoc/>
    public async Task<List<long>> OwnRole(BaseIdInput input)
    {
        var relations = await _relationService.GetRelationListByObjectIdAndCategory(input.Id, CateGoryConst.Relation_SYS_USER_HAS_ROLE);
        return relations.Select(it => it.TargetId.ToLong()).ToList();
    }

    #endregion

    #region 新增

    /// <inheritdoc/>
    public async Task Add(UserAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysUser = input.Adapt<SysUser>();//实体转换
        //默认头像
        sysUser.Avatar = AvatarUtil.GetNameImageBase64(sysUser.Name);
        //获取默认密码
        sysUser.Password = await GetDefaultPassWord();//设置密码
        sysUser.UserStatus = DevDictConst.COMMON_STATUS_ENABLE;//默认状态
        await InsertAsync(sysUser);//添加数据
    }
    #endregion

    #region 编辑

    /// <inheritdoc/>
    public async Task Edit(UserEditInput input)
    {
        await CheckInput(input);//检查参数
        var exist = await GetUserById(input.Id);//获取用户信息
        if (exist != null)
        {
            var isSuperAdmin = exist.Account == RoleConst.SuperAdmin;//判断是否有超管
            if (isSuperAdmin && !UserManager.SuperAdmin)
                throw Oops.Bah($"不可修改系统内置超管用户账号");
            var name = exist.Name;//姓名
            var sysUser = input.Adapt<SysUser>();//实体转换
            if (name != input.Name)
                //默认头像
                sysUser.Avatar = AvatarUtil.GetNameImageBase64(input.Name);
            if (await Context.Updateable(sysUser).IgnoreColumns(it =>
            new
            {
                //忽略更新字段
                it.Password,
                it.LastLoginAddress,
                it.LastLoginDevice,
                it.LastLoginIp,
                it.LastLoginTime,
                it.LatestLoginAddress,
                it.LatestLoginDevice,
                it.LatestLoginIp,
                it.LatestLoginTime
            }).ExecuteCommandAsync() > 0)//修改数据
                DeleteUserFromRedis(sysUser.Id);//用户缓存到redis
        }

    }

    /// <inheritdoc/>
    public async Task DisableUser(BaseIdInput input)
    {
        var sysUser = await GetUserById(input.Id);//获取用户信息
        if (sysUser != null)
        {
            var isSuperAdmin = sysUser.Account == RoleConst.SuperAdmin;//判断是否有超管
            if (isSuperAdmin)
                throw Oops.Bah($"不可禁用系统内置超管用户账号");
            CheckSelf(input.Id, SimpleAdminConst.Disable);//判断是不是自己
            //设置状态为禁用
            if (await UpdateAsync(it => new SysUser { UserStatus = DevDictConst.COMMON_STATUS_DISABLED }, it => it.Id == input.Id))
                DeleteUserFromRedis(input.Id);//从redis删除用户信息
        }

    }
    /// <inheritdoc/>
    public async Task EnableUser(BaseIdInput input)
    {
        CheckSelf(input.Id, SimpleAdminConst.Enable);//判断是不是自己
        //设置状态为启用
        if (await UpdateAsync(it => new SysUser { UserStatus = DevDictConst.COMMON_STATUS_ENABLE }, it => it.Id == input.Id))
            DeleteUserFromRedis(input.Id);//从redis删除用户信息
    }

    /// <inheritdoc/>
    public async Task ResetPassword(BaseIdInput input)
    {
        var password = await GetDefaultPassWord(true);//获取默认密码,这里不走Aop所以需要加密一下
        //重置密码
        if (await UpdateAsync(it => new SysUser { Password = password }, it => it.Id == input.Id))
            DeleteUserFromRedis(input.Id);//从redis删除用户信息
    }
    /// <inheritdoc />
    public async Task GrantRole(UserGrantRoleInput input)
    {
        var sysUser = await GetUserById(input.Id);//获取用户信息
        if (sysUser != null)
        {
            var isSuperAdmin = sysUser.Account == RoleConst.SuperAdmin;//判断是否有超管
            if (isSuperAdmin)
                throw Oops.Bah($"不能给超管分配角色");
            CheckSelf(input.Id, SimpleAdminConst.GrantRole);//判断是不是自己
            //给用户赋角色
            await _relationService.SaveRelationBatch(CateGoryConst.Relation_SYS_USER_HAS_ROLE, input.Id, input.RoleIdList.Select(it => it.ToString()).ToList(), null, true);
            DeleteUserFromRedis(input.Id);//从redis删除用户信息
        }
    }


    #endregion

    #region 删除

    /// <inheritdoc/>
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            var containsSuperAdmin = await IsAnyAsync(it => it.Account == RoleConst.SuperAdmin && ids.Contains(it.Id));//判断是否有超管
            if (containsSuperAdmin)
                throw Oops.Bah($"不可删除系统内置超管用户");
            if (ids.Contains(UserManager.UserId))
                throw Oops.Bah($"不可删除自己");

            //需要更新兼任信息的用户列表
            var updatePositionJsonUser = new List<SysUser>();
            //获取兼任主管不是空的用户信息
            var PositionJsons = await GetListAsync(it => !SqlFunc.IsNullOrEmpty(it.PositionJson), it => new SysUser { Id = it.Id, PositionJson = it.PositionJson });
            PositionJsons.ForEach(position =>
            {
                bool update = false;//是否要更新标致

                //过滤主管是空的
                var positionJson = position.PositionJson.Where(it => it.DirectorId != null).ToList();
                //遍历兼任信息
                positionJson.ForEach(it =>
                {
                    if (it.DirectorId != null)//如果主管ID不是空的
                    {
                        if (ids.Contains(it.DirectorId.Value))//如果是兼任主管
                        {
                            it.DirectorId = null;//移除
                            update = true;//需要更新

                        }
                    }
                });
                if (update)//如果需要更新主管信息
                    updatePositionJsonUser.Add(position);
            });
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                //清除该用户作为主管信息
                await UpdateAsync(it => new SysUser { DirectorId = null }, it => ids.Contains(it.DirectorId.Value));
                //如果有兼任主管就清除兼任主管信息
                if (updatePositionJsonUser.Count > 0)
                    await Context.Updateable(updatePositionJsonUser).UpdateColumns(it => it.PositionJson).ExecuteCommandAsync();
                //删除用户
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());

            });
            if (result.IsSuccess)//如果成功了
            {
                DeleteUserFromRedis(ids);//redis删除用户
                // TODO 此处需要将这些用户踢下线，并永久注销这些用户
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
    public void DeleteUserFromRedis(long userId)
    {
        DeleteUserFromRedis(new List<long> { userId });
    }


    /// <inheritdoc />
    public void DeleteUserFromRedis(List<long> ids)
    {
        var userIds = ids.Select(it => it.ToString()).ToArray();//id转string列表
        var sysUsers = _simpleRedis.HashGet<SysUser>(RedisConst.Redis_SysUser, userIds);//获取用户列表
        sysUsers = sysUsers.Where(it => it != null).ToList();//过滤掉不存在的
        if (sysUsers.Count > 0)
        {
            var accounts = sysUsers.Select(it => it.Account).ToArray();//账号集合
            var phones = sysUsers.Select(it => it.Phone).ToArray();//手机号集合
            //删除用户信息
            _simpleRedis.HashDel<SysUser>(RedisConst.Redis_SysUser, userIds);
            //删除账号
            _simpleRedis.HashDel<long>(RedisConst.Redis_SysUserAccount, accounts);
            //删除手机
            if (phones != null)
                _simpleRedis.HashDel<long>(RedisConst.Redis_SysUserPhone, phones);
        }
    }


    #endregion

    #region 导入导出

    /// <inheritdoc/>
    public async Task<FileStreamResult> Template()
    {
        var templateName = "用户信息";
        //var result = _importExportService.GenerateLocalTemplate(templateName);
        var result = await _importExportService.GenerateTemplate<SysUserImportInput>(templateName);
        return result;

    }

    /// <inheritdoc/>
    public async Task<ImportPreviewOutput<SysUserImportInput>> Preview(ImportPreviewInput input)
    {
        var importPreview = await _importExportService.GetImportPreview<SysUserImportInput>(input.File);
        importPreview.Data = await CheckImport(importPreview.Data);//检查导入数据
        return importPreview;
    }

    /// <inheritdoc/>
    public async Task<ImportResultOutPut<SysUserImportInput>> Import(ImportResultInput<SysUserImportInput> input)
    {
        var data = await CheckImport(input.Data, true);//检查数据格式
        var result = _importExportService.GetImportResultPreview(data, out List<SysUserImportInput> importData);
        var sysUsers = importData.Adapt<List<SysUser>>();//转实体
        await SetUserDefault(sysUsers);//设置默认值
        //await InsertRangeAsync(sysUsers);//导入用户
        DbContext.Db.Fastest<SysUser>().BulkCopy(sysUsers);//大数据导入

        return result;

    }

    /// <inheritdoc/>
    public async Task<FileStreamResult> Export(UserPageInput input)
    {
        var genTests = await List(input);
        var data = genTests.Adapt<List<SysUserExportOutput>>();//转为Dto
        var result = await _importExportService.Export(data, "用户信息");
        return result;
    }

    /// <inheritdoc/>
    public async Task<List<T>> CheckImport<T>(List<T> data, bool clearError = false) where T : SysUserImportInput
    {
        #region 校验要用到的数据
        var accounts = data.Select(it => it.Account).ToList();//当前导入数据账号列表
        var phones = data.Where(it => !string.IsNullOrEmpty(it.Phone)).Select(it => it.Phone).ToList();//当前导入数据手机号列表
        var emails = data.Where(it => !string.IsNullOrEmpty(it.Email)).Select(it => it.Email).ToList();//当前导入数据邮箱列表
        var sysUsers = await GetListAsync(it => true, it => new SysUser { Account = it.Account, Phone = it.Phone, Email = it.Email });//获取数据用户信息
        var dbAccounts = sysUsers.Select(it => it.Account).ToList();//数据库账号列表
        var dbPhones = sysUsers.Where(it => !string.IsNullOrEmpty(it.Phone)).Select(it => it.Phone).ToList();//数据库手机号列表
        var dbEmails = sysUsers.Where(it => !string.IsNullOrEmpty(it.Email)).Select(it => it.Email).ToList();//邮箱账号列表
        var sysOrgs = await _sysOrgService.GetListAsync();
        var sysPositions = await _sysPositionService.GetListAsync();
        var dicts = await _dictService.GetListAsync();
        #endregion
        data.ForEach(async it =>
        {
            if (clearError)//如果需要清除错误
            {
                it.ErrorInfo = new Dictionary<string, string>();
                it.HasError = false;
            }
            #region 校验账号
            if (dbAccounts.Contains(it.Account))
                it.ErrorInfo.Add(nameof(it.Account), $"系统已存在账号{it.Account}");
            if (accounts.Where(u => u == it.Account).Count() > 1)
                it.ErrorInfo.Add(nameof(it.Account), $"账号重复");
            #endregion
            #region 校验手机号
            if (!string.IsNullOrEmpty(it.Phone))
            {
                if (dbPhones.Contains(it.Phone))
                    it.ErrorInfo.Add(nameof(it.Phone), $"系统已存在手机号{it.Phone}的用户");
                if (phones.Where(u => u == it.Phone).Count() > 1)
                    it.ErrorInfo.Add(nameof(it.Phone), $"手机号重复");
            }
            #endregion
            #region 校验邮箱
            if (!string.IsNullOrEmpty(it.Email))
            {
                if (dbEmails.Contains(it.Email))
                    it.ErrorInfo.Add(nameof(it.Email), $"系统已存在邮箱{it.Email}");
                if (emails.Where(u => u == it.Email).Count() > 1)
                    it.ErrorInfo.Add(nameof(it.Email), $"邮箱重复");
            }
            #endregion
            #region 校验部门和职位
            if (!string.IsNullOrEmpty(it.OrgName))
            {
                var org = sysOrgs.Where(u => u.Names == it.OrgName).FirstOrDefault();
                if (org != null) it.OrgId = org.Id;//赋值组织Id
                else it.ErrorInfo.Add(nameof(it.OrgName), $"部门{org}不存在");
            }
            //校验职位
            if (!string.IsNullOrEmpty(it.PositionName))
            {
                if (string.IsNullOrEmpty(it.OrgName)) it.ErrorInfo.Add(nameof(it.PositionName), $"请填写部门");
                else
                {
                    //根据部门ID和职位名判断是否有职位
                    var position = sysPositions.FirstOrDefault(u => u.OrgId == it.OrgId && u.Name == it.PositionName);
                    if (position != null) it.PositionId = position.Id;
                    else it.ErrorInfo.Add(nameof(it.PositionName), $"职位{it.PositionName}不存在");
                }

            }
            #endregion
            #region 校验性别等字典
            var genders = await _dictService.GetValuesByDictValue(DevDictConst.GENDER, dicts);
            if (!genders.Contains(it.Gender)) it.ErrorInfo.Add(nameof(it.Gender), $"性别只能是男和女");
            if (!string.IsNullOrEmpty(it.Nation))
            {
                var nations = await _dictService.GetValuesByDictValue(DevDictConst.NATION, dicts);
                if (!nations.Contains(it.Nation)) it.ErrorInfo.Add(nameof(it.Nation), $"不存在的民族");
            }
            if (!string.IsNullOrEmpty(it.IdCardType))
            {
                var idCarTypes = await _dictService.GetValuesByDictValue(DevDictConst.IDCARD_TYPE, dicts);
                if (!idCarTypes.Contains(it.IdCardType)) it.ErrorInfo.Add(nameof(it.IdCardType), $"证件类型错误");
            }
            if (!string.IsNullOrEmpty(it.CultureLevel))
            {
                var cultrueLevels = await _dictService.GetValuesByDictValue(DevDictConst.CULTURE_LEVEL, dicts);
                if (!cultrueLevels.Contains(it.CultureLevel)) it.ErrorInfo.Add(nameof(it.CultureLevel), $"文化程度有误");
            }
            #endregion
            if (it.ErrorInfo.Count > 0) it.HasError = true;//如果错误信息数量大于0则表示有错误


        });
        data = data.OrderByDescending(it => it.HasError).ToList();//排序
        return data;

    }

    /// <inheritdoc/>
    public async Task SetUserDefault(List<SysUser> sysUsers)
    {
        var defaultPassword = await GetDefaultPassWord(true);//默认密码

        //默认值赋值                                            
        sysUsers.ForEach(user =>
        {
            user.UserStatus = DevDictConst.COMMON_STATUS_ENABLE;//状态
            user.Phone = CryptogramUtil.Sm4Encrypt(user.Phone);//手机号
            user.Password = defaultPassword;//默认密码
            user.Avatar = AvatarUtil.GetNameImageBase64(user.Name);//默认头像

        });
    }
    #endregion

    #region 方法

    /// <summary>
    /// 获取默认密码
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetDefaultPassWord(bool isSm4 = false)
    {
        //获取默认密码
        var defaultPassword = (await _configService.GetByConfigKey(CateGoryConst.Config_SYS_BASE, DevConfigConst.SYS_DEFAULT_PASSWORD)).ConfigValue;
        return isSm4 ? CryptogramUtil.Sm4Encrypt(defaultPassword) : defaultPassword;//判断是否需要加密
    }

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysUser"></param>
    private async Task CheckInput(SysUser sysUser)
    {

        //判断账号重复,直接从redis拿
        var account_Id = await GetIdByAccount(sysUser.Account);
        if (account_Id > 0 && account_Id != sysUser.Id)
            throw Oops.Bah($"存在重复的账号:{sysUser.Account}");
        //如果手机号不是空
        if (!string.IsNullOrEmpty(sysUser.Phone))
        {
            if (!sysUser.Phone.MatchPhoneNumber())//验证手机格式
                throw Oops.Bah($"手机号码：{sysUser.Phone} 格式错误");
            var phone_Id = await GetIdByPhone(sysUser.Phone);
            if (phone_Id > 0 && sysUser.Id != phone_Id)//判断重复
                throw Oops.Bah($"存在重复的手机号:{sysUser.Phone}");
            sysUser.Phone = CryptogramUtil.Sm4Encrypt(sysUser.Phone);
        }
        //如果邮箱不是空
        if (!string.IsNullOrEmpty(sysUser.Email))
        {
            var (ismatch, match) = sysUser.Email.MatchEmail();//验证邮箱格式
            if (!ismatch)
                throw Oops.Bah($"邮箱：{sysUser.Email} 格式错误");
            if (await IsAnyAsync(it => it.Email == sysUser.Email && it.Id != sysUser.Id))
                throw Oops.Bah($"存在重复的邮箱:{sysUser.Email}");
        }
    }

    /// <summary>
    /// 检查是否为自己
    /// </summary>
    /// <param name="id"></param>
    /// <param name="operate">操作名称</param>
    private void CheckSelf(long id, string operate)
    {
        if (id == UserManager.UserId)//如果是自己
        {
            throw Oops.Bah($"禁止{operate}自己");
        }
    }

    /// <summary>
    /// 根据日期计算年龄
    /// </summary>
    /// <param name="birthdate"></param>
    /// <returns></returns>
    public int GetAgeByBirthdate(DateTime birthdate)
    {
        DateTime now = DateTime.Now;
        int age = now.Year - birthdate.Year;
        if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
        {
            age--;
        }
        return age < 0 ? 0 : age;

    }


    /// <summary>
    /// 获取Sqlsugar的ISugarQueryable
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<ISugarQueryable<SysUser>> GetQuery(UserPageInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级机构
        var query = Context.Queryable<SysUser>().LeftJoin<SysOrg>((u, o) => u.OrgId == o.Id)
         .LeftJoin<SysPosition>((u, o, p) => u.PositionId == p.Id)
         .WhereIF(input.OrgId > 0, u => orgIds.Contains(u.OrgId))//根据组织
         .WhereIF(input.Expression != null, input.Expression?.ToExpression())//动态查询
         .WhereIF(!string.IsNullOrEmpty(input.UserStatus), u => u.UserStatus == input.UserStatus)//根据状态查询
         .WhereIF(!string.IsNullOrEmpty(input.SearchKey), u => u.Name.Contains(input.SearchKey) || u.Account.Contains(u.Account))//根据关键字查询
         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"u.{input.SortField} {input.SortOrder}")
         .OrderBy(u => u.Id)//排序
         .Select((u, o, p) => new SysUser { Id = u.Id.SelectAll(), OrgName = o.Name, PositionName = p.Name, OrgNames = o.Names })
         .Mapper(u =>
         {
             u.Password = null;//密码清空
         });
        return query;
    }
    #endregion
}


