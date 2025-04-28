using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace WPFDeskManager
{
    internal class PopMenu
    {
        public ContextMenu Menu;

        public PopMenu()
        {
            this.Menu = new ContextMenu();
            this.Menu.Opacity = 1;
            this.Menu.Padding = new Thickness(5);
            this.Menu.Background = new SolidColorBrush(Color.FromArgb(220, 34, 34, 34));
            this.Menu.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            this.Menu.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            this.Menu.BorderThickness = new Thickness(1);
            ApplyContextMenuTemplate();
            //this.Pop.PlacementTarget = path;
            //this.Pop.Opened += (s, e) =>
            //{
            //    ContextMenu contextMenu = s as ContextMenu;
            //    Path sourceControl = (Path)(contextMenu.PlacementTarget as FrameworkElement);
            //};
        }

        public void AddMenuItem(string text, Action<object, RoutedEventArgs> onClick)
        {
            MenuItem item = new MenuItem
            {
                Header = text,
                Padding = new Thickness(8, 4, 8, 4),
                FontWeight = FontWeights.SemiBold,
                Background = new SolidColorBrush(Color.FromArgb(150, 34, 34, 34)),
                Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                BorderThickness = new Thickness(0),
            };
            ApplyMenuItemTemplate(item);

            item.MouseEnter += (s, e) =>
            {
                item.Background = new SolidColorBrush(Color.FromArgb(220, 0, 255, 255));
                item.Foreground = new SolidColorBrush(Colors.Black);
            };

            item.MouseLeave += (s, e) =>
            {
                item.Background = new SolidColorBrush(Color.FromArgb(150, 34, 34, 34));
                item.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            };

            item.Click += (s, e) =>
            {
                var menuItem = s as MenuItem;
                var contextMenu = menuItem?.Parent as ContextMenu;
                var sourceControl = contextMenu?.PlacementTarget;

                onClick?.Invoke(s, e);
            };

            this.Menu.Items.Add(item);
        }

        private void ApplyContextMenuTemplate()
        {
            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.IsItemsHostProperty, true);
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(ContextMenu.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(ContextMenu.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(ContextMenu.BorderThicknessProperty));
            border.SetValue(Border.SnapsToDevicePixelsProperty, true);
            border.AppendChild(stackPanel);

            ControlTemplate template = new ControlTemplate(typeof(ContextMenu));
            template.VisualTree = border;

            this.Menu.Template = template;
        }

        private void ApplyMenuItemTemplate(MenuItem item)
        {
            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            contentPresenter.SetValue(ContentPresenter.MarginProperty, new Thickness(8, 4, 8, 4));
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(MenuItem.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(MenuItem.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(MenuItem.BorderThicknessProperty));
            border.AppendChild(contentPresenter);

            ControlTemplate template = new ControlTemplate(typeof(MenuItem));
            template.VisualTree = border;

            item.Template = template;
        }
    }
}
