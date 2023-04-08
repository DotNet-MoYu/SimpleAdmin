

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

        var loginEvent = (NoticeEvent)context.Source.Payload;//获取参数
        //客户端ID列表
        var clientIds = new List<string>();
        //遍历token列表获取客户端ID列表
        loginEvent.TokenInfos.ForEach(it =>
        {
            clientIds.AddRange(it.ClientIds);
        });
        // 获取插件选项
        var pluginsOptions = App.GetOptions<PluginSettingsOptions>();
        //根据通知类型获取对应的服务
        var noticeComponent = pluginsOptions.NoticeComponent.ToString().ToLower();
        var noticeService = _namedServiceProvider.GetService<ISingleton>(noticeComponent);//获取服务
        await noticeService.UserLoginOut(loginEvent.UserId, clientIds, loginEvent.Message);//发送消息

    }





    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T GetService<T>()
    {
        // 创建新的作用域
        using var scope = _scopeFactory.CreateScope();
        // 解析服务
        T scopedProcessingService = scope.ServiceProvider.GetRequiredService<T>();
        return scopedProcessingService;
    }
}
