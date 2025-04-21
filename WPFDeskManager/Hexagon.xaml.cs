using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    /// <summary>
    /// Hexagon.xaml 的交互逻辑
    /// </summary>
    public partial class Hexagon : Window
    {
        public Hexagon()
        {
            InitializeComponent();
            CreateHexagon();
        }

        public Hexagon(BitmapSource image) : this()
        {
            this.Icon.Source = image;
        }

        private void CreateHexagon()
        {
            double width = this.Width;
            double height = this.Height;
            double centerX = width / 2;
            double centerY = height / 2;
            double radius = Math.Min(width, height) / 2 - 10;

            PointCollection points = new PointCollection();

            // 计算正六边形的六个顶点
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);
                points.Add(new Point(x, y));
            }

            // 绘制正六边形
            PathFigure figure = new PathFigure { StartPoint = points[0], IsClosed = true };
            for (int i = 1; i < points.Count; i++)
            {
                figure.Segments.Add(new LineSegment(points[i], true));
            }

            PathGeometry geo = new PathGeometry();
            geo.Figures.Add(figure);

            this.HexagonPath.Data = geo;
        }

        private void Hexagon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null)
            {
                return;
            }

            foreach (string file in files)
            {
                IconInfo iconInfo = Common.GetIcon(file);
                if (iconInfo != null)
                {
                    Common.CreateChild(iconInfo);
                }
            }
        }
    }
}
