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
    public async Task UserLoginOut(string userId, List<string> clientIds, string message)
    {
        var hubContexxt = GetHubContext();

        //发送消息给用户
        await hubContexxt.Clients.Users(clientIds).LoginOut(message);
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
    #endregion
}
