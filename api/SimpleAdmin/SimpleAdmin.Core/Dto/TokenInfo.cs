namespace SimpleAdmin.Core;

/// <summary>
/// Token信息
/// </summary>
public class TokenInfo
{

    /// <summary>
    /// MQTT客户端ID列表
    /// </summary>
    public List<string> ClientIds { get; set; } = new List<string>();


    /// <summary>
    /// 设备
    /// </summary>
    public string Device { get; set; }


    /// <summary>
    /// 登录端
    /// </summary>
    public LoginClientTypeEnum LoginClientType { get; set; }


    /// <summary>
    /// 过期时间
    /// </summary>
    public int Expire { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 超时时间
    /// </summary>
    public DateTime TokenTimeout { get; set; }

    /// <summary>
    /// token剩余有效期
    /// </summary>
    public string TokenRemain { get; set; }

    /// <summary>
    /// token剩余有效期百分比
    /// </summary>
    public double TokenRemainPercent { get; set; }
}
