// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "角色管理")]
[Route("sys/limit/[controller]")]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;
    private readonly IResourceService _resourceService;
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysUserService _sysUserService;

    public RoleController(IRoleService roleService, IResourceService resourceService, ISysOrgService sysOrgService,
        ISysUserService sysUserService)
    {
        _roleService = roleService;
        _resourceService = resourceService;
        _sysOrgService = sysOrgService;
        _sysUserService = sysUserService;
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
    public async Task Delete([FromBody] BaseIdListInput input)
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
        return await _roleService.OwnResource(input, CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);
    }

    /// <summary>
    /// 给角色授权资源
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantResource")]
    [DisplayName("角色授权资源")]
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
    [DisplayName("角色授权权限")]
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
