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
/// 角色管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "角色管理")]
[Route("sys/limit/[controller]")]
[SuperAdmin]
public class RoleController : BaseController
{
    private readonly ISysRoleService _sysRoleService;
    private readonly IResourceService _resourceService;

    public RoleController(ISysRoleService sysRoleService, IResourceService resourceService)
    {
        _sysRoleService = sysRoleService;
        _resourceService = resourceService;
    }

    /// <summary>
    /// 角色分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] RolePageInput input)
    {
        return await _sysRoleService.Page(input);
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
        await _sysRoleService.Add(input);
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
        await _sysRoleService.Edit(input);
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
        await _sysRoleService.Delete(input);
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
        return await _sysRoleService.OwnResource(input, CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);
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
        await _sysRoleService.GrantResource(input);
    }

    /// <summary>
    /// 获取权限授权树
    /// </summary>
    /// <returns></returns>
    [HttpGet("permissionTreeSelector")]
    public async Task<dynamic> PermissionTreeSelector([FromQuery] BaseIdInput input)
    {
        return await _sysRoleService.RolePermissionTreeSelector(input);
    }

    /// <summary>
    /// 获取角色拥有权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownPermission")]
    public async Task<dynamic> OwnPermission([FromQuery] BaseIdInput input)
    {
        return await _sysRoleService.OwnPermission(input);
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
        await _sysRoleService.GrantPermission(input);
    }


    /// <summary>
    /// 获取角色下的用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownUser")]
    public async Task<dynamic> OwnUser([FromQuery] BaseIdInput input)
    {
        return await _sysRoleService.OwnUser(input);
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
        await _sysRoleService.GrantUser(input);
    }

    /// <summary>
    /// 获取角色树
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    [DisplayName("获取角色树")]
    public async Task<dynamic> Tree([FromQuery] RoleTreeInput input)
    {
        return await _sysRoleService.Tree(input);
    }


    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _sysRoleService.Detail(input);
    }

    /// <summary>
    /// 获取角色选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("roleSelector")]
    public async Task<dynamic> RoleSelector([FromQuery] RoleSelectorInput input)
    {
        return await _sysRoleService.RoleSelector(input);
    }
}
