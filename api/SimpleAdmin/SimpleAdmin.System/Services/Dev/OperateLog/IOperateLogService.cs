namespace SimpleAdmin.System;

/// <summary>
/// 操作日志服务
/// </summary>
public interface IOperateLogService : ITransient
{
    /// <summary>
    /// 根据分类删除操作日志
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns></returns>
    Task Delete(string category);

    /// <summary>
    /// 日志详情
    /// </summary>
    /// <param name="input">id</param>
    /// <returns>日志详情</returns>
    Task<DevLogOperate> Detail(BaseIdInput input);

    /// <summary>
    /// 操作日志分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页列表</returns>
    Task<SqlSugarPagedList<DevLogOperate>> Page(OperateLogPageInput input);
    /// <summary>
    /// 操作日志周统计
    /// </summary>
    /// <param name="day">天数</param>
    /// <returns>统计结果</returns>
    Task<List<OperateLogDayStatisticsOutput>> StatisticsByDay(int day);
    /// <summary>
    /// 操作日志总览
    /// </summary>
    /// <returns>异常日志和操作日志的数量</returns>
    Task<List<OperateLogTotalCountOutpu>> TotalCount();
}
