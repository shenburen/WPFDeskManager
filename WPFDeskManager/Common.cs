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
        public static void CreateRoot()
        {
            BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/Images/root.png"));
            Hexagon hexagon = new Hexagon(new HexagonInfo
            {
                Name = "Root",
                Icon = icon,
                IsRoot = true,
            });
            hexagon.Show();
        }

        public static void CreateChild(HexagonInfo iconInfo)
        {
            Hexagon hexagon = new Hexagon(iconInfo);
            hexagon.Show();
        }

        public static HexagonInfo? GetIcon(string path)
        {
            try
            {
                string ext = Path.GetExtension(path).ToLower();
                string targetPath = "";
                Icon? icon = null;

                if (ext == ".lnk")
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);

                    if (!System.IO.File.Exists(path))
                    {
                        return null;
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
                    return null;
                }

                BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));

                return new HexagonInfo
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    Icon = bitmapSource,
                    TargetPath = targetPath,
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
