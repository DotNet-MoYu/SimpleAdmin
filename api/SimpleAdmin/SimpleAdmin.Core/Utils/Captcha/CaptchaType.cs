namespace SimpleAdmin.Core.Utils;

/// <summary>
/// 验证码类型
/// </summary>
public enum CaptchaType
{
    /// <summary>
    /// 纯数字验证码
    /// </summary>
    [Description("纯数字验证码")]
    NUM = 0,

    /// <summary>
    /// 数字加字母验证码
    /// </summary>
    [Description("数字加字母验证码")]
    CHAR = 1,

    /// <summary>
    /// 数字运算验证码
    /// </summary>
    [Description("数字运算验证码")]
    ARITH = 2,
}