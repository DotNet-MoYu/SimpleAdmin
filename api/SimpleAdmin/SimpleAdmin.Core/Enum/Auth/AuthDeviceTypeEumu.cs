using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 登录设备类型枚举
/// </summary>
public enum AuthDeviceTypeEumu
{
    /// <summary>
    /// PC端
    /// </summary>
    [Description("PC端")]
    PC,
    /// <summary>
    /// 移动端
    /// </summary>
    [Description("移动端")]
    APP,
    /// <summary>
    /// 小程序
    /// </summary>
    [Description("小程序")]
    MINI

}
