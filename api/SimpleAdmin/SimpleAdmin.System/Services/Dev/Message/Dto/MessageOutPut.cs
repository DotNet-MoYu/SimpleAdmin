namespace SimpleAdmin.System;

/// <summary>
/// 消息详情
/// </summary>
public class MessageDetailOutPut : DevMessage
{
    public List<ReceiveInfo> ReceiveInfoList { get; set; } = new List<ReceiveInfo>();

    /// <summary>
    /// 接收信息类
    /// </summary>

    public class ReceiveInfo
    {
        /// <summary>
        /// 接收人ID
        /// </summary>
        public long ReceiveUserId { get; set; }

        /// <summary>
        /// 接收人姓名
        /// </summary>
        public string ReceiveUserName { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool Read { get; set; }
    }
}