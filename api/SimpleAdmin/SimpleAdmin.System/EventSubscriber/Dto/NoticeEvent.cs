namespace SimpleAdmin.System;

/// <summary>
/// 用户登出事件
/// </summary>
public class UserLoginOutEvent
{

    /// <summary>
    /// 用户Id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// token信息
    /// </summary>

    public List<TokenInfo> TokenInfos { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Message { get; set; }

}

/// <summary>
/// 新消息事件
/// </summary>
public class NewMessageEvent
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public List<string> UserIds { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Message { get; set; } = "收到一条新的消息!";
}