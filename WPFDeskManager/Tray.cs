using Hardcodet.Wpf.TaskbarNotification;
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
        public TaskbarIcon TrayIcon;

        /// <summary>
        /// 弹出菜单
        /// </summary>
        private PopupMenu Popup;

        #region Win32 API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string? lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_COMMAND = 0x0111;
        private static readonly IntPtr TOGGLE_SHOW_DESKTOP_ICONS = new IntPtr(0x7402);
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Tray()
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/logo.ico"));
            this.TrayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon(streamInfo.Stream),
                Visibility = Visibility.Visible,
                ToolTipText = "桌面整理工具",
            };

            this.Popup = new PopupMenu();
            this.Popup.AddMenuItem("切换桌面图标", this.SwitchDesktopEvent);
            this.Popup.AddMenuItem("退出", this.ExitApplication);
            this.TrayIcon.ContextMenu = this.Popup.Menu;
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            this.TrayIcon.Dispose();
        }

        /// <summary>
        /// 切换桌面图标显示状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchDesktopEvent(object sender, RoutedEventArgs e)
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
        /// 退出应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
