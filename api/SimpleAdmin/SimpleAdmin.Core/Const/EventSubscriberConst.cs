using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 事件总线常量
/// </summary>
public class EventSubscriberConst
{
    /// <summary>
    /// 登录
    /// </summary>
    public const string Login = "用户登录";


    /// <summary>
    /// 登出
    /// </summary>
    public const string LoginOut = "用户登出";


    /// <summary>
    /// 访问日志
    /// </summary>
    public const string LogVisit = "访问日志";


    /// <summary>
    /// 操作日志
    /// </summary>
    public const string LogOperate = "操作日志";

    /// <summary>
    /// 清除用户缓存
    /// </summary>
    public const string ClearUserCache = "清除用户缓存";

}
