namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// mqtt常量
/// </summary>
public class MqttConst
{
    /// <summary>
    /// mqtt认证登录信息key
    /// </summary>
    public const string CACHE_MQTT_CLIENT_USER = CacheConst.CACHE_PREFIX_WEB + "MqttClientUser:";

    /// <summary>
    /// mqtt主题前缀
    /// </summary>
    public const string MQTT_TOPIC_PREFIX = "SimpleAdmin/";

    /// <summary>
    /// 登出
    /// </summary>
    public const string MQTT_MESSAGE_LOGIN_OUT = "LoginOut";

    /// <summary>
    /// 新消息
    /// </summary>
    public const string MQTT_MESSAGE_NEW = "NewMessage";

    /// <summary>
    /// 修改密码
    /// </summary>
    public const string MQTT_MESSAGE_UPDATE_PASSWORD = "UpdatePassword";
}
