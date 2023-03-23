

namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// <inheritdoc cref="INoticeService"/>
/// </summary>
[Injection(Named = "mqtt")]
public class MqttNoticeService : INoticeService
{
    private readonly IMqttClientManager _mqttClientManager;

    public MqttNoticeService(IMqttClientManager mqttClientManager)
    {
        this._mqttClientManager = mqttClientManager;
    }

    /// <inheritdoc/>
    public async Task UserLoginOut(string userId, List<string> clientIds, string message)
    {
        //发送通知下线消息
        await _mqttClientManager.GetClient().PublishAsync(MqttConst.Mqtt_TopicPrefix + userId, new MqttMessage
        {
            Data = new { Message = message, ClientIds = clientIds },
            MsgType = MqttConst.Mqtt_Message_LoginOut
        });
    }
}
