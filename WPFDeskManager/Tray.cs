using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Resources;

namespace WPFDeskManager
{
    internal class Tray
    {
        /*public TaskbarIcon TrayIcon;*/

        public Tray()
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/logo.ico"));
            /*this.TrayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon(streamInfo.Stream),
                ToolTipText = "桌面整理工具"
            };*/
        }
    }
}
