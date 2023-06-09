using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Linq.Expressions;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="ISysUserService"/>
/// </summary>
public class SysUserService : DbRepository<SysUser>, ISysUserService
{
    private readonly ILogger<ILogger> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IRelationService _relationService;
    private readonly IResourceService _resourceService;
    private readonly ISysOrgService _sysOrgService;
    private readonly IRoleService _roleService;
    private readonly IImportExportService _importExportService;
    private readonly ISysPositionService _sysPositionService;
    private readonly IDictService _dictService;
    private readonly IConfigService _configService;
    private readonly IBatchEditService _batchEditService;

    public SysUserService(ILogger<ILogger> logger, ISimpleCacheService simpleCacheService,
        IRelationService relationService,
        IResourceService resourceService,
        ISysOrgService orgService,
        IRoleService roleService,
        IImportExportService importExportService,
        ISysPositionService sysPositionService,
        IDictService dictService,
        IConfigService configService, IBatchEditService updateBatchService)
    {
        _logger = logger;
        _simpleCacheService = simpleCacheService;
        _relationService = relationService;
        _resourceService = resourceService;
        _sysOrgService = orgService;
        _roleService = roleService;
        _importExportService = importExportService;
        _sysPositionService = sysPositionService;
        _dictService = dictService;
        _configService = configService;
        _batchEditService = updateBatchService;
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
        var userId = _simpleCacheService.HashGetOne<long>(CacheConst.Cache_SysUserPhone, phone);
        if (userId == 0)
        {
            phone = CryptogramUtil.Sm4Encrypt(phone);//SM4加密一下
            //单查获取用户手机号对应的账号
            userId = await GetFirstAsync(it => it.Phone == phone, it => it.Id);
            if (userId > 0)
            {
                //插入Redis
                _simpleCacheService.HashAdd(CacheConst.Cache_SysUserPhone, phone, userId);
            }
        }
        return userId;
    }

    /// <inheritdoc/>
    public async Task<SysUser> GetUserById(long userId)
    {
        //先从Redis拿
        var sysUser = _simpleCacheService.HashGetOne<SysUser>(CacheConst.Cache_SysUser, userId.ToString());
        if (sysUser == null)
        {
            sysUser = await GetUserFromDb(userId);//从数据库拿用户信息
        }
        return sysUser;
    }

    /// <inheritdoc/>
    public async Task<T> GetUserById<T>(long userId)
    {
        //先从Redis拿
        var sysUser = _simpleCacheService.HashGetOne<T>(CacheConst.Cache_SysUser, userId.ToString());
        if (sysUser == null)
        {
            var user = await GetUserFromDb(userId);//从数据库拿用户信息
            if (sysUser != null)
            {
                sysUser = user.Adapt<T>();
            }
        }
        return sysUser;
    }




