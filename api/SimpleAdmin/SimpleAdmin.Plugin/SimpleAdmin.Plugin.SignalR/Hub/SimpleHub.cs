// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.SignalR;

/// <summary>
/// <inheritdoc cref="ISimpleHub"/>
/// </summary>
//[Authorize]
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
            var tokenInfos = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.CACHE_USER_TOKEN, userId);
            if (tokenInfos != null)
            {
                if (ifConnect)
                {
                    //获取redis中当前token
                    var tokenInfo = tokenInfos.Where(it => it.Token == token).FirstOrDefault();
                    if (tokenInfo != null)
                    {
                        tokenInfo.ClientIds.Add(userIdentifier);//添加到客户端列表
                        _simpleCacheService.HashAdd(CacheConst.CACHE_USER_TOKEN, userId, tokenInfos);//更新Redis
                    }
                }
                else
                {
                    //获取当前客户端ID所在的token信息
                    var tokenInfo = tokenInfos.Where(it => it.ClientIds.Contains(userIdentifier)).FirstOrDefault();
                    if (tokenInfo != null)
                    {
                        tokenInfo.ClientIds.RemoveWhere(it => it == userIdentifier);//从客户端列表删除
                        _simpleCacheService.HashAdd(CacheConst.CACHE_USER_TOKEN, userId, tokenInfos);//更新Redis
                    }
                }
            }
        }
    }

    #endregion 方法
}
