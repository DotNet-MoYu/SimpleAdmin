using Furion.InstantMessaging;
using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System
{
    /// <summary>
    /// <inheritdoc cref="ISimpleHub"/>
    /// </summary>
    //[Authorize]
    [MapHub("/hubs/simple")]
    public class SimpleHub : Hub<ISimpleHub>
    {
        private readonly ISimpleRedis _simpleRedis;

        public SimpleHub(ISimpleRedis simpleRedis)
        {
            this._simpleRedis = simpleRedis;
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
        /// <param name="input"></param>
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
                var tokenInfos = _simpleRedis.HashGetOne<List<TokenInfo>>(RedisConst.Redis_UserToken, userId);
                if (tokenInfos != null)
                {
                    if (ifConnect)
                    {
                        //获取redis中当前token
                        var tokenInfo = tokenInfos.Where(it => it.Token == token).FirstOrDefault();
                        if (tokenInfo != null)
                        {
                            tokenInfo.ClientIds.Add(userIdentifier);//添加到客户端列表
                            _simpleRedis.HashAdd(RedisConst.Redis_UserToken, userId, tokenInfos);//更新Redis
                        }
                    }
                    else
                    {
                        //获取当前客户端ID所在的token信息
                        var tokenInfo = tokenInfos.Where(it => it.ClientIds.Contains(userIdentifier)).FirstOrDefault();
                        if (tokenInfo != null)
                        {
                            tokenInfo.ClientIds.RemoveWhere(it => it == userIdentifier);//从客户端列表删除
                            _simpleRedis.HashAdd(RedisConst.Redis_UserToken, userId, tokenInfos);//更新Redis
                        }
                    }
                }
            }
        }
        #endregion

    }
}
