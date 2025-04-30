namespace WPFDeskManager
{
    internal class Global
    {
        /// <summary>
        /// 主窗口
        /// </summary>
        public static MainWindow? MainWindow = null;

        /// <summary>
        /// 图标列表
        /// </summary>
        public static Dictionary<int, IconBoxInfo> IconBoxInfos = new Dictionary<int, IconBoxInfo>();

        /// <summary>
        /// 全局初始化
        /// </summary>
        public static void Init()
        {
            MainWindow = new MainWindow();
            MainWindow.Show();

            Tray.CreateTray();
            ShortcutKey.CreateShortcutKey();
        }

        /// <summary>
        /// 全局清理
        /// </summary>
        public static void Dispose()
        {
            Tray.Dispose();
            ShortcutKey.Dispose();
        }
    }
}
