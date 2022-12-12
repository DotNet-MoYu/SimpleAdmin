using IPTools.Core;
using UAParser;

namespace SimpleAdmin.System;

/// <summary>
/// 认证模块事件总线
/// </summary>
public class AuthEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly IRedisCacheManager _redisCacheManager;

    private readonly SqlSugarScope _db;
    public AuthEventSubscriber(IRedisCacheManager redisCacheManager)
    {
        _db = DbContext.Db;
        this._redisCacheManager = redisCacheManager;
    }

    /// <summary>
    /// 登录事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.Login)]
    public async Task Login(EventHandlerExecutingContext context)
    {
        var loginEvent = (LoginEvent)context.Source.Payload;//获取参数
        //var clientInfo = loginEvent.ClientInfo;//获取上下文
        var LoginAddress = GetLoginAddress(loginEvent.Ip);
        var sysUser = loginEvent.SysUser;
        #region 重新赋值属性,设置本次登录信息为最新的信息
        _db.Tracking(sysUser);//创建跟踪,只更新修改字段
        sysUser.LastLoginAddress = sysUser.LatestLoginAddress;
        sysUser.LastLoginDevice = sysUser.LatestLoginDevice;
        sysUser.LastLoginIp = sysUser.LatestLoginIp;
        sysUser.LastLoginTime = sysUser.LatestLoginTime;
        sysUser.LatestLoginAddress = LoginAddress;
        sysUser.LatestLoginDevice = loginEvent.Device.ToString();
        sysUser.LatestLoginIp = loginEvent.Ip;
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
    [EventSubscribe(EventSubscriberConst.LoginOut)]
    public async Task LoginOut(EventHandlerExecutingContext context)
    {
        var loginEvent = (LoginEvent)context.Source.Payload;//获取参数
        var clientInfo = loginEvent.ClientInfo;//获取上下文


    }

    /// <summary>
    /// 解析IP地址
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    private string GetLoginAddress(string ip)
    {
        try
        {
            var ipInfo = IpTool.Search(ip);//解析IP信息
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
