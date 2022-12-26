using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 字典常量
/// </summary>
public class DevDictConst
{
    #region 系统字典
    #region 系统通用状态
    /// <summary>
    /// 启用
    /// </summary>
    public const string COMMON_STATUS_ENABLE = "ENABLE";

    /// <summary>
    /// 停用
    /// </summary>
    public const string COMMON_STATUS_DISABLED = "DISABLED";
    #endregion

    #region   在线用户状态
    /// <summary>
    /// 在线
    /// </summary>
    public const string ONLINE_STATUS_ONLINE = "ONLINE";

    /// <summary>
    /// 离线
    /// </summary>
    public const string ONLINE_STATUS_OFFLINE = "OFFLINE";

    #endregion

    #endregion

    #region 业务字典
    #endregion
}
