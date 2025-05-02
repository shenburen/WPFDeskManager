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
            this.LoadIconInfo();
            MouseEvent.SetHook(ActionMouseMove, ActionMouseLeftUp); ;

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

        /// <summary>
        /// 保存图标信息
        /// </summary>
        private void SaveIconInfo()
        {
            Func<IconBoxInfo, IconSerialization> save = (item) =>
            {
                return new IconSerialization
                {
                    CenterX = item.CenterX,
                    CenterY = item.CenterY,
                    IconType = item.IconType,
                    SvgName = item.SvgName,
                    TargetPath = item.TargetPath,
                    IsRoot = item.IsRoot,
                    IconName = item.IconName,
                };
            };

            Dictionary<int, IconBoxInfo> list = Global.IconBoxInfos;
            Dictionary<int, IconBoxInfo> roots = list.Where(info => info.Value.IsRoot).ToDictionary(info => info.Key, info => info.Value);

            Serialization serialization = new Serialization();
            foreach (var root in roots)
            {
                IconSerialization p = save(root.Value);
                serialization.Icons.Add(p);
                if (root.Value.Hexagon != null)
                {
                    list.Remove(root.Value.Hexagon.GetHashCode());
                }

                foreach (IconBoxInfo child in root.Value.Children)
                {
                    IconSerialization s = save(child);
                    p.Children.Add(s);
                    if (child.Hexagon != null)
                    {
                        list.Remove(child.Hexagon.GetHashCode());
                    }
                }
            }

            foreach (var single in list)
            {
                IconSerialization p = save(single.Value);
                serialization.Icons.Add(p);
                if (single.Value.Hexagon != null)
                {
                    list.Remove(single.Value.Hexagon.GetHashCode());
                }
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            string json = JsonSerializer.Serialize(serialization, options);

            SerializerHelper.SaveIconToFile(json);
        }

        /// <summary>
        /// 加载图标信息
        /// </summary>
        private void LoadIconInfo()
        {
            Action createDefault = () =>
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
            };

            string? json = SerializerHelper.LoadIconFromFile();

            if (json == null)
            {
                createDefault();
                return;
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };
            Serialization? serialization = JsonSerializer.Deserialize<Serialization>(json, options);

            if (serialization == null)
            {
                createDefault();
                return;
            }

            foreach (IconSerialization root in serialization.Icons)
            {
                IconBoxInfo rootInfo = new IconBoxInfo
                {
                    CenterX = root.CenterX,
                    CenterY = root.CenterY,
                    IconType = root.IconType,
                    TargetPath = root.TargetPath,
                    IconName = root.IconName,
                    SvgName = root.SvgName,
                    IsRoot = root.IsRoot,
                };
                IconBox.CreateIconBox(this, this.ActionMouseLeftDown, rootInfo);

                foreach (IconSerialization child in root.Children)
                {
                    IconBoxInfo childInfo = new IconBoxInfo
                    {
                        CenterX = child.CenterX,
                        CenterY = child.CenterY,
                        IconType = child.IconType,
                        TargetPath = child.TargetPath,
                        IconName = child.IconName,
                        IsRoot = child.IsRoot,
                    };
                    IconBox.CreateIconBox(this, this.ActionMouseLeftDown, childInfo);

                    childInfo.Parent = rootInfo;
                    rootInfo.Children.Add(childInfo);

                    IconBoxHelper.UpdateIconSnapMap(rootInfo, childInfo);
                    foreach (IconBoxInfo target in rootInfo.Children)
                    {
                        IconBoxHelper.UpdateIconSnapMap(target, childInfo);
                    }
                }
            }
        }
    }
}
