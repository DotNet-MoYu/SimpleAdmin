namespace SimpleAdmin.Web.Core;

/// <summary>
/// 用户个人信息控制器
/// </summary>
[ApiDescriptionSettings(Tag = "用户个人信息控制器")]
[Route("sys/[controller]")]
public class UserCenterController : IDynamicApiController
{
    private readonly IRoleService _roleService;
    private readonly IUserCenterService _userCenterService;
    private readonly ISysUserService _sysUserService;
    private readonly ISysPositionService _sysPositionService;
    private readonly ISysOrgService _sysOrgService;

    public UserCenterController(IUserCenterService userCenterService, ISysUserService sysUserService, ISysPositionService sysPositionService,
        ISysOrgService sysOrgService, IRoleService roleService)
    {
        _userCenterService = userCenterService;
        _sysUserService = sysUserService;
        _sysPositionService = sysPositionService;
        _sysOrgService = sysOrgService;
        _roleService = roleService;
    }

    /// <summary>
    /// 获取个人菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginMenu")]
    public async Task<dynamic> LoginMenu()
    {
        return await _userCenterService.GetOwnMenu();
    }

    /// <summary>
    /// 获取个人工作台
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginWorkbench")]
    public async Task<dynamic> LoginWorkbench()
    {
        return await _userCenterService.GetLoginWorkbench();
    }

    /// <summary>
    /// 编辑个人信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateUserInfo")]
    [DisplayName("编辑个人信息")]
    public async Task UpdateUserInfo([FromBody] UpdateInfoInput input)
    {
        await _userCenterService.UpdateUserInfo(input);
    }

    /// <summary>
    /// 更新签名
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateSignature")]
    [DisplayName("更新签名")]
    public async Task UpdateSignature([FromBody] UpdateSignatureInput input)
    {
        await _userCenterService.UpdateSignature(input);
    }

    /// <summary>
    /// 获取组织架构
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginOrgTree")]
    public async Task<dynamic> LoginOrgTree()
    {
        return await _userCenterService.LoginOrgTree();
    }

    /// <summary>
    /// 编辑工作台
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateUserWorkbench")]
    [DisplayName("编辑工作台")]
    public async Task UpdateUserWorkbench([FromBody] UpdateWorkbenchInput input)
    {
        await _userCenterService.UpdateWorkbench(input);
    }

    /// <summary>
    /// 获取登录用户的站内信分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("loginUnreadMessagePage")]
    public async Task<dynamic> LoginUnreadMessagePage([FromQuery] MessagePageInput input)
    {
        return await _userCenterService.LoginMessagePage(input);
    }

    /// <summary>
    /// 读取登录用户站内信详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("loginUnreadMessageDetail")]
    public async Task<dynamic> LoginUnreadMessageDetail([FromQuery] BaseIdInput input)
    {
        return await _userCenterService.LoginMessageDetail(input);
    }

    /// <summary>
    /// 未读消息数
    /// </summary>
    /// <returns></returns>
    [HttpGet("UnReadCount")]
    public async Task<dynamic> UnReadCount()
    {
        return await _userCenterService.UnReadCount();
    }

    /// <summary>
    /// 删除我的消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("deleteMessage")]
    [DisplayName("删除个人消息")]
    public async Task DeleteMessage([FromBody] BaseIdInput input)
    {
        await _userCenterService.DeleteMyMessage(input);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updatePassword")]
    [DisplayName("修改密码")]
    public async Task UpdatePassword([FromBody] UpdatePasswordInput input)
    {
        await _userCenterService.UpdatePassword(input);
    }

    /// <summary>
    /// 修改头像
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateAvatar")]
    [DisplayName("修改头像")]
    public async Task<dynamic> UpdateAvatar([FromForm] BaseFileInput input)
    {
        return await _userCenterService.UpdateAvatar(input);
    }


    /// <summary>
    ///  根据id集合获取用户集合
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("getUserListByIdList")]
    public async Task<dynamic> GetUserListByIdList([FromBody] IdListInput input)
    {
        return await _sysUserService.GetUserListByIdList(input);
    }

    /// <summary>
    ///  根据id集合获取职位集合
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("getPositionListByIdList")]
    public async Task<dynamic> GetPositionListByIdList([FromBody] IdListInput input)
    {
        return await _sysPositionService.GetPositionListByIdList(input);
    }

    /// <summary>
    ///  根据id集合获取组织集合
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("getOrgListByIdList")]
    public async Task<dynamic> GetOrgListByIdList([FromBody] IdListInput input)
    {
        return await _sysOrgService.GetOrgListByIdList(input);
    }

    /// <summary>
    ///  根据id集合获取角色集合
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("getRoleListByIdList")]
    public async Task<dynamic> GetRoleListByIdList([FromBody] IdListInput input)
    {
        return await _roleService.GetRoleListByIdList(input);
    }
}