using IWshRuntimeLibrary;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDeskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ShortcutInfo> Shortcuts { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            ShortcutList.ItemsSource = Shortcuts;

            double radius = 100;
            double w = Math.Sqrt(3) * radius;
            double h = 2 * radius;

            for (int row = 0; row < 2; row++)
            {
                for (int col = 0; col < 2; col++)
                {
                    double offsetX = col * w * 0.75;
                    double offsetY = row * h + (col % 2 == 1 ? h / 2 : 0);

                    var hex = new Hexagon();
                    hex.Left = offsetX + 100; // 初始偏移
                    hex.Top = offsetY + 100;
                    hex.Show();

                    WindowsManager.AllHexagons.Add(hex);
                }
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
                if (System.IO.Path.GetExtension(file).ToLower() == ".lnk")
                {
                    var shortcut = ParseShortcut(file);
                    if (shortcut != null)
                    {
                        Shortcuts.Add(shortcut);
                    }
                }
            }
        }

        private ShortcutInfo? ParseShortcut(string lnkPath)
        {
            try
            {
                var shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(lnkPath);

                var icon = GetIconImage(shortcut.TargetPath);

                return new ShortcutInfo
                {
                    Name = System.IO.Path.GetFileNameWithoutExtension(lnkPath),
                    TargetPath = shortcut.TargetPath,
                    Description = shortcut.Description,
                    Icon = icon
                };
            }
            catch
            {
                return null;
            }
        }

        private BitmapSource? GetIconImage(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            if (icon == null)
            {
                return null;
            }

            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));
        }
    }
}