using IPTools.Core;
using UAParser;

namespace SimpleAdmin.System;

/// <summary>
/// 登录事件参数
/// </summary>
public class LoginEvent
{

    /// <summary>
    /// 请求上下文
    /// </summary>
    public ClientInfo ClientInfo { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public SysUser SysUser { get; set; }


    /// <summary>
    /// 地址
    /// </summary>
    public IpInfo IpInfo { get; set; }

    /// <summary>
    /// 登录设备
    /// </summary>
    public AuthDeviceTypeEumu Device { get; set; }


    /// <summary>
    /// Tokens
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public int Expire { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    public DateTime DateTime = DateTime.Now;
}
