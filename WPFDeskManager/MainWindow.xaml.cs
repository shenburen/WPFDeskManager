using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFDeskManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point CurrentLoc = new Point();
        private IconBox? CurrentPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            new IconBox(this.MainPanel, 400, 200, HexagonMouseLeftButtonDown);
            new IconBox(this.MainPanel, 600, 400, HexagonMouseLeftButtonDown);
            new IconBox(this.MainPanel, 800, 600, HexagonMouseLeftButtonDown);
        }

        private void HexagonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Path path)
            {
                return;
            }

            if (!Global.IconBoxes.TryGetValue(path.GetHashCode(), out IconBox? iconBox))
            {
                return;
            }

            this.CurrentLoc = e.GetPosition(this);
            this.CurrentPath = iconBox;
            this.MainPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#19000000"));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.CurrentPath == null)
            {
                return;
            }

            Point loc = e.GetPosition(this.MainPanel);
            this.CurrentPath.Update(loc, this.CurrentLoc);
            this.CurrentLoc = loc;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.CurrentPath == null)
            {
                return;
            }

            Point loc = e.GetPosition(this);
            this.CurrentPath.Updated(loc);

            this.CurrentPath = null;
            this.MainPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
        }

        private void CreateIconBox(double centerX, double centerY)
        {
            //ContextMenu contextMenu = new ContextMenu();
            //contextMenu.Background = new SolidColorBrush(Color.FromArgb(220, 34, 34, 34)); // 深灰黑
            //contextMenu.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255)); // 亮青色
            //contextMenu.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            //contextMenu.BorderThickness = new Thickness(1);
            //contextMenu.Padding = new Thickness(5);
            //contextMenu.Opacity = 0.95;
            //contextMenu.PlacementTarget = path;
            //contextMenu.Opened += (s, e) =>
            //{
            //    ContextMenu contextMenu = s as ContextMenu;
            //    Path sourceControl = (Path)(contextMenu.PlacementTarget as FrameworkElement);
            //};
            //ApplyTechStyleToContextMenu(contextMenu);

            //// 给菜单加一点小动画（弹出时渐变）
            //contextMenu.Opened += (s, e) =>
            //{
            //    DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
            //    contextMenu.BeginAnimation(ContextMenu.OpacityProperty, fadeIn);
            //};

            //MenuItem rotateItem = CreateTechMenuItem("旋转", () =>
            //{
            //    MessageBox.Show("1");
            //});
            //MenuItem copyItem = CreateTechMenuItem("复制", () =>
            //{
            //    MessageBox.Show("1");
            //});
            //MenuItem deleteItem = CreateTechMenuItem("删除", () =>
            //{
            //    MessageBox.Show("1");
            //});

            //contextMenu.Items.Add(rotateItem);
            //contextMenu.Items.Add(copyItem);
            //contextMenu.Items.Add(new Separator());
            //contextMenu.Items.Add(deleteItem);

            //path.ContextMenu = contextMenu;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private MenuItem CreateTechMenuItem(string text, Action onClick)
        {
            MenuItem item = new MenuItem
            {
                Header = text,
                Background = new SolidColorBrush(Color.FromArgb(150, 34, 34, 34)),
                Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                BorderThickness = new Thickness(0),
                Padding = new Thickness(8, 4, 8, 4),
                FontWeight = FontWeights.SemiBold,
            };
            ApplyTechStyleToMenuItem(item);

            item.MouseEnter += (s, e) =>
            {
                item.Background = new SolidColorBrush(Color.FromArgb(220, 0, 255, 255)); // 鼠标悬停高亮
                item.Foreground = new SolidColorBrush(Colors.Black); // 字体颜色反色
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

                onClick?.Invoke();
            };

            return item;
        }

        private void ApplyTechStyleToMenuItem(MenuItem item)
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

        private void ApplyTechStyleToContextMenu(ContextMenu menu)
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

            menu.Template = template;
        }
    }
}
