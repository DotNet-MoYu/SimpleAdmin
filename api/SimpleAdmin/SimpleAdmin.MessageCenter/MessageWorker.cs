using NewLife.MQTT;
using SimpleAdmin.Cache;
using SimpleAdmin.Core;
using SimpleAdmin.SqlSugar;
using SimpleAdmin.System;

namespace SimpleAdmin.MessageCenter;

public class MessageWorker : BackgroundService
{
    private readonly ILogger<MessageWorker> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IMqttClientManager _mqttClientManager;
    private readonly MqttClient _mqttClient;

    public MessageWorker(ILogger<MessageWorker> logger, ISimpleCacheService simpleCacheService, IMqttClientManager mqttClientManager)
    {
        _logger = logger;
        _simpleCacheService = simpleCacheService;
        _mqttClientManager = mqttClientManager;
        _mqttClient = mqttClientManager.GetClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //��ȡ�ӳٶ���
        var queue = _simpleCacheService.GetDelayQueue<long>(CacheConst.CACHE_NOTIFICATION);
        while (!stoppingToken.IsCancellationRequested)
        {
            //һ����ʮ���������һ������queue.TakeOneAsync(-1);-1�ǳ�ʱʱ�䣬Ĭ��0����Զ������������ʾֱ�ӷ��أ���������
            var data = await queue.TakeOneAsync(-1);
            if (data != 0)
            {
                _logger.LogDebug($"�������յ���Ϣ,��ϢID:{data},ʱ�䣺{DateTime.Now}");
                var db = DbContext.DB.CopyNew();
                //��ȡ��Ϣ
                var message = await db.Queryable<SysMessage>().InSingleAsync(data);
                if (message != null)
                {
                    message.Status = SysDictConst.MESSAGE_STATUS_ALREADY;
                    //��ȡ�����͵���Ϣ
                    var messageUsers = await db.Queryable<SysMessageUser>()
                        .Where(it => it.MessageId == message.Id && it.Status == SysDictConst.MESSAGE_STATUS_READY).ToListAsync();
                    var hasError = false;
                    //��������
                    var result = await db.UseTranAsync(async () =>
                    {
                        messageUsers.ForEach(it =>
                        {
                            try
                            {
                                //������Ϣ
                                _mqttClient.PublishAsync(MqttConst.MQTT_TOPIC_PREFIX + it.UserId, new MqttMessage()
                                {
                                    MsgType = MqttConst.MQTT_MESSAGE_NEW,
                                    Data = new MessageData()
                                    {
                                        Subject = message.Subject,
                                        Content = message.Content
                                    }
                                });
                                it.Status = SysDictConst.MESSAGE_STATUS_ALREADY;
                                it.UpdateTime = DateTime.Now;
                            }
                            catch (Exception e)
                            {
                                hasError = true;
                                _logger.LogError($"������Ϣʧ��:{e.Message}");
                            }
                        });
                        await db.Updateable(messageUsers).ExecuteCommandAsync();
                        await db.Updateable(message).ExecuteCommandAsync();
                    });
                    //�����ʧ�ܵģ���д�����ӳٶ���
                    if (hasError)
                    {
                        _logger.LogDebug($"��ʧ�ܵ���Ϣ�����·����ӳٶ���");
                        queue.Add(message.Id, 5);
                    }
                }
                queue.Acknowledge(data);//���߶����Ѿ������˵�����
                _logger.LogDebug("���ѳɹ�");
            }
            else
            {
                _logger.LogDebug("�����ߴӶ�����û���õ�����:" + DateTime.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
