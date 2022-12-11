using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 登录端类型枚举
/// </summary>
public enum LoginClientTypeEnum
{
    /// <summary>
    /// B端
    /// </summary>
    [Description("B端")]
    B,
    /// <summary>
    /// C端
    /// </summary>
    [Description("C端")]
    C
}
