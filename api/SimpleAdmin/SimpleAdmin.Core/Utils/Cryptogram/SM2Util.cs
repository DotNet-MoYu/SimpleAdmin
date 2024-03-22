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
