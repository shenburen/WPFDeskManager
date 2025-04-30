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
        private Action<IconBox, Point> MouseLeftButtonDown;

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
        public static void CreateIconBox(MainWindow window, Action<IconBox, Point> mouseLeftButtonDown, IconBoxInfo iconBoxInfo)
        {
            IconBox iconBox = new IconBox(window, mouseLeftButtonDown, iconBoxInfo);
            iconBoxInfo.Self = iconBox;
            if (iconBox.IconBoxInfo.Hexagon != null)
            {
                Global.IconBoxes.Add(iconBox.IconBoxInfo.Hexagon.GetHashCode(), iconBox);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="canvas">主容器</param>
        /// <param name="mouseLeftButtonDown">鼠标事件</param>
        /// <param name="iconBoxInfo">图标配置</param>
        public IconBox(MainWindow window, Action<IconBox, Point> mouseLeftButtonDown, IconBoxInfo iconBoxInfo)
        {
            this.MainWindow = window;
            this.MouseLeftButtonDown = mouseLeftButtonDown;
            this.IconBoxInfo = iconBoxInfo;

            // 六边形
            this.IconBoxInfo.Hexagon = new Path
            {
                Data = IconBoxHelper.CreateHexagonGeo(),
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
            Canvas.SetTop(this.IconBoxInfo.Hexagon, this.IconBoxInfo.CenterY);
            Canvas.SetLeft(this.IconBoxInfo.Hexagon, this.IconBoxInfo.CenterX);
            this.IconBoxInfo.Hexagon.Drop += Path_Drop;
            this.IconBoxInfo.Hexagon.MouseEnter += Path_MouseEnter;
            this.IconBoxInfo.Hexagon.MouseLeave += Path_MouseLeave;
            this.IconBoxInfo.Hexagon.MouseLeftButtonUp += Path_MouseLeftButtonUp;
            this.IconBoxInfo.Hexagon.MouseLeftButtonDown += Path_MouseLeftButtonDown;
            this.MainWindow.MainPanel.Children.Add(this.IconBoxInfo.Hexagon);

            // 图标
            this.IconBoxInfo.IconImage = new Image
            {
                Width = Config.IconSize,
                Height = Config.IconSize,
                IsHitTestVisible = false,
            };
            if (this.IconBoxInfo.IconType == 1 && this.IconBoxInfo.SvgName != null) // SVG图标
            {
                this.IconBoxInfo.IconImage.Source = IconBoxHelper.GetSvgFromResources(this.IconBoxInfo.SvgName);
            }
            else if (this.IconBoxInfo.IconType == 2 && this.IconBoxInfo.TargetPath != null) // 文件图标
            {
                IconBoxHelper.GetIconFromFiles(this.IconBoxInfo.TargetPath, out string? targetPath, out BitmapSource? image);
                this.IconBoxInfo.TargetPath = targetPath;
                this.IconBoxInfo.IconImage.Source = image;
            }
            Canvas.SetTop(this.IconBoxInfo.IconImage, this.IconBoxInfo.CenterY - this.IconBoxInfo.IconImage.Height / 2);
            Canvas.SetLeft(this.IconBoxInfo.IconImage, this.IconBoxInfo.CenterX - this.IconBoxInfo.IconImage.Width / 2);
            this.MainWindow.MainPanel.Children.Add(this.IconBoxInfo.IconImage);

            IconBoxHelper.CreateHexagonSnap(this.IconBoxInfo);
            this.CreateContextMenu();
        }

        /// <summary>
        /// 更新六边形位置
        /// </summary>
        /// <param name="locNow">鼠标当前的位置</param>
        /// <param name="locOld">鼠标之前的位置</param>
        public void Update(Point locNow, Point locOld)
        {
            IconBoxHelper.ChangeIconBoxLoc(this.IconBoxInfo, locNow, locOld);

            foreach (IconBoxInfo child in this.IconBoxInfo.Children)
            {
                if (child.Self == null)
                {
                    continue;
                }
                IconBoxHelper.ChangeIconBoxLoc(child.Self.IconBoxInfo, locNow, locOld);
            }
        }

        /// <summary>
        /// 结束更新时更新一下自身信息
        /// </summary>
        /// <param name="locNow">鼠标当前的位置</param>
        /// <param name="locOld">鼠标之前的位置</param>
        public void Updated(Point locNow, Point locOld)
        {
            if (!this.IconBoxInfo.IsRoot && this.IconBoxInfo.Parent == null && IconBoxHelper.SnapToIconBox(this, out locNow))
            {
                this.IconBoxInfo.CenterY = locNow.Y;
                this.IconBoxInfo.CenterX = locNow.X;
            }

            IconBoxHelper.ChangeIconBoxLoc(this.IconBoxInfo, locNow, locOld);
            IconBoxHelper.CreateHexagonSnap(this.IconBoxInfo);

            foreach (IconBoxInfo child in this.IconBoxInfo.Children)
            {
                if (child.Self == null)
                {
                    continue;
                }
                IconBoxHelper.ChangeIconBoxLoc(child.Self.IconBoxInfo, locNow, locOld);
                IconBoxHelper.CreateHexagonSnap(child.Self.IconBoxInfo);
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
                if (this.IconBoxInfo.Hexagon != null)
                {
                    Global.IconBoxes.Remove(this.IconBoxInfo.Hexagon.GetHashCode());
                }
                this.MainWindow.MainPanel.Children.Remove(this.IconBoxInfo.Hexagon);
                this.MainWindow.MainPanel.Children.Remove(this.IconBoxInfo.IconImage);
            });

            if (this.IconBoxInfo.Hexagon != null)
            {
                this.IconBoxInfo.Hexagon.ContextMenu = this.Popup.Menu;
            }
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
            if (this.IconBoxInfo.IconImage != null)
            {
                this.IconBoxInfo.IconImage.Source = IconBoxHelper.GetSvgFromResources("pack://application:,,,/Assets/icon-" + menuItem?.Header + ".svg");
            }
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
                SnapPoint? snap = IconBoxHelper.GetDropLoc(this.IconBoxInfo);
                if (snap == null)
                {
                    continue;
                }

                IconBoxInfo iconBoxInfo = new IconBoxInfo
                {
                    CenterY = snap.Point.Y,
                    CenterX = snap.Point.X,
                    IconType = 2,
                    TargetPath = file,
                    Parent = this.IconBoxInfo,
                };
                CreateIconBox(this.MainWindow, this.MouseLeftButtonDown, iconBoxInfo);

                if (this.IconBoxInfo.IsRoot)
                {
                    this.IconBoxInfo.Children.Add(iconBoxInfo);
                    IconBoxHelper.UpdateIconSnapMap(this.IconBoxInfo, iconBoxInfo);
                }
                else
                {
                    this.IconBoxInfo.Parent?.Children.Add(iconBoxInfo);
                }

                foreach (IconBoxInfo target in iconBoxInfo.Parent.Children)
                {
                    IconBoxHelper.UpdateIconSnapMap(target, iconBoxInfo);
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
            if (this.IconBoxInfo.Hexagon == null)
            {
                return;
            }
            IconBoxHelper.AnimateStrokeColor(this.IconBoxInfo.Hexagon, Colors.Cyan);
            IconBoxHelper.AnimateShadowOpacity(this.IconBoxInfo.Hexagon, Colors.Cyan, 0.7, 15);
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IconBoxInfo.Hexagon == null)
            {
                return;
            }
            IconBoxHelper.AnimateStrokeColor(this.IconBoxInfo.Hexagon, Color.FromRgb(68, 68, 68));
            IconBoxHelper.AnimateShadowOpacity(this.IconBoxInfo.Hexagon, Color.FromRgb(0, 150, 200), 0.4, 8);
        }

        /// <summary>
        /// 鼠标左键抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DateTime now = DateTime.Now;
            if ((now - this.LastClickTime).TotalMilliseconds <= Config.DoubleClickTime && System.IO.File.Exists(this.IconBoxInfo.TargetPath))
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = this.IconBoxInfo.TargetPath,
                    UseShellExecute = true,
                };
                try
                {
                    Process.Start(psi);
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
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
            // 鼠标压下一个图标的时候，需要把图标和其父节点的吸附关系清空
            if (!this.IconBoxInfo.IsRoot && this.IconBoxInfo.Parent != null)
            {
                IconBoxHelper.ClearIconSnapMap(this.IconBoxInfo);
            }

            Point point = e.GetPosition(this.MainWindow);
            this.MouseLeftButtonDown.Invoke(this, point);
        }
    }
}
