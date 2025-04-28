using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace WPFDeskManager
{
    internal class PopMenu
    {
        /// <summary>
        /// 菜单
        /// </summary>
        public ContextMenu Menu;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PopMenu()
        {
            this.Menu = new ContextMenu();
            ApplyContextMenuTemplate();
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="text">名称</param>
        /// <param name="onClick">点击事件</param>
        public void AddMenuItem(string text, Action<object, RoutedEventArgs> onClick)
        {
            MenuItem item = new MenuItem
            {
                Header = text,
                FontSize = 14,
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
            };
            ApplyMenuItemTemplate(item);

            item.MouseEnter += (s, e) =>
            {
                item.Foreground = new SolidColorBrush(Colors.Cyan);
            };

            item.MouseLeave += (s, e) =>
            {
                item.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            };

            item.Click += (s, e) =>
            {
                onClick?.Invoke(s, e);
            };

            this.Menu.Items.Add(item);
        }

        /// <summary>
        /// 重写 ContextMenu 的模板
        /// </summary>
        private void ApplyContextMenuTemplate()
        {
            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.IsItemsHostProperty, true);
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.PaddingProperty, new Thickness(6));
            border.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromArgb(220, 0, 0, 0)));
            border.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(68, 68, 68)));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            border.AppendChild(stackPanel);

            ControlTemplate template = new ControlTemplate(typeof(ContextMenu));
            template.VisualTree = border;

            this.Menu.Template = template;
        }

        /// <summary>
        /// 重写 MenuItem 的模板
        /// </summary>
        /// <param name="item">MenuItem</param>
        private void ApplyMenuItemTemplate(MenuItem item)
        {
            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            contentPresenter.SetValue(ContentPresenter.MarginProperty, new Thickness(6));
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.AppendChild(contentPresenter);

            ControlTemplate template = new ControlTemplate(typeof(MenuItem));
            template.VisualTree = border;

            item.Template = template;
        }
    }
}
