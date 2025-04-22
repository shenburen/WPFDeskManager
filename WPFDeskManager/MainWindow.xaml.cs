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
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            this.CreateHexagon();
        }

        private void CreateHexagon()
        {
            double radius = 32;
            double centerX = this.Width / 2;
            double centerY = this.Height / 2;

            PointCollection points = new PointCollection();
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

            Path path = new Path();
            path.Fill = new SolidColorBrush(Colors.LightBlue);
            path.Stroke = new SolidColorBrush(Colors.DarkBlue);
            path.StrokeThickness = 2;
            path.Data = geo;

            path.MouseLeftButtonDown += Path_MouseLeftButtonDown;
            path.MouseLeftButtonUp += Path_MouseLeftButtonUp;
            path.MouseMove += Path_MouseMove;
            path.MouseEnter += Path_MouseEnter;
            path.MouseLeave += Path_MouseLeave;

            this.MainPanel.Children.Add(path);
        }

        private bool isDragging = false;
        private Point startPoint;

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Path path)
            {
                isDragging = true;
                startPoint = e.GetPosition(this.MainPanel);
                path.CaptureMouse();
            }
        }

        private void Path_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Path path)
            {
                isDragging = false;
                path.ReleaseMouseCapture();
            }
        }

        private void Path_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && sender is Path path)
            {
                Point currentPoint = e.GetPosition(this.MainPanel);
                double offsetX = currentPoint.X - startPoint.X;
                double offsetY = currentPoint.Y - startPoint.Y;

                // 更新 Path 的位置
                Canvas.SetLeft(path, Canvas.GetLeft(path) + offsetX);
                Canvas.SetTop(path, Canvas.GetTop(path) + offsetY);

                startPoint = currentPoint;
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
