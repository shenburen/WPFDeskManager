using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;

namespace WPFDeskManager
{
    internal class IconBox
    {
        /// <summary>
        /// 六边形
        /// </summary>
        public Path Hexagon;

        /// <summary>
        /// 图标
        /// </summary>
        public Image IconImage;

        /// <summary>
        /// 吸附点
        /// </summary>
        public List<SnapPoint> SnapPoints = new List<SnapPoint>();

        /// <summary>
        /// 上次点击时间
        /// </summary>
        private DateTime LastClickTime;

        /// <summary>
        /// 菜单
        /// </summary>
        private PopupMenu Popup = new PopupMenu();

        /// <summary>
        /// 主窗体
        /// </summary>
        private MainWindow MainWindow;

        /// <summary>
        /// 鼠标压下事件
        /// </summary>
        private Action<object, MouseButtonEventArgs> MouseLeftButtonDown;

        /// <summary>
        /// 图标配置
        /// </summary>
        public IconBoxInfo IconBoxInfo;

        /// <summary>
        /// 创建图标框
        /// </summary>
        /// <param name="canvas">主容器</param>
        /// <param name="mouseLeftButtonDown">鼠标事件</param>
        /// <param name="iconBoxInfo">图标配置</param>
        public static void CreateIconBox(MainWindow window, Action<object, MouseButtonEventArgs> mouseLeftButtonDown, IconBoxInfo iconBoxInfo)
        {
            IconBox iconBox = new IconBox(window, mouseLeftButtonDown, iconBoxInfo);
            iconBoxInfo.Self = iconBox;
            Global.IconBoxes.Add(iconBox.Hexagon.GetHashCode(), iconBox);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="canvas">主容器</param>
        /// <param name="mouseLeftButtonDown">鼠标事件</param>
        /// <param name="iconBoxInfo">图标配置</param>
        public IconBox(MainWindow window, Action<object, MouseButtonEventArgs> mouseLeftButtonDown, IconBoxInfo iconBoxInfo)
        {
            this.MainWindow = window;
            this.MouseLeftButtonDown = mouseLeftButtonDown;
            this.IconBoxInfo = iconBoxInfo;

            // 六边形
            this.Hexagon = new Path
            {
                Data = Common.CreateHexagonGeo(),
                Fill = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(0, 150, 200),
                    Opacity = 0.4,
                    BlurRadius = 8,
                    ShadowDepth = 0,
                },
                Stroke = new SolidColorBrush(Color.FromRgb(68, 68, 68)),
                StrokeThickness = 2,
                AllowDrop = true,
            };
            Canvas.SetTop(this.Hexagon, this.IconBoxInfo.CenterY);
            Canvas.SetLeft(this.Hexagon, this.IconBoxInfo.CenterX);
            this.Hexagon.Drop += Path_Drop;
            this.Hexagon.MouseEnter += Path_MouseEnter;
            this.Hexagon.MouseLeave += Path_MouseLeave;
            this.Hexagon.MouseLeftButtonUp += Path_MouseLeftButtonUp;
            this.Hexagon.MouseLeftButtonDown += Path_MouseLeftButtonDown;
            this.MainWindow.MainPanel.Children.Add(this.Hexagon);

            // 图标
            this.IconImage = new Image
            {
                Width = Config.IconSize,
                Height = Config.IconSize,
                IsHitTestVisible = false,
            };
            if (this.IconBoxInfo.IconType == 1 && this.IconBoxInfo.SvgName != null) // SVG图标
            {
                this.IconImage.Source = Common.GetSvgFromResources(this.IconBoxInfo.SvgName);
            }
            else if (this.IconBoxInfo.IconType == 2 && this.IconBoxInfo.TargetPath != null) // 文件图标
            {
                Common.GetIconFromFiles(this.IconBoxInfo.TargetPath, out string? targetPath, out BitmapSource? image);
                this.IconBoxInfo.TargetPath = targetPath;
                this.IconImage.Source = image;
            }
            Canvas.SetTop(this.IconImage, this.IconBoxInfo.CenterY - this.IconImage.Height / 2);
            Canvas.SetLeft(this.IconImage, this.IconBoxInfo.CenterX - this.IconImage.Width / 2);
            this.MainWindow.MainPanel.Children.Add(this.IconImage);

            Common.CreateHexagonSnap(this);
            this.CreateContextMenu();
        }

        /// <summary>
        /// 更新六边形位置
        /// </summary>
        /// <param name="locNow">鼠标当前的位置</param>
        /// <param name="locOld">鼠标之前的位置</param>
        public void Update(Point locNow, Point locOld)
        {
            Common.ChangeIconBoxLoc(this, locNow, locOld);

            foreach (IconBoxInfo child in this.IconBoxInfo.Children)
            {
                if (child.Self == null)
                {
                    continue;
                }
                Common.ChangeIconBoxLoc(child.Self, locNow, locOld);
            }
        }

