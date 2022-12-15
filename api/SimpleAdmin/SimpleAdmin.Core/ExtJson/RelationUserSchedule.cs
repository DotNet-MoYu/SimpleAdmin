using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// SYS_USER_SCHEDULE_DATA
/// 用户日程扩展
/// </summary>
public class RelationUserSchedule
{

    /// <summary>
    /// 日程日期
    /// </summary>
    public virtual string ScheduleDate { get; set; }

    /// <summary>
    /// 日程时间
    /// </summary>
    public virtual string ScheduleTime { get; set; }


    /// <summary>
    /// 日程内容
    /// </summary>
    public virtual string ScheduleContent { get; set; }


    /// <summary>
    /// 用户id
    /// </summary>
    public long ScheduleUserId { get; set; }


    /// <summary>
    /// 用户姓名
    /// </summary>
    public string ScheduleUserName { get; set; }


}
