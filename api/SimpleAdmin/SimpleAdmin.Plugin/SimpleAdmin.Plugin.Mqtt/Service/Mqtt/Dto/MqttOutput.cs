namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// mqtt登录参数输出
/// </summary>
public class MqttParameterOutput
{
    /// <summary>
    /// 地址
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>

    public string Password { get; set; }

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 主题列表
    /// </summary>
    public List<string> Topics { get; set; }
}

/// <summary>
/// mqtt认证输出
/// </summary>
public class MqttAuthOutput
{
    /// <summary>
    /// 结果 "allow" | "deny" | "ignore", // Default `"ignore"`
    /// </summary>
    public string Result { get; set; } = "deny";

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public bool Is_superuser { get; set; } = false;
}