        /// <summary>
        /// 结束更新时更新一下自身信息
        /// </summary>
        /// <param name="locNow">鼠标当前的位置</param>
        /// <param name="locOld">鼠标之前的位置</param>
        public void Updated(Point locNow, Point locOld)
        {
            if (!this.IconBoxInfo.IsRoot && this.IconBoxInfo.Parent == null && Common.SnapToIconBox(this, out locNow))
            {
                this.IconBoxInfo.CenterY = locNow.Y;
                this.IconBoxInfo.CenterX = locNow.X;
            }

            Common.ChangeIconBoxLoc(this, locNow, locOld);
            Common.CreateHexagonSnap(this);

            foreach (IconBoxInfo child in this.IconBoxInfo.Children)
            {
                if (child.Self == null)
                {
                    continue;
                }
                Common.ChangeIconBoxLoc(child.Self, locNow, locOld);
                Common.CreateHexagonSnap(child.Self);
            }
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        private void CreateContextMenu()
        {
            this.Popup = new PopupMenu();

            if (this.IconBoxInfo.IsRoot)
            {
                MenuItem switchIcon = this.Popup.AddMenuItem("切换图标");
                this.Popup.AddMenuItem(switchIcon, "牡羊座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "金牛座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "双子座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "巨蟹座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "狮子座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "处女座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "天秤座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "天蝎座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "射手座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "摩羯座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "水瓶座", this.SwitchHexagonIcon);
                this.Popup.AddMenuItem(switchIcon, "双鱼座", this.SwitchHexagonIcon);

                this.Popup.AddMenuItem("添加", (object sender, RoutedEventArgs e) =>
                {
                    IconBoxInfo iconBoxInfo = new IconBoxInfo
                    {
                        CenterX = this.MainWindow.Width / 2,
                        CenterY = this.MainWindow.Height / 2,
                        IconType = 1,
                        SvgName = "pack://application:,,,/Assets/icon-金牛座.svg",
                        IsRoot = true,
                    };
                    CreateIconBox(this.MainWindow, this.MouseLeftButtonDown, iconBoxInfo);
                });
            }
            this.Popup.AddMenuItem("删除", (object sender, RoutedEventArgs e) =>
            {
                Global.IconBoxes.Remove(this.Hexagon.GetHashCode());
                this.MainWindow.MainPanel.Children.Remove(this.Hexagon);
                this.MainWindow.MainPanel.Children.Remove(this.IconImage);
            });

            this.Hexagon.ContextMenu = this.Popup.Menu;
        }

        /// <summary>
        /// 切换图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SwitchHexagonIcon(object sender, RoutedEventArgs e)
        {
            MenuItem? menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                return;
            }

            this.IconBoxInfo.SvgName = "pack://application:,,,/Assets/icon-" + menuItem?.Header + ".svg";
            this.IconImage.Source = Common.GetSvgFromResources("pack://application:,,,/Assets/icon-" + menuItem?.Header + ".svg");
        }

        /// <summary>
        /// 文件拖入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null)
            {
                return;
            }

            foreach (string file in files)
            {
                foreach (SnapPoint snap in this.SnapPoints)
                {
                    if (!snap.IsSnapped)
                    {
                        snap.IsSnapped = true;

                        IconBoxInfo iconBoxInfo = new IconBoxInfo
                        {
                            CenterY = snap.Point.Y,
                            CenterX = snap.Point.X,
                            IconType = 2,
                            TargetPath = file,
                            Parent = this.IconBoxInfo,
                        };
                        this.IconBoxInfo.Children.Add(iconBoxInfo);

                        CreateIconBox(this.MainWindow, this.MouseLeftButtonDown, iconBoxInfo);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            Common.AnimateStrokeColor(this.Hexagon, Colors.Cyan);
            Common.AnimateShadowOpacity(this.Hexagon, Colors.Cyan, 0.7, 15);
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            Common.AnimateStrokeColor(this.Hexagon, Color.FromRgb(68, 68, 68));
            Common.AnimateShadowOpacity(this.Hexagon, Color.FromRgb(0, 150, 200), 0.4, 8);
        }

        /// <summary>
        /// 鼠标左键抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Path_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DateTime now = DateTime.Now;
            if ((now - this.LastClickTime).TotalMilliseconds <= 300 && System.IO.File.Exists(this.IconBoxInfo.TargetPath))
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = this.IconBoxInfo.TargetPath,
                    UseShellExecute = true,
                };
                Process.Start(psi);
            }
            this.LastClickTime = now;
        }

        /// <summary>
        /// 鼠标左键压下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // TODO: 这里处理一些东西
            this.MouseLeftButtonDown.Invoke(sender, e);
        }
    }
}
