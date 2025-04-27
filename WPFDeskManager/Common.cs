using IWshRuntimeLibrary;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace WPFDeskManager
{
    public class Common
    {
        /// <summary>
        /// 从快捷方式和文件中获取图标
        /// </summary>
        /// <param name="path">目标文件</param>
        /// <param name="targetPath">目标实际应用的路径</param>
        /// <param name="iconImage">获取的图标</param>
        /// <returns>执行是否成功</returns>
        public static bool GetIconFromFiles(string path, out string? targetPath, out BitmapSource? iconImage)
        {
            targetPath = null;
            iconImage = null;

            try
            {
                Icon? icon = null;
                string ext = Path.GetExtension(path).ToLower();

                if (ext == ".lnk")
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);

                    if (!System.IO.File.Exists(path))
                    {
                        return false;
                    }

                    targetPath = shortcut.TargetPath;
                    icon = Icon.ExtractAssociatedIcon(shortcut.TargetPath);
                }
                else
                {
                    targetPath = path;
                    icon = Icon.ExtractAssociatedIcon(path);
                }

                if (icon == null)
                {
                    return false;
                }

                iconImage = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从“资源”中获取图标
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>图标</returns>
        public static BitmapImage GetSvgFromResources(string path)
        {
            BitmapImage bitmap = new BitmapImage();

            try
            {
                StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(path));

                MemoryStream memStream = new MemoryStream();
                StreamSvgConverter converter = new StreamSvgConverter(new WpfDrawingSettings());
                converter.Convert(streamInfo.Stream, memStream);

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memStream;
                bitmap.EndInit();

                return bitmap;
            }
            catch
            {
                return bitmap;
            }
        }
    }
}
