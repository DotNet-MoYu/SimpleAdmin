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
    public async Task UpdateUserInfo([FromBody] UserUpdateInfoInput input)
    {
        await _userCenterService.UpdateUserInfo(input);
    }
}
