using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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

        #region Win32 API
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // 将窗口设置为工具窗口
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            exStyle |= WS_EX_TOOLWINDOW;
            exStyle &= ~WS_EX_APPWINDOW;

            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            MouseEvent.SetHook(ActionMouseMove, ActionMouseLeftUp);

            new IconBox(this.MainPanel, this.Width / 2, this.Height / 2, ActionMouseLeftDown);
        }

        protected override void OnClosed(EventArgs e)
        {
            MouseEvent.Unhook();
            base.OnClosed(e);
        }

        private void ActionMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Path path)
            {
                return;
            }

            if (!Global.IconBoxes.TryGetValue(path.GetHashCode(), out IconBox? iconBox))
            {
                return;
            }

            this.CurrentPath = iconBox;
            this.CurrentLoc = e.GetPosition(this);
        }

        private void ActionMouseLeftUp(Point loc)
        {
            if (this.CurrentPath == null)
            {
                return;
            }

            this.CurrentPath.Updated(loc);
            this.CurrentPath = null;
        }

        private void ActionMouseMove(Point loc)
        {
            if (this.CurrentPath == null)
            {
                return;
            }

            this.CurrentPath.Update(loc, this.CurrentLoc);
            this.CurrentLoc = loc;
        }
    }
}
