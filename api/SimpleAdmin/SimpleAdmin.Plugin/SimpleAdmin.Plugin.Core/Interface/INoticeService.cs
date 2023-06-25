namespace SimpleAdmin.Plugin.Core;

/// <summary>
/// 通知服务
/// </summary>
public interface INoticeService : ISingleton
{
    /// <summary>
    /// 通知用户下线
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="clientIds">clientId列表</param>
    /// <param name="message">通知内容</param>
    /// <returns></returns>
    Task UserLoginOut(string userId, List<string> clientIds, string message);

    /// <summary>
    /// 通知用户修改密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="clientIds">clientId列表</param>
    /// <param name="message">通知内容</param>
    /// <returns></returns>
    Task UpdatePassWord(string userId, List<string> clientIds, string message);

    /// <summary>
    /// 收到新的消息
    /// </summary>
    /// <param name="userIds">用户Id列表</param>
    /// <param name="clientIds">clientId列表</param>
    /// <param name="message"></param>
    /// <returns></returns>
    Task NewMesage(List<string> userIds, List<string> clientIds, string message);
}