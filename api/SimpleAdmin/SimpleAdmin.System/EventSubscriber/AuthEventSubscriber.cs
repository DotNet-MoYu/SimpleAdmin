// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。


using IP2Region.Net.Abstractions;

namespace SimpleAdmin.System;

/// <summary>
/// 认证模块事件总线
/// </summary>
public class AuthEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly ISearcher _searcher;

    public IServiceProvider Services { get; }
    private readonly SqlSugarScope _db;

    public AuthEventSubscriber(ISimpleCacheService simpleCacheService, IServiceProvider services, ISearcher searcher)
    {
        _db = DbContext.DB;
        _simpleCacheService = simpleCacheService;
        _searcher = searcher;
        Services = services;
    }

    /// <summary>
    /// 登录事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.LOGIN_B)]
    public async Task Login(EventHandlerExecutingContext context)
    {
        var loginEvent = (LoginEvent)context.Source.Payload;//获取参数
        var loginAddress = GetLoginAddress(loginEvent.Ip);
        var sysUser = loginEvent.SysUser;

        #region 登录/密码策略

        var key = SystemConst.CACHE_LOGIN_ERROR_COUNT + sysUser.Account;//获取登录错误次数Key值
        _simpleCacheService.Remove(key);//移除登录错误次数

        // 创建新的作用域
        using var scope = Services.CreateScope();
        // 解析服务
        var configService = scope.ServiceProvider.GetRequiredService<IConfigService>();
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        var pwdRemindUpdateTime = sysUser.PwdRemindUpdateTime;//获取上次提醒修改密码时间
        var loginPolicy = await configService.GetConfigsByCategory(CateGoryConst.CONFIG_PWD_POLICY);//获取密码策略
        //获取用户token列表
        var tokenInfos = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.CACHE_USER_TOKEN, sysUser.Id.ToString());
        var userToken = tokenInfos.Where(it => it.Token == loginEvent.Token).FirstOrDefault();
        if (userToken != null)
        {
            var subject = "请及时修改密码";
            //如果上次修改密码时间为空
            if (pwdRemindUpdateTime == null)
            {
                var pwdUpdateDefault = loginPolicy.First(x => x.ConfigKey == SysConfigConst.PWD_UPDATE_DEFAULT).ConfigValue.ToBoolean();//获取初始化提醒
                //如果密码初始化提醒为true
                if (pwdUpdateDefault)
                {
                    await messageService.Send(new MessageSendInput
                    {
                        Subject = subject,
                        Content = "请及时修改初始密码",
                        Category = CateGoryConst.MESSAGE_INFORM,
                        ReceiverIdList = new List<long> { sysUser.Id }
                    });
                }
                sysUser.PwdRemindUpdateTime = DateTime.Now;//设置提醒时密码时间为当前时间
            }
            else
            {
                var pwdRemind = loginPolicy.First(x => x.ConfigKey == SysConfigConst.PWD_REMIND).ConfigValue.ToBoolean();//获取密码提醒天数
                if (pwdRemind)
                {
                    var pwdRemindDay = loginPolicy.First(x => x.ConfigKey == SysConfigConst.PWD_REMIND_DAY).ConfigValue.ToInt();//获取密码提醒时间
                    if (DateTime.Now - pwdRemindUpdateTime > TimeSpan.FromDays(pwdRemindDay))
                    {
                        await messageService.Send(new MessageSendInput
                        {
                            Subject = subject,
                            Content = $"已超过{pwdRemindDay}天未修改密码,请及时修改密码",
                            Category = CateGoryConst.MESSAGE_INFORM,
                            ReceiverIdList = new List<long> { sysUser.Id }
                        });
                    }
                    sysUser.PwdRemindUpdateTime = DateTime.Now;//设置提醒时密码时间为当前时间,避免重复提醒
                }
            }
        }

        #endregion 登录/密码策略

        #region 重新赋值属性,设置本次登录信息为最新的信息

        sysUser.LastLoginAddress = sysUser.LatestLoginAddress;
        sysUser.LastLoginDevice = sysUser.LatestLoginDevice;
        sysUser.LastLoginIp = sysUser.LatestLoginIp;
        sysUser.LastLoginTime = sysUser.LatestLoginTime;
        sysUser.LatestLoginAddress = loginAddress;
        sysUser.LatestLoginDevice = loginEvent.Device.ToString();
        sysUser.LatestLoginIp = loginEvent.Ip;
        sysUser.LatestLoginTime = loginEvent.DateTime;

        #endregion 重新赋值属性,设置本次登录信息为最新的信息

        //更新用户登录信息
        if (await _db.UpdateableWithAttr(sysUser).UpdateColumns(it => new
        {
            it.LastLoginAddress,
            it.LastLoginDevice,
            it.LastLoginIp,
            it.LastLoginTime,
            it.LatestLoginAddress,
            it.LatestLoginDevice,
            it.LatestLoginIp,
            it.LatestLoginTime,
            LastUpdatePwdTime = it.PwdRemindUpdateTime
        }).ExecuteCommandAsync() > 0)
            _simpleCacheService.HashAdd(SystemConst.CACHE_SYS_USER, sysUser.Id.ToString(), sysUser);//更新Redis信息

        await Task.CompletedTask;
    }

    /// <summary>
    /// 登出事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.LOGIN_OUT_B)]
    public async Task LoginOut(EventHandlerExecutingContext context)
    {
        _ = (LoginEvent)context.Source.Payload;//获取参数
    }

    /// <summary>
    /// 解析IP地址
    /// </summary>
    /// <param name="ip">ip地址</param>
    /// <returns></returns>
    private string GetLoginAddress(string ip)
    {
        try
        {
            var ipInfo = _searcher.Search(ip).Replace("0|", "");//解析ip并格式化
            return ipInfo;
        }
        catch
        {
            return "未知";
        }
    }
}
