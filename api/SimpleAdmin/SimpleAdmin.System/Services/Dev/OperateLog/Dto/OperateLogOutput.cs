namespace SimpleAdmin.System;

/// <summary>
/// 操作日志周统计输出
/// </summary>
public class OperateLogDayStatisticsOutput
{
    /// <summary>
    /// 日期
    /// </summary>
    public string Date { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; set; }
}

/// <summary>
/// 操作日志统计
/// </summary>
public class OperateLogTotalCountOutpu : VisitLogTotalCountOutput
{
}