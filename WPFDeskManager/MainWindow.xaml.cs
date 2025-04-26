using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDeskManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int Radius = 32;
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
            this.CreateIconBox(200, 200);
            this.CreateIconBox(400, 400);
            this.CreateIconBox(600, 600);
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
            Path path = new Path();
            path.Fill = new SolidColorBrush(Colors.LightBlue);
            path.Stroke = new SolidColorBrush(Colors.DarkBlue);
            path.StrokeThickness = 2;
            path.Data = this.CreateHexagonGeo(centerX, centerY);

            path.MouseLeftButtonDown += Path_MouseLeftButtonDown;
            path.MouseEnter += Path_MouseEnter;
            path.MouseLeave += Path_MouseLeave;

            this.MainPanel.Children.Add(path);

            IconBox iconBox = new IconBox
            {
                CenterX = centerX,
                CenterY = centerY,
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

            this.CurrentPath.HexagonPath.Data = this.CreateHexagonGeo(this.CurrentPath.CenterX, this.CurrentPath.CenterY);
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
                this.CurrentPath.HexagonPath.Data = this.CreateHexagonGeo(this.CurrentPath.CenterX, this.CurrentPath.CenterY);
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
                double x = centerX + this.Radius * Math.Cos(angle);
                double y = centerY + this.Radius * Math.Sin(angle);
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
                double x = centerX + this.Radius * Math.Cos(Math.PI / 6) * 2 * Math.Cos(angle);
                double y = centerY + this.Radius * Math.Cos(Math.PI / 6) * 2 * Math.Sin(angle);
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

                double minDistance = this.Radius * Math.Cos(Math.PI / 6) * 2 - this.SnapDistance;
                double maxDistance = this.Radius * Math.Cos(Math.PI / 6) * 2 + this.SnapDistance;

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
