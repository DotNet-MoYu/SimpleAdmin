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
/// SM2工具类
/// </summary>
public class SM2CryptoUtil
{
    #region 获取公钥私钥

    /// <summary>
    /// 获取公钥私钥
    /// </summary>
    /// <returns></returns>
    public static SM2Model GetKey()
    {
        var sm2 = SM2.Instance;
        var key = sm2.ecc_key_pair_generator.GenerateKeyPair();
        var ecpriv = (ECPrivateKeyParameters)key.Private;
        var ecpub = (ECPublicKeyParameters)key.Public;
        var privateKey = ecpriv.D;
        var publicKey = ecpub.Q;
        var sM2Model = new SM2Model();
        sM2Model.PrivateKey = Encoding.UTF8.GetString(Hex.Encode(privateKey.ToByteArray())).ToUpper();
        sM2Model.PublicKey = Encoding.UTF8.GetString(Hex.Encode(publicKey.GetEncoded())).ToUpper();
        return sM2Model;
    }

    #endregion 获取公钥私钥

    #region 加密

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="publickey">公钥</param>
    /// <param name="sourceData">需要加密的值</param>
    /// <returns>加密结果</returns>
    public static string Encrypt(string publickey, string sourceData)
    {
        var data = Encrypt(Hex.Decode(publickey), Encoding.UTF8.GetBytes(sourceData));
        return data;
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="publicKey">公钥</param>
    /// <param name="data">需要加密的值</param>
    /// <returns></returns>
    public static string Encrypt(byte[] publicKey, byte[] data)
    {
        if (null == publicKey || publicKey.Length == 0)
        {
            return null;
        }
        if (data == null || data.Length == 0)
        {
            return null;
        }

        var source = new byte[data.Length];
        Array.Copy(data, 0, source, 0,
            data.Length);

        var cipher = new Cipher();
        var sm2 = SM2.Instance;

        var userKey = sm2.ecc_curve.DecodePoint(publicKey);

        var c1 = cipher.Init_enc(sm2, userKey);
        cipher.Encrypt(source);

        var c3 = new byte[32];
        cipher.Dofinal(c3);

        var sc1 = Encoding.UTF8.GetString(Hex.Encode(c1.GetEncoded()));
        var sc2 = Encoding.UTF8.GetString(Hex.Encode(source));
        var sc3 = Encoding.UTF8.GetString(Hex.Encode(c3));

        return (sc1 + sc2 + sc3).ToUpper();
    }

    #endregion 加密

    #region 解密

    /// <summary>
    ///
    /// </summary>
    /// <param name="privateKey"></param>
    /// <param name="encryptedData"></param>
    /// <returns></returns>
    public static string Decrypt(string privateKey, string encryptedData)
    {
        var data = Encoding.UTF8.GetString(Decrypt(Hex.Decode(privateKey), Hex.Decode(encryptedData)));
        return data;
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="privateKey"></param>
    /// <param name="encryptedData"></param>
    /// <returns></returns>
    public static byte[] Decrypt(byte[] privateKey, byte[] encryptedData)
    {
        if (null == privateKey || privateKey.Length == 0)
        {
            return null;
        }
        if (encryptedData == null || encryptedData.Length == 0)
        {
            return null;
        }

        var data = Encoding.UTF8.GetString(Hex.Encode(encryptedData));

        var c1Bytes = Hex.Decode(Encoding.UTF8.GetBytes(data.Substring(0, 130)));
        var c2Len = encryptedData.Length - 97;
        var c2 = Hex.Decode(Encoding.UTF8.GetBytes(data.Substring(130, 2 * c2Len)));
        var c3 = Hex.Decode(Encoding.UTF8.GetBytes(data.Substring(130 + 2 * c2Len, 64)));

        var sm2 = SM2.Instance;
        var userD = new BigInteger(1, privateKey);

        //ECPoint c1 = sm2.ecc_curve.DecodePoint(c1Bytes);

        var c1 = sm2.ecc_curve.DecodePoint(c1Bytes);
        var cipher = new Cipher();
        cipher.Init_dec(userD, c1);
        cipher.Decrypt(c2);
        cipher.Dofinal(c3);

        return c2;
    }

    #endregion 解密

    private class Cipher
    {
        private int ct;
        private ECPoint p2;
        private SM3Digest sm3keybase;
        private SM3Digest sm3c3;
        private byte[] key;
        private byte keyOff;

        public Cipher()
        {
            ct = 1;
            key = new byte[32];
            keyOff = 0;
        }

        public static byte[] byteConvert32Bytes(BigInteger n)
        {
            byte[] tmpd;
            if (n == null)
            {
                return null;
            }

            if (n.ToByteArray().Length == 33)
            {
                tmpd = new byte[32];
                Array.Copy(n.ToByteArray(), 1, tmpd, 0,
                    32);
            }
            else if (n.ToByteArray().Length == 32)
            {
                tmpd = n.ToByteArray();
            }
            else
            {
                tmpd = new byte[32];
                for (var i = 0; i < 32 - n.ToByteArray().Length; i++)
                {
                    tmpd[i] = 0;
                }
                Array.Copy(n.ToByteArray(), 0, tmpd, 32 - n.ToByteArray().Length,
                    n.ToByteArray().Length);
            }
            return tmpd;
        }

        private void Reset()
        {
            sm3keybase = new SM3Digest();
            sm3c3 = new SM3Digest();

            var p = byteConvert32Bytes(p2.Normalize().XCoord.ToBigInteger());
            sm3keybase.BlockUpdate(p, 0, p.Length);
            sm3c3.BlockUpdate(p, 0, p.Length);

            p = byteConvert32Bytes(p2.Normalize().YCoord.ToBigInteger());
            sm3keybase.BlockUpdate(p, 0, p.Length);
            ct = 1;
            NextKey();
        }

        private void NextKey()
        {
            var sm3keycur = new SM3Digest(sm3keybase);
            sm3keycur.Update((byte)(ct >> 24 & 0xff));
            sm3keycur.Update((byte)(ct >> 16 & 0xff));
            sm3keycur.Update((byte)(ct >> 8 & 0xff));
            sm3keycur.Update((byte)(ct & 0xff));
            sm3keycur.DoFinal(key, 0);
            keyOff = 0;
            ct++;
        }

        public ECPoint Init_enc(SM2 sm2, ECPoint userKey)
        {
            var key = sm2.ecc_key_pair_generator.GenerateKeyPair();
            var ecpriv = (ECPrivateKeyParameters)key.Private;
            var ecpub = (ECPublicKeyParameters)key.Public;
            var k = ecpriv.D;
            var c1 = ecpub.Q;
            p2 = userKey.Multiply(k);
            Reset();
            return c1;
        }

        public void Encrypt(byte[] data)
        {
            sm3c3.BlockUpdate(data, 0, data.Length);
            for (var i = 0; i < data.Length; i++)
            {
                if (keyOff == key.Length)
                {
                    NextKey();
                }
                data[i] ^= key[keyOff++];
            }
        }

        public void Init_dec(BigInteger userD, ECPoint c1)
        {
            p2 = c1.Multiply(userD);
            Reset();
        }

        public void Decrypt(byte[] data)
        {
            for (var i = 0; i < data.Length; i++)
            {
                if (keyOff == key.Length)
                {
                    NextKey();
                }
                data[i] ^= key[keyOff++];
            }

            sm3c3.BlockUpdate(data, 0, data.Length);
        }

        public void Dofinal(byte[] c3)
        {
            var p = byteConvert32Bytes(p2.Normalize().YCoord.ToBigInteger());
            sm3c3.BlockUpdate(p, 0, p.Length);
            sm3c3.DoFinal(c3, 0);
            Reset();
        }
    }

    private class SM2
    {
        public static SM2 Instance
        {
            get
            {
                return new SM2();
            }
        }

        public static SM2 InstanceTest
        {
            get
            {
                return new SM2();
            }
        }

        public static readonly string[] sm2_param =
        {
            "FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF",// p,0
            "FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC",// a,1
            "28E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93",// b,2
            "FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123",// n,3
            "32C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7",// gx,4
            "BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0"// gy,5
        };

        public string[] ecc_param = sm2_param;

        public readonly BigInteger ecc_p;
        public readonly BigInteger ecc_a;
        public readonly BigInteger ecc_b;
        public readonly BigInteger ecc_n;
        public readonly BigInteger ecc_gx;
        public readonly BigInteger ecc_gy;

        public readonly ECCurve ecc_curve;
        public readonly ECPoint ecc_point_g;

        public readonly ECDomainParameters ecc_bc_spec;

        public readonly ECKeyPairGenerator ecc_key_pair_generator;

        private SM2()
        {
            ecc_param = sm2_param;

            ECFieldElement ecc_gx_fieldelement;
            ECFieldElement ecc_gy_fieldelement;

            ecc_p = new BigInteger(ecc_param[0], 16);
            ecc_a = new BigInteger(ecc_param[1], 16);
            ecc_b = new BigInteger(ecc_param[2], 16);
            ecc_n = new BigInteger(ecc_param[3], 16);
            ecc_gx = new BigInteger(ecc_param[4], 16);
            ecc_gy = new BigInteger(ecc_param[5], 16);

            ecc_gx_fieldelement = new FpFieldElement(ecc_p, ecc_gx);
            ecc_gy_fieldelement = new FpFieldElement(ecc_p, ecc_gy);

            ecc_curve = new FpCurve(ecc_p, ecc_a, ecc_b);
            ecc_point_g = new FpPoint(ecc_curve, ecc_gx_fieldelement, ecc_gy_fieldelement);

            ecc_bc_spec = new ECDomainParameters(ecc_curve, ecc_point_g, ecc_n);

            ECKeyGenerationParameters ecc_ecgenparam;
            ecc_ecgenparam = new ECKeyGenerationParameters(ecc_bc_spec, new SecureRandom());

            ecc_key_pair_generator = new ECKeyPairGenerator();
            ecc_key_pair_generator.Init(ecc_ecgenparam);
        }

        public virtual byte[] Sm2GetZ(byte[] userId, ECPoint userKey)
        {
            var sm3 = new SM3Digest();
            byte[] p;
            // userId length
            var len = userId.Length * 8;
            sm3.Update((byte)(len >> 8 & 0x00ff));
            sm3.Update((byte)(len & 0x00ff));

            // userId
            sm3.BlockUpdate(userId, 0, userId.Length);

            // a,b
            p = ecc_a.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            p = ecc_b.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            // gx,gy
            p = ecc_gx.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            p = ecc_gy.ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);

            // x,y
            p = userKey.AffineXCoord.ToBigInteger().ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);
            p = userKey.AffineYCoord.ToBigInteger().ToByteArray();
            sm3.BlockUpdate(p, 0, p.Length);

            // Z
            var md = new byte[sm3.GetDigestSize()];
            sm3.DoFinal(md, 0);

            return md;
        }
    }

    public class SM2Model
    {
        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
