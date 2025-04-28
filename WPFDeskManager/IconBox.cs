using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPFDeskManager
{
    internal class IconBox
    {
        /// <summary>
        /// 图标尺寸
        /// </summary>
        private int IconSize = 32;

        /// <summary>
        /// 六边形半径
        /// </summary>
        private int HexagonRadius = 32;

        /// <summary>
        /// 吸附距离
        /// </summary>
        private int SnapDistance = 10;

        /// <summary>
        /// 实际指向的路径
        /// </summary>
        public string? TargetPath;

        /// <summary>
        /// 中心点X
        /// </summary>
        public double CenterX;

        /// <summary>
        /// 中心点Y
        /// </summary>
        public double CenterY;

        /// <summary>
        /// 图标
        /// </summary>
        public Image IconImage;

        /// <summary>
        /// 六边形
        /// </summary>
        public Path Hexagon;

        /// <summary>
        /// 吸附店
        /// </summary>
        public List<SnapPoint> SnapPoints = new List<SnapPoint>();

        /// <summary>
        /// 主容器
        /// </summary>
        public Canvas MainPanel;

        /// <summary>
        /// 菜单
        /// </summary>
        public PopMenu Pop = new PopMenu();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_canvas">主容器</param>
        /// <param name="_centerX">位置X</param>
        /// <param name="_centerY">位置Y</param>
        /// <param name="mouseLeftButtonDown">鼠标事件</param>
        public IconBox(Canvas _canvas, double _centerX, double _centerY, Action<object, MouseButtonEventArgs> mouseLeftButtonDown)
        {
            this.MainPanel = _canvas;
            this.CenterX = _centerX;
            this.CenterY = _centerY;

            // 六边形
            this.Hexagon = new Path
            {
                Data = this.CreateHexagonGeo(),
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
            Canvas.SetTop(this.Hexagon, this.CenterY);
            Canvas.SetLeft(this.Hexagon, this.CenterX);
            this.Hexagon.Drop += Path_Drop;
            this.Hexagon.MouseEnter += Path_MouseEnter;
            this.Hexagon.MouseLeave += Path_MouseLeave;
            this.Hexagon.MouseLeftButtonDown += (s, e) =>
            {
                mouseLeftButtonDown.Invoke(s, e);
            };
            this.MainPanel.Children.Add(this.Hexagon);

            // 图标
            this.IconImage = new Image
            {
                Width = this.IconSize,
                Height = this.IconSize,
                IsHitTestVisible = false,
                Source = Common.GetSvgFromResources("pack://application:,,,/Assets/icon-金牛座.svg"),
            };
            Canvas.SetTop(this.IconImage, this.CenterY - this.IconImage.Height / 2);
            Canvas.SetLeft(this.IconImage, this.CenterX - this.IconImage.Width / 2);
            this.MainPanel.Children.Add(this.IconImage);

            Global.IconBoxes.Add(this.Hexagon.GetHashCode(), this);

            this.CreateHexagonSnap();
            this.CreateContextMenu();
        }

        /// <summary>
        /// 更新六边形位置
        /// </summary>
        /// <param name="locNow">鼠标当前的位置</param>
        /// <param name="locOld">鼠标之前的位置</param>
        public void Update(Point locNow, Point locOld)
        {
            double offsetX = this.CenterX - locOld.X;
            double offsetY = this.CenterY - locOld.Y;

            this.CenterX = locNow.X + offsetX;
            this.CenterY = locNow.Y + offsetY;

            Canvas.SetTop(this.Hexagon, this.CenterY);
            Canvas.SetLeft(this.Hexagon, this.CenterX);
            Canvas.SetTop(this.IconImage, this.CenterY - this.IconImage.Height / 2);
            Canvas.SetLeft(this.IconImage, this.CenterX - this.IconImage.Width / 2);
        }

        /// <summary>
        /// 结束更新时更新一下自身信息
        /// </summary>
        /// <param name="locNow">鼠标当前的位置</param>
        public void Updated(Point locNow)
        {
            if (this.SnapToIconBox(out locNow))
            {
                this.CenterX = locNow.X;
                this.CenterY = locNow.Y;
            }

            Canvas.SetTop(this.Hexagon, this.CenterY);
            Canvas.SetLeft(this.Hexagon, this.CenterX);
            Canvas.SetTop(this.IconImage, this.CenterY - this.IconImage.Height / 2);
            Canvas.SetLeft(this.IconImage, this.CenterX - this.IconImage.Width / 2);
            this.CreateHexagonSnap();
        }

        /// <summary>
        /// 创建六边形的几何图形
        /// </summary>
        /// <returns>图形</returns>
        private PathGeometry CreateHexagonGeo()
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                double x = this.HexagonRadius * Math.Cos(angle);
                double y = this.HexagonRadius * Math.Sin(angle);
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

        /// <summary>
        /// 计算六边形的吸附点
        /// </summary>
        /// <returns></returns>
        private void CreateHexagonSnap()
        {
            List<SnapPoint> snapPoints = new List<SnapPoint>();

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i + Math.PI / 6;
                double x = this.CenterX + this.HexagonRadius * Math.Cos(Math.PI / 6) * 2 * Math.Cos(angle);
                double y = this.CenterY + this.HexagonRadius * Math.Cos(Math.PI / 6) * 2 * Math.Sin(angle);
                snapPoints.Add(new SnapPoint { Point = new Point(x, y) });
            }

            this.SnapPoints = snapPoints;
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        private void CreateContextMenu()
        {
            this.Pop = new PopMenu();

            this.Pop.AddMenuItem("添加", (object sender, RoutedEventArgs e) =>
            {
                MenuItem? menuItem = sender as MenuItem;
                if (menuItem == null)
                {
                    return;
                }
                ContextMenu? contextMenu = menuItem?.Parent as ContextMenu;
                if (contextMenu == null)
                {
                    return;
                }
                if (contextMenu.PlacementTarget is not Path path)
                {
                    return;
                }
                MessageBox.Show(path.GetHashCode().ToString());
            });
            this.Pop.AddMenuItem("删除", (object sender, RoutedEventArgs e) =>
            {

            });

            this.Hexagon.ContextMenu = this.Pop.Menu;
            this.Pop.Menu.PlacementTarget = this.Hexagon;
        }

        /// <summary>
        /// 判断六边形是否有其它可吸附的六边形
        /// </summary>
        /// <param name="point">鼠标位置</param>
        /// <returns>是否存在可吸附的六边形</returns>
        private bool SnapToIconBox(out Point point)
        {
            foreach (var item in Global.IconBoxes)
            {
                if (item.Value == this)
                {
                    continue;
                }

                double snapDistance = Math.Sqrt(Math.Pow(this.CenterX - item.Value.CenterX, 2) +
                                                Math.Pow(this.CenterY - item.Value.CenterY, 2));

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

                    double distance = Math.Sqrt(Math.Pow(this.CenterX - snap.Point.X, 2) +
                                                Math.Pow(this.CenterY - snap.Point.Y, 2));

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

        /// <summary>
        /// 文件拖入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null)
            {
                return;
            }

            foreach (string file in files)
            {
                if (!Common.GetIconFromFiles(file, out string? targetPath, out BitmapSource? iconImage))
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

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            this.AnimateShadowOpacity(Colors.Cyan, 0.7, 15);
            this.AnimateStrokeColor(Colors.Cyan);
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateShadowOpacity(Color.FromRgb(0, 150, 200), 0.4, 8);
            AnimateStrokeColor(Color.FromRgb(68, 68, 68));
        }

        /// <summary>
        /// 阴影动画
        /// </summary>
        /// <param name="toColor">颜色</param>
        /// <param name="toOpacity">透明度</param>
        /// <param name="toShadowDepth">阴影深度</param>
        private void AnimateShadowOpacity(Color toColor, double toOpacity, int toShadowDepth)
        {
            if (this.Hexagon.Effect is not DropShadowEffect effect)
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

        /// <summary>
        /// 边框颜色动画
        /// </summary>
        /// <param name="toColor">颜色</param>
        private void AnimateStrokeColor(Color toColor)
        {
            SolidColorBrush? strokeBrush = this.Hexagon.Stroke as SolidColorBrush;
            if (strokeBrush == null)
            {
                strokeBrush = new SolidColorBrush();
                this.Hexagon.Stroke = strokeBrush;
            }

            strokeBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                To = toColor,
                Duration = TimeSpan.FromMilliseconds(200)
            });
        }
    }

    /// <summary>
    /// 吸附点对象
    /// </summary>
    internal class SnapPoint
    {
        /// <summary>
        /// 是否吸附
        /// </summary>
        public bool IsSnapped { get; set; }

        /// <summary>
        /// 吸附点
        /// </summary>
        public Point Point { get; set; } = new Point();
    }
}
