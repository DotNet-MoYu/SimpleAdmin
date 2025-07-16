// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.Mobile;

/// <summary>
/// 用户个人信息控制器
/// </summary>
[ApiDescriptionSettings("Mobile", Tag = "移动端用户个人信息控制器")]
[Route("mobile/UserCenter")]
public class MobileUserCenterController : IDynamicApiController
{
    private readonly IMobileUserCenterService _mobileUserCenterService;
    private readonly IUserCenterService _userCenterService;

    public MobileUserCenterController(IUserCenterService userCenterService, IMobileUserCenterService mobileUserCenterService)
    {
        _mobileUserCenterService = mobileUserCenterService;
        _userCenterService = userCenterService;
    }

    /// <summary>
    /// 获取个人菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginMobileMenu")]
    public async Task<dynamic> LoginMobileMenu()
    {
        return await _mobileUserCenterService.GetOwnMobileMenu();
    }


    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("getLoginUser")]
    public async Task<dynamic> GetLoginUser()
    {
        return await _mobileUserCenterService.GetLoginUser();
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
    public async Task<dynamic> LoginUnreadMessageDetail([FromQuery] MessageDetailInput input)
    {
        return await _userCenterService.LoginMessageDetail(input);
    }
}
