namespace SimpleAdmin.Web.Core.Controllers.Systems.System;

/// <summary>
/// 用户个人信息控制器
/// </summary>
[ApiDescriptionSettings(Tag = "用户个人信息控制器")]
[Route("sys/[controller]")]
public class UserCenterController : IDynamicApiController
{
    private readonly IUserCenterService _userCenterService;

    public UserCenterController(IUserCenterService userCenterService)
    {
        _userCenterService = userCenterService;
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
    [Description("编辑个人信息")]
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
    [Description("更新签名")]
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
    [Description("编辑工作台")]
    public async Task updateUserWorkbench([FromBody] UpdateWorkbenchInput input)
    {
        await _userCenterService.UpdateWorkbench(input);
    }
}
