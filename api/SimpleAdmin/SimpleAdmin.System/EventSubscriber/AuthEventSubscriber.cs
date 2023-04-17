using IPTools.Core;

namespace SimpleAdmin.System;

/// <summary>
/// 认证模块事件总线
/// </summary>
public class AuthEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly ISimpleCacheService _simpleCacheService;
    public IServiceProvider _services { get; }
    private readonly SqlSugarScope _db;

    public AuthEventSubscriber(ISimpleCacheService simpleCacheService, IServiceProvider services)
    {
        _db = DbContext.Db;
        this._simpleCacheService = simpleCacheService;
        this._services = services;
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
        }).ExecuteCommandAsync() > 0)
            _simpleCacheService.HashAdd(CacheConst.Cache_SysUser, sysUser.Id.ToString(), sysUser); //更新Redis信息
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
            List<string> LoginAddressList = new List<string>() { ipInfo.Country, ipInfo.Province, ipInfo.City, ipInfo.NetworkOperator };//定义登录地址列表
            var LoginAddress = string.Join("|", LoginAddressList.Where(it => it != "0").ToList());//过滤掉0的信息并用|连接成字符串
            return LoginAddress;
        }
        catch
        {
            return "未知";
        }
    }
}