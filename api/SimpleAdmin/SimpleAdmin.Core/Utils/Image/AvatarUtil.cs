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
/// 头像功能
/// </summary>
public static class AvatarUtil
{
    #region 姓名生成图片处理

    /// <summary>
    /// 获取姓名对应的颜色值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetNameColor(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length <= 0)
            throw new Exception("name不能为空");
        //获取名字第一个字,转换成 16进制 图片
        var str = "";
        foreach (var item in name)
        {
            str += Convert.ToUInt16(item);
        }
        if (str.Length < 4)
        {
            str += new Random().Next(100, 1000);
        }
        var color = "#" + str.Substring(1, 3);
        return color;
    }

    /// <summary>
    /// 获取姓名对应的图片
    /// </summary>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static SKBitmap GetNameImage(string name, int width = 100, int height = 100)
    {
        var color = GetNameColor(name);//获取颜色
        var nameLength = name.Length;//获取姓名长度
        var nameWritten = name;//需要绘制的文字
        if (nameLength > 2)//如果名字长度超过2个
        {
            // 如果用户输入的姓名大于等于3个字符，截取后面两位
            var firstName = name.Substring(0, 1);
            if (IsChinese(firstName))
            {
                // 截取倒数两位汉字
                nameWritten = name.Substring(name.Length - 2);
            }
            else
            {
                // 截取前面的两个英文字母
                nameWritten = name.Substring(0, 2).ToUpper();
            }
        }
        var bmp = new SKBitmap(width, height);
        using (var canvas = new SKCanvas(bmp))
        {
            canvas.DrawColor(SKColor.Parse(color));
            using (var sKPaint = new SKPaint())
            {
                sKPaint.Color = SKColors.White;//字体颜色
                sKPaint.TextSize = 25;//字体大小
                sKPaint.IsAntialias = true;//开启抗锯齿
                sKPaint.Typeface = SKTypeface.FromFamilyName("微软雅黑");//字体
                var size = new SKRect();
                sKPaint.MeasureText(nameWritten, ref size);//计算文字宽度以及高度
                var temp = (bmp.Width - size.Size.Width) / 2;
                var temp1 = (bmp.Height - size.Size.Height) / 2;
                canvas.DrawText(nameWritten, temp, temp1 - size.Top, sKPaint);//画文字
            }
        }

        return bmp;
    }

    /// <summary>
    /// 获取图片base64格式
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    public static string GetNameImageBase64(string name, int width = 100, int height = 100)
    {
        var img = GetNameImage(name, width, height);
        var imgByte = img.ImgToBase64String();
        return "data:image/png;base64," + imgByte;
    }

    /// <summary>
    /// 用 正则表达式 判断字符是不是汉字
    /// </summary>
    /// <param name="text">待判断字符或字符串</param>
    /// <returns>真：是汉字；假：不是</returns>
    private static bool IsChinese(string text)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u4e00-\u9fbb]");
    }

    #endregion 姓名生成图片处理
}
