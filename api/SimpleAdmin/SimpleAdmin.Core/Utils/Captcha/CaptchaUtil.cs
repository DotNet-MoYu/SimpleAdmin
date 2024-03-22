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
/// 生成验证码功能
/// </summary>
public static class CaptchaUtil
{
    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <param name="length">验证码数</param>
    /// <param name="width">长度</param>
    /// <param name="height">高度</param>
    /// <param name="fontSize">字体大小</param>
    /// <param name="type">类型 0：数字 1：字符 2：计算</param>
    /// <returns></returns>
    public static CaptchaInfo CreateCaptcha(CaptchaType type = CaptchaType.CHAR, int length = 4, int width = 170,
        int height = 50, int fontSize = 20)
    {
        //初始化验证码
        string charCode;
        var resultCode = "";
        switch (type.ToString())
        {
            case "NUM":
                charCode = CreateNumCode(length);
                break;

            case "ARITH":
                charCode = CreateArithCode(out resultCode);
                break;

            default:
                charCode = CreateCharCode(length);
                break;
        }
        var imageGenerator = new DefaultCaptchaImageGenerator();
        var imageGeneratorOption = new CaptchaImageGeneratorOption
        {
            // 必须设置
            ForegroundColors = DefaultColors.Instance.Colors,
            Width = width,
            Height = height,
            FontSize = fontSize,
            TextBold = true,
            BubbleCount = 1,
            FontFamily = DefaultFontFamilys.Instance.Actionj
        };

        var bytes = imageGenerator.Generate(charCode, imageGeneratorOption);

        var captchaInfo = new CaptchaInfo
        {
            Code = type.ToString() == "ARITH" ? resultCode : charCode,
            Image = bytes
        };
        return captchaInfo;
    }

    /// <summary>
    /// 获取数字验证码
    /// </summary>
    /// <param name="n">验证码数</param>
    /// <returns></returns>
    public static string CreateNumCode(int n)
    {
        char[] numChar = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var charCode = string.Empty;
        var random = new Random();
        for (var i = 0; i < n; i++)
        {
            charCode += numChar[random.Next(numChar.Length)];
        }
        return charCode;
    }

    /// <summary>
    /// 获取字符验证码
    /// </summary>
    /// <param name="n">验证码数</param>
    /// <returns></returns>
    public static string CreateCharCode(int n)
    {
        char[] strChar =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
            'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        var charCode = string.Empty;
        var random = new Random();
        for (var i = 0; i < n; i++)
        {
            charCode += strChar[random.Next(strChar.Length)];
        }
        return charCode;
    }

    /// <summary>
    /// 获取运算符验证码
    /// </summary>
    /// <returns></returns>
    public static string CreateArithCode(out string resultCode)
    {
        string checkCode;
        var random = new Random();
        var intFirst = random.Next(1, 10);//生成第一个数字
        var intSec = random.Next(1, 10);//生成第二个数字
        int intTemp;
        switch (random.Next(1, 3).ToString())
        {
            case "2":
                if (intFirst < intSec)
                {
                    intTemp = intFirst;
                    intFirst = intSec;
                    intSec = intTemp;
                }
                checkCode = intFirst + "-" + intSec + "=";
                resultCode = (intFirst - intSec).ToString();
                break;

            default:
                checkCode = intFirst + "+" + intSec + "=";
                resultCode = (intFirst + intSec).ToString();
                break;
        }
        return checkCode;
    }
}
