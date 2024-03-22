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
/// B端登录控制器
/// </summary>
[ApiDescriptionSettings(Tag = "B端权限校验")]
[Route("sys/auth/b")]
public class AuthBController : BaseController
{
    private readonly IAuthService _authService;

    public AuthBController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// B端获取图片验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet("getPicCaptcha")]
    [AllowAnonymous]
    public async Task<dynamic> GetPicCaptcha()
    {
        return await _authService.GetCaptchaInfo();
    }

    /// <summary>
    /// B端获取手机验证码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("getPhoneValidCode")]
    [AllowAnonymous]
    public async Task<dynamic> GetPhoneValidCode([FromQuery] GetPhoneValidCodeInput input)
    {
        return await _authService.GetPhoneValidCode(input, LoginClientTypeEnum.B);
    }

    /// <summary>
    /// B端登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [DisplayName(EventSubscriberConst.LOGIN_B)]
    public async Task<dynamic> Login(LoginInput input)
    {
        return await _authService.Login(input, LoginClientTypeEnum.B);
    }

    /// <summary>
    /// B端手机号登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("loginByPhone")]
    public async Task<dynamic> LoginByPhone(LoginByPhoneInput input)
    {
        return await _authService.LoginByPhone(input, LoginClientTypeEnum.B);
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    [DisplayName(EventSubscriberConst.LOGIN_OUT_B)]
    public async Task LoginOut([FromBody] LoginOutInput input)
    {
        await _authService.LoginOut(input.Token, LoginClientTypeEnum.B);
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("getLoginUser")]
    public async Task<dynamic> GetLoginUser()
    {
        return await _authService.GetLoginUser();
    }
}
