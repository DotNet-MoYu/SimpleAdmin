// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Application;

/// <summary>
/// <inheritdoc cref="IUserService"/>
/// </summary>
public class UserService : DbRepository<SysUser>, IUserService
{
    private readonly ISysUserService _sysUserService;
    private readonly IRoleService _roleService;
    private readonly ISysOrgService _sysOrgService;
    private readonly IImportExportService _importExportService;

    public UserService(
        ISysUserService sysUserService,
        IRoleService roleService,
        ISysOrgService sysOrgService,
        IImportExportService importExportService
    )
    {
        _sysUserService = sysUserService;
        _roleService = roleService;
        _sysOrgService = sysOrgService;
        _importExportService = importExportService;
    }

    #region 查询

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysUser>> Page(UserPageInput input)
    {
        var query = await GetQuery(input);
        //分页查询
        var pageInfo = await _sysUserService.Page(query);
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task<List<long>> OwnRole(BaseIdInput input)
    {
        return await _sysUserService.OwnRole(input);//获取角色
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<UserSelectorOutPut>> UserSelector(UserSelectorInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope == null)
            return await _sysUserService.UserSelector(input);//查询
        if (dataScope.Count > 0)
        {
            input.OrgIds = dataScope;//赋值机构列表
            return await _sysUserService.UserSelector(input);//查询
        }
        //返回自己
        return new SqlSugarPagedList<UserSelectorOutPut>
        {
            Total = 1,
            Pages = 1,
            HasPrevPages = false,
            HasNextPages = false,
            List = new[]
            {
                new UserSelectorOutPut
                {
                    Account = UserManager.UserAccount, Id = UserManager.UserId,
                    Name = UserManager.Name, OrgId = UserManager.OrgId
                }
            }
        };
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<SysRole>> RoleSelector(RoleSelectorInput input)
    {
        var sysRoles = new SqlSugarPagedList<SysRole>()
        {
            Total = 1,
            Pages = 1,
            HasPrevPages = false,
            HasNextPages = false,
            List = new List<SysRole>()
        };
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope == null)
            sysRoles = await _roleService.RoleSelector(input);//获取角色选择器列表
        else if (dataScope.Count > 0)//如果有机构
        {
            input.OrgIds = dataScope;//将数据范传进去
            sysRoles = await _roleService.RoleSelector(input);//获取角色选择器列表
        }
        return sysRoles;
    }

    /// <inheritdoc />
    public async Task<SysUser> Detail(BaseIdInput input)
    {
        var user = await _sysUserService.Detail(input);
        return user;
    }

    #endregion 查询

    #region 新增

    /// <inheritdoc/>
    public async Task Add(UserAddInput input)
    {
        await CheckInput(input, SimpleAdminConst.Add);//检查参数
        await _sysUserService.Add(input);//添加
    }

    #endregion 新增

    #region 编辑

    /// <inheritdoc/>
    public async Task Edit(UserEditInput input)
    {
        await CheckInput(input, SimpleAdminConst.Edit);//检查参数
        await _sysUserService.Edit(input);//编辑
    }

    /// <inheritdoc/>
    public async Task Edits(BatchEditInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        var ids = input.Ids;
        ;//获取用户id
        var sysUsers = await GetListAsync(it => ids.Contains(it.Id),
            it => new SysUser { OrgId = it.OrgId, CreateUserId = it.CreateUserId });//根据用户ID获取机构id、
        sysUsers.ForEach(it =>
        {
            if (dataScope != null && !dataScope.Contains(it.OrgId)
                && it.CreateUserId != UserManager.UserId)
                throw Oops.Bah(ErrorCodeEnum.A0004);//如果不包含机构id并且不是自己创建的
        });
        await _sysUserService.Edits(input);
    }

    /// <inheritdoc/>
    public async Task DisableUser(BaseIdInput input)
    {
        await CheckPermission(input.Id, SimpleAdminConst.Disable);//检查权限
        await _sysUserService.DisableUser(input);//禁用
    }

    /// <inheritdoc/>
    public async Task EnableUser(BaseIdInput input)
    {
        await CheckPermission(input.Id, SimpleAdminConst.Enable);//检查权限
        await _sysUserService.EnableUser(input);//启用
    }

    /// <inheritdoc/>
    public async Task GrantRole(UserGrantRoleInput input)
    {
        await CheckPermission(input.Id, SimpleAdminConst.Disable);//检查权限
        await _sysUserService.GrantRole(input);//授权
    }

    /// <inheritdoc/>
    public async Task ResetPassword(BaseIdInput input)
    {
        await CheckPermission(input.Id, SimpleAdminConst.ResestPwd);//检查权限
        await _sysUserService.ResetPassword(input);//重置密码
    }

    #endregion 编辑

    #region 删除

    /// <inheritdoc/>
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        //获取用户下信息
        var users = await GetListAsync(it => ids.Contains(it.Id),
            it => new SysUser { OrgId = it.OrgId, Id = it.Id });
        if (dataScope == null)//全部数据权限
        {
            await _sysUserService.Delete(input);//删除
            return;
        }
        if (dataScope.Count > 0)//如果有机构
        {
            var orgIds = users.Select(it => it.OrgId).ToList();//获取所有用户机构列表
            if (!dataScope.ContainsAll(orgIds))
                throw Oops.Bah($"您没有权限删除这些人员");
            await _sysUserService.Delete(input);//删除
        }
        else
        {
            throw Oops.Bah($"您没有权限删除这些人员");
        }
    }

