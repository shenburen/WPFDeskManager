using Hardcodet.Wpf.TaskbarNotification;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Resources;

namespace WPFDeskManager
{
    internal class Tray
    {
        /// <summary>
        /// 托盘图标
        /// </summary>
        public static TaskbarIcon? TrayIcon { get; set; }

        /// <summary>
        /// 弹出菜单
        /// </summary>
        private static PopupMenu? Popup { get; set; }

        #region Win32 API
        private const uint WM_COMMAND = 0x0111;
        private static readonly IntPtr TOGGLE_SHOW_DESKTOP_ICONS = new IntPtr(0x7402);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string? lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion

        /// <summary>
        /// 创建托盘图标
        /// </summary>
        public static void CreateTray()
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/logo.ico"));
            TrayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon(streamInfo.Stream),
                Visibility = Visibility.Visible,
                ToolTipText = "桌面整理工具",
            };

            Popup = new PopupMenu();
            Popup.AddMenuItem("切换桌面", SwitchDesktopEvent);
            Popup.AddMenuItem("保存当前", SaveNowIcon);
            Popup.AddMenuItem("设置", OpenSetting);
            Popup.AddMenuItem("退出", ExitApplication);
            TrayIcon.ContextMenu = Popup.Menu;
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public static void Dispose()
        {
            TrayIcon?.Dispose();
        }

        /// <summary>
        /// 切换桌面图标显示状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SwitchDesktopEvent(object sender, RoutedEventArgs e)
        {
            IntPtr progman = FindWindow("Progman", null);
            IntPtr shellViewWin = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (shellViewWin == IntPtr.Zero)
            {
                return;
            }

            SendMessage(shellViewWin, WM_COMMAND, TOGGLE_SHOW_DESKTOP_ICONS, IntPtr.Zero);
        }

        /// <summary>
        /// 保存桌面图标当前状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SaveNowIcon(object sender, RoutedEventArgs e)
        {
            if (SerializerHelper.SaveToFile())
            {
                Debug.WriteLine("保存成功！");
            }
            else
            {
                Debug.WriteLine("保存失败，发生了不可预估的错误！");
            }
        }

        /// <summary>
        /// 打开设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OpenSetting(object sender, RoutedEventArgs e)
        {
            Global.Setting = new Setting();
            Global.Setting.Show();
        }

        /// <summary>
        /// 退出应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ExitApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
