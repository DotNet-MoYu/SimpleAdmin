namespace SimpleAdmin.Web.Core;

/// <summary>
/// B端登录控制器
/// </summary>
[ApiDescriptionSettings(Tag = "B端权限校验")]
[Route("auth/b")]
public class AuthBController : IDynamicApiController
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
    public dynamic GetPicCaptcha()
    {
        return _authService.GetCaptchaInfo();
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
    [DisplayName(EventSubscriberConst.LoginB)]
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
    [HttpPost("loginOut")]
    [DisplayName(EventSubscriberConst.LoginOutB)]
    public async Task LoginOut(LoginOutIput input)
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
