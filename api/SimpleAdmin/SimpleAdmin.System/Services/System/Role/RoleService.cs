using SimpleAdmin.Core.Extension;

namespace SimpleAdmin.System;


/// <inheritdoc cref="IRoleService"/>
//[Injection(Proxy = typeof(GlobalDispatchProxy))]
public class RoleService : DbRepository<SysRole>, IRoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly ISimpleRedis _simpleRedis;
    private readonly IRelationService _relationService;
    private readonly ISysOrgService _sysOrgService;
    private readonly IResourceService _resourceService;
    private readonly IEventPublisher _eventPublisher;

    public RoleService(ILogger<RoleService> logger,
                       ISimpleRedis simpleRedis,
                       IRelationService relationService,
                       ISysOrgService sysOrgService,
                       IResourceService resourceService,
                       IEventPublisher eventPublisher)
    {
        this._logger = logger;
        this._simpleRedis = simpleRedis;
        _relationService = relationService;
        this._sysOrgService = sysOrgService;
        this._resourceService = resourceService;
        this._eventPublisher = eventPublisher;
    }

    /// <summary>
    /// 获取所有橘色
    /// </summary>
    /// <returns></returns>
    public async override Task<List<SysRole>> GetListAsync()
    {
        //先从Redis拿
        var sysRoles = _simpleRedis.Get<List<SysRole>>(RedisConst.Redis_SysRole);
        if (sysRoles == null)
        {
            //redis没有就去数据库拿
            sysRoles = await base.GetListAsync();
            if (sysRoles.Count > 0)
            {
                //插入Redis
                _simpleRedis.Set(RedisConst.Redis_SysRole, sysRoles);

            }
        }
        return sysRoles;
    }

    /// <inheritdoc/>
    public async Task<List<SysRole>> GetRoleListByUserId(long userId)
    {
        List<SysRole> cods = new List<SysRole>();//角色代码集合
        var roleList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.Relation_SYS_USER_HAS_ROLE);//根据用户ID获取角色ID
        var roleIdList = roleList.Select(x => x.TargetId.ToLong()).ToList();//角色ID列表
        if (roleIdList.Count > 0)
        {
            cods = await GetListAsync(it => roleIdList.Contains(it.Id));
        }
        return cods;
    }


    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysRole>> Page(RolePageInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级机构
        var query = Context.Queryable<SysRole>()
                         .WhereIF(input.OrgId > 0, it => orgIds.Contains(it.OrgId.Value))//根据组织
                         .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类
                         .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
                         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
                         .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(RoleAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysRole = input.Adapt<SysRole>();//实体转换
        sysRole.Code = RandomHelper.CreateRandomString(10);//赋值Code
        if (await InsertAsync(sysRole))//插入数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(RoleEditInput input)
    {
        //判断是否超管
        if (input.Code == RoleConst.SuperAdmin)
            throw Oops.Bah($"不可编辑超管角色");
        await CheckInput(input);//检查参数
        var role = await GetFirstAsync(it => it.Id == input.Id);//获取角色
        if (role != null)
        {
            var permissions = new List<SysRelation>();
            //如果改了默认数据权限
            if (role.DefaultDataScope.ScopeCategory != input.DefaultDataScope.ScopeCategory)
            {
                //获取角色授权列表
                var relations = await _relationService.GetRelationByCategory(CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION);
                //找到当前角色的
                permissions = relations.Where(it => it.ObjectId == input.Id).ToList();
                permissions.ForEach(it =>
                {
                    var rolePermission = it.ExtJson.ToJsonEntity<RelationRolePermission>();//扩展信息转实体
                    //如果表里的数据范围是默认数据范围才更新,已经自定义了的数据范围不更新
                    if (rolePermission.ScopeCategory == role.DefaultDataScope.ScopeCategory && Enumerable.SequenceEqual(rolePermission.ScopeDefineOrgIdList, role.DefaultDataScope.ScopeDefineOrgIdList))
                    {
                        //重新赋值数据范围
                        rolePermission.ScopeCategory = input.DefaultDataScope.ScopeCategory;
                        rolePermission.ScopeDefineOrgIdList = input.DefaultDataScope.ScopeDefineOrgIdList;
                        it.ExtJson = rolePermission.ToJson();
                    }

                });
            }
            var sysRole = input.Adapt<SysRole>();//实体转换
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                await UpdateAsync(sysRole);//更新角色
                if (permissions.Any())//如果有授权权限就更新
                    await Context.Updateable(permissions).ExecuteCommandAsync();
            });
            if (result.IsSuccess)//如果成功了
            {
                await RefreshCache();//刷新缓存
                if (permissions.Any())//如果有授权权
                    await _relationService.RefreshCache(CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION);//关系表刷新SYS_ROLE_HAS_PERMISSION缓存
                await _eventPublisher.PublishAsync(EventSubscriberConst.ClearUserCache, new List<long> { input.Id });//清除角色下用户缓存
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
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            var sysRoles = await GetListAsync();//获取所有角色
            var hasSuperAdmin = sysRoles.Any(it => it.Code == RoleConst.SuperAdmin && ids.Contains(it.Id));//判断是否有超级管理员
            if (hasSuperAdmin) throw Oops.Bah($"不可删除系统内置超管角色");

            //数据库是string所以这里转下
            var targetIds = ids.Select(it => it.ToString()).ToList();
            //定义删除的关系
            var delRelations = new List<string> { CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE, CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION };
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除按钮
                var relationRep = base.ChangeRepository<DbRepository<SysRelation>>();//切换仓储
                //删除关系表角色与资源关系，角色与权限关系
                await relationRep.DeleteAsync(it => ids.Contains(it.ObjectId) && delRelations.Contains(it.Category));
                //删除关系表角色与用户关系
                await relationRep.DeleteAsync(it => targetIds.Contains(it.TargetId) && it.Category == CateGoryConst.Relation_SYS_USER_HAS_ROLE);
            });
            if (result.IsSuccess)//如果成功了
            {
                await RefreshCache();//刷新缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_USER_HAS_ROLE);//关系表刷新SYS_USER_HAS_ROLE缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);//关系表刷新SYS_ROLE_HAS_RESOURCE缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION);//关系表刷新SYS_ROLE_HAS_PERMISSION缓存
                await _eventPublisher.PublishAsync(EventSubscriberConst.ClearUserCache, ids);//清除角色下用户缓存
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
    public async Task<RoleOwnResourceOutput> OwnResource(BaseIdInput input)
    {
        RoleOwnResourceOutput roleOwnResource = new RoleOwnResourceOutput() { Id = input.Id };//定义结果集
        List<RelationRoleResuorce> GrantInfoList = new List<RelationRoleResuorce>();//已授权信息集合
        //获取关系列表
        var relations = await _relationService.GetRelationListByObjectIdAndCategory(input.Id, CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);
        //遍历关系表
        relations.ForEach(it =>
        {
            //将扩展信息转为实体
            var relationRole = it.ExtJson.ToJsonEntity<RelationRoleResuorce>();
            GrantInfoList.Add(relationRole);//添加到已授权信息
        });
        roleOwnResource.GrantInfoList = GrantInfoList;//赋值已授权信息
        return roleOwnResource;
    }


    /// <inheritdoc />
    public async Task GrantResource(GrantResourceInput input)
    {
        var menuIds = input.GrantInfoList.Select(it => it.MenuId).ToList();//菜单ID
        var extJsons = input.GrantInfoList.Select(it => it.ToJson()).ToList();//拓展信息
        var relationRoles = new List<SysRelation>();//要添加的角色资源和授权关系表
        var sysRole = (await GetListAsync()).Where(it => it.Id == input.Id).FirstOrDefault();//获取角色
        if (sysRole != null)
        {
            #region 角色资源处理
            //遍历角色列表
            for (int i = 0; i < menuIds.Count; i++)
            {
                //将角色资源添加到列表
                relationRoles.Add(new SysRelation
                {
                    ObjectId = sysRole.Id,
                    TargetId = menuIds[i].ToString(),
                    Category = CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE,
                    ExtJson = extJsons == null ? null : extJsons[i]
                });
            }
            #endregion
            #region 角色权限处理.
            var relationRolePer = new List<SysRelation>();//要添加的角色有哪些权限列表
            var defaultDataScope = sysRole.DefaultDataScope;//获取默认数据范围

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
                        ObjectId = sysRole.Id,
                        TargetId = it.ApiRoute,
                        Category = CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION,
                        ExtJson = new RelationRolePermission { ApiUrl = it.ApiRoute, ScopeCategory = defaultDataScope.ScopeCategory, ScopeDefineOrgIdList = defaultDataScope.ScopeDefineOrgIdList }.ToJson()
                    });

                });

            }
            relationRoles.AddRange(relationRolePer);//合并列表
            #endregion
            #region 保存数据库
            //事务
            var result = await itenant.UseTranAsync(async () =>
           {
               var relatioRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
               //如果不是代码生成,就删除老的
               if (!input.IsCodeGen)
                   await relatioRep.DeleteAsync(it => it.ObjectId == sysRole.Id && (it.Category == CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION || it.Category == CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE));
               await relatioRep.InsertRangeAsync(relationRoles);//添加新的
           });
            if (result.IsSuccess)//如果成功了
            {
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);//刷新关系缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION);//刷新关系缓存
                await _eventPublisher.PublishAsync(EventSubscriberConst.ClearUserCache, new List<long> { input.Id });//发送事件清除角色下用户缓存
            }
            else
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0003);
            }
            #endregion

        }

    }

    /// <inheritdoc />
    public async Task<RoleOwnPermissionOutput> OwnPermission(BaseIdInput input)
    {
        RoleOwnPermissionOutput roleOwnPermission = new RoleOwnPermissionOutput { Id = input.Id };//定义结果集
        List<RelationRolePermission> GrantInfoList = new List<RelationRolePermission>();//已授权信息集合
        //获取关系列表
        var relations = await _relationService.GetRelationListByObjectIdAndCategory(input.Id, CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION);
        //遍历关系表
        relations.ForEach(it =>
        {
            //将扩展信息转为实体
            var relationPermission = it.ExtJson.ToJsonEntity<RelationRolePermission>();
            GrantInfoList.Add(relationPermission);//添加到已授权信息
        });
        roleOwnPermission.GrantInfoList = GrantInfoList;//赋值已授权信息
        return roleOwnPermission;
    }

    /// <inheritdoc />
    public async Task GrantPermission(GrantPermissionInput input)
    {
        var sysRole = (await GetListAsync()).Where(it => it.Id == input.Id).FirstOrDefault();//获取角色
        if (sysRole != null)
        {
            var apiUrls = input.GrantInfoList.Select(it => it.ApiUrl).ToList();//apiurl列表
            var extJsons = input.GrantInfoList.Select(it => it.ToJson()).ToList();//拓展信息
            await _relationService.SaveRelationBatch(CateGoryConst.Relation_SYS_ROLE_HAS_PERMISSION, input.Id, apiUrls, extJsons, true);//添加到数据库
            await _eventPublisher.PublishAsync(EventSubscriberConst.ClearUserCache, new List<long> { input.Id });//清除角色下用户缓存
        }
    }


    /// <inheritdoc />
    public async Task<List<long>> OwnUser(BaseIdInput input)
    {
        //获取关系列表
        var relations = await _relationService.GetRelationListByTargetIdAndCategory(input.Id.ToString(), CateGoryConst.Relation_SYS_USER_HAS_ROLE);
        return relations.Select(it => it.ObjectId).ToList();
    }


    /// <inheritdoc />
    public async Task GrantUser(GrantUserInput input)
    {

        var sysRelations = new List<SysRelation>();//关系列表
        //遍历用户ID
        input.GrantInfoList.ForEach(it =>
        {
            sysRelations.Add(new SysRelation
            {
                ObjectId = it,
                TargetId = input.Id.ToString(),
                Category = CateGoryConst.Relation_SYS_USER_HAS_ROLE
            });
        });

        //事务
        var result = await itenant.UseTranAsync(async () =>
       {
           var relationRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
           var targetId = input.Id.ToString();//目标ID转string
           await relationRep.DeleteAsync(it => it.TargetId == targetId && it.Category == CateGoryConst.Relation_SYS_USER_HAS_ROLE);//删除老的
           await relationRep.InsertRangeAsync(sysRelations);//添加新的
       });
        if (result.IsSuccess)//如果成功了
        {
            await _relationService.RefreshCache(CateGoryConst.Relation_SYS_USER_HAS_ROLE);//刷新关系表SYS_USER_HAS_ROLE缓存
            await _eventPublisher.PublishAsync(EventSubscriberConst.ClearUserCache, new List<long> { input.Id });//清除角色下用户缓存

        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }

    }

    /// <inheritdoc />
    public async Task<List<SysRole>> RoleSelector(RoleSelectorInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级组织
        //如果机构ID列表不为空
        if (input.OrgIds != null)
        {
            orgIds = orgIds.Where(it => input.OrgIds.Contains(it)).ToList();//包含在机构ID列表中的组织ID
        }
        var result = await Context.Queryable<SysRole>()
                         .WhereIF(orgIds.Count > 0, it => orgIds.Contains(it.OrgId.Value))//组织ID
                         .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//分类
                         .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
                         .ToListAsync();
        return result;
    }

    /// <inheritdoc />
    public async Task<List<string>> RolePermissionTreeSelector(BaseIdInput input)
    {
        List<string> permissionTreeSelectors = new List<string>();//授权树结果集
        //获取角色资源关系
        var relationsRes = await _relationService.GetRelationByCategory(CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);
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

    /// <inheritdoc />
    public async Task RefreshCache()
    {

        _simpleRedis.Remove(RedisConst.Redis_SysRole);//删除KEY
        await GetListAsync();//重新缓存
    }

    #region 方法
    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysRole"></param>
    private async Task CheckInput(SysRole sysRole)
    {
        //判断分类
        if (sysRole.Category != CateGoryConst.Role_GLOBAL && sysRole.Category != CateGoryConst.Role_ORG)
            throw Oops.Bah($"角色所属分类错误:{sysRole.Category}");
        //如果是机构角色orgId不能为空
        if (sysRole.Category == CateGoryConst.Role_ORG && sysRole.OrgId == null)
            throw Oops.Bah($"orgId不能为空");
        else if (sysRole.Category == CateGoryConst.Role_GLOBAL)//如果是全局
            sysRole.OrgId = null;//机构id设null

        var sysRoles = await GetListAsync();//获取所有
        var repeatName = sysRoles.Any(it => it.OrgId == sysRole.OrgId && it.Name == sysRole.Name && it.Id != sysRole.Id);//是否有重复角色名称
        if (repeatName)//如果有
        {
            if (sysRole.OrgId == null)
                throw Oops.Bah($"存在重复的全局角色:{sysRole.Name}");
            else
                throw Oops.Bah($"同组织下存在重复的角色:{sysRole.Name}");
        }
    }

    #endregion
}
