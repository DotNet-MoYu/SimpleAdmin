

namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// <inheritdoc cref="INoticeService"/>
/// </summary>
[Injection(Named = "mqtt")]
public class MqttNoticeService : INoticeService
{


    public MqttNoticeService()
    {

    }

    /// <inheritdoc/>
    public async Task UserLoginOut(string userId, List<string> clientIds, string message)
    {
        var _mqttClientManager = GetMqttClientManager();
        //发送通知下线消息
        await _mqttClientManager.GetClient().PublishAsync(MqttConst.Mqtt_TopicPrefix + userId, new MqttMessage
        {
            Data = new { Message = message, ClientIds = clientIds },
            MsgType = MqttConst.Mqtt_Message_LoginOut
        });
    }

    /// <summary>
    /// 获取hubContext
    /// </summary>
    /// <returns></returns>
    private IMqttClientManager GetMqttClientManager()
    {
        //解析服务
        var service = App.GetService<IMqttClientManager>();
        return service;
    }
}
