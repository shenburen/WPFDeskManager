using Microsoft.Win32;
using System.Diagnostics;

namespace WPFDeskManager
{
    // 待办事项：
    //   本地持久化
    //   系统通知：这个问题还挺棘手，需要指定一个windows版本，并且因为引用的SDK过大，所以暂时先搁置。
    //   设置界面
    //   悬停提示
    //   根节点点击隐藏所有图标
    //   解链
    internal class Global
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public const string AppName = "WPFDeskManager";

        /// <summary>
        /// 主窗口
        /// </summary>
        public static MainWindow? MainWindow { get; set; } = null;

        /// <summary>
        /// 设置窗口
        /// </summary>
        public static Setting? Setting { get; set; } = null;

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

            SetStartup(false);
        }

        /// <summary>
        /// 全局清理
        /// </summary>
        public static void Dispose()
        {
            Tray.Dispose();
            ShortcutKey.Dispose();
        }

        /// <summary>
        /// 注册开启启动
        /// </summary>
        /// <param name="enable">是否开启启动</param>
        private static void SetStartup(bool enable)
        {
            ProcessModule? mainModule = Process.GetCurrentProcess().MainModule;
            if (mainModule == null)
            {
                return;
            }

            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (registryKey == null)
            {
                return;
            }

            if (enable)
            {
                registryKey.SetValue(AppName, $"\"{mainModule.FileName}\"");
            }
            else
            {
                registryKey.DeleteValue(AppName, false);
            }
        }
    }
}
