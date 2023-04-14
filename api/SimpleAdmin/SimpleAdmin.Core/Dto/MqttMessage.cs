namespace SimpleAdmin.Core;

/// <summary>
/// mqtt消息
/// </summary>
public class MqttMessage
{
    /// <summary>
    /// 消息分类
    /// </summary>
    public string MsgType { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    public DateTime DetTime { get; set; } = DateTime.Now;
}