    /// <inheritdoc/>
    public async Task<long> GetIdByAccount(string account)
    {
        //先从Redis拿
        var userId = _simpleCacheService.HashGetOne<long>(CacheConst.Cache_SysUserAccount, account);
        if (userId == 0)
        {
            //单查获取用户账号对应ID
            userId = await GetFirstAsync(it => it.Account == account, it => it.Id);
            if (userId != 0)
            {
                //插入Redis
                _simpleCacheService.HashAdd(CacheConst.Cache_SysUserAccount, account, userId);
            }
        }
        return userId;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetButtonCodeList(long userId)
    {
        var buttonCodeList = new List<string>();//按钮ID集合
        //获取用户资源集合
        var resourceList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.Relation_SYS_USER_HAS_RESOURCE);
        var buttonIdList = new List<long>();//按钮ID集合
        if (resourceList.Count == 0)//如果有表示用户单独授权了不走用户角色
        {
            //获取用户角色关系集合
            var roleList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.Relation_SYS_USER_HAS_ROLE);
            var roleIdList = roleList.Select(x => x.TargetId.ToLong()).ToList();//角色ID列表
            if (roleIdList.Count > 0)//如果该用户有角色
            {
                resourceList = await _relationService.GetRelationListByObjectIdListAndCategory(roleIdList, CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);//获取资源集合
            }
        }
        resourceList.ForEach(it =>
        {
            if (!string.IsNullOrEmpty(it.ExtJson)) buttonIdList.AddRange(it.ExtJson.ToJsonEntity<RelationRoleResuorce>().ButtonInfo);//如果有按钮权限，将按钮ID放到buttonIdList
        });
        if (buttonIdList.Count > 0)
        {
            buttonCodeList = await _resourceService.GetCodeByIds(buttonIdList, CateGoryConst.Resource_BUTTON);
        }
        return buttonCodeList;
    }

    /// <inheritdoc/>
    public async Task<List<DataScope>> GetPermissionListByUserId(long userId, long orgId)
    {
        var permissions = new List<DataScope>();//权限集合
        var sysRelations = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.Relation_SYS_USER_HAS_PERMISSION);//根据用户ID获取用户权限
        if (sysRelations.Count == 0)//如果有表示用户单独授权了不走用户角色
        {
            var roleIdList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.Relation_SYS_USER_HAS_ROLE);//根据用户ID获取角色ID
            if (roleIdList.Count > 0)//如果角色ID不为空
            {
                //获取角色权限信息
                sysRelations = await _relationService.GetRelationListByObjectIdListAndCategory(roleIdList.Select(it => it.TargetId.ToLong()).ToList(), CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION);
            }
        }
        var relationGroup = sysRelations.GroupBy(it => it.TargetId).ToList();//根据目标ID,也就是接口名分组，因为存在一个用户多个角色
        var orgs = await _sysOrgService.GetListAsync();//获取所有机构
        var scopeAllList = orgs.Select(it => it.Id).ToList();//获取所有机构ID
        //遍历分组
        foreach (var it in relationGroup)
        {
            var scopeSet = new HashSet<long>();//定义不可重复列表
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
            permissions.Add(new DataScope
            {
                ApiUrl = it.Key,
                DataScopes = scopeSet.ToList()
            });//将改URL的权限集合加入权限集合列表
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

    /// <inheritdoc />
    public async Task<RoleOwnResourceOutput> OwnResource(BaseIdInput input)
    {
        return await _roleService.OwnResource(input, CateGoryConst.Relation_SYS_USER_HAS_RESOURCE);
    }

    /// <inheritdoc />
    public async Task<RoleOwnPermissionOutput> OwnPermission(BaseIdInput input)
    {
        var roleOwnPermission = new RoleOwnPermissionOutput
        {
            Id = input.Id
        };//定义结果集
        var grantInfoList = new List<RelationRolePermission>();//已授权信息集合
        //获取关系列表
        var relations = await _relationService.GetRelationListByObjectIdAndCategory(input.Id, CateGoryConst.Relation_SYS_USER_HAS_PERMISSION);
        //遍历关系表
        relations.ForEach(it =>
        {
            //将扩展信息转为实体
            var relationPermission = it.ExtJson.ToJsonEntity<RelationRolePermission>();
            grantInfoList.Add(relationPermission);//添加到已授权信息
        });
        roleOwnPermission.GrantInfoList = grantInfoList;//赋值已授权信息
        return roleOwnPermission;
    }

    /// <inheritdoc />
    public async Task<List<string>> UserPermissionTreeSelector(BaseIdInput input)
    {
        var permissionTreeSelectors = new List<string>();//授权树结果集
        //获取用户资源关系
        var relationsRes = await _relationService.GetRelationByCategory(CateGoryConst.Relation_SYS_USER_HAS_RESOURCE);
        var menuIds = relationsRes.Where(it => it.ObjectId == input.Id).Select(it => it.TargetId.ToLong()).ToList();
        if (menuIds.Any())
        {
            //获取菜单信息
            var menus = await _resourceService.GetMenuByMenuIds(menuIds);
            //获取权限授权树
            var permissions = _resourceService.PermissionTreeSelector(menus.Select(it => it.Path).ToList());
            if (permissions.Count > 0)
            {
                permissionTreeSelectors = permissions.Select(it => it.PermissionName).ToList();//返回授权树权限名称列表
            }
        }
        return permissionTreeSelectors;
    }

    #endregion 查询

    #region 新增

    /// <inheritdoc/>
    public async Task Add(UserAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysUser = input.Adapt<SysUser>();//实体转换
        //默认头像
        sysUser.Avatar = AvatarUtil.GetNameImageBase64(sysUser.Name);
        //获取默认密码
        sysUser.Password = await GetDefaultPassWord(true);//设置密码
        sysUser.UserStatus = DevDictConst.COMMON_STATUS_ENABLE;//默认状态
        await InsertAsync(sysUser);//添加数据
    }

    #endregion 新增

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
            {
                DeleteUserFromRedis(sysUser.Id);//删除用户缓存
                //删除用户token缓存
                _simpleCacheService.HashDel<List<TokenInfo>>(CacheConst.Cache_UserToken, new string[] { sysUser.Id.ToString() });
            }
        }
    }

    /// <inheritdoc/>
    public async Task Edits(BatchEditInput input)
    {
        //获取参数字典
        var data = await _batchEditService.GetUpdateBatchConfigDict(input.Code, input.Columns);
        if (data.Count > 0)
        {
            await Context.Updateable<SysUser>(data).Where(it => input.Ids.Contains(it.Id)).ExecuteCommandAsync();
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
            if (await UpdateAsync(it => new SysUser
            {
                UserStatus = DevDictConst.COMMON_STATUS_DISABLED
            }, it => it.Id == input.Id))
                DeleteUserFromRedis(input.Id);//从redis删除用户信息
        }
    }

    /// <inheritdoc/>
    public async Task EnableUser(BaseIdInput input)
    {
        CheckSelf(input.Id, SimpleAdminConst.Enable);//判断是不是自己
        //设置状态为启用
        if (await UpdateAsync(it => new SysUser
        {
            UserStatus = DevDictConst.COMMON_STATUS_ENABLE
        }, it => it.Id == input.Id))
            DeleteUserFromRedis(input.Id);//从redis删除用户信息
    }

    /// <inheritdoc/>
    public async Task ResetPassword(BaseIdInput input)
    {
        var password = await GetDefaultPassWord(true);//获取默认密码,这里不走Aop所以需要加密一下
        //重置密码
        if (await UpdateAsync(it => new SysUser
        {
            Password = password
        }, it => it.Id == input.Id))
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

    /// <inheritdoc />
    public async Task GrantResource(UserGrantResourceInput input)
    {
        var menuIds = input.GrantInfoList.Select(it => it.MenuId).ToList();//菜单ID
        var extJsons = input.GrantInfoList.Select(it => it.ToJson()).ToList();//拓展信息
        var relationRoles = new List<SysRelation>();//要添加的用户资源和授权关系表
        var sysUser = await GetUserById(input.Id);//获取用户
        if (sysUser != null)
        {
            #region 用户资源处理

            //遍历角色列表
            for (var i = 0; i < menuIds.Count; i++)
            {
                //将用户资源添加到列表
                relationRoles.Add(new SysRelation
                {
                    ObjectId = sysUser.Id,
                    TargetId = menuIds[i].ToString(),
                    Category = CateGoryConst.Relation_SYS_USER_HAS_RESOURCE,
                    ExtJson = extJsons == null ? null : extJsons[i]
                });
            }

            #endregion 用户资源处理

            #region 用户权限处理.

            var relationRolePer = new List<SysRelation>();//要添加的角色有哪些权限列表
            var defaultDataScope = input.DefaultDataScope;//获取默认数据范围

            //获取菜单信息
            var menus = await _resourceService.GetMenuByMenuIds(menuIds);
            if (menus.Count > 0)
            {
                //获取权限授权树
                var permissions = _resourceService.PermissionTreeSelector(menus.Select(it => it.Path).ToList());
                permissions.ForEach(it =>
                {
                    //新建角色权限关系
                    relationRolePer.Add(new SysRelation
                    {
                        ObjectId = sysUser.Id,
                        TargetId = it.ApiRoute,
                        Category = CateGoryConst.Relation_SYS_USER_HAS_PERMISSION,
                        ExtJson = new RelationRolePermission
                        {
                            ApiUrl = it.ApiRoute,
                            ScopeCategory = defaultDataScope.ScopeCategory,
                            ScopeDefineOrgIdList = defaultDataScope.ScopeDefineOrgIdList
                        }.ToJson()
                    });
                });
            }
            relationRoles.AddRange(relationRolePer);//合并列表

            #endregion 用户权限处理.

            #region 保存数据库

            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                var relatioRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
                await relatioRep.DeleteAsync(it =>
                    it.ObjectId == sysUser.Id && (it.Category == CateGoryConst.Relation_SYS_USER_HAS_PERMISSION || it.Category == CateGoryConst.Relation_SYS_USER_HAS_RESOURCE));
                await relatioRep.InsertRangeAsync(relationRoles);//添加新的
            });
            if (result.IsSuccess)//如果成功了
            {
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_USER_HAS_PERMISSION);//刷新关系缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_USER_HAS_RESOURCE);//刷新关系缓存
                DeleteUserFromRedis(input.Id);//删除该用户缓存
            }
            else
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0003);
            }

            #endregion 保存数据库
        }
    }

    /// <inheritdoc />
    public async Task GrantPermission(GrantPermissionInput input)
    {
        var sysUser = await GetUserById(input.Id);//获取用户
        if (sysUser != null)
        {
            var apiUrls = input.GrantInfoList.Select(it => it.ApiUrl).ToList();//apiurl列表
            var extJsons = input.GrantInfoList.Select(it => it.ToJson()).ToList();//拓展信息
            await _relationService.SaveRelationBatch(CateGoryConst.Relation_SYS_USER_HAS_PERMISSION, input.Id, apiUrls, extJsons, true);//添加到数据库
            DeleteUserFromRedis(input.Id);
        }
    }

    #endregion 编辑

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
            var positionJsons = await GetListAsync(it => !SqlFunc.IsNullOrEmpty(it.PositionJson), it => new SysUser
            {
                Id = it.Id,
                PositionJson = it.PositionJson
            });
            positionJsons.ForEach(position =>
            {
                var update = false;//是否要更新标致

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
            //定义删除的关系
            var delRelations = new List<string> { CateGoryConst.Relation_SYS_USER_HAS_RESOURCE, CateGoryConst.Relation_SYS_USER_HAS_PERMISSION, CateGoryConst.Relation_SYS_USER_HAS_ROLE };
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                //清除该用户作为主管信息
                await UpdateAsync(it => new SysUser
                {
                    DirectorId = null
                }, it => ids.Contains(it.DirectorId.Value));
                //如果有兼任主管就清除兼任主管信息
                if (updatePositionJsonUser.Count > 0)
                    await Context.Updateable(updatePositionJsonUser).UpdateColumns(it => it.PositionJson).ExecuteCommandAsync();
                //删除用户
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());

                var relationRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
                //删除关系表用户与资源关系，用户与权限关系,用户与角色关系
                await relationRep.DeleteAsync(it => ids.Contains(it.ObjectId) && delRelations.Contains(it.Category));
            });
            if (result.IsSuccess)//如果成功了
            {
                DeleteUserFromRedis(ids);//redis删除用户
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_USER_HAS_ROLE);//关系表刷新SYS_USER_HAS_ROLE缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_USER_HAS_RESOURCE);//关系表刷新SYS_USER_HAS_ROLE缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_USER_HAS_PERMISSION);//关系表刷新SYS_USER_HAS_ROLE缓存
                // TODO 此处需要将这些用户踢下线，并永久注销这些用户
                var idArray = ids.Select(it => it.ToString()).ToArray();
                //从列表中删除
                _simpleCacheService.HashDel<List<TokenInfo>>(CacheConst.Cache_UserToken, idArray);
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
        DeleteUserFromRedis(new List<long>
        {
            userId
        });
    }

    /// <inheritdoc />
    public void DeleteUserFromRedis(List<long> ids)
    {
        var userIds = ids.Select(it => it.ToString()).ToArray();//id转string列表
        var sysUsers = _simpleCacheService.HashGet<SysUser>(CacheConst.Cache_SysUser, userIds);//获取用户列表
        sysUsers = sysUsers.Where(it => it != null).ToList();//过滤掉不存在的
        if (sysUsers.Count > 0)
        {
            var accounts = sysUsers.Select(it => it.Account).ToArray();//账号集合
            var phones = sysUsers.Select(it => it.Phone).ToArray();//手机号集合
            //删除用户信息
            _simpleCacheService.HashDel<SysUser>(CacheConst.Cache_SysUser, userIds);
            //删除账号
            _simpleCacheService.HashDel<long>(CacheConst.Cache_SysUserAccount, accounts);
            //删除手机
            if (phones != null)
                _simpleCacheService.HashDel<long>(CacheConst.Cache_SysUserPhone, phones);
        }
    }

    #endregion 删除

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
        var result = _importExportService.GetImportResultPreview(data, out var importData);
        var sysUsers = importData.Adapt<List<SysUser>>();//转实体
        await SetUserDefault(sysUsers);//设置默认值
        await InsertOrBulkCopy(sysUsers);// 数据导入
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
        var sysUsers = await GetListAsync(it => true, it => new SysUser
        {
            Account = it.Account,
            Phone = it.Phone,
            Email = it.Email
        });//获取数据用户信息
        var dbAccounts = sysUsers.Select(it => it.Account).ToList();//数据库账号列表
        var dbPhones = sysUsers.Where(it => !string.IsNullOrEmpty(it.Phone)).Select(it => it.Phone).ToList();//数据库手机号列表
        var dbEmails = sysUsers.Where(it => !string.IsNullOrEmpty(it.Email)).Select(it => it.Email).ToList();//邮箱账号列表
        var sysOrgs = await _sysOrgService.GetListAsync();
        var sysPositions = await _sysPositionService.GetListAsync();
        var dicts = await _dictService.GetListAsync();

        #endregion 校验要用到的数据

        foreach (var item in data)
        {
            if (clearError)//如果需要清除错误
            {
                item.ErrorInfo = new Dictionary<string, string>();
                item.HasError = false;
            }

            #region 校验账号

            if (dbAccounts.Contains(item.Account))
                item.ErrorInfo.Add(nameof(item.Account), $"系统已存在账号{item.Account}");
            if (accounts.Where(u => u == item.Account).Count() > 1)
                item.ErrorInfo.Add(nameof(item.Account), $"账号重复");

            #endregion 校验账号

            #region 校验手机号

            if (!string.IsNullOrEmpty(item.Phone))
            {
                if (dbPhones.Contains(item.Phone))
                    item.ErrorInfo.Add(nameof(item.Phone), $"系统已存在手机号{item.Phone}的用户");
                if (phones.Where(u => u == item.Phone).Count() > 1)
                    item.ErrorInfo.Add(nameof(item.Phone), $"手机号重复");
            }

            #endregion 校验手机号

            #region 校验邮箱

            if (!string.IsNullOrEmpty(item.Email))
            {
                if (dbEmails.Contains(item.Email))
                    item.ErrorInfo.Add(nameof(item.Email), $"系统已存在邮箱{item.Email}");
                if (emails.Where(u => u == item.Email).Count() > 1)
                    item.ErrorInfo.Add(nameof(item.Email), $"邮箱重复");
            }

            #endregion 校验邮箱

            #region 校验部门和职位

            if (!string.IsNullOrEmpty(item.OrgName))
            {
                var org = sysOrgs.Where(u => u.Names == item.OrgName).FirstOrDefault();
                if (org != null) item.OrgId = org.Id;//赋值组织Id
                else item.ErrorInfo.Add(nameof(item.OrgName), $"部门{org}不存在");
            }
            //校验职位
            if (!string.IsNullOrEmpty(item.PositionName))
            {
                if (string.IsNullOrEmpty(item.OrgName)) item.ErrorInfo.Add(nameof(item.PositionName), $"请填写部门");
                else
                {
                    //根据部门ID和职位名判断是否有职位
                    var position = sysPositions.FirstOrDefault(u => u.OrgId == item.OrgId && u.Name == item.PositionName);
                    if (position != null) item.PositionId = position.Id;
                    else item.ErrorInfo.Add(nameof(item.PositionName), $"职位{item.PositionName}不存在");
                }
            }

            #endregion 校验部门和职位

            #region 校验性别等字典

            var genders = await _dictService.GetValuesByDictValue(DevDictConst.GENDER, dicts);
            if (!genders.Contains(item.Gender)) item.ErrorInfo.Add(nameof(item.Gender), $"性别只能是男和女");
            if (!string.IsNullOrEmpty(item.Nation))
            {
                var nations = await _dictService.GetValuesByDictValue(DevDictConst.NATION, dicts);
                if (!nations.Contains(item.Nation)) item.ErrorInfo.Add(nameof(item.Nation), $"不存在的民族");
            }
            if (!string.IsNullOrEmpty(item.IdCardType))
            {
                var idCarTypes = await _dictService.GetValuesByDictValue(DevDictConst.IDCARD_TYPE, dicts);
                if (!idCarTypes.Contains(item.IdCardType)) item.ErrorInfo.Add(nameof(item.IdCardType), $"证件类型错误");
            }
            if (!string.IsNullOrEmpty(item.CultureLevel))
            {
                var cultrueLevels = await _dictService.GetValuesByDictValue(DevDictConst.CULTURE_LEVEL, dicts);
                if (!cultrueLevels.Contains(item.CultureLevel)) item.ErrorInfo.Add(nameof(item.CultureLevel), $"文化程度有误");
            }

            #endregion 校验性别等字典

            if (item.ErrorInfo.Count > 0) item.HasError = true;//如果错误信息数量大于0则表示有错误
        }
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

    #endregion 导入导出

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
        var accountId = await GetIdByAccount(sysUser.Account);
        if (accountId > 0 && accountId != sysUser.Id)
            throw Oops.Bah($"存在重复的账号:{sysUser.Account}");
        //如果手机号不是空
        if (!string.IsNullOrEmpty(sysUser.Phone))
        {
            if (!sysUser.Phone.MatchPhoneNumber())//验证手机格式
                throw Oops.Bah($"手机号码：{sysUser.Phone} 格式错误");
            var phoneId = await GetIdByPhone(sysUser.Phone);
            if (phoneId > 0 && sysUser.Id != phoneId)//判断重复
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
        var now = DateTime.Now;
        var age = now.Year - birthdate.Year;
        if (now.Month < birthdate.Month || now.Month == birthdate.Month && now.Day < birthdate.Day)
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
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), u => u.Name.Contains(input.SearchKey) || u.Account.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"u.{input.SortField} {input.SortOrder}")
            .OrderBy(u => u.Id)//排序
            .Select((u, o, p) => new SysUser
            {
                Id = u.Id.SelectAll(),
                OrgName = o.Name,
                PositionName = p.Name,
                OrgNames = o.Names
            })
            .Mapper(u =>
            {
                u.Password = null;//密码清空
                u.Phone = CryptogramUtil.Sm4Decrypt(u.Phone);//手机号解密
            });
        return query;
    }


    /// <summary>
    /// 数据库获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    private async Task<SysUser> GetUserFromDb(long userId)
    {
        var sysUser = await Context.Queryable<SysUser>()
            .LeftJoin<SysOrg>((u, o) => u.OrgId == o.Id)//连表
            .LeftJoin<SysPosition>((u, o, p) => u.PositionId == p.Id)//连表
            .Where(u => u.Id == userId)
            .Select((u, o, p) => new SysUser
            {
                Id = u.Id.SelectAll(),
                OrgName = o.Name,
                PositionName = p.Name
            })
            .FirstAsync();
        if (sysUser != null)
        {
            sysUser.Password = CryptogramUtil.Sm4Decrypt(sysUser.Password);//解密密码
            sysUser.Phone = CryptogramUtil.Sm4Decrypt(sysUser.Phone);//解密手机号
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
            _simpleCacheService.HashAdd(CacheConst.Cache_SysUser, sysUser.Id.ToString(), sysUser);
            return sysUser;
        }
        return null;
    }

    #endregion 方法
}