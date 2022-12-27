

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="INoticeService"/>
/// </summary>
public class NoticeService : INoticeService
{
    private readonly WebSettingsOptions webSettings;

    public NoticeService(IOptions<WebSettingsOptions> options)
    {
        this.webSettings = options.Value;
    }


    /// <inheritdoc/>
    public async Task LoginOut(string userId, List<TokenInfo> tokenInfos, string message)
    {
        //客户端ID列表
        var clientIds = new List<string>();
        //遍历token列表获取客户端ID列表
        tokenInfos.ForEach(it =>
        {
            clientIds.AddRange(it.ClientIds);
        });
        if (webSettings.UseMqtt)
        {
            var mqttService = App.GetService<IMqttService>();//获取mqtt服务

            //发送其他客户端登录消息
            await mqttService.LoginOut(userId, clientIds, message);
        }
        else
        {

            //获取signalr实例
            var signalr = App.GetService<IHubContext<SimpleHub, ISimpleHub>>();
            //发送其他客户端登录消息
            await signalr.Clients.Users(clientIds).LoginOut(message);
        }
    }

}
