namespace SimpleAdmin.System;

/// <summary>
/// 权限认证输入
/// </summary>
public class AuthInput
{
}

public class ValidCodeInput
{
    /// <summary>
    /// 验证码
    /// </summary>
    public string ValidCode { get; set; }

    /// <summary>
    /// 请求号
    /// </summary>
    public string ValidCodeReqNo { get; set; }
}

/// <summary>
/// 登录输入参数
/// </summary>
public class LoginInput : ValidCodeInput
{
    /// <summary>
    /// 账号
    ///</summary>
    /// <example>superAdmin</example>
    [Required(ErrorMessage = "账号不能为空")]
    public string Account { get; set; }

    /// <summary>
    /// 密码
    ///</summary>
    ///<example>04fc514b346f14b23d7cf5e6f64663b030512aa380a9e7d311288ed1e8be7b863ae5ee0bb570df2405fc9daff2b2d1ac627a0fbbd49ef2c6b8fac4fd5e4b9a1b7120999bdc0a8e425aa37abab3aec6f9f3570775ff901f2845e957b0c2d0542e21fbf1bbb65c04</example>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; }

    /// <summary>
    /// 设备类型，默认PC
    /// </summary>
    /// <example>0</example>
    public AuthDeviceTypeEumu Device { get; set; } = AuthDeviceTypeEumu.PC;
}

/// <summary>
/// 登出输入参数
/// </summary>
public class LoginOutIput
{
    /// <summary>
    /// token
    /// </summary>
    public string Token { get; set; }
}

/// <summary>
/// 获取短信验证码输入
/// </summary>
public class GetPhoneValidCodeInput : ValidCodeInput
{
    /// <summary>
    /// 手机号
    /// </summary>
    [Required(ErrorMessage = "手机号不能为空")]
    [Phone(ErrorMessage = "手机号格式不对")]
    public string Phone { get; set; }
}

/// <summary>
/// 手机号登录输入
/// </summary>
public class LoginByPhoneInput : GetPhoneValidCodeInput
{
    /// <summary>
    /// 设备类型，默认PC
    /// </summary>
    /// <example>0</example>
    public AuthDeviceTypeEumu Device { get; set; } = AuthDeviceTypeEumu.PC;
}