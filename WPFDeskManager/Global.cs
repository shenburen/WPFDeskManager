namespace WPFDeskManager
{
    internal class Global
    {
        public static Tray? Tray = null;

        public static MainWindow? MainWindow = null;

        public static Dictionary<int, IconBox> IconBoxes = new Dictionary<int, IconBox>();

        public static void Init()
        {
            Tray = new Tray();
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        public static void Dispose()
        {
            Tray?.TrayIcon.Dispose();
        }
    }
}
