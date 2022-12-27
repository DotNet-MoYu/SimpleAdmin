namespace SimpleAdmin.System;

/// <summary>
///Mqtt服务
/// </summary>
public interface IMqttService : ITransient
{
    /// <summary>
    /// mqtt登录http认证
    /// </summary>
    /// <param name="input">认证参数</param>
    /// <returns>认证结果</returns>
    Task<MqttAuthOutput> Auth(MqttAuthInput input);

    /// <summary>
    /// 获取mqtt登录参数
    /// </summary>
    /// <returns>登录参数</returns>
    Task<MqttParameterOutput> GetParameter();

    /// <summary>
    /// 注销登录
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="clienIds">客户端ID列表</param>
    /// <param name="message">内容</param>
    /// <returns></returns>
    Task LoginOut(string userId, List<string> clienIds, string message);
}
