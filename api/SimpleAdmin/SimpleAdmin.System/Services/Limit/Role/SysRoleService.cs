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

/// <inheritdoc cref="ISysRoleService"/>
//[Injection(Proxy = typeof(GlobalDispatchProxy))]
public class SysRoleService : DbRepository<SysRole>, ISysRoleService
{
    private readonly ILogger<SysRoleService> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IRelationService _relationService;
    private readonly ISysOrgService _sysOrgService;
    private readonly IResourceService _resourceService;
    private readonly IEventPublisher _eventPublisher;

    public SysRoleService(ILogger<SysRoleService> logger, ISimpleCacheService simpleCacheService, IRelationService relationService,
        ISysOrgService sysOrgService, IResourceService resourceService, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _simpleCacheService = simpleCacheService;
        _relationService = relationService;
        _sysOrgService = sysOrgService;
        _resourceService = resourceService;
        _eventPublisher = eventPublisher;
    }

    #region 查询

    /// <summary>
    /// 获取所有
    /// </summary>
    /// <returns></returns>
    public override async Task<List<SysRole>> GetListAsync()
    {
        //先从Redis拿
        var sysRoles = _simpleCacheService.Get<List<SysRole>>(SystemConst.CACHE_SYS_ROLE);
        if (sysRoles == null)
        {
            //redis没有就去数据库拿
            sysRoles = await base.GetListAsync();
            if (sysRoles.Count > 0)
            {
                //插入Redis
                _simpleCacheService.Set(SystemConst.CACHE_SYS_ROLE, sysRoles);
            }
        }
        return sysRoles;
    }

