using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Resources;

namespace WPFDeskManager
{
    internal class Tray
    {
        public TaskbarIcon TrayIcon;

        public PopupMenu Popup;

        #region Win32 API
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string? lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const uint WM_COMMAND = 0x0111;
        public static readonly IntPtr TOGGLE_SHOW_DESKTOP_ICONS = new IntPtr(0x7402);
        #endregion

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

        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
