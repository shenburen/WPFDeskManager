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
            hideDesktopIcon.Click += (s, e) => { MessageBox.Show("隐藏桌面图标"); };
            contextMenu.Items.Add(hideDesktopIcon);

            MenuItem showDesktopIcon = new MenuItem { Header = "显示桌面图标" };
            showDesktopIcon.Click += (s, e) => { MessageBox.Show("显示桌面图标"); };
            contextMenu.Items.Add(showDesktopIcon);

            MenuItem exitApplication = new MenuItem { Header = "退出" };
            exitApplication.Click += (s, e) => { MessageBox.Show("退出"); };
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
    }
}
