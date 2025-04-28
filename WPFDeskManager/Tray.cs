using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Resources;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WPFDeskManager
{
    internal class Tray
    {
        public TaskbarIcon TrayIcon;

        public Tray()
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem hideDesktopIcon = new MenuItem { Header = "隐藏桌面图标" };
            hideDesktopIcon.Click += HideDesktopEvent;
            contextMenu.Items.Add(hideDesktopIcon);

            MenuItem showDesktopIcon = new MenuItem { Header = "显示桌面图标" };
            showDesktopIcon.Click += ShowDesktopEvent;
            contextMenu.Items.Add(showDesktopIcon);

            MenuItem exitApplication = new MenuItem { Header = "退出" };
            exitApplication.Click += ExitApplication;
            contextMenu.Items.Add(exitApplication);

            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/logo.ico"));
            this.TrayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon(streamInfo.Stream),
                Visibility = Visibility.Visible,
                ToolTipText = "桌面整理工具",
                ContextMenu = contextMenu,
            };
        }

        private void HideDesktopEvent(object sender, RoutedEventArgs e)
        {

        }

        private void ShowDesktopEvent(object sender, RoutedEventArgs e)
        {

        }

        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
