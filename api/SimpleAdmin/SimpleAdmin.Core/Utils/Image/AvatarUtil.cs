using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;

namespace SimpleAdmin.Core.Utils
{
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
            string str = "";
            foreach (var item in name)
            {
                str += Convert.ToUInt16(item);
            }
            if (str.Length < 4)
            {
                str += new Random().Next(100, 1000);
            }
            string color = "#" + str.Substring(1, 3);
            return color;
        }
        /// <summary>
        /// 获取姓名对应的图片 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GetNameImage(string name, int width = 100, int height = 100)
        {
            string color = GetNameColor(name);//获取颜色
            var nameLength = name.Length;//获取姓名长度
            string nameWritten = name;//需要绘制的文字
            if (nameLength > 2)//如果名字长度超过2个
            {
                // 如果用户输入的姓名大于等于3个字符，截取后面两位
                string firstName = name.Substring(0, 1);
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
            //string firstName = name.Substring(0, 1);
            Bitmap img = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img);
            Brush brush = new SolidBrush(ColorTranslator.FromHtml(color));
            g.FillRectangle(brush, 0, 0, width, height);
            //填充文字
            Font font = new Font("微软雅黑", 25);
            SizeF firstSize = g.MeasureString(nameWritten, font);
            g.DrawString(nameWritten, font, Brushes.White, new PointF((img.Width - firstSize.Width) / 2, (img.Height - firstSize.Height) / 2));
            g.Dispose();
            return img;
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
            Bitmap img = GetNameImage(name, width, height);
            var imgByte = img.ImgToBase64String();
            return $"data:image/png;base64," + imgByte;
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
        #endregion

    }
}
