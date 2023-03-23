using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System;

/// <summary>
/// 事件总线常量
/// </summary>
public class EventSubscriberConst
{

    #region AuthEventSubscriber
    /// <summary>
    /// B端登录
    /// </summary>
    public const string LoginB = "B端登录";


    /// <summary>
    /// B端登录
    /// </summary>
    public const string LoginOutB = "B端登出";


    #endregion

    #region UserEventSubscriber
    /// <summary>
    /// 清除用户缓存
    /// </summary>
    public const string ClearUserCache = "清除用户缓存";
    #endregion

    #region NoticeEventSubscriber

    /// <summary>
    /// 通知用户下线
    /// </summary>
    public const string UserLoginOut = "通知用户下线";
    #endregion

}
