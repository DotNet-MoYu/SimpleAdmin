namespace SimpleAdmin.System;

/// <summary>
/// 通知事件总线
/// </summary>
public class NoticeEventSubsciber : IEventSubscriber, ISingleton
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly INamedServiceProvider<INoticeService> _namedServiceProvider;

    public IServiceScopeFactory _scopeFactory { get; }
    private readonly SqlSugarScope _db;

    public NoticeEventSubsciber(ISimpleCacheService simpleCacheService, IServiceScopeFactory scopeFactory, INamedServiceProvider<INoticeService> namedServiceProvider)
    {
        _db = DbContext.Db;
        this._simpleCacheService = simpleCacheService;
        this._scopeFactory = scopeFactory;
        this._namedServiceProvider = namedServiceProvider;
    }

    /// <summary>
    /// 通知用户下线事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.UserLoginOut)]
    public async Task UserLoginOut(EventHandlerExecutingContext context)
    {
        var loginEvent = (UserLoginOutEvent)context.Source.Payload;//获取参数
        //客户端ID列表
        var clientIds = new List<string>();
        //遍历token列表获取客户端ID列表
        loginEvent.TokenInfos.ForEach(it =>
        {
            clientIds.AddRange(it.ClientIds);
        });
        await GetNoticeService().UserLoginOut(loginEvent.UserId, clientIds, loginEvent.Message);//发送消息
    }

    /// <summary>
    /// 有新的消息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.NewMessage)]
    public async Task NewMessage(EventHandlerExecutingContext context)
    {
        var newMessageEvent = (NewMessageEvent)context.Source.Payload;//获取参数

        var clientIds = new List<string>();
        //获取用户token列表
        var tokenInfos = _simpleCacheService.HashGet<List<TokenInfo>>(CacheConst.Cache_UserToken, newMessageEvent.UserIds.ToArray());
        tokenInfos.ForEach(it =>
        {
            if (it != null)
            {
                it = it.Where(it => it.TokenTimeout > DateTime.Now).ToList();//去掉登录超时的
                //遍历token
                it.ForEach(it =>
                {
                    clientIds.AddRange(it.ClientIds);//添加到客户端ID列表
                });
            }
        });

        await GetNoticeService().NewMesage(newMessageEvent.UserIds, clientIds, newMessageEvent.Message);//发送消息
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T GetService<T>() where T : class, new()
    {
        // 创建新的作用域
        using var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 获取通知服务
    /// </summary>
    /// <returns></returns>
    private INoticeService GetNoticeService()
    {
        // 获取插件选项
        var pluginsOptions = App.GetOptions<PluginSettingsOptions>();
        //根据通知类型获取对应的服务
        var noticeComponent = pluginsOptions.NoticeComponent.ToString().ToLower();
        var noticeService = _namedServiceProvider.GetService<ISingleton>(noticeComponent);//获取服务
        return noticeService;
    }
}