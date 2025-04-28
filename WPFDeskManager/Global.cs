namespace WPFDeskManager
{
    internal class Global
    {
        public static Tray? Tray = null;

        public static MainWindow? MainWindow = null;

        public static Dictionary<int, IconBox> IconBoxes = new Dictionary<int, IconBox>();

        /// <summary>
        /// 全局初始化
        /// </summary>
        public static void Init()
        {
            Tray = new Tray();
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        /// <summary>
        /// 全局清理
        /// </summary>
        public static void Dispose()
        {
            Tray?.TrayIcon.Dispose();
        }
    }
}
