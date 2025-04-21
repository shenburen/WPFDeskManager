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
        public static ShortcutInfo? GetIcon(string path)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);

                if (!System.IO.File.Exists(path))
                {
                    return null;
                }

                Icon icon = Icon.ExtractAssociatedIcon(shortcut.TargetPath);
                if (icon == null)
                {
                    return null;
                }

                BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));

                return new ShortcutInfo
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    TargetPath = shortcut.TargetPath,
                    Description = shortcut.Description,
                    Icon = bitmapSource,
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
