using IPTools.Core;
using Masuit.Tools.DateTimeExt;
using Org.BouncyCastle.Crypto.Digests;
using UAParser;

namespace SimpleAdmin.System;

/// <summary>
/// 认证模块事件总线
/// </summary>
public class AuthEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly IRedisCacheManager _redisCacheManager;
    public IServiceProvider _services { get; }
    private readonly SqlSugarScope _db;
    public AuthEventSubscriber(IRedisCacheManager redisCacheManager, IServiceProvider services)
    {
        _db = DbContext.Db;
        this._redisCacheManager = redisCacheManager;
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
        var LoginAddress = GetLoginAddress(loginEvent.IpInfo);
        var sysUser = loginEvent.SysUser;
        #region 重新赋值属性,设置本次登录信息为最新的信息
        _db.Tracking(sysUser);//创建跟踪,只更新修改字段
        sysUser.LastLoginAddress = sysUser.LatestLoginAddress;
        sysUser.LastLoginDevice = sysUser.LatestLoginDevice;
        sysUser.LastLoginIp = sysUser.LatestLoginIp;
        sysUser.LastLoginTime = sysUser.LatestLoginTime;
        sysUser.LatestLoginAddress = LoginAddress;
        sysUser.LatestLoginDevice = loginEvent.Device.ToString();
        sysUser.LatestLoginIp = loginEvent.IpInfo.IpAddress;
        sysUser.LatestLoginTime = loginEvent.DateTime;
        #endregion
        //更新用户信息
        if (await _db.UpdateableWithAttr(sysUser).ExecuteCommandAsync() > 0)
            _redisCacheManager.HashAdd(RedisConst.Redis_SysUser, sysUser.Id.ToString(), sysUser); //更新Redis信息
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
        var loginEvent = (LoginEvent)context.Source.Payload;//获取参数

    }

    /// <summary>
    /// 解析IP地址
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    private string GetLoginAddress(IpInfo ipInfo)
    {
        try
        {
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
