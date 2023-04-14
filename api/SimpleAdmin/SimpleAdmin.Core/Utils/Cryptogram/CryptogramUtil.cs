namespace SimpleAdmin.Core.Utils;

/// <summary>
/// 加解密功能
/// </summary>
public class CryptogramUtil
{
    #region SM2

    /// <summary>
    /// SM2解密
    /// </summary>
    /// <param name="str">密文</param>
    /// <returns>明文</returns>
    public static string Sm2Decrypt(string str)
    {
        // 解密
        if (!string.IsNullOrWhiteSpace(str))
            return SM2Util.Decrypt(str);
        else return "";
    }

    /// <summary>
    /// SM2加密
    /// </summary>
    /// <param name="str">明文</param>
    /// <returns>密文</returns>
    public static string Sm2Encrypt(string str)
    {
        // 加密
        if (!string.IsNullOrWhiteSpace(str))
            return SM2Util.Encrypt(str);
        else return "";
    }

    #endregion SM2

    #region Sm4

    /// <summary>
    /// SM4解密
    /// </summary>
    /// <param name="str">密文</param>
    /// <returns>明文</returns>
    public static string Sm4Decrypt(string str)
    {
        if (!string.IsNullOrWhiteSpace(str))// 解密
            return SM4Util.Decrypt(new SM4Util { Data = str });
        else
            return "";
    }

    /// <summary>
    /// SM4加密
    /// </summary>
    /// <param name="str">明文</param>
    /// <returns>密文</returns>
    public static string Sm4Encrypt(string str)
    {
        if (!string.IsNullOrWhiteSpace(str))// 加密
            return SM4Util.Encrypt(new SM4Util { Data = str });
        else
            return "";
    }

    #endregion Sm4
}