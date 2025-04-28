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

        public PopMenu Pop;

        public Tray()
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/logo.ico"));
            this.TrayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon(streamInfo.Stream),
                Visibility = Visibility.Visible,
                ToolTipText = "桌面整理工具",
            };

            this.Pop = new PopMenu();

            this.Pop.AddMenuItem("隐藏桌面图标", this.HideDesktopEvent);
            this.Pop.AddMenuItem("显示桌面图标", this.ShowDesktopEvent);
            this.Pop.AddMenuItem("退出", this.ExitApplication);

            this.TrayIcon.ContextMenu = this.Pop.Menu;
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
