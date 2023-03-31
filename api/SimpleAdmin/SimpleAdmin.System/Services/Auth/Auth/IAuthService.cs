namespace SimpleAdmin.System;

/// <summary>
/// 权限校验服务
/// </summary>
public interface IAuthService : ITransient
{

    /// <summary>
    /// 获取图形验证码
    /// </summary>
    /// <returns></returns>
    PicValidCodeOutPut GetCaptchaInfo();

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    Task<LoginUserOutput> GetLoginUser();

    /// <summary>
    /// 获取手机短信验证码
    /// </summary>
    /// <param name="input">验证码参数</param>
    /// <param name="loginClientType">登录类型</param>
    /// <returns></returns>
    Task<string> GetPhoneValidCode(GetPhoneValidCodeInput input, LoginClientTypeEnum loginClientType);

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="input">登录参数</param>
    /// <param name="loginClientType">登录类型</param>
    /// <returns>Token信息</returns>
    Task<LoginOutPut> Login(LoginInput input, LoginClientTypeEnum loginClientType);

    /// <summary>
    /// 手机号登录
    /// </summary>
    /// <param name="input">登录参数</param>
    /// <param name="loginClientType">登录端类型</param>
    /// <returns>Token信息</returns>
    Task<LoginOutPut> LoginByPhone(LoginByPhoneInput input, LoginClientTypeEnum loginClientType);

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <param name="token">token</param>
    /// <param name="loginClientType">登出类型</param>
    /// <returns></returns>
    Task LoginOut(string token, LoginClientTypeEnum loginClientType);
}
