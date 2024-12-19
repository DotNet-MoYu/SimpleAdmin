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
        //获取延迟队列
        var queue = _simpleCacheService.GetDelayQueue<long>(CacheConst.CACHE_NOTIFICATION);
        while (!stoppingToken.IsCancellationRequested)
        {
            //一次拿十条，如果拿一条就用queue.TakeOneAsync(-1);-1是超时时间，默认0秒永远阻塞；负数表示直接返回，不阻塞。
            var data = await queue.TakeOneAsync(-1);
            if (data != 0)
            {
                _logger.LogDebug($"消费者收到消息,消息ID:{data},时间：{DateTime.Now}");
                var db = DbContext.DB.CopyNew();
                //获取消息
                var message = await db.Queryable<SysMessage>().InSingleAsync(data);
                if (message != null)
                {
                    message.Status = SysDictConst.MESSAGE_STATUS_ALREADY;
                    //获取待发送的消息
                    var messageUsers = await db.Queryable<SysMessageUser>()
                        .Where(it => it.MessageId == message.Id && it.Status == SysDictConst.MESSAGE_STATUS_READY).ToListAsync();
                    var hasError = false;
                    //开启事务
                    var result = await db.UseTranAsync(async () =>
                    {
                        messageUsers.ForEach(it =>
                        {
                            try
                            {
                                //发送消息
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
                                _logger.LogError($"发送消息失败:{e.Message}");
                            }
                        });
                        await db.Updateable(messageUsers).ExecuteCommandAsync();
                        await db.Updateable(message).ExecuteCommandAsync();
                    });
                    //如果有失败的，重写发到延迟队列
                    if (hasError)
                    {
                        _logger.LogDebug($"有失败的消息，重新发到延迟队列");
                        queue.Add(message.Id, 5);
                    }
                }
                queue.Acknowledge(data);//告诉队列已经消费了的数据
                _logger.LogDebug("消费成功");
            }
            else
            {
                _logger.LogDebug("消费者从队列中没有拿到数据:" + DateTime.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
