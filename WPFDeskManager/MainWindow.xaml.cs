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
            if (this.CurrentPath != null)
            {
                this.CurrentPath = null;
                this.MainPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
            }
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
                HexagonPath = path
            };
            IconBoxManager.IconBoxes.Add(path.GetHashCode(), iconBox);
        }

        private void UpdateIconBoxLoc(Point loc)
        {
            if (this.CurrentPath == null)
            {
                return;
            }

            this.SnapToIconBox(out loc);

            double offsetX = this.CurrentPath.CenterX - this.CurrentLoc.X;
            double offsetY = this.CurrentPath.CenterY - this.CurrentLoc.Y;

            this.CurrentPath.HexagonPath.Data = this.CreateHexagonGeo(loc.X + offsetX, loc.Y + offsetY);

            this.CurrentLoc = loc;
            this.CurrentPath.CenterX = loc.X + offsetX;
            this.CurrentPath.CenterY = loc.Y + offsetY;
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

        private void SnapToIconBox(out Point point)
        {
            foreach (var item in IconBoxManager.IconBoxes)
            {
                if (item.Value == this.CurrentPath)
                {
                    continue;
                }

                double distance = Math.Sqrt(Math.Pow(this.CurrentPath.CenterX - item.Value.CenterX, 2) +
                    Math.Pow(this.CurrentPath.CenterY - item.Value.CenterY, 2));
                if (distance < (this.Radius * 2 - 10) || distance > (this.Radius * 2 + 10))
                {
                    continue;
                }

                double dx = this.CurrentPath.CenterX - item.Value.CenterX;
                double dy = this.CurrentPath.CenterY - item.Value.CenterY;

                double factor = this.Radius / Math.Sqrt(dx * dx + dy * dy);

                point = new Point(item.Value.CenterX + dx * factor, item.Value.CenterY + dy * factor);

                return;
            }
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
