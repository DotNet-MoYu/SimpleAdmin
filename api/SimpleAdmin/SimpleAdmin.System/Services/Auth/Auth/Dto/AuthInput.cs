// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

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
    public AuthDeviceTypeEnum Device { get; set; } = AuthDeviceTypeEnum.PC;

    /// <summary>
    /// 租户ID
    /// </summary>
    public long? TenantId { get; set; }
}

/// <summary>
/// 登出输入参数
/// </summary>
public class LoginOutInput
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
    public AuthDeviceTypeEnum Device { get; set; } = AuthDeviceTypeEnum.PC;

    /// <summary>
    /// 租户ID
    /// </summary>
    public long? TenantId { get; set; }
}
