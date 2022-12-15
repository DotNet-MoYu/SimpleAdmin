using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 站内信与接收用户拓展
/// MSG_TO_USER
/// </summary>
public class RelationMsgUser
{
    /// <summary>
    /// 是否已读
    /// </summary>
    public bool Read { get; set; } = false;
}
