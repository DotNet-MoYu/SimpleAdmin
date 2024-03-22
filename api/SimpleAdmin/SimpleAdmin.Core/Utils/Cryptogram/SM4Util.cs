// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Core.Utils;

//加密和解密结构相同，只不过，解密密钥是加密密钥的逆序
/// <summary>
/// Sm4算法
/// 对标国际DES算法
/// </summary>
public class SM4Util
{
    public SM4Util()
    {
        Key = "1814546261730461";//密钥长度必须为16字节。
        Iv = "0000000000000000";
        HexString = false;
        CryptoMode = Sm4CryptoEnum.ECB;
    }

    /// <summary>
    /// 数据
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// 秘钥
    /// </summary>
    public string Key { get; }//不同的key，加密出来的数据不一样，所以此处设定好key以后，禁止修改

    /// <summary>
    /// 向量
    /// </summary>
    public string Iv { get; set; }

    /// <summary>
    /// 明文是否是十六进制
    /// </summary>
    public bool HexString { get; }//set;

    /// <summary>
    /// 加密模式(默认ECB)
    /// 统一改为ECB模式
    /// </summary>
    public Sm4CryptoEnum CryptoMode { get; }

    #region 加密

    public static string Encrypt(SM4Util entity)
    {
        return entity.CryptoMode == Sm4CryptoEnum.CBC ? EncryptCBC(entity) : EncryptECB(entity);
    }

    /// <summary>
    /// ECB加密
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string EncryptECB(SM4Util entity)
    {
        var ctx = new Sm4Context
        {
            IsPadding = true
        };
        var keyBytes = entity.HexString ? Hex.Decode(entity.Key) : Encoding.Default.GetBytes(entity.Key);
        var sm4 = new SM4CryptoUtil();
        sm4.SetKeyEnc(ctx, keyBytes);
        var encrypted = sm4.Sm4CryptEcb(ctx, Encoding.Default.GetBytes(entity.Data));
        return Encoding.Default.GetString(Hex.Encode(encrypted));
    }

    /// <summary>
    /// CBC加密
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string EncryptCBC(SM4Util entity)
    {
        var ctx = new Sm4Context
        {
            IsPadding = true
        };
        var keyBytes = entity.HexString ? Hex.Decode(entity.Key) : Encoding.Default.GetBytes(entity.Key);
        var ivBytes = entity.HexString ? Hex.Decode(entity.Iv) : Encoding.Default.GetBytes(entity.Iv);
        var sm4 = new SM4CryptoUtil();
        sm4.SetKeyEnc(ctx, keyBytes);
        var encrypted = sm4.Sm4CryptCbc(ctx, ivBytes, Encoding.Default.GetBytes(entity.Data));
        return Convert.ToBase64String(encrypted);
    }

    #endregion 加密

    #region 解密

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string Decrypt(SM4Util entity)
    {
        return entity.CryptoMode == Sm4CryptoEnum.CBC ? DecryptCBC(entity) : DecryptECB(entity);
    }

    /// <summary>
    ///  ECB解密
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string DecryptECB(SM4Util entity)
    {
        var ctx = new Sm4Context
        {
            IsPadding = true,
            Mode = 0
        };
        var keyBytes = entity.HexString ? Hex.Decode(entity.Key) : Encoding.Default.GetBytes(entity.Key);
        var sm4 = new SM4CryptoUtil();
        sm4.Sm4SetKeyDec(ctx, keyBytes);
        var decrypted = sm4.Sm4CryptEcb(ctx, Hex.Decode(entity.Data));
        return Encoding.Default.GetString(decrypted);
    }

    /// <summary>
    /// CBC解密
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string DecryptCBC(SM4Util entity)
    {
        var ctx = new Sm4Context
        {
            IsPadding = true,
            Mode = 0
        };
        var keyBytes = entity.HexString ? Hex.Decode(entity.Key) : Encoding.Default.GetBytes(entity.Key);
        var ivBytes = entity.HexString ? Hex.Decode(entity.Iv) : Encoding.Default.GetBytes(entity.Iv);
        var sm4 = new SM4CryptoUtil();
        sm4.Sm4SetKeyDec(ctx, keyBytes);
        var decrypted = sm4.Sm4CryptCbc(ctx, ivBytes, Convert.FromBase64String(entity.Data));
        return Encoding.Default.GetString(decrypted);
    }

    #endregion 解密

    /// <summary>
    /// 加密类型
    /// </summary>
    public enum Sm4CryptoEnum
    {
        /// <summary>
        /// ECB(电码本模式)
        /// </summary>
        [Description("ECB模式")]
        ECB = 0,

        /// <summary>
        /// CBC(密码分组链接模式)
        /// </summary>
        [Description("CBC模式")]
        CBC = 1
    }
}
