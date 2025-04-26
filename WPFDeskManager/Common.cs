using IWshRuntimeLibrary;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    public class Common
    {
        public static bool GetIcon(string path, out string? targetPath, out BitmapSource? iconImage)
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
    }
}
