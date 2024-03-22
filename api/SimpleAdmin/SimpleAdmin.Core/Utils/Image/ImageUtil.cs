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

public static class ImageUtil
{
    /// <summary>
    /// 图片转换成base64
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public static string ImgToBase64String(SKImage img)
    {
        try
        {
            var p = img.Encode(SKEncodedImageFormat.Png, 100);
            var arr = p.ToArray();
            return Convert.ToBase64String(arr);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 图片转换成base64
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static string ImgToBase64String(this SKBitmap bmp)
    {
        try
        {
            var img = SKImage.FromBitmap(bmp);
            var p = img.Encode(SKEncodedImageFormat.Png, 100);
            var arr = p.ToArray();
            return Convert.ToBase64String(arr);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// base64转bitmap
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static Bitmap GetBitmapFromBase64(this string base64String)
    {
        var b = Convert.FromBase64String(base64String);
        var ms = new MemoryStream(b);
        var bitmap = new Bitmap(ms);
        ms.Close();
        return bitmap;
    }

    /// <summary>
    /// base64转bitmap
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static SKBitmap GetSkBitmapFromBase64(this string base64String)
    {
        var b = Convert.FromBase64String(base64String);
        var bitmap = SKBitmap.Decode(b);
        return bitmap;
    }

    /// <summary>
    /// base64转image格式
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static string ToImageBase64(this string base64String)
    {
        return "data:image/png;base64," + base64String;
    }

    /// <summary>
    /// 重新修改尺寸
    /// </summary>
    /// <param name="imgToResize">图片</param>
    /// <param name="size">尺寸</param>
    /// <returns></returns>
    public static Bitmap ResizeImage(Image imgToResize, Size size)
    {
        //获取图片宽度
        var sourceWidth = imgToResize.Width;
        //获取图片高度
        var sourceHeight = imgToResize.Height;

        float nPercent;
        //计算宽度的缩放比例
        var nPercentW = size.Width / (float)sourceWidth;
        //计算高度的缩放比例
        var nPercentH = size.Height / (float)sourceHeight;

        if (nPercentH < nPercentW)
            nPercent = nPercentH;
        else
            nPercent = nPercentW;
        //期望的宽度
        var destWidth = (int)(sourceWidth * nPercent);
        //期望的高度
        var destHeight = (int)(sourceHeight * nPercent);

        var b = new Bitmap(destWidth, destHeight);
        var g = Graphics.FromImage(b);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //绘制图像
        g.DrawImage(imgToResize, 0, 0, destWidth,
            destHeight);
        g.Dispose();
        return b;
    }

    /// <summary>
    /// Resize图片
    /// </summary>
    /// <param name="bmp">原始Bitmap </param>
    /// <param name="newW">新的宽度</param>
    /// <param name="newH">新的高度</param>
    /// <returns>处理以后的图片</returns>
    public static Bitmap ResizeImage(this Bitmap bmp, int newW, int newH)
    {
        try
        {
            var b = new Bitmap(newW, newH);
            var g = Graphics.FromImage(b);
            // 插值算法的质量
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            g.Dispose();
            return b;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Resize图片
    /// </summary>
    /// <param name="bmp">原始Bitmap </param>
    /// <param name="newW">新的宽度</param>
    /// <param name="newH">新的高度</param>
    /// <returns>处理以后的图片</returns>
    public static SKBitmap ResizeImage(this SKBitmap bmp, int newW, int newH)
    {
        try
        {
            var b = new SKBitmap(newW, newH);
            var resized = b.Resize(new SKImageInfo(newW, newH), SKFilterQuality.High);
            if (resized is null)
            {
                return null;
            }
            var image = SKImage.FromBitmap(resized);
            return b;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取缩略图
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="w">宽</param>
    /// <param name="h">高</param>
    /// <returns></returns>
    public static Image GetPicThumbnail(this Bitmap bmp, int w, int h)
    {
        try
        {
            var thumbnail = bmp.GetThumbnailImage(w, h, () => false, IntPtr.Zero);
            return thumbnail;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// 获取缩略图
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="w">宽</param>
    /// <param name="h">高</param>
    /// <returns></returns>
    public static SKImage GetPicThumbnail(this SKBitmap bmp, int w, int h)
    {
        try
        {
            var resized = bmp.Resize(new SKImageInfo(w, h), SKFilterQuality.Medium);
            if (resized is null)
            {
                return null;
            }
            var image = SKImage.FromBitmap(resized);
            return image;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
