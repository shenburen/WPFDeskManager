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
        /// <summary>
        /// 当前鼠标位置
        /// </summary>
        private Point CurrentLoc = new Point();

        /// <summary>
        /// 当前选择的图标
        /// </summary>
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
    }
}
