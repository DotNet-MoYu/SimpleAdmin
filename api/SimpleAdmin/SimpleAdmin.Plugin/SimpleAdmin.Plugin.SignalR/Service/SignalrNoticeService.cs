// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.SignalR;

/// <summary>
/// <inheritdoc cref="INoticeService"/>
/// </summary>
[Injection(Named = "signalr")]
public class SignalrNoticeService : INoticeService
{
    public SignalrNoticeService()
    {
    }

    /// <inheritdoc/>
    public async Task UpdatePassWord(string userId, List<string> clientIds, string message)
    {
        //发送消息给用户
        await GetHubContext().Clients.Users(clientIds).NewMessage(message);
    }

    /// <inheritdoc/>
    public async Task NewMesage(List<string> userIds, List<string> clientIds, string message)
    {
        //发送消息给用户
        await GetHubContext().Clients.Users(clientIds).NewMessage(message);
    }

    /// <inheritdoc/>
    public async Task UserLoginOut(string userId, List<string> clientIds, string message)
    {
        //发送消息给用户
        await GetHubContext().Clients.Users(clientIds).LoginOut(message);
    }

    #region MyRegion

    /// <summary>
    /// 获取hubContext
    /// </summary>
    /// <returns></returns>
    private IHubContext<SimpleHub, ISimpleHub> GetHubContext()
    {
        //解析服务
        var service = App.GetService<IHubContext<SimpleHub, ISimpleHub>>();
        return service;
    }

    #endregion MyRegion
}