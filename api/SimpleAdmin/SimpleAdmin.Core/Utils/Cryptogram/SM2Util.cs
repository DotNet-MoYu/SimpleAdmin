namespace SimpleAdmin.Core.Utils;

/// <summary>
/// SM2加密解密
/// </summary>
public class SM2Util
{
    /// <summary>
    /// 公钥
    /// </summary>
    public static string PublicKey = App.GetConfig<string>("Cryptogram:SM2:PublicKey");

    /// <summary>
    /// 私钥
    /// </summary>
    public static string PrivateKey = App.GetConfig<string>("Cryptogram:SM2:PrivateKey");

    /// <summary>
    /// 公钥加密明文
    /// </summary>
    /// <param name="plainText">明文</param>
    /// <returns>密文</returns>
    public static string Encrypt(string plainText)
    {
        return SM2CryptoUtil.Encrypt(PublicKey, plainText);
    }

    /// <summary>
    /// 私钥解密密文
    /// </summary>
    /// <param name="cipherText">密文</param>
    /// <returns>明文</returns>
    public static string Decrypt(string cipherText)
    {
        if (!cipherText.StartsWith("04")) cipherText = "04" + cipherText;//如果不是04开头加上04
        return SM2CryptoUtil.Decrypt(PrivateKey, cipherText);
    }
}