    /// <inheritdoc/>
    public async Task<List<SysRole>> GetRoleListByUserId(long userId)
    {
        var cods = new List<SysRole>();//角色代码集合
        var roleList = await _relationService.GetRelationListByObjectIdAndCategory(userId, CateGoryConst.RELATION_SYS_USER_HAS_ROLE);//根据用户ID获取角色ID
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
            .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.OrgId.Value))//数据权限
            .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}").OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task<List<RoleTreeOutput>> Tree(RoleTreeInput input)
    {
        var result = new List<RoleTreeOutput>();//返回结果
        var sysOrgList = await _sysOrgService.GetListAsync(false);//获取所有机构
        var sysRoles = await GetListAsync();//获取所有角色
        if (input.OrgIds != null)//根据数据范围查
        {
            sysOrgList = sysOrgList.Where(it => input.OrgIds.Contains(it.Id)).ToList();//在指定组织列表查询
            sysRoles = sysRoles.Where(it => input.OrgIds.Contains(it.OrgId.GetValueOrDefault())).ToList();//在指定职位列表查询
        }
        var topOrgList = sysOrgList.Where(it => it.ParentId == 0).ToList();//获取顶级机构
        var globalRole = sysRoles.Where(it => it.Category == CateGoryConst.ROLE_GLOBAL).ToList();//获取全局角色
        if (globalRole.Count > 0)
        {
            result.Add(new RoleTreeOutput()
            {
                Id = CommonUtils.GetSingleId(),
                Name = "全局角色",
                Children = globalRole.Select(it => new RoleTreeOutput
                {
                    Id = it.Id,
                    Name = it.Name,
                    IsRole = true
                }).ToList()
            });//添加全局角色
        }
        //遍历顶级机构
        foreach (var org in topOrgList)
        {
            var childIds = await _sysOrgService.GetOrgChildIds(org.Id, true, sysOrgList);//获取机构下的所有子级ID
            var childRoles = sysRoles.Where(it => it.OrgId != null && childIds.Contains(it.OrgId.Value)).ToList();//获取机构下的所有角色
            if (childRoles.Count == 0)
                continue;
            var roleTreeOutput = new RoleTreeOutput
            {
                Id = org.Id,
                Name = org.Name,
                IsRole = false
            };//实例化角色树
            childRoles.ForEach(it =>
            {
                roleTreeOutput.Children.Add(new RoleTreeOutput()
                {
                    Id = it.Id,
                    Name = it.Name,
                    IsRole = true
                });
            });
            result.Add(roleTreeOutput);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<RoleOwnResourceOutput> OwnResource(BaseIdInput input, string category)
    {
        var roleOwnResource = new RoleOwnResourceOutput { Id = input.Id };//定义结果集
        var grantInfoList = new List<RelationRoleResource>();//已授权信息集合
        //获取关系列表
        var relations = await _relationService.GetRelationListByObjectIdAndCategory(input.Id, category);
        //遍历关系表
        relations.ForEach(it =>
        {
            //将扩展信息转为实体
            var relationRole = it.ExtJson.ToJsonEntity<RelationRoleResource>();
            grantInfoList.Add(relationRole);//添加到已授权信息
        });
        roleOwnResource.GrantInfoList = grantInfoList;//赋值已授权信息
        return roleOwnResource;
    }

    /// <inheritdoc />
    public async Task<RoleOwnPermissionOutput> OwnPermission(BaseIdInput input)
    {
        var roleOwnPermission = new RoleOwnPermissionOutput { Id = input.Id };//定义结果集
        var grantInfoList = new List<RelationRolePermission>();//已授权信息集合
        //获取关系列表
        var relations = await _relationService.GetRelationListByObjectIdAndCategory(input.Id, CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION);
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
    public async Task<List<UserSelectorOutPut>> OwnUser(BaseIdInput input)
    {
        //获取关系列表
        var relations = await _relationService.GetRelationListByTargetIdAndCategory(input.Id.ToString(), CateGoryConst.RELATION_SYS_USER_HAS_ROLE);
        var userIds = relations.Select(it => it.ObjectId).ToList();
        var userList = await Context.Queryable<SysUser>().Where(it => userIds.Contains(it.Id)).Select<UserSelectorOutPut>().ToListAsync();
        return userList;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<RoleSelectorOutPut>> RoleSelector(RoleSelectorInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级组织
        //如果机构ID列表不为空
        if (input.OrgIds != null)
        {
            orgIds = orgIds.Where(it => input.OrgIds.Contains(it)).ToList();//包含在机构ID列表中的组织ID
        }
        var result = await Context.Queryable<SysRole>()
            .WhereIF(input.OrgId == 0, it => it.Category == CateGoryConst.ROLE_GLOBAL)//不传机构ID就是全局角色
            .WhereIF(orgIds.Count > 0, it => orgIds.Contains(it.OrgId.Value))//组织ID
            .WhereIF(!string.IsNullOrEmpty(input.Name), it => it.Name.Contains(input.Name))//根据关键字查询
            .Select<RoleSelectorOutPut>()
            .ToPagedListAsync(input.PageNum, input.PageSize);
        return result;
    }

    /// <inheritdoc />
    public async Task<List<string>> RolePermissionTreeSelector(BaseIdInput input)
    {
        var permissionTreeSelectors = new List<string>();//授权树结果集
        //获取单页信息
        var spa = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_SPA);
        var spaIds = spa.Select(it => it.Id).ToList();
        if (spaIds.Any())
        {
            //获取权限授权树
            var permissions = _resourceService.PermissionTreeSelector(spa.Select(it => it.Path).ToList());
            if (permissions.Count > 0)
                permissionTreeSelectors.AddRange(permissions.Select(it => it.PermissionName).ToList());//返回授权树权限名称列表
        }
        //获取角色资源关系
        var relationsRes = await _relationService.GetRelationByCategory(CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);
        var menuIds = relationsRes.Where(it => it.ObjectId == input.Id).Select(it => it.TargetId.ToLong()).ToList();
        if (menuIds.Any())
        {
            //获取菜单信息
            var menus = await _resourceService.GetResourcesByIds(menuIds, CateGoryConst.RESOURCE_MENU);
            //获取权限授权树
            var permissions = _resourceService.PermissionTreeSelector(menus.Select(it => it.Path).ToList());
            if (permissions.Count > 0)
                permissionTreeSelectors.AddRange(permissions.Select(it => it.PermissionName).ToList());//返回授权树权限名称列表
        }
        return permissionTreeSelectors;
    }


    /// <inheritdoc />
    public async Task<List<SysRole>> GetRoleListByIdList(IdListInput input)
    {
        var roles = await GetListAsync();
        var roleList = roles.Where(it => input.IdList.Contains(it.Id)).ToList();// 获取指定ID的岗位列表
        return roleList;
    }

    /// <inheritdoc />
    public async Task<SysRole> Detail(BaseIdInput input)
    {
        var roles = await GetListAsync();
        var role = roles.Where(it => it.Id == input.Id).FirstOrDefault();
        return role;
    }

    #endregion

    #region 新增

    /// <inheritdoc />
    public async Task Add(RoleAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysRole = input.Adapt<SysRole>();//实体转换
        if (await InsertAsync(sysRole))//插入数据
            await RefreshCache();//刷新缓存
    }

    #endregion

    #region 编辑

    /// <inheritdoc />
    public async Task Edit(RoleEditInput input)
    {
        //判断是否超管
        if (input.Code == SysRoleConst.SUPER_ADMIN)
            throw Oops.Bah("不可编辑超管角色");
        await CheckInput(input);//检查参数
        var role = await GetFirstAsync(it => it.Id == input.Id);//获取角色
        if (role != null)
        {
            var permissions = new List<SysRelation>();
            //如果改了默认数据权限
            if (role.DefaultDataScope.ScopeCategory != input.DefaultDataScope.ScopeCategory)
            {
                //获取角色授权列表
                var relations = await _relationService.GetRelationByCategory(CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION);
                //找到当前角色的
                permissions = relations.Where(it => it.ObjectId == input.Id).ToList();
                permissions.ForEach(it =>
                {
                    var rolePermission = it.ExtJson.ToJsonEntity<RelationRolePermission>();//扩展信息转实体
                    //如果表里的数据范围是默认数据范围才更新,已经自定义了的数据范围不更新
                    if (rolePermission.ScopeCategory == role.DefaultDataScope.ScopeCategory
                        && Enumerable.SequenceEqual(rolePermission.ScopeDefineOrgIdList, role.DefaultDataScope.ScopeDefineOrgIdList))
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
            var result = await Tenant.UseTranAsync(async () =>
            {
                await UpdateAsync(sysRole);//更新角色
                if (permissions.Any())//如果有授权权限就更新
                    await Context.Updateable(permissions).ExecuteCommandAsync();
            });
            if (result.IsSuccess)//如果成功了
            {
                await RefreshCache();//刷新缓存
                if (permissions.Any())//如果有授权
                    await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION);//关系表刷新SYS_ROLE_HAS_PERMISSION缓存
                await _eventPublisher.PublishAsync(EventSubscriberConst.CLEAR_USER_CACHE, new List<long> { input.Id });//清除角色下用户缓存
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
    public async Task GrantResource(GrantResourceInput input)
    {
        var allMenus = await _resourceService.GetListByCategory(SysResourceConst.MENU);//获取所有菜单
        var spas = await _resourceService.GetListByCategory(SysResourceConst.SPA);
        var menuIds = input.GrantInfoList.Select(it => it.MenuId).ToList();//菜单ID
        var extJsons = input.GrantInfoList.Select(it => it.ToJson()).ToList();//拓展信息
        var relationRoles = new List<SysRelation>();//要添加的角色资源和授权关系表
        var sysRole = (await GetListAsync()).Where(it => it.Id == input.Id).FirstOrDefault();//获取角色
        if (sysRole != null)
        {
            #region 角色模块处理

            var menusList = allMenus.Where(it => menuIds.Contains(it.Id)).ToList();//获取菜单信息
            //获取我的模块信息Id列表
            var moduleIds = menusList.Select(it => it.Module.Value).Distinct().ToList();
            moduleIds.ForEach(it =>
            {
                //将角色资源添加到列表
                relationRoles.Add(new SysRelation
                {
                    ObjectId = sysRole.Id,
                    TargetId = it.ToString(),
                    Category = CateGoryConst.RELATION_SYS_ROLE_HAS_MODULE
                });
            });

            #endregion 角色模块处理

            #region 角色资源处理

            //遍历菜单列表
            for (var i = 0; i < menuIds.Count; i++)
            {
                //将角色资源添加到列表
                relationRoles.Add(new SysRelation
                {
                    ObjectId = sysRole.Id,
                    TargetId = menuIds[i].ToString(),
                    Category = CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE,
                    ExtJson = extJsons?[i]
                });
            }

            #endregion 角色资源处理

            #region 角色权限处理.

            var relationRolePer = new List<SysRelation>();//要添加的角色有哪些权限列表
            var defaultDataScope = sysRole.DefaultDataScope;//获取默认数据范围

            //获取菜单信息
            var menus = allMenus.Where(it => menuIds.Contains(it.Id)).ToList();
            menus.AddRange(spas);//单页id添加到列表
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
                        Category = CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION,
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

            #endregion 角色权限处理.

            #region 保存数据库

            //事务
            var result = await Tenant.UseTranAsync(async () =>
            {
                var relationRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
                //如果不是代码生成,就删除老的
                if (!input.IsCodeGen)
                    await relationRep.DeleteAsync(it => it.ObjectId == sysRole.Id && (it.Category == CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION
                        || it.Category == CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE
                        || it.Category == CateGoryConst.RELATION_SYS_ROLE_HAS_MODULE));
                await relationRep.InsertRangeAsync(relationRoles);//添加新的
            });
            if (result.IsSuccess)//如果成功了
            {
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);//刷新关系缓存
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION);//刷新关系缓存
                await _eventPublisher.PublishAsync(EventSubscriberConst.CLEAR_USER_CACHE, new List<long> { input.Id });//发送事件清除角色下用户缓存
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
        var sysRole = (await GetListAsync()).Where(it => it.Id == input.Id).FirstOrDefault();//获取角色
        if (sysRole != null)
        {
            var apiUrls = input.GrantInfoList.Select(it => it.ApiUrl).ToList();//apiUrl列表
            var extJsons = input.GrantInfoList.Select(it => it.ToJson()).ToList();//拓展信息
            await _relationService.SaveRelationBatch(CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION, input.Id, apiUrls, extJsons,
                true);//添加到数据库
            await _eventPublisher.PublishAsync(EventSubscriberConst.CLEAR_USER_CACHE, new List<long> { input.Id });//清除角色下用户缓存
        }
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
                Category = CateGoryConst.RELATION_SYS_USER_HAS_ROLE
            });
        });

        //事务
        var result = await Tenant.UseTranAsync(async () =>
        {
            var relationRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
            var targetId = input.Id.ToString();//目标ID转string
            await relationRep.DeleteAsync(it => it.TargetId == targetId && it.Category == CateGoryConst.RELATION_SYS_USER_HAS_ROLE);//删除老的
            await relationRep.InsertRangeAsync(sysRelations);//添加新的
        });
        if (result.IsSuccess)//如果成功了
        {
            await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_USER_HAS_ROLE);//刷新关系表SYS_USER_HAS_ROLE缓存
            await _eventPublisher.PublishAsync(EventSubscriberConst.CLEAR_USER_CACHE, new List<long> { input.Id });//清除角色下用户缓存
        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }
    }

    #endregion

    #region 删除

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        if (ids.Count > 0)
        {
            var sysRoles = await GetListAsync();//获取所有角色
            var hasSuperAdmin = sysRoles.Any(it => it.Code == SysRoleConst.SUPER_ADMIN && ids.Contains(it.Id));//判断是否有超级管理员
            if (hasSuperAdmin) throw Oops.Bah("不可删除系统内置超管角色");

            //数据库是string所以这里转下
            var targetIds = ids.Select(it => it.ToString()).ToList();
            //定义删除的关系
            var delRelations = new List<string>
            {
                CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE,
                CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION,
                CateGoryConst.RELATION_SYS_ROLE_HAS_MODULE
            };
            //事务
            var result = await Tenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除按钮
                var relationRep = ChangeRepository<DbRepository<SysRelation>>();//切换仓储
                //删除关系表角色与资源关系，角色与权限关系
                await relationRep.DeleteAsync(it => ids.Contains(it.ObjectId) && delRelations.Contains(it.Category));
                //删除关系表角色与用户关系
                await relationRep.DeleteAsync(it => targetIds.Contains(it.TargetId) && it.Category == CateGoryConst.RELATION_SYS_USER_HAS_ROLE);
            });
            if (result.IsSuccess)//如果成功了
            {
                await RefreshCache();//刷新缓存
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_USER_HAS_ROLE);//关系表刷新SYS_USER_HAS_ROLE缓存
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);//关系表刷新Relation_SYS_ROLE_HAS_RESOURCE缓存
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_PERMISSION);//关系表刷新Relation_SYS_ROLE_HAS_PERMISSION缓存
                await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_MODULE);//关系表刷新RELATION_SYS_ROLE_HAS_MODULE缓存
                await _eventPublisher.PublishAsync(EventSubscriberConst.CLEAR_USER_CACHE, ids);//清除角色下用户缓存
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
    public async Task RefreshCache()
    {
        _simpleCacheService.Remove(SystemConst.CACHE_SYS_ROLE);//删除KEY
        await GetListAsync();//重新缓存
    }

    #endregion

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysRole"></param>
    private async Task CheckInput(SysRole sysRole)
    {
        //判断分类
        if (sysRole.Category != CateGoryConst.ROLE_GLOBAL && sysRole.Category != CateGoryConst.ROLE_ORG)
            throw Oops.Bah($"角色所属分类错误:{sysRole.Category}");
        //如果是机构角色orgId不能为空
        if (sysRole is { Category: CateGoryConst.ROLE_ORG, OrgId: null })
            throw Oops.Bah("orgId不能为空");
        if (sysRole.Category == CateGoryConst.ROLE_GLOBAL)//如果是全局
            sysRole.OrgId = null;//机构id设null

        var sysRoles = await GetListAsync();//获取所有
        var repeatName = sysRoles.Any(it => it.OrgId == sysRole.OrgId && it.Name == sysRole.Name && it.Id != sysRole.Id);//是否有重复角色名称
        if (repeatName)//如果有
        {
            if (sysRole.OrgId == null)
                throw Oops.Bah($"存在重复的全局角色:{sysRole.Name}");
            throw Oops.Bah($"同组织下存在重复的角色:{sysRole.Name}");
        }
        //如果code没填
        if (string.IsNullOrEmpty(sysRole.Code))
        {
            sysRole.Code = RandomHelper.CreateRandomString(10);//赋值Code
        }
        else
        {
            //判断是否有相同的Code
            if (sysRoles.Any(it => it.Code == sysRole.Code && it.Id != sysRole.Id))
                throw Oops.Bah($"存在重复的编码:{sysRole.Code}");
        }
    }

    #endregion 方法
}
