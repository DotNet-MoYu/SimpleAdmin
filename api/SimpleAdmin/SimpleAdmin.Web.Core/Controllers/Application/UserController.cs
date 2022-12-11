using SimpleAdmin.Application;

namespace SimpleAdmin.Web.Core.Controllers.Application;

/// <summary>
/// 人员管理控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "人员管理")]
[Route("/biz/user")]
[RolePermission]
public class UserController : IDynamicApiController
{
    private readonly IUserService _userService;
    private readonly IOrgService _orgService;
    private readonly IPositionService _positionService;

    public UserController(IUserService userService, IOrgService orgService, IPositionService positionService)
    {
        this._userService = userService;
        this._orgService = orgService;
        this._positionService = positionService;
    }

    /// <summary>
    /// 获取组织树选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("orgTreeSelector")]
    [Description("机构树查询")]
    public async Task<dynamic> OrgTreeSelector()
    {
        return await _orgService.Tree();
    }



    /// <summary>
    /// 人员分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    [Description("人员分页查询")]
    public async Task<dynamic> Page([FromQuery] UserPageInput input)
    {
        return await _userService.Page(input);
    }

    /// <summary>
    /// 获取人员选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("userSelector")]
    [Description("人员选择器")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _userService.UserSelector(input);
    }

    /// <summary>
    /// 岗位选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("positionSelector")]
    [Description("岗位选择器")]
    public async Task<dynamic> PositionSelector([FromQuery] PositionSelectorInput input)
    {
        return await _positionService.PositionSelector(input);
    }


    /// <summary>
    /// 添加人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [Description("添加人员")]
    public async Task Add([FromBody] UserAddInput input)
    {
        await _userService.Add(input);
    }


    /// <summary>
    /// 修改人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [Description("修改人员")]
    public async Task Edit([FromBody] UserEditInput input)
    {
        await _userService.Edit(input);
    }


    /// <summary>
    /// 删除人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [Description("删除人员")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _userService.Delete(input);
    }

    /// <summary>
    /// 禁用人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("disableUser")]
    [Description("禁用人员")]
    public async Task DisableUser([FromBody] BaseIdInput input)
    {
        await _userService.DisableUser(input);
    }

    /// <summary>
    /// 启用人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("enableUser")]
    [Description("启用人员")]
    public async Task EnableUser([FromBody] BaseIdInput input)
    {
        await _userService.EnableUser(input);
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
        await _userService.ResetPassword(input);
    }

    /// <summary>
    /// 获取人员拥有角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownRole")]
    [Description("获取人员拥有角色")]
    public async Task<dynamic> OwnRole([FromQuery] BaseIdInput input)
    {
        return await _userService.OwnRole(input);
    }

    /// <summary>
    /// 给人员授权角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantRole")]
    [Description("授权角色")]
    public async Task GrantRole([FromBody] UserGrantRoleInput input)
    {
        await _userService.GrantRole(input);
    }

    /// <summary>
    /// 获取角色选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("roleSelector")]
    [Description("角色选择器")]
    public async Task<dynamic> RoleSelector([FromQuery] RoleSelectorInput input)
    {
        return await _userService.RoleSelector(input);
    }

}
