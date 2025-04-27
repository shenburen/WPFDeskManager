using Hardcodet.Wpf.TaskbarNotification;
using System.Drawing;
using System.Windows;
using System.Windows.Resources;

namespace WPFDeskManager
{
    internal class TrayManager
    {
        public static void CreateTrayIcon()
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/logo.ico"));
            Global.TrayIcon = new TaskbarIcon
            {
                Icon = new Icon(streamInfo.Stream),
                ToolTipText = "桌面整理工具"
            };
        }
    }
}
