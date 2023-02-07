namespace SimpleAdmin.Web.Core.Controllers.System.System;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "角色管理")]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;
    private readonly IResourceService _resourceService;
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysUserService _sysUserService;

    public RoleController(IRoleService roleService, IResourceService resourceService, ISysOrgService sysOrgService, ISysUserService sysUserService)
    {
        this._roleService = roleService;
        this._resourceService = resourceService;
        this._sysOrgService = sysOrgService;
        this._sysUserService = sysUserService;
    }


    /// <summary>
    /// 角色分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] RolePageInput input)
    {
        return await _roleService.Page(input);
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加角色")]
    public async Task Add([FromBody] RoleAddInput input)
    {
        await _roleService.Add(input);
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改角色")]
    public async Task Edit([FromBody] RoleEditInput input)
    {
        await _roleService.Edit(input);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    [HttpPost("delete")]
    [DisplayName("删除角色")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _roleService.Delete(input);
    }

    /// <summary>
    /// 获取角色授权资源树
    /// </summary>
    /// <returns></returns>
    [HttpGet("resourceTreeSelector")]
    public async Task<dynamic> ResourceTreeSelector()
    {
        return await _resourceService.ResourceTreeSelector();
    }

    /// <summary>
    /// 获取角色拥有资源
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownResource")]
    public async Task<dynamic> OwnResource([FromQuery] BaseIdInput input)
    {
        return await _roleService.OwnResource(input);
    }

    /// <summary>
    /// 给角色授权资源
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantResource")]
    [DisplayName("角色授权")]
    public async Task GrantResource([FromBody] GrantResourceInput input)
    {
        await _roleService.GrantResource(input);
    }

    /// <summary>
    /// 获取权限授权树
    /// </summary>
    /// <returns></returns>
    [HttpGet("permissionTreeSelector")]
    public async Task<dynamic> PermissionTreeSelector([FromQuery] BaseIdInput input)
    {
        return await _roleService.RolePermissionTreeSelector(input);
    }


    /// <summary>
    /// 获取角色拥有权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownPermission")]
    public async Task<dynamic> OwnPermission([FromQuery] BaseIdInput input)
    {
        return await _roleService.OwnPermission(input);
    }

    /// <summary>
    /// 给角色授权权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantPermission")]
    [DisplayName("角色授权")]
    public async Task GrantPermission([FromBody] GrantPermissionInput input)
    {
        await _roleService.GrantPermission(input);
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
    /// 获取用户选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("userSelector")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _sysUserService.UserSelector(input);
    }

    /// <summary>
    /// 获取角色下的用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownUser")]
    public async Task<dynamic> OwnUser([FromQuery] BaseIdInput input)
    {
        return await _roleService.OwnUser(input);
    }

    /// <summary>
    /// 给角色授权用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantUser")]
    [DisplayName("角色授权")]
    public async Task GrantUser([FromBody] GrantUserInput input)
    {
        await _roleService.GrantUser(input);
    }

}
