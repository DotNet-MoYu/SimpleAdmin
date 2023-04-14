namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// mqtt认证参数
/// </summary>
public class MqttAuthInput
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; }
}