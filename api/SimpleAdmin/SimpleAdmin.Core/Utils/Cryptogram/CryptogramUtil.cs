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
        return SM2Util.Decrypt(str);
    }

    /// <summary>
    /// SM2加密
    /// </summary>
    /// <param name="str">明文</param>
    /// <returns>密文</returns>
    public static string Sm2Encrypt(string str)
    {
        // 加密
        return SM2Util.Encrypt(str);
    }

    #endregion

    #region Sm4
    /// <summary>
    /// SM4解密
    /// </summary>
    /// <param name="str">密文</param>
    /// <returns>明文</returns>
    public static string Sm4Decrypt(string str)
    {
        if (!string.IsNullOrEmpty(str))
            return SM4Util.Decrypt(new SM4Util { Data = str });// 解密
        else
            return null;
    }

    /// <summary>
    /// SM4加密
    /// </summary>
    /// <param name="str">明文</param>
    /// <returns>密文</returns>
    public static string Sm4Encrypt(string str)
    {
        if (!string.IsNullOrEmpty(str))
            return SM4Util.Encrypt(new SM4Util { Data = str });            // 加密
        else
            return null;

    }

    #endregion

}
