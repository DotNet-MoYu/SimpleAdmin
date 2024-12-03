namespace SimpleAdmin.Plugin.Mqtt;

public class MqttNoticeService
{
    public MqttNoticeService()
    {
    }

    /// <inheritdoc/>
    public async Task UpdatePassWord(string userId, List<string> clientIds, string message)
    {
        var _mqttClientManager = GetMqttClientManager();
        //发送修改密码消息
        await _mqttClientManager.GetClient().PublishAsync(MqttConst.MQTT_TOPIC_PREFIX + userId, new MqttMessage
        {
            Data = new { Message = message, ClientIds = clientIds },
            MsgType = MqttConst.MQTT_MESSAGE_UPDATE_PASSWORD
        });
    }

    /// <inheritdoc/>
    public async Task NewMesage(List<string> userIds, List<string> clientIds, string message)
    {
        var _mqttClientManager = GetMqttClientManager();
        //遍历用户Id
        foreach (var userId in userIds)
        {
            //发送消息
            await _mqttClientManager.GetClient().PublishAsync(MqttConst.MQTT_TOPIC_PREFIX + userId, new MqttMessage
            {
                Data = new { Message = message },
                MsgType = MqttConst.MQTT_MESSAGE_NEW
            });
        }
    }

    /// <inheritdoc/>
    public async Task UserLoginOut(string userId, List<string> clientIds, string message)
    {
        var _mqttClientManager = GetMqttClientManager();
        //发送通知下线消息
        await _mqttClientManager.GetClient().PublishAsync(MqttConst.MQTT_TOPIC_PREFIX + userId, new MqttMessage
        {
            Data = new { Message = message, ClientIds = clientIds },
            MsgType = MqttConst.MQTT_MESSAGE_LOGIN_OUT
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
