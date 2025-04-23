using System.Windows;
using System.Windows.Controls;
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
        private Point CurrentLoc = new Point();
        private IconBox? CurrentPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            this.CreateIconBox();
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
                double offsetX = this.CurrentPath.CenterX - this.CurrentLoc.X;
                double offsetY = this.CurrentPath.CenterY - this.CurrentLoc.Y;

                Point loc = e.GetPosition(this);
                this.CurrentPath.CenterX = loc.X + offsetX;
                this.CurrentPath.CenterY = loc.Y + offsetY;

                this.CurrentPath = null;
            }
        }

        private void CreateIconBox()
        {
            double centerX = this.Width / 2;
            double centerY = this.Height / 2;

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

            double offsetX = this.CurrentPath.CenterX - this.CurrentLoc.X;
            double offsetY = this.CurrentPath.CenterY - this.CurrentLoc.Y;

            this.CurrentPath.HexagonPath.Data = this.CreateHexagonGeo(loc.X + offsetX, loc.Y + offsetY);
        }

        private PathGeometry CreateHexagonGeo(double centerX, double centerY)
        {
            PointCollection points = new PointCollection();

            double radius = 32;
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);
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

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Path path)
            {
                if (IconBoxManager.IconBoxes.TryGetValue(path.GetHashCode(), out IconBox? iconBox))
                {
                    this.CurrentLoc = e.GetPosition(this);
                    this.CurrentPath = iconBox;
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
