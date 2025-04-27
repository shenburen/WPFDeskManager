using Hardcodet.Wpf.TaskbarNotification;

namespace WPFDeskManager
{
    internal class Global
    {
        public static TaskbarIcon? TrayIcon = null;

        public static MainWindow? MainWindow = null;

        public static Dictionary<int, IconBox> IconBoxes = new Dictionary<int, IconBox>();

        public static void Init()
        {
            Tray.CreateTrayIcon();
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        public static void Dispose()
        {
            TrayIcon?.Dispose();
        }
    }
}
