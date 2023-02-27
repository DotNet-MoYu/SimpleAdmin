using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Application;

/// <summary>
/// <inheritdoc cref="IUserService"/>
/// </summary>
public class UserService : DbRepository<SysUser>, IUserService
{
    private readonly ISysUserService _sysUserService;
    private readonly IRoleService _roleService;
    private readonly ISysOrgService _sysOrgService;

    public UserService(ISysUserService sysUserService, IRoleService roleService, ISysOrgService sysOrgService)
    {
        this._sysUserService = sysUserService;
        this._roleService = roleService;
        this._sysOrgService = sysOrgService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysUser>> Page(UserPageInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        //动态查询条件
        var exp = Expressionable.Create<SysUser>();
        exp.And(u => u.Account != RoleConst.SuperAdmin);
        exp.OrIF(dataScope.Count > 0, u => dataScope.Contains(u.OrgId));//用户机构在数据范围内
        exp.OrIF(dataScope.Count == 0, u => u.Id == UserManager.UserId);//用户ID等于自己
        input.Expression = exp;
        //分页查询
        var pageInfo = await _sysUserService.Page(input);
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task Add(UserAddInput input)
    {
        await CheckInput(input, SimpleAdminConst.Add);//检查参数
        await _sysUserService.Add(input);//添加
    }

    /// <inheritdoc/>
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        //获取用户下信息
        var users = await GetListAsync(it => ids.Contains(it.Id), it => new SysUser { OrgId = it.OrgId, Id = it.Id });
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

    /// <inheritdoc/>
    public async Task Edit(UserEditInput input)
    {
        await CheckInput(input, SimpleAdminConst.Edit);//检查参数
        await _sysUserService.Edit(input);//编辑
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
        await CheckPermission(input.Id.Value, SimpleAdminConst.Disable);//检查权限
        await _sysUserService.GrantRole(input);//授权
    }

    /// <inheritdoc/>
    public async Task<List<long>> OwnRole(BaseIdInput input)
    {
        return await _sysUserService.OwnRole(input);//获取角色
    }

    /// <inheritdoc/>

    public async Task ResetPassword(BaseIdInput input)
    {
        await CheckPermission(input.Id, SimpleAdminConst.ResestPwd);//检查权限
        await _sysUserService.ResetPassword(input);//重置密码
    }

    /// <inheritdoc/>
    public async Task<List<UserSelectorOutPut>> UserSelector(UserSelectorInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope.Count > 0)
        {
            input.OrgIds = dataScope;//赋值机构列表
            return await _sysUserService.UserSelector(input);//查询
        }
        else
        {
            //返回自己
            return new List<UserSelectorOutPut> { new UserSelectorOutPut { Account = UserManager.UserAccount, Id = UserManager.UserId, Name = UserManager.Name, OrgId = UserManager.OrgId } };
        }

    }
    /// <inheritdoc />
    public async Task<List<SysRole>> RoleSelector(RoleSelectorInput input)
    {
        List<SysRole> sysRoles = new List<SysRole>();
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope.Count > 0)//如果有机构
        {
            input.OrgIds = dataScope;//将数据范传进去
            sysRoles = await _roleService.RoleSelector(input);//获取角色选择器列表
        }
        return sysRoles;
    }
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

        if (dataScope.Count > 0)//如果有机构
        {
            //获取用户信息
            var user = await _sysUserService.GetUserById(userId);
            if (!dataScope.Contains(user.OrgId))//判断用户机构ID是否在数据范围
                throw Oops.Bah(errorMessage);
        }
        else
        {
            if (userId != UserManager.UserId)//如果不是自己
                throw Oops.Bah(errorMessage);
        }
    }
    #endregion
}
