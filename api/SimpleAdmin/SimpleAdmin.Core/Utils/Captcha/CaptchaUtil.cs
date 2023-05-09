using Lazy.Captcha.Core.Generator.Image.Option;
using Lazy.Captcha.Core.Generator.Image;
using Lazy.Captcha.Core;
using SkiaSharp;

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
    /// <param name="heigh">高度</param>
    /// <param name="fontSize">字体大小</param>
    /// <param name="type">类型 0：数字 1：字符 2：计算</param>
    /// <returns></returns>
    public static CaptchaInfo CreateCaptcha(CaptchaType type = CaptchaType.CHAR, int length = 4, int width = 170, int heigh = 50, int fontSize = 20)
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
        var imageGeneratorOption = new CaptchaImageGeneratorOption()
        {
            // 必须设置
            ForegroundColors = DefaultColors.Instance.Colors,
            Width = width,
            Height = heigh,
            FontSize = fontSize,
            TextBold = true,
            BubbleCount = 1,
            FontFamily = DefaultFontFamilys.Instance.Actionj
        };

        var bytes = imageGenerator.Generate(charCode, imageGeneratorOption);

        var captchaInfo = new CaptchaInfo()
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
        var checkCode = "";
        var random = new Random();
        var intFirst = random.Next(1, 10);//生成第一个数字
        var intSec = random.Next(1, 10);//生成第二个数字
        var intTemp = 0;
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