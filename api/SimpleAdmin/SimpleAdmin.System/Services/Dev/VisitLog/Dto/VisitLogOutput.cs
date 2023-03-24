namespace SimpleAdmin.System;


/// <summary>
/// 周统计输出
/// </summary>
public class VisitLogDayStatisticsOutput
{
    /// <summary>
    /// 日期
    /// </summary>
    public string Date { get; set; }

    /// <summary>
    /// 登录次数
    /// </summary>
    public int LoginCount { get; set; }

    /// <summary>
    /// 登出次数
    /// </summary>
    public int LogoutCount { get; set; }
}

/// <summary>
/// 访问日志总数输出
/// </summary>
public class VisitLogTotalCountOutput
{
    /// <summary>
    /// 类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Value { get; set; }
}
