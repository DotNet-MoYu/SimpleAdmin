using IPTools.Core;
using SimpleAdmin.Plugin.Core;

namespace SimpleAdmin.System;

/// <summary>
/// 认证模块事件总线
/// </summary>
public class AuthEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly INamedServiceProvider<INoticeService> _namedServiceProvider;
    public IServiceProvider _services { get; }
    private readonly SqlSugarScope _db;

    public AuthEventSubscriber(ISimpleCacheService simpleCacheService, IServiceProvider services, INamedServiceProvider<INoticeService> namedServiceProvider)
    {
        _db = DbContext.Db;
        _simpleCacheService = simpleCacheService;
        _namedServiceProvider = namedServiceProvider;
        _services = services;
    }

    /// <summary>
    /// 登录事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.LoginB)]
    public async Task Login(EventHandlerExecutingContext context)
    {
        var loginEvent = (LoginEvent)context.Source.Payload;//获取参数
        var LoginAddress = GetLoginAddress(loginEvent.Ip);
        var sysUser = loginEvent.SysUser;

        #region 登录/密码策略

        var key = SystemConst.Cache_LoginErrorCount + sysUser.Account;//获取登录错误次数Key值
        _simpleCacheService.Remove(key);//移除登录错误次数

        // 创建新的作用域
        using var scope = _services.CreateScope();
        // 解析服务
        var configService = scope.ServiceProvider.GetRequiredService<IConfigService>();
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        var pwdRemindUpdateTime = sysUser.PwdRemindUpdateTime;//获取上次提醒修改密码时间
        var loginPolicy = await configService.GetListByCategory(CateGoryConst.Config_PWD_POLICY);//获取密码策略
        //获取用户token列表
        var tokenInfos = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.Cache_UserToken, sysUser.Id.ToString());
        var userToken = tokenInfos.Where(it => it.Token == loginEvent.Token).FirstOrDefault();
        if (userToken != null)
        {
            var subject = "请及时修改密码";
            //如果上次修改密码时间为空
            if (pwdRemindUpdateTime == null)
            {
                var pwdUpdateDefault = loginPolicy.First(x => x.ConfigKey == DevConfigConst.PWD_UPDATE_DEFAULT).ConfigValue.ToBoolean();//获取初始化提醒
                //如果密码初始化提醒为true
                if (pwdUpdateDefault)
                {
                    await messageService.Send(new MessageSendInput()
                    {
                        Subject = subject,
                        Content = $"请及时修改初始密码",
                        Category = CateGoryConst.Message_INFORM,
                        ReceiverIdList = new List<long>() { sysUser.Id }
                    });
                }
                sysUser.PwdRemindUpdateTime = DateTime.Now;//设置提醒时密码时间为当前时间
            }
            else
            {
                var pwdRemind = loginPolicy.First(x => x.ConfigKey == DevConfigConst.PWD_REMIND).ConfigValue.ToBoolean();//获取密码提醒天数
                if (pwdRemind)
                {
                    var pwdRemindDay = loginPolicy.First(x => x.ConfigKey == DevConfigConst.PWD_REMIND_DAY).ConfigValue.ToInt();//获取密码提醒时间
                    if (DateTime.Now - pwdRemindUpdateTime > TimeSpan.FromDays(pwdRemindDay))
                    {
                        await messageService.Send(new MessageSendInput()
                        {
                            Subject = subject,
                            Content = $"已超过{pwdRemindDay}天未修改密码,请及时修改密码",
                            Category = CateGoryConst.Message_INFORM,
                            ReceiverIdList = new List<long>() { sysUser.Id }
                        });
                    }
                    sysUser.PwdRemindUpdateTime = DateTime.Now;//设置提醒时密码时间为当前时间,避免重复提醒
                }
            }
        }

        #endregion

        #region 重新赋值属性,设置本次登录信息为最新的信息

        sysUser.LastLoginAddress = sysUser.LatestLoginAddress;
        sysUser.LastLoginDevice = sysUser.LatestLoginDevice;
        sysUser.LastLoginIp = sysUser.LatestLoginIp;
        sysUser.LastLoginTime = sysUser.LatestLoginTime;
        sysUser.LatestLoginAddress = LoginAddress;
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
            _simpleCacheService.HashAdd(SystemConst.Cache_SysUser, sysUser.Id.ToString(), sysUser);//更新Redis信息


        await Task.CompletedTask;
    }

    /// <summary>
    /// 登出事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.LoginOutB)]
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
            var ipInfo = IpTool.Search(ip);
            var LoginAddressList = new List<string>() { ipInfo.Country, ipInfo.Province, ipInfo.City, ipInfo.NetworkOperator };//定义登录地址列表
            var LoginAddress = string.Join("|", LoginAddressList.Where(it => it != "0").ToList());//过滤掉0的信息并用|连接成字符串
            return LoginAddress;
        }
        catch
        {
            return "未知";
        }
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