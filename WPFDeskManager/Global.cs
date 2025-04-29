namespace WPFDeskManager
{
    internal class Global
    {
        /// <summary>
        /// 托盘图标
        /// </summary>
        public static Tray? Tray = null;

        /// <summary>
        /// 快捷键
        /// </summary>
        public static ShortcutKey? ShortcutKey = null;

        /// <summary>
        /// 主窗口
        /// </summary>
        public static MainWindow? MainWindow = null;

        /// <summary>
        /// 图标列表
        /// </summary>
        public static Dictionary<int, IconBox> IconBoxes = new Dictionary<int, IconBox>();

        /// <summary>
        /// 全局初始化
        /// </summary>
        public static void Init()
        {
            MainWindow = new MainWindow();
            MainWindow.Show();

            Tray = new Tray();
            ShortcutKey = new ShortcutKey();
        }

        /// <summary>
        /// 全局清理
        /// </summary>
        public static void Dispose()
        {
            Tray?.Dispose();
            ShortcutKey?.Dispose();
        }
    }
}
