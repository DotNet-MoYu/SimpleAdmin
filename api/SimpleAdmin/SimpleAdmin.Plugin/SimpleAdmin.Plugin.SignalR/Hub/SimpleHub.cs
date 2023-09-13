using Furion.InstantMessaging;
using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;

namespace SimpleAdmin.Plugin.SignalR;

/// <summary>
/// <inheritdoc cref="ISimpleHub"/>
/// </summary>
[Authorize]
[MapHub("/hubs/simple")]
[Authorize]
public class SimpleHub : Hub<ISimpleHub>
{
    private readonly ISimpleCacheService _simpleCacheService;

    public SimpleHub(ISimpleCacheService simpleCacheService)
    {
        _simpleCacheService = simpleCacheService;
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext().Request.Query["access_token"];//获取token
        if (!string.IsNullOrEmpty(token))
        {
            var userIdentifier = Context.UserIdentifier;//自定义的Id
            UpdateRedis(userIdentifier, token);//更新redis
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdentifier = Context.UserIdentifier;//自定义的Id
        UpdateRedis(userIdentifier, null, false);//更新redis
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task LoginOut(string userId)
    {
        await Clients.User(userId).LoginOut("退出登录");
    }

    #region 方法

    /// <summary>
    /// 更新redis
    /// </summary>
    /// <param name="userIdentifier">用户id</param>
    /// <param name="token">token</param>
    /// <param name="ifConnect">是否是上线</param>
    private void UpdateRedis(string userIdentifier, string token, bool ifConnect = true)
    {
        var userId = userIdentifier.Split("_")[0];//分割取第一个
        if (!string.IsNullOrEmpty(userId))
        {
            //获取redis当前用户的token信息列表
            var tokenInfos = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.Cache_UserToken, userId);
            if (tokenInfos != null)
            {
                if (ifConnect)
                {
                    //获取redis中当前token
                    var tokenInfo = tokenInfos.Where(it => it.Token == token).FirstOrDefault();
                    if (tokenInfo != null)
                    {
                        tokenInfo.ClientIds.Add(userIdentifier);//添加到客户端列表
                        _simpleCacheService.HashAdd(CacheConst.Cache_UserToken, userId, tokenInfos);//更新Redis
                    }
                }
                else
                {
                    //获取当前客户端ID所在的token信息
                    var tokenInfo = tokenInfos.Where(it => it.ClientIds.Contains(userIdentifier)).FirstOrDefault();
                    if (tokenInfo != null)
                    {
                        tokenInfo.ClientIds.RemoveWhere(it => it == userIdentifier);//从客户端列表删除
                        _simpleCacheService.HashAdd(CacheConst.Cache_UserToken, userId, tokenInfos);//更新Redis
                    }
                }
            }
        }
    }

    #endregion 方法
}