    #endregion 删除

    #region 导入导出

    /// <inheritdoc/>
    public async Task<FileStreamResult> Template()
    {
        var templateName = "人员信息.xlsx";
        //var result = _importExportService.GenerateLocalTemplate(templateName);
        var result = await _importExportService.GenerateTemplate<BizUserImportInput>(templateName);
        return result;
    }

    /// <inheritdoc/>
    public async Task<dynamic> Preview(ImportPreviewInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope == null || dataScope.Count > 0)
        {
            var importPreview =
                await _importExportService.GetImportPreview<BizUserImportInput>(input.File);
            importPreview.Data = await CheckImport(importPreview.Data, dataScope);//检查导入数据
            return importPreview;
        }
        else
            throw Oops.Bah("您无权导入用户");
    }

    /// <inheritdoc/>
    public async Task<dynamic> Export(UserPageInput input)
    {
        var query = await GetQuery(input);
        var users = await _sysUserService.List(query);
        var data = users.Adapt<List<SysUserExportOutput>>();//转为Dto
        var result = await _importExportService.Export(data, "人员信息");
        return result;
    }

    /// <inheritdoc/>
    public async Task<ImportResultOutPut<BizUserImportInput>> Import(
        ImportResultInput<BizUserImportInput> input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope.Count > 0)
        {
            var data = await CheckImport(input.Data, dataScope, true);//检查数据格式
            var result = _importExportService.GetImportResultPreview(data, out var importData);
            var sysUsers = importData.Adapt<List<SysUser>>();//转实体
            await _sysUserService.SetUserDefault(sysUsers);//设置用户默认值
            await InsertOrBulkCopy(sysUsers);// 数据导入
            return result;
        }
        else throw Oops.Bah("您无权导入用户");
    }

    #endregion 导入导出

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysUser"></param>
    /// <param name="operate">操作类型</param>
    private async Task CheckInput(SysUser sysUser, string operate)
    {
        var errorMessage = $"您没有权限{operate}该机构下的人员";
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope.Count > 0)//如果有机构
        {
            if (!dataScope.Contains(sysUser.OrgId))//判断父ID是否在数据范围
                throw Oops.Bah(errorMessage);
        }
        else
        {
            //如果ID大于0表示编辑
            if (sysUser.Id > 0)
            {
                //检查权限
                await CheckPermission(sysUser.Id, operate, dataScope);
            }
            else
            {
                throw Oops.Bah(errorMessage);
            }
        }
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="operate">操作名称</param>
    /// <param name="dataScope">数据范围</param>
    /// <returns></returns>
    private async Task CheckPermission(long userId, string operate, List<long> dataScope = null)
    {
        var errorMessage = $"您没有权限{operate}该机构下的人员";
        //获取数据范围
        if (dataScope == null)
            dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope is { Count: > 0 })//如果有机构
        {
            //获取用户信息
            var user = await _sysUserService.GetUserById(userId);
            if (!dataScope.Contains(user.OrgId))//判断用户机构ID是否在数据范围
                throw Oops.Bah(errorMessage);
        }
        else if (dataScope is { Count: 0 })//如果没有机构
        {
            if (userId != UserManager.UserId)//如果不是自己
                throw Oops.Bah(errorMessage);
        }
    }

    /// <summary>
    /// 检查导入数据
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="dataScope">数据范围ID数组</param>
    /// <param name="clearError">是否初始化错误</param>
    /// <returns></returns>
    public async Task<List<BizUserImportInput>> CheckImport(List<BizUserImportInput> data,
        List<long> dataScope, bool clearError = false)
    {
        var errorMessage = $"没有权限";
        //先经过系统用户检查
        var bizUsers = await _sysUserService.CheckImport(data, clearError);
        bizUsers.ForEach(it =>
        {
            //如果机构没有错误
            if (!it.ErrorInfo.ContainsKey(nameof(it.OrgName)))
            {
                //判断是否包含数据范围,如果不包含
                if (dataScope != null && !dataScope.Contains(it.OrgId))
                {
                    it.ErrorInfo.Add(nameof(it.OrgName), errorMessage);
                    if (!it.ErrorInfo.ContainsKey(nameof(it.PositionName)))//如果机构没错
                        it.ErrorInfo.Add(nameof(it.PositionName), errorMessage);
                }
            }
            if (it.ErrorInfo.Count > 0) it.HasError = true;//如果错误信息数量大于0则表示有错误
        });
        bizUsers = bizUsers.OrderByDescending(it => it.HasError).ToList();//排序
        return bizUsers;
    }

    /// <summary>
    /// 获取查询条件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<UserPageInput> GetQuery(UserPageInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        //动态查询条件
        var exp = Expressionable.Create<SysUser>();
        exp.And(u => u.Account != RoleConst.SuperAdmin);
        exp.AndIF(dataScope is { Count: > 0 }, u => dataScope.Contains(u.OrgId));//用户机构在数据范围内
        exp.AndIF(dataScope is { Count: 0 }, u => u.Id == UserManager.UserId);//用户ID等于自己
        input.Expression = exp;
        return input;
    }

    #endregion 方法
}