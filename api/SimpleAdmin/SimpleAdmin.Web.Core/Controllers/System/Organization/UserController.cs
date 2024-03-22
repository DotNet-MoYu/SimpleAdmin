// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "用户管理")]
[Route("sys/organization/[controller]")]
[SuperAdmin]
public class UserController : BaseController
{
    private readonly ISysUserService _sysUserService;
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysPositionService _sysPositionService;
    private readonly ISysRoleService _sysRoleService;

    public UserController(ISysUserService sysUserService, ISysOrgService sysOrgService, ISysPositionService sysPositionService,
        ISysRoleService sysRoleService)
    {
        _sysUserService = sysUserService;
        _sysOrgService = sysOrgService;
        _sysPositionService = sysPositionService;
        _sysRoleService = sysRoleService;
    }

    #region Get请求

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
    [HttpGet("selector")]
    public async Task<dynamic> Selector([FromQuery] UserSelectorInput input)
    {
        return await _sysUserService.Selector(input);
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
    /// 获取用户拥有资源
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownResource")]
    public async Task<dynamic> OwnResource([FromQuery] BaseIdInput input)
    {
        return await _sysUserService.OwnResource(input);
    }

    /// <summary>
    /// 获取用户拥有权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownPermission")]
    public async Task<dynamic> OwnPermission([FromQuery] BaseIdInput input)
    {
        return await _sysUserService.OwnPermission(input);
    }

    /// <summary>
    /// 获取权限授权树
    /// </summary>
    /// <returns></returns>
    [HttpGet("permissionTreeSelector")]
    public async Task<dynamic> PermissionTreeSelector([FromQuery] BaseIdInput input)
    {
        return await _sysUserService.UserPermissionTreeSelector(input);
    }

    /// <summary>
    /// 导入预览
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("preview")]
    [DisableRequestSizeLimit]
    [SuppressMonitor]
    public async Task<dynamic> Preview([FromForm] ImportPreviewInput input)
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

    /// <summary>
    /// 用户详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    [DisplayName("用户详情")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _sysUserService.Detail(input);
    }

    #endregion Get请求

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
    /// 批量修改用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edits")]
    [DisplayName("批量修改用户")]
    public async Task Edits([FromBody] BatchEditInput input)
    {
        await _sysUserService.Edits(input);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除用户")]
    public async Task Delete([FromBody] BaseIdListInput input)
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
    /// 给用户授权资源
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantResource")]
    [DisplayName("用户授权资源")]
    public async Task GrantResource([FromBody] UserGrantResourceInput input)
    {
        await _sysUserService.GrantResource(input);
    }

    /// <summary>
    /// 给用户授权权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantPermission")]
    [DisplayName("用户授权权限")]
    public async Task GrantPermission([FromBody] GrantPermissionInput input)
    {
        await _sysUserService.GrantPermission(input);
    }

    /// <summary>
    /// 用户导入
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("import")]
    [DisplayName("用户导入")]
    public async Task<dynamic> Import([SuppressMonitor][FromBody] ImportResultInput<SysUserImportInput> input)
    {
        return await _sysUserService.Import(input);
    }

    /// <summary>
    /// 用户导出
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("export")]
    [DisplayName("用户导出")]
    public async Task<dynamic> Export([FromBody] UserPageInput input)
    {
        return await _sysUserService.Export(input);
    }

    #endregion Post请求
}
