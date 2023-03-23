using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System;

/// <summary>
/// 通知事件参数
/// </summary>
public class NoticeEvent
{

    /// <summary>
    /// 用户Id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// token信息
    /// </summary>

    public List<TokenInfo> TokenInfos { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Message { get; set; }

}
