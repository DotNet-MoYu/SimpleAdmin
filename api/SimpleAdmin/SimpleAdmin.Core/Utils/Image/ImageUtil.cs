using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;

namespace SimpleAdmin.Core.Utils;

public static class ImageUtil
{
    /// <summary>
    /// bitmap转byte数组
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static byte[] GetBytesFromBitmap(this Bitmap bitmap)
    {
        MemoryStream ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Bmp);
        byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
        ms.Close();
        return bytes;
    }

    /// <summary>
    /// 图片转换成base64
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static string ImgToBase64String(this Bitmap bmp)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 图片转换成base64
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static string ImgToBase64String(Image bmp)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] arr = new byte[ms.Length]; ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length); ms.Close();
            return Convert.ToBase64String(arr);
        }
        catch (Exception)
        {
            return "";
        }
    }

    /// <summary>
    /// base64转bitmap
    /// </summary>
    /// <param name="base64string"></param>
    /// <returns></returns>
    public static Bitmap GetBitmapFromBase64(this string base64string)
    {
        byte[] b = Convert.FromBase64String(base64string);
        MemoryStream ms = new MemoryStream(b);
        Bitmap bitmap = new Bitmap(ms);
        ms.Close();
        return bitmap;
    }

    /// <summary>
    /// base64转image格式
    /// </summary>
    /// <param name="base64string"></param>
    /// <returns></returns>
    public static string ToImageBase64(this string base64string)
    {
        return "data:image/png;base64," + base64string;
    }

    /// <summary>
    /// 重新修改尺寸
    /// </summary>
    /// <param name="imgToResize">图片</param>
    /// <param name="size">尺寸</param>
    /// <returns></returns>
    public static Bitmap ResizeImage(System.DrawingCore.Image imgToResize, Size size)
    {
        //获取图片宽度
        int sourceWidth = imgToResize.Width;
        //获取图片高度
        int sourceHeight = imgToResize.Height;

        float nPercent = 0;
        float nPercentW = 0;
        float nPercentH = 0;
        //计算宽度的缩放比例
        nPercentW = (size.Width / (float)sourceWidth);
        //计算高度的缩放比例
        nPercentH = (size.Height / (float)sourceHeight);

        if (nPercentH < nPercentW)
            nPercent = nPercentH;
        else
            nPercent = nPercentW;
        //期望的宽度
        int destWidth = (int)(sourceWidth * nPercent);
        //期望的高度
        int destHeight = (int)(sourceHeight * nPercent);

        Bitmap b = new Bitmap(destWidth, destHeight);
        Graphics g = Graphics.FromImage(b);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //绘制图像
        g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
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
            Bitmap b = new Bitmap(newW, newH);
            Graphics g = Graphics.FromImage(b);
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
            Image thumbnail = bmp.GetThumbnailImage(
      w, h, () => false, IntPtr.Zero);
            return thumbnail;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}