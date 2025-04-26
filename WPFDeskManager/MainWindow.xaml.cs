using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
                Fill = new SolidColorBrush(Colors.LightBlue),
                Stroke = new SolidColorBrush(Colors.DarkBlue),
                StrokeThickness = 2,
                Data = this.CreateHexagonGeo(0, 0),
            };
            Canvas.SetTop(path, centerY);
            Canvas.SetLeft(path, centerX);
            path.MouseEnter += Path_MouseEnter;
            path.MouseLeave += Path_MouseLeave;
            path.MouseLeftButtonDown += Path_MouseLeftButtonDown;

            Image icon = new Image
            {
                Width = this.IconSize,
                Height = this.IconSize,
                IsHitTestVisible = false,
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/root.png")),
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
            IconBoxManager.IconBoxes.Add(path.GetHashCode(), iconBox);
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
            if (this.CurrentPath != null)
            {
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

                this.CurrentPath = null;
                this.MainPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
            }
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
            foreach (var item in IconBoxManager.IconBoxes)
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

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Path path)
            {
                if (IconBoxManager.IconBoxes.TryGetValue(path.GetHashCode(), out IconBox? iconBox))
                {
                    this.CurrentLoc = e.GetPosition(this);
                    this.CurrentPath = iconBox;
                    this.MainPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#19000000"));
                }
            }
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Path path)
            {
                path.Fill = new SolidColorBrush(Colors.LightGreen);
            }
        }

        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Path path)
            {
                path.Fill = new SolidColorBrush(Colors.LightBlue);
            }
        }
    }
}
