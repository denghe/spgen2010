using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SPGen2010
{
    public static class ImageSourceHelper
    {
        /// <summary>
        /// 返回一个 /Images 目录下的图片的 ImageSource
        /// </summary>
        /// <param name="fn">Image 目录下的文件名，带扩展名</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage NewImageSource(this string fn)
        {
            return new BitmapImage(new Uri(@"pack://application:,,,/SPGen2010;component/Images/" + fn));
        }
    }
}
