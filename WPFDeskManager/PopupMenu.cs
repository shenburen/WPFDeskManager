using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace WPFDeskManager
{
    internal class PopupMenu
    {
        /// <summary>
        /// 菜单
        /// </summary>
        public ContextMenu Menu { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PopupMenu()
        {
            this.Menu = new ContextMenu();
            ApplyContextMenuTemplate();
        }

        /// <summary>
        /// 添加菜单项
        /// </summary>
        /// <param name="text">名称</param>
        /// <param name="onClick">点击事件</param>
        /// <returns>菜单项</returns>
        public MenuItem AddMenuItem(string text, Action<object, RoutedEventArgs>? onClick = null)
        {
            MenuItem item = this.CreateMenuItem(text, onClick);
            this.Menu.Items.Add(item);
            return item;
        }

        /// <summary>
        /// 添加子菜单项
        /// </summary>
        /// <param name="menuItem">被添加的菜单项</param>
        /// <param name="text">名称</param>
        /// <param name="onClick">点击事件</param>
        /// <returns>菜单项</returns>
        public MenuItem AddMenuItem(MenuItem menuItem, string text, Action<object, RoutedEventArgs>? onClick = null)
        {
            MenuItem item = this.CreateMenuItem(text, onClick);
            menuItem.Items.Add(item);
            return item;
        }

        /// <summary>
        /// 创建菜单项
        /// </summary>
        /// <param name="text">名称</param>
        /// <param name="onClick">点击事件</param>
        /// <returns>菜单项</returns>
        private MenuItem CreateMenuItem(string text, Action<object, RoutedEventArgs>? onClick = null)
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
                if (onClick != null)
                {
                    onClick?.Invoke(s, e);
                }
            };

            return item;
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
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
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

            FrameworkElementFactory itemsPresenter = new FrameworkElementFactory(typeof(ItemsPresenter));

            FrameworkElementFactory itemsBorder = new FrameworkElementFactory(typeof(Border));
            itemsBorder.SetValue(Border.PaddingProperty, new Thickness(6));
            itemsBorder.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromArgb(220, 0, 0, 0)));
            itemsBorder.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(68, 68, 68)));
            itemsBorder.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            itemsBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            itemsBorder.AppendChild(itemsPresenter);

            FrameworkElementFactory popup = new FrameworkElementFactory(typeof(Popup));
            popup.SetValue(Popup.IsOpenProperty, new TemplateBindingExtension(MenuItem.IsSubmenuOpenProperty));
            popup.SetValue(Popup.PlacementProperty, PlacementMode.Right);
            popup.SetValue(Popup.AllowsTransparencyProperty, true);
            popup.AppendChild(itemsBorder);

            FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
            grid.AppendChild(border);
            grid.AppendChild(popup);

            ControlTemplate template = new ControlTemplate(typeof(MenuItem));
            template.VisualTree = grid;

            item.Template = template;
        }
    }
}
