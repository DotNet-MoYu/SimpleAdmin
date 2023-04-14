namespace SimpleAdmin.System;

/// <summary>
/// 会话分页查询
/// </summary>
public class SessionPageInput : BasePageInput
{
    /// <summary>
    /// 账号
    ///</summary>
    public virtual string Account { get; set; }

    /// <summary>
    /// 姓名
    ///</summary>

    public virtual string Name { get; set; }

    /// <summary>
    /// 最新登录ip
    ///</summary>
    public string LatestLoginIp { get; set; }
}

/// <summary>
/// Token退出参数
/// </summary>
public class ExitTokenInput : BaseIdInput
{
    /// <summary>
    /// token
    /// </summary>
    [Required(ErrorMessage = "Tokens不能为空")]
    public List<string> Tokens { get; set; }
}