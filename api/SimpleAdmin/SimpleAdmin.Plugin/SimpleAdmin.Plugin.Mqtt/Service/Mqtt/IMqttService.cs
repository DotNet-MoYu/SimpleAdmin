namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
///Mqtt服务
/// </summary>
public interface IMqttService : ITransient
{
    /// <summary>
    /// mqtt登录http认证
    /// </summary>
    /// <param name="input">认证参数</param>
    /// <param name="userId">用户Id</param>
    /// <returns>认证结果</returns>
    Task<MqttAuthOutput> Auth(MqttAuthInput input);

    /// <summary>
    /// 获取mqtt登录web端参数
    /// </summary>
    /// <returns>登录参数</returns>
    Task<MqttParameterOutput> GetWebLoginParameter();
}
