namespace SimpleAdmin.System;

/// <summary>
/// 首页服务
/// </summary>
public interface IIndexService : ITransient
{
    /// <summary>
    /// 添加日程
    /// </summary>
    /// <param name="input">日程参数</param>
    /// <returns></returns>
    Task AddSchedule(ScheduleAddInput input);

    /// <summary>
    /// 删除日程
    /// </summary>
    /// <param name="input">id列表</param>
    /// <returns></returns>
    Task DeleteSchedule(BaseIdListInput input);

    /// <summary>
    /// 获取日程列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>日程列表</returns>
    Task<List<ScheduleListOutput>> ScheduleList(ScheduleListInput input);
}