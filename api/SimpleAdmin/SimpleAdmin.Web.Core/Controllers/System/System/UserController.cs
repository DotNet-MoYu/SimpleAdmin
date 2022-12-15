using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.System.System;


/// <summary>
/// 用户管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "用户管理")]
public class UserController : BaseController
{
    private readonly ISysUserService _sysUserService;
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysPositionService _sysPositionService;
    private readonly IRoleService _roleService;

    public UserController(ISysUserService sysUserService, ISysOrgService sysOrgService, ISysPositionService sysPositionService, IRoleService roleService)
    {
        this._sysUserService = sysUserService;
        this._sysOrgService = sysOrgService;
        this._sysPositionService = sysPositionService;
        this._roleService = roleService;
    }

    /// <summary>
    /// 获取组织树选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("orgTreeSelector")]
    public async Task<dynamic> OrgTreeSelector()
    {
        return await _sysOrgService.Tree();
    }

    /// <summary>
    /// 获取角色选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("roleSelector")]
    public async Task<dynamic> RoleSelector([FromQuery] RoleSelectorInput input)
    {
        return await _roleService.RoleSelector(input);
    }


    /// <summary>
    /// 用户分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] UserPageInput input)
    {
        return await _sysUserService.Page(input);
    }

    /// <summary>
    /// 获取用户选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("userSelector")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _sysUserService.UserSelector(input);
    }

    /// <summary>
    /// 职位选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("positionSelector")]
    public async Task<dynamic> PositionSelector([FromQuery] PositionSelectorInput input)
    {
        return await _sysPositionService.PositionSelector(input);
    }


    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [Description("添加用户")]
    public async Task Add([FromBody] UserAddInput input)
    {
        await _sysUserService.Add(input);
    }


    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [Description("修改用户")]
    public async Task Edit([FromBody] UserEditInput input)
    {
        await _sysUserService.Edit(input);
    }


    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [Description("删除用户")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _sysUserService.Delete(input);
    }

    /// <summary>
    /// 禁用用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("disableUser")]
    [Description("禁用用户")]
    public async Task DisableUser([FromBody] BaseIdInput input)
    {
        await _sysUserService.DisableUser(input);
    }

    /// <summary>
    /// 启用用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("enableUser")]
    [Description("启用用户")]
    public async Task EnableUser([FromBody] BaseIdInput input)
    {
        await _sysUserService.EnableUser(input);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("resetPassword")]
    [Description("重置密码")]
    public async Task ResetPassword([FromBody] BaseIdInput input)
    {
        await _sysUserService.ResetPassword(input);
    }

    /// <summary>
    /// 获取用户拥有角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownRole")]
    public async Task<dynamic> OwnRole([FromQuery] BaseIdInput input)
    {
        return await _sysUserService.OwnRole(input);
    }

    /// <summary>
    /// 给用户授权角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantRole")]
    [Description("授权角色")]
    public async Task GrantRole([FromBody] UserGrantRoleInput input)
    {
        await _sysUserService.GrantRole(input);
    }

}
