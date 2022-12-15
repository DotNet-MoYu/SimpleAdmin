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
        {
            _redisCacheManager.HashAdd(RedisConst.Redis_SysUser, sysUser.Id.ToString(), sysUser); //更新Redis信息
            WriteTokenToRedis(loginEvent, LoginClientTypeEnum.B);//写入token到redis
        }
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
        RemoveTokenFromRedis(loginEvent, LoginClientTypeEnum.B);//删除用户token

    }

    /// <summary>
    /// 写入用户token到redis
    /// </summary>
    /// <param name="loginEvent">登录事件参数</param>
    /// <param name="loginClientType">登录类型</param>
    private void WriteTokenToRedis(LoginEvent loginEvent, LoginClientTypeEnum loginClientType)
    {
        var key = loginClientType == LoginClientTypeEnum.B ? RedisConst.Redis_UserTokenB : RedisConst.Redis_UserTokenC;
        //获取token列表
        List<TokenInfo> tokenInfos = GetTokenInfos(loginEvent.SysUser.Id);
        var tokenTimeout = loginEvent.DateTime.AddMinutes(loginEvent.Expire);
        //生成token信息
        var tokenInfo = new TokenInfo
        {
            Device = loginEvent.Device.ToString(),
            Expire = loginEvent.Expire,
            TokenTimeout = tokenTimeout,
            Token = loginEvent.Token,
        };
        //判断是否单点登录
        var a = false;
        if (a)
        {
            tokenInfos = new List<TokenInfo> { tokenInfo };//直接就一个
        }
        else
        {
            //判断是否是空的
            if (tokenInfos != null)
            {
                tokenInfos.Add(tokenInfo);
            }
            else
            {
                tokenInfos = new List<TokenInfo> { tokenInfo };//直接就一个
            }

        }
        //添加到token列表
        _redisCacheManager.HashAdd(key, loginEvent.SysUser.Id.ToString(), tokenInfos);
    }

    /// <summary>
    /// redis删除用户token
    /// </summary>
    /// <param name="loginEvent">登录事件参数</param>
    /// <param name="loginClientType">登录类型</param>
    private void RemoveTokenFromRedis(LoginEvent loginEvent, LoginClientTypeEnum loginClientType)
    {
        var key = loginClientType == LoginClientTypeEnum.B ? RedisConst.Redis_UserTokenB : RedisConst.Redis_UserTokenC;
        //获取token列表
        List<TokenInfo> tokenInfos = GetTokenInfos(loginEvent.SysUser.Id);
        if (tokenInfos != null)
        {
            //获取当前用户的token
            var token = tokenInfos.Where(it => it.Token == loginEvent.Token).FirstOrDefault();
            if (token != null)
                tokenInfos.Remove(token);
            if (tokenInfos.Count > 0)
            {
                //更新token列表
                _redisCacheManager.HashAdd(key, loginEvent.SysUser.Id.ToString(), tokenInfos);
            }
            else
            {
                //从列表中删除
                _redisCacheManager.HashDel<List<TokenInfo>>(key, new string[] { loginEvent.SysUser.Id.ToString() });
            }
        }

    }

    /// <summary>
    /// 获取用户token列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>token列表</returns>
    private List<TokenInfo> GetTokenInfos(long userId)
    {
        //redis获取用户token列表
        List<TokenInfo> tokenInfos = _redisCacheManager.HashGetOne<List<TokenInfo>>(RedisConst.Redis_UserTokenB, userId.ToString());
        if (tokenInfos != null)
        {
            tokenInfos = tokenInfos.Where(it => it.TokenTimeout > DateTime.Now).ToList();//去掉登录超时的
        }
        return tokenInfos;

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
