using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;

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
        private Point CurrentLoc { get; set; } = new Point();

        /// <summary>
        /// 当前选择的图标
        /// </summary>
        private IconBox? CurrentPath { get; set; }

        #region Win32 API
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口初始化完成后
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // 将窗口设置为工具窗口
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW;
            exStyle &= ~WS_EX_APPWINDOW;
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);

            // 将窗口设置为最顶层
            SetWindowPos(hwnd, new IntPtr(-1), 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// 窗口内容渲染完成后
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            MouseEvent.SetHook(ActionMouseMove, ActionMouseLeftUp);

            Serialization? serialization = this.LoadIconInfo();
            if (serialization == null)
            {
                IconBoxInfo iconBoxInfo = new IconBoxInfo
                {
                    CenterY = this.Height / 2,
                    CenterX = this.Width / 2,
                    IconType = 1,
                    SvgName = "pack://application:,,,/Assets/icon-金牛座.svg",
                    IsRoot = true,
                };
                IconBox.CreateIconBox(this, this.ActionMouseLeftDown, iconBoxInfo);
            }
        }

        /// <summary>
        /// 窗口关闭时
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            this.SaveIconInfo();
            MouseEvent.Unhook();
            base.OnClosed(e);
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="loc">鼠标位置</param>
        private void ActionMouseMove(Point loc)
        {
            if (this.CurrentPath == null)
            {
                return;
            }

            this.CurrentPath.Update(loc, this.CurrentLoc);
        }

        /// <summary>
        /// 鼠标左键抬起事件
        /// </summary>
        /// <param name="loc">鼠标位置</param>
        private void ActionMouseLeftUp(Point loc)
        {
            if (this.CurrentPath == null)
            {
                return;
            }

            this.CurrentPath.Updated(loc, this.CurrentLoc);
            this.CurrentPath = null;
        }

        /// <summary>
        /// 鼠标左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActionMouseLeftDown(IconBox iconBox, Point point)
        {
            this.CurrentPath = iconBox;
            this.CurrentLoc = point;
        }

        private void SaveIconInfo()
        {
            Dictionary<int, IconBoxInfo> infos = new Dictionary<int, IconBoxInfo>(Global.IconBoxInfos);
            Serialization serialization = new Serialization();

            Dictionary<int, IconBoxInfo> list = infos.Where(info => info.Value.IsRoot).ToDictionary(info => info.Key, info => info.Value);
            foreach (var item in list)
            {
                IconSerialization icon = new IconSerialization
                {
                    CenterX = item.Value.CenterX,
                    CenterY = item.Value.CenterY,
                    IconType = item.Value.IconType,
                    SvgName = item.Value.SvgName,
                    TargetPath = item.Value.TargetPath,
                    IsRoot = item.Value.IsRoot,
                    Image = item.Value.IconImage?.Source,
                };
                serialization.Icons.Add(icon);
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            string json = JsonSerializer.Serialize(serialization, options);

            SerializerHelper.SaveIconToFile(json);
        }

        private Serialization LoadIconInfo()
        {
            string? json = SerializerHelper.LoadIconFromFile();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            return JsonSerializer.Deserialize<Serialization>(json, options);
        }
    }
}
