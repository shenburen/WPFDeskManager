using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Input;

namespace WPFDeskManager
{
    internal class Tray
    {
        public TaskbarIcon TrayIcon;

        private Popup TrayPopup;

        public Tray()
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/logo.ico"));
            this.TrayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon(streamInfo.Stream),
                ToolTipText = "桌面整理工具"
            };

            this.TrayPopup = new Popup
            {
                Placement = PlacementMode.MousePoint,
                StaysOpen = false,
                PopupAnimation = PopupAnimation.None,
                AllowsTransparency = true,
            };
            this.TrayIcon.TrayRightMouseUp += (s, e) =>
            {
                this.TrayPopup.IsOpen = true;

                DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
                this.TrayPopup.Child.Opacity = 0;
                this.TrayPopup.Child.BeginAnimation(UIElement.OpacityProperty, animation);
            };

            CreatePopup();
        }

        private void CreatePopup()
        {
            StackPanel stackPanel = new StackPanel
            {
                Width = 140,
                Margin = new Thickness(6),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Orientation = Orientation.Vertical,
                SnapsToDevicePixels = true
            };

            this.AddMenuItem(stackPanel, "显示桌面图标", () => MessageBox.Show("打开 点击"));
            this.AddMenuItem(stackPanel, "隐藏桌面图标", () => MessageBox.Show("设置 点击"));
            this.AddSeparator(stackPanel);
            this.AddMenuItem(stackPanel, "退出", () => Application.Current.Shutdown());

            this.TrayPopup.Child = new Border
            {
                Child = stackPanel,
                Background = stackPanel.Background,
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Color.FromRgb(42, 42, 42)),
                BorderThickness = new Thickness(1),
            };
        }

        private void AddMenuItem(Panel parent, string text, Action onClick)
        {
            Label label = new Label
            {
                Cursor = Cursors.Hand,
                Content = text,
                Padding = new Thickness(6),
                FontSize = 14,
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                HorizontalContentAlignment = HorizontalAlignment.Left,
            };

            label.MouseEnter += (s, e) =>
            {
                label.Background = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                label.Foreground = Brushes.White;
            };

            label.MouseLeave += (s, e) =>
            {
                label.Background = Brushes.Transparent;
                label.Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204));
            };

            label.MouseLeftButtonUp += (s, e) =>
            {
                this.TrayPopup.IsOpen = false;
                onClick?.Invoke();
            };

            parent.Children.Add(label);
        }

        private void AddSeparator(Panel parent)
        {
            Border line = new Border
            {
                Margin = new Thickness(6, 4, 6, 4),
                Height = 1,
                Background = new SolidColorBrush(Color.FromRgb(45, 45, 45)),
            };

            parent.Children.Add(line);
        }
    }
}
