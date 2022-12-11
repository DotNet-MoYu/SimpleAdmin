using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core.Utils;

/// <summary>
/// 验证码信息
/// </summary>
public class CaptchaInfo
{

    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// 验证码数据流
    /// </summary>
    public byte[] Image { get; set; }
    /// <summary>
    /// base64
    /// </summary>
    public string Base64Str { get { return "data:image/png;base64," + Convert.ToBase64String(Image); } }

}
