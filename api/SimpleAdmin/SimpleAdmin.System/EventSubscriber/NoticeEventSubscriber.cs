// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 通知事件总线
/// </summary>
public class NoticeEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly ISimpleCacheService _simpleCacheService;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SqlSugarScope _db;

    public NoticeEventSubscriber(ISimpleCacheService simpleCacheService, IServiceScopeFactory scopeFactory)
    {
        _db = DbContext.DB;
        _simpleCacheService = simpleCacheService;
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// 通知用户下线事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.USER_LOGIN_OUT)]
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
    }

    /// <summary>
    /// 有新的消息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.NEW_MESSAGE)]
    public async Task NewMessage(EventHandlerExecutingContext context)
    {
        var newMessageEvent = (NewMessageEvent)context.Source.Payload;//获取参数

        var clientIds = new List<string>();
        //获取用户token列表
        var tokenInfos = _simpleCacheService.HashGet<List<TokenInfo>>(CacheConst.CACHE_USER_TOKEN, newMessageEvent.UserIds.ToArray());
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
}
