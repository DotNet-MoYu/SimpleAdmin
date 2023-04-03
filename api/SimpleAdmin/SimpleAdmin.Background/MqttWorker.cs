

using Masuit.Tools;
using NewLife.MQTT;
using SimpleAdmin.Core;
using SimpleAdmin.Plugin.Cache;
using SimpleMQTT;

namespace SimpleAdmin.Background;

/// <summary>
/// mqtt后台任务
/// </summary>
public class MqttWorker : BackgroundService
{
    private readonly ILogger<MqttWorker> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly MqttClient _mqtt;

    public MqttWorker(ILogger<MqttWorker> logger, ISimpleCacheService simpleCacheService, IMqttClientManager mqttClientManager)
    {
        _logger = logger;
        this._simpleCacheService = simpleCacheService;
        this._mqtt = mqttClientManager.GetClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //订阅设备上下线主题
        await _mqtt.SubscribeAsync("$SYS/brokers/+/clients/+/+", (e) =>
        {
            var topicList = e.Topic.Split("/");//根据/分割
            var clientId = topicList[topicList.Length - 2];//获取客户端ID
            if (clientId.Contains("_"))//判断客户端ID是否有下划线有下划线表示是web用户登录
            {
                var userId = clientId.Split("_")[0];
                //获取redis当前用户的token信息列表
                var tokenInfos = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.Cache_UserToken, userId);
                if (tokenInfos != null)
                {
                    var connectEvent = topicList.Last();//获取连接事件判断上线还是下线
                    if (connectEvent == "connected")//如果是上线
                    {
                        _logger.LogInformation($"设备{clientId}上线了");
                        var token = _simpleCacheService.Get<string>(CacheConst.Cache_MqttClientUser + clientId);//获取mqtt客户端ID对应的用户token
                        if (token == null) return;//没有token就直接退出
                        //获取redis中当前token
                        var tokenInfo = tokenInfos.Where(it => it.Token == token).FirstOrDefault();
                        if (tokenInfo != null)
                        {
                            tokenInfo.ClientIds.Add(clientId);//添加到客户端列表
                            _simpleCacheService.HashAdd(CacheConst.Cache_UserToken, userId, tokenInfos);//更新Redis
                        }

                    }
                    else //下线
                    {
                        _logger.LogInformation($"设备{clientId}下线了");
                        //获取当前客户端ID所在的token信息
                        var tokenInfo = tokenInfos.Where(it => it.ClientIds.Contains(clientId)).FirstOrDefault();
                        if (tokenInfo != null)
                        {
                            tokenInfo.ClientIds.RemoveWhere(it => it == clientId);//从客户端列表删除
                            _simpleCacheService.HashAdd(CacheConst.Cache_UserToken, userId, tokenInfos);//更新Redis
                        }
                    }
                }
            }
        });
    }
}