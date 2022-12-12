namespace SimpleAdmin.System;

/// <summary>
/// 站内信服务
/// </summary>
public interface IMessageService : ITransient
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<SqlSugarPagedList<DevMessage>> Page(MessagePageInput input);

    /// <summary>
    /// 发送站内信
    /// </summary>
    /// <param name="input">站内信信息</param>
    /// <returns></returns>
    Task Send(MessageSendInput input);
}
