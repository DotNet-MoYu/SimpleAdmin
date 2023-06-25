namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// mqtt常量
/// </summary>
public class MqttConst
{
    /// <summary>
    /// mqtt认证登录信息key
    /// </summary>
    public const string Cache_MqttClientUser = CacheConst.Cache_Prefix_Web + "MqttClientUser:";

    /// <summary>
    /// mqtt主题前缀
    /// </summary>
    public const string Mqtt_TopicPrefix = "SimpleAdmin/";

    /// <summary>
    /// 登出
    /// </summary>
    public const string Mqtt_Message_LoginOut = "LoginOut";

    /// <summary>
    /// 新消息
    /// </summary>
    public const string Mqtt_Message_New = "NewMessage";

    /// <summary>
    /// 修改密码
    /// </summary>
    public const string Mqtt_Message_UpdatePassword = "UpdatePassword";
}