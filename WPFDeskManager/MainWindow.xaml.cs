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
        private int IconSize = 32;
        private int HexagonRadius = 32;
        private int SnapDistance = 10;

        private Point CurrentLoc = new Point();
        private IconBox? CurrentPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            this.CreateIconBox(400, 200);
            this.CreateIconBox(600, 400);
            this.CreateIconBox(800, 600);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.UpdateIconBoxLoc(e.GetPosition(this));
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            this.FinishedMoveIconBox(e.GetPosition(this));
        }

        private void CreateIconBox(double centerX, double centerY)
        {
            Path path = new Path
            {
                Data = this.CreateHexagonGeo(0, 0),
                Fill = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(0, 150, 200),
                    Opacity = 0.4,
                    BlurRadius = 8,
                    ShadowDepth = 0,
                },
                Stroke = new SolidColorBrush(Color.FromRgb(68, 68, 68)),
                StrokeThickness = 2,
                AllowDrop = true,
            };
            Canvas.SetTop(path, centerY);
            Canvas.SetLeft(path, centerX);
            path.Drop += Path_Drop;
            path.MouseEnter += Path_MouseEnter;
            path.MouseLeave += Path_MouseLeave;
            path.MouseLeftButtonDown += Path_MouseLeftButtonDown;

            BitmapImage image = Common.GetSvgFromResources("pack://application:,,,/Assets/root.svg");

            Image icon = new Image
            {
                Width = this.IconSize,
                Height = this.IconSize,
                IsHitTestVisible = false,
                Source = image,
            };
            Canvas.SetTop(icon, centerY - icon.Height / 2);
            Canvas.SetLeft(icon, centerX - icon.Width / 2);

            this.MainPanel.Children.Add(path);
            this.MainPanel.Children.Add(icon);

            IconBox iconBox = new IconBox
            {
                CenterX = centerX,
                CenterY = centerY,
                IconImage = icon,
                HexagonPath = path,
                SnapPoints = this.CreateHexagonSnap(centerX, centerY),
            };
            Global.IconBoxes.Add(path.GetHashCode(), iconBox);



            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Background = new SolidColorBrush(Color.FromArgb(220, 34, 34, 34)); // 深灰黑
            contextMenu.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255)); // 亮青色
            contextMenu.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            contextMenu.BorderThickness = new Thickness(1);
            contextMenu.Padding = new Thickness(5);
            contextMenu.Opacity = 0.95;
            contextMenu.PlacementTarget = path;
            contextMenu.Opened += (s, e) =>
            {
                ContextMenu contextMenu = s as ContextMenu;
                Path sourceControl = (Path)(contextMenu.PlacementTarget as FrameworkElement);
            };
            ApplyTechStyleToContextMenu(contextMenu);

            // 给菜单加一点小动画（弹出时渐变）
            contextMenu.Opened += (s, e) =>
            {
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200)));
                contextMenu.BeginAnimation(ContextMenu.OpacityProperty, fadeIn);
            };

            MenuItem rotateItem = CreateTechMenuItem("旋转", () =>
            {
                MessageBox.Show("1");
            });
            MenuItem copyItem = CreateTechMenuItem("复制", () =>
            {
                MessageBox.Show("1");
            });
            MenuItem deleteItem = CreateTechMenuItem("删除", () =>
            {
                MessageBox.Show("1");
            });

            contextMenu.Items.Add(rotateItem);
            contextMenu.Items.Add(copyItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(deleteItem);

            path.ContextMenu = contextMenu;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateIconBoxLoc(Point loc)
        {
            if (this.CurrentPath == null)
            {
                return;
            }

            double offsetX = this.CurrentPath.CenterX - this.CurrentLoc.X;
            double offsetY = this.CurrentPath.CenterY - this.CurrentLoc.Y;

            this.CurrentLoc = loc;
            this.CurrentPath.CenterX = loc.X + offsetX;
            this.CurrentPath.CenterY = loc.Y + offsetY;

            Canvas.SetTop(this.CurrentPath.HexagonPath, this.CurrentPath.CenterY);
            Canvas.SetLeft(this.CurrentPath.HexagonPath, this.CurrentPath.CenterX);
            Canvas.SetTop(this.CurrentPath.IconImage, this.CurrentPath.CenterY - this.CurrentPath.IconImage.Height / 2);
            Canvas.SetLeft(this.CurrentPath.IconImage, this.CurrentPath.CenterX - this.CurrentPath.IconImage.Width / 2);
        }

        private void FinishedMoveIconBox(Point loc)
        {
            if (this.CurrentPath == null)
            {
                return;
            }

            if (this.SnapToIconBox(out loc))
            {
                this.CurrentPath.CenterX = loc.X;
                this.CurrentPath.CenterY = loc.Y;
            }
            Canvas.SetTop(this.CurrentPath.HexagonPath, this.CurrentPath.CenterY);
            Canvas.SetLeft(this.CurrentPath.HexagonPath, this.CurrentPath.CenterX);
            Canvas.SetTop(this.CurrentPath.IconImage, this.CurrentPath.CenterY - this.CurrentPath.IconImage.Height / 2);
            Canvas.SetLeft(this.CurrentPath.IconImage, this.CurrentPath.CenterX - this.CurrentPath.IconImage.Width / 2);
            this.CurrentPath.SnapPoints = this.CreateHexagonSnap(this.CurrentPath.CenterX, this.CurrentPath.CenterY);

            AnimateShadowOpacity(this.CurrentPath.HexagonPath, Color.FromRgb(0, 150, 200), 0.4, 8);
            AnimateStrokeColor(this.CurrentPath.HexagonPath, Color.FromRgb(68, 68, 68));

            this.CurrentPath = null;
            this.MainPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
        }

        private PathGeometry CreateHexagonGeo(double centerX, double centerY)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                double x = centerX + this.HexagonRadius * Math.Cos(angle);
                double y = centerY + this.HexagonRadius * Math.Sin(angle);
                points.Add(new Point(x, y));
            }

            PathFigure figure = new PathFigure { StartPoint = points[0], IsClosed = true };
            for (int i = 1; i < points.Count; i++)
            {
                figure.Segments.Add(new LineSegment(points[i], true));
            }

            PathGeometry geo = new PathGeometry();
            geo.Figures.Add(figure);

            return geo;
        }

        private List<SnapPoint> CreateHexagonSnap(double centerX, double centerY)
        {
            List<SnapPoint> snapPoints = new List<SnapPoint>();

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i + Math.PI / 6;
                double x = centerX + this.HexagonRadius * Math.Cos(Math.PI / 6) * 2 * Math.Cos(angle);
                double y = centerY + this.HexagonRadius * Math.Cos(Math.PI / 6) * 2 * Math.Sin(angle);
                snapPoints.Add(new SnapPoint { Point = new Point(x, y) });
            }

            return snapPoints;
        }

        private bool SnapToIconBox(out Point point)
        {
            if (this.CurrentPath == null)
            {
                return false;
            }

            foreach (var item in Global.IconBoxes)
            {
                if (item.Value == this.CurrentPath)
                {
                    continue;
                }

                double snapDistance = Math.Sqrt(Math.Pow(this.CurrentPath.CenterX - item.Value.CenterX, 2) +
                                                Math.Pow(this.CurrentPath.CenterY - item.Value.CenterY, 2));

                double minDistance = this.HexagonRadius * Math.Cos(Math.PI / 6) * 2 - this.SnapDistance;
                double maxDistance = this.HexagonRadius * Math.Cos(Math.PI / 6) * 2 + this.SnapDistance;

                if (snapDistance < minDistance || snapDistance > maxDistance)
                {
                    continue;
                }

                bool isFinished = false;
                double nearestDistance = double.MaxValue;
                foreach (SnapPoint snap in item.Value.SnapPoints)
                {
                    if (snap.IsSnapped)
                    {
                        continue;
                    }

                    double distance = Math.Sqrt(Math.Pow(this.CurrentPath.CenterX - snap.Point.X, 2) +
                                                Math.Pow(this.CurrentPath.CenterY - snap.Point.Y, 2));

                    if (distance < nearestDistance)
                    {
                        point = snap.Point;
                        isFinished = true;
                        nearestDistance = distance;
                    }
                }

                if (!isFinished)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void Path_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null)
            {
                return;
            }

            foreach (string file in files)
            {
                string? targetPath;
                BitmapSource? iconImage;
                if (!Common.GetIconFromFiles(file, out targetPath, out iconImage))
                {
                    continue;
                }

                if (sender is not Path path)
                {
                    continue;
                }

                if (!Global.IconBoxes.TryGetValue(path.GetHashCode(), out IconBox? iconBox))
                {
                    continue;
                }

                iconBox.TargetPath = targetPath;
                iconBox.IconImage.Source = iconImage;
            }
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not Path path)
            {
                return;
            }
        }

        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not Path path)
            {
                return;
            }
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

            AnimateShadowOpacity(path, Colors.Cyan, 0.7, 15);
            AnimateStrokeColor(path, Colors.Cyan);
        }

        private void AnimateShadowOpacity(Path path, Color toColor, double toOpacity, int toShadowDepth)
        {
            if (path.Effect is not DropShadowEffect effect)
            {
                return;
            }

            effect.BeginAnimation(DropShadowEffect.ColorProperty, new ColorAnimation
            {
                To = toColor,
                Duration = TimeSpan.FromMilliseconds(200)
            });
            effect.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation
            {
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(200)
            });
            effect.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation
            {
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(200)
            });
        }

        private void AnimateStrokeColor(Path path, Color toColor)
        {
            SolidColorBrush? strokeBrush = path.Stroke as SolidColorBrush;
            if (strokeBrush == null)
            {
                strokeBrush = new SolidColorBrush();
                path.Stroke = strokeBrush;
            }

            strokeBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                To = toColor,
                Duration = TimeSpan.FromMilliseconds(200)
            });
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
            RemoveGlyphArea(item);

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

        private void RemoveGlyphArea(MenuItem item)
        {
            ControlTemplate template = new ControlTemplate(typeof(MenuItem));

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(MenuItem.BackgroundProperty));

            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            contentPresenter.SetValue(ContentPresenter.MarginProperty, new Thickness(5, 2, 5, 2));
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            item.Template = template;
        }

        private void ApplyTechStyleToMenuItem(MenuItem item)
        {
            ControlTemplate template = new ControlTemplate(typeof(MenuItem));

            // 整体外框
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(MenuItem.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(MenuItem.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(MenuItem.BorderThicknessProperty));

            // 里面只放真正的内容，没有Icon Grid了！
            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            contentPresenter.SetValue(ContentPresenter.MarginProperty, new Thickness(8, 4, 8, 4));
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);

            border.AppendChild(contentPresenter);

            template.VisualTree = border;

            item.Template = template;
        }

        private void ApplyTechStyleToContextMenu(ContextMenu menu)
        {
            menu.Background = new SolidColorBrush(Color.FromArgb(230, 20, 20, 20)); // 深黑
            menu.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255)); // 科技蓝边
            menu.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            menu.BorderThickness = new Thickness(1);
            menu.Padding = new Thickness(5);

            // 重要：自定义ContextMenu自己的Template
            ControlTemplate template = new ControlTemplate(typeof(ContextMenu));

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(ContextMenu.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(ContextMenu.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(ContextMenu.BorderThicknessProperty));
            border.SetValue(Border.SnapsToDevicePixelsProperty, true);

            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.IsItemsHostProperty, true);
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

            border.AppendChild(stackPanel);
            template.VisualTree = border;

            menu.Template = template;
        }

    }
}
