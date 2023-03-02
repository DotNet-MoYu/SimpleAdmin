

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


    #region Get请求
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
    /// 导入预览
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("preview")]
    [DisableRequestSizeLimit]
    [SuppressMonitor]
    public async Task<dynamic> Preview([FromForm] BaseImportPreviewInput input)
    {
        return await _sysUserService.Preview(input);
    }

    /// <summary>
    /// 模板下载
    /// </summary>
    /// <returns></returns>
    [HttpGet(template: "template")]
    [SuppressMonitor]
    public async Task<dynamic> Template()
    {
        return await _sysUserService.Template();
    }
    #endregion

    #region Post请求

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加用户")]
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
    [DisplayName("修改用户")]
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
    [DisplayName("删除用户")]
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
    [DisplayName("禁用用户")]
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
    [DisplayName("启用用户")]
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
    [DisplayName("重置密码")]
    public async Task ResetPassword([FromBody] BaseIdInput input)
    {
        await _sysUserService.ResetPassword(input);
    }


    /// <summary>
    /// 给用户授权角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantRole")]
    [DisplayName("授权角色")]
    public async Task GrantRole([FromBody] UserGrantRoleInput input)
    {
        await _sysUserService.GrantRole(input);
    }

    /// <summary>
    /// 用户导入
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("import")]
    [DisplayName("用户导入")]

    public async Task<dynamic> Import([SuppressMonitor][FromBody] BaseImportResultInput<SysUserImportInput> input)
    {
        return await _sysUserService.Import(input);
    }


    /// <summary>
    /// 用户导出
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("export")]
    [DisplayName("导出")]
    public async Task<dynamic> Export([FromBody] UserPageInput input)
    {
        return await _sysUserService.Export(input);
    }
    #endregion








}
