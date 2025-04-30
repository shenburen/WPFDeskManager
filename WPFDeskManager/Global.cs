namespace WPFDeskManager
{
    // 待办事项：
    // 本地持久化
    // 系统通知
    // 开机自启动
    // 设置界面
    // 悬停提示
    // 根节点点击隐藏所有图标
    // 解链
    internal class Global
    {
        /// <summary>
        /// 主窗口
        /// </summary>
        public static MainWindow? MainWindow { get; set; } = null;

        /// <summary>
        /// 图标列表
        /// </summary>
        public static Dictionary<int, IconBoxInfo> IconBoxInfos { get; set; } = new Dictionary<int, IconBoxInfo>();

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
