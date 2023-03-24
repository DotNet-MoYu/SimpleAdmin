namespace SimpleAdmin.System;


/// <summary>
/// 访问日志分页输入
/// </summary>
public class VisitLogPageInput : BasePageInput
{
    /// <summary>
    /// 分类
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }
}


/// <summary>
/// 访问日志删除输入
/// </summary>
public class VisitLogDeleteInput
{
    /// <summary>
    /// 分类
    /// </summary>
    public string Category { get; set; }
}
