using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 通用错误码
/// </summary>
[ErrorCodeType]
public enum ErrorCodeEnum
{
    /// <summary>
    /// 数据不存在
    /// </summary>
    [ErrorCodeItemMetadata("数据不存在")]
    A0001,

    /// <summary>
    /// 删除失败
    /// </summary>
    [ErrorCodeItemMetadata("删除失败")]
    A0002,
    /// <summary>
    /// 操作失败
    /// </summary>
    [ErrorCodeItemMetadata("操作失败")]
    A0003,
    /// <summary>
    /// 没有权限
    /// </summary>
    [ErrorCodeItemMetadata("没有权限")]
    A0004,
}
