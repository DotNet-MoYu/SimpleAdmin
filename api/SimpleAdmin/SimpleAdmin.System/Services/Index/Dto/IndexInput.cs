namespace SimpleAdmin.System;

/// <summary>
/// 日程列表查询参数
/// </summary>
public class ScheduleListInput
{
    /// <summary>
    /// 日程日期
    /// </summary>
    [Required(ErrorMessage = "ScheduleDate不能为空")]
    public string ScheduleDate { get; set; }
}
public class ScheduleAddInput : RelationUserSchedule
{
    /// <summary>
    /// 日程日期
    /// </summary>
    [Required(ErrorMessage = "scheduleDate不能为空")]
    public override string ScheduleDate { get; set; }
    /// <summary>
    /// 日程内容
    /// </summary>
    [Required(ErrorMessage = "ScheduleContent不能为空")]
    public override string ScheduleContent { get; set; }

    /// <summary>
    /// 日程时间
    /// </summary>
    [Required(ErrorMessage = "ScheduleTime 不能为空")]
    public override string ScheduleTime { get; set; }
}
