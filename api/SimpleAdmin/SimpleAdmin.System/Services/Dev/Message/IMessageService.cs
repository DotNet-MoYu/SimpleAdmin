namespace SimpleAdmin.System;

/// <summary>
/// 站内信服务
/// </summary>
public interface IMessageService : ITransient
{
    /// <summary>
    /// 删除消息
    /// </summary>
    /// <param name="input">id列表</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 获取消息详情
    /// </summary>
    /// <param name="input">消息ID</param>
    /// <param name="isSelf">是否是自己</param>
    /// <returns>消息详情</returns>
    Task<MessageDetailOutPut> Detail(BaseIdInput input, bool isSelf = false);

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
