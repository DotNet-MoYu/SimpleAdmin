namespace SimpleAdmin.System;

/// <summary>
/// 访问日志服务
/// </summary>

public interface IVisitLogService : ITransient
{
    /// <summary>
    /// 根据分类删除
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns></returns>
    Task Delete(string category);

    /// <summary>
    /// 访问日志分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>日志列表</returns>
    Task<SqlSugarPagedList<DevLogVisit>> Page(VisitLogPageInput input);

    /// <summary>
    /// 根统计N天来登录和登出数量
    /// </summary>
    /// <param name="day">天使</param>
    /// <returns>统计信息</returns>
    Task<List<VisitLogDayStatisticsOutput>> StatisticsByDay(int day);

    /// <summary>
    /// 统计登录登出总览
    /// </summary>
    /// <returns>登录和登出的数量</returns>
    Task<List<VisitLogTotalCountOutput>> TotalCount();
}
