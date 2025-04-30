using IWshRuntimeLibrary;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Diagnostics;

namespace WPFDeskManager
{
    internal class IconBoxHelper
    {
        /// <summary>
        /// 创建六边形的几何图形
        /// </summary>
        /// <returns>图形</returns>
        public static PathGeometry CreateHexagonGeo()
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                double x = Config.HexagonRadius * Math.Cos(angle);
                double y = Config.HexagonRadius * Math.Sin(angle);
                points.Add(new System.Windows.Point(x, y));
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

        /// <summary>
        /// 从“资源”中获取图标
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>图标</returns>
        public static BitmapImage GetSvgFromResources(string path)
        {
            BitmapImage bitmap = new BitmapImage();

            try
            {
                StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(path));

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                StreamSvgConverter converter = new StreamSvgConverter(new WpfDrawingSettings());
                converter.Convert(streamInfo.Stream, memStream);

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memStream;
                bitmap.EndInit();

                return bitmap;
            }
            catch
            {
                return bitmap;
            }
        }

        /// <summary>
        /// 从快捷方式和文件中获取图标
        /// </summary>
        /// <param name="path">目标文件</param>
        /// <param name="targetPath">目标实际应用的路径</param>
        /// <param name="iconImage">获取的图标</param>
        /// <returns>执行是否成功</returns>
        public static bool GetIconFromFiles(string path, out string? targetPath, out BitmapSource? iconImage)
        {
            targetPath = null;
            iconImage = null;

            try
            {
                Icon? icon = null;
                string ext = System.IO.Path.GetExtension(path).ToLower();

                if (System.IO.Directory.Exists(path))
                {
                    targetPath = path;
                    iconImage = GetSvgFromResources("pack://application:,,,/Assets/icon-文件夹.svg");
                }
                else if (System.IO.File.Exists(path))
                {
                    if (ext == ".lnk")
                    {
                        WshShell shell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);

                        if (!System.IO.File.Exists(path))
                        {
                            return false;
                        }

                        targetPath = shortcut.TargetPath;
                        icon = Icon.ExtractAssociatedIcon(shortcut.TargetPath);
                    }
                    else
                    {
                        targetPath = path;
                        icon = Icon.ExtractAssociatedIcon(path);
                    }

                    if (icon == null)
                    {
                        return false;
                    }

                    iconImage = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新图标位置
        /// </summary>
        /// <param name="iconBoxInfo">图标信息</param>
        /// <param name="locNow">新位置</param>
        /// <param name="locOld">旧位置</param>
        public static void ChangeIconBoxLoc(IconBoxInfo iconBoxInfo, System.Windows.Point locNow, System.Windows.Point locOld, bool updateCenter = false)
        {
            double offsetY = iconBoxInfo.CenterY - locOld.Y;
            double offsetX = iconBoxInfo.CenterX - locOld.X;

            double newPosY = locNow.Y + offsetY;
            double newPosX = locNow.X + offsetX;

            Canvas.SetTop(iconBoxInfo.Hexagon, newPosY);
            Canvas.SetLeft(iconBoxInfo.Hexagon, newPosX);
            if (iconBoxInfo.IconImage != null)
            {
                Canvas.SetTop(iconBoxInfo.IconImage, newPosY - iconBoxInfo.IconImage.Height / 2);
                Canvas.SetLeft(iconBoxInfo.IconImage, newPosX - iconBoxInfo.IconImage.Width / 2);
            }

            if (updateCenter)
            {
                iconBoxInfo.CenterY = newPosY;
                iconBoxInfo.CenterX = newPosX;
            }
        }

        /// <summary>
        /// 计算六边形的吸附点
        /// </summary>
        /// <param name="iconBoxInfo">图标信息</param>
        public static void CreateHexagonSnap(IconBoxInfo iconBoxInfo)
        {
            // 没有则添加，有则更新
            if (iconBoxInfo.SnapPoints.Count == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    double angle = Math.PI / 3 * i + Math.PI / 6;
                    double y = iconBoxInfo.CenterY + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Sin(angle);
                    double x = iconBoxInfo.CenterX + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Cos(angle);
                    iconBoxInfo.SnapPoints.Add(new SnapPoint { Point = new System.Windows.Point(x, y) });
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    double angle = Math.PI / 3 * i + Math.PI / 6;
                    double y = iconBoxInfo.CenterY + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Sin(angle);
                    double x = iconBoxInfo.CenterX + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Cos(angle);
                    iconBoxInfo.SnapPoints[i].Point = new System.Windows.Point(x, y);
                }
            }
        }

        /// <summary>
        /// 获取拖入文件应该存在的位置
        /// </summary>
        /// <param name="iconBoxInfo">图标信息</param>
        /// <returns>生成位置</returns>
        public static SnapPoint? GetDropLoc(IconBoxInfo iconBoxInfo)
        {
            if (iconBoxInfo == null)
            {
                return null;
            }

            SnapPoint? resSnap = null;
            List<SnapPoint> save = iconBoxInfo.SnapPoints;
            Func<List<SnapPoint>, bool> func = (List<SnapPoint> list) =>
            {
                save = new List<SnapPoint>();
                foreach (SnapPoint snap in list)
                {
                    if (snap.IsSnapped && snap.IconBoxInfo != null)
                    {
                        save.AddRange(snap.IconBoxInfo.SnapPoints);
                        continue;
                    }

                    resSnap = snap;
                    return true;
                }

                return false;
            };

            while (true)
            {
                if (func(save))
                {
                    break;
                }
            }

            return resSnap;
        }

        /// <summary>
        /// 更新与目标的吸附关系
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="self">自身</param>
        public static void UpdateIconSnapMap(IconBoxInfo target, IconBoxInfo self)
        {
            if (target == self)
            {
                return;
            }

            foreach (SnapPoint snap in target.SnapPoints)
            {
                if (snap.IsSnapped)
                {
                    continue;
                }

                double distance = Math.Sqrt(Math.Pow(self.CenterX - snap.Point.X, 2) +
                                            Math.Pow(self.CenterY - snap.Point.Y, 2));

                if (distance > (Config.HexagonRadius / 2))
                {
                    continue;
                }

                snap.IsSnapped = true;
                snap.IconBoxInfo = self;

                int index = target.SnapPoints.IndexOf(snap);
                if (index > 2)
                {
                    index -= 3;
                }
                else
                {
                    index += 3;
                }

                if (self.SnapPoints[index].IsSnapped)
                {
                    Debug.WriteLine("更新吸附点的时候有异常情况！");
                }
                self.SnapPoints[index].IsSnapped = true;
                self.SnapPoints[index].IconBoxInfo = target;

                break;
            }
        }

        /// <summary>
        /// 清理自身的吸附关系
        /// </summary>
        /// <param name="self">自身</param>
        public static void ClearIconSnapMap(IconBoxInfo self)
        {
            foreach (SnapPoint snap in self.SnapPoints)
            {
                if (snap.IsSnapped)
                {
                    if (snap.IconBoxInfo != null)
                    {
                        foreach (SnapPoint tar in snap.IconBoxInfo.SnapPoints)
                        {
                            if (tar.IconBoxInfo == self)
                            {
                                tar.IsSnapped = false;
                                tar.IconBoxInfo = null;
                                break;
                            }
                        }
                    }

                    snap.IsSnapped = false;
                    snap.IconBoxInfo = null;
                }
            }

            self.Parent?.Children.Remove(self);
            self.Parent = null;
        }

        /// <summary>
        /// 判断六边形是否有其它可吸附的六边形
        /// </summary>
        /// <param name="iconBox">图标</param>
        /// <param name="point">鼠标位置</param>
        /// <returns>是否存在可吸附的六边形</returns>
        public static bool SnapToIconBox(IconBoxInfo iconBoxInfo, out System.Windows.Point locNow, System.Windows.Point locOld)
        {
            double offsetY = iconBoxInfo.CenterY - locOld.Y;
            double offsetX = iconBoxInfo.CenterX - locOld.X;

            double newPosY = locNow.Y + offsetY;
            double newPosX = locNow.X + offsetX;

            foreach (var item in Global.IconBoxInfos)
            {
                if (item.Value == iconBoxInfo)
                {
                    continue;
                }

                double snapDistance = Math.Sqrt(Math.Pow(newPosY - item.Value.CenterY, 2) +
                                                Math.Pow(newPosX - item.Value.CenterX, 2));

                double maxDistance = Config.HexagonRadius * Math.Cos(Math.PI / 6) * 2 + Config.SnapDistance;

                if (snapDistance > maxDistance)
                {
                    continue;
                }

                // 找到了最近的图标
                int index = -1;
                double nearestDistance = double.MaxValue;
                foreach (SnapPoint snap in item.Value.SnapPoints)
                {
                    if (snap.IsSnapped)
                    {
                        continue;
                    }

                    double distance = Math.Sqrt(Math.Pow(newPosY - snap.Point.Y, 2) +
                                                Math.Pow(newPosX - snap.Point.X, 2));

                    if (distance < nearestDistance)
                    {
                        index = item.Value.SnapPoints.IndexOf(snap);
                        nearestDistance = distance;
                    }
                }

                if (index < 0)
                {
                    continue;
                }

                SnapPoint snapPoint = item.Value.SnapPoints[index];
                locNow = new System.Windows.Point(snapPoint.Point.X - offsetX, snapPoint.Point.Y - offsetY);

                if (item.Value.IsRoot)
                {
                    iconBoxInfo.Parent = item.Value;
                    item.Value.Children.Add(iconBoxInfo);
                }
                else
                {
                    iconBoxInfo.Parent = item.Value.Parent;
                    item.Value.Parent?.Children.Add(iconBoxInfo);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 边框颜色动画
        /// </summary>
        /// <param name="toColor">颜色</param>
        public static void AnimateStrokeColor(Path path, System.Windows.Media.Color toColor)
        {
            SolidColorBrush? strokeBrush = path.Stroke as SolidColorBrush;
            if (strokeBrush == null)
            {
                strokeBrush = new SolidColorBrush();
                path.Stroke = strokeBrush;
            }

            strokeBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                To = toColor,
                Duration = TimeSpan.FromMilliseconds(200)
            });
        }

        /// <summary>
        /// 阴影动画
        /// </summary>
        /// <param name="toColor">颜色</param>
        /// <param name="toOpacity">透明度</param>
        /// <param name="toShadowDepth">阴影深度</param>
        public static void AnimateShadowOpacity(Path path, System.Windows.Media.Color toColor, double toOpacity, int toShadowDepth)
        {
            if (path.Effect is not DropShadowEffect effect)
            {
                return;
            }

            effect.BeginAnimation(DropShadowEffect.ColorProperty, new ColorAnimation
            {
                To = toColor,
                Duration = TimeSpan.FromMilliseconds(200)
            });
            effect.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation
            {
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(200)
            });
            effect.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation
            {
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(200)
            });
        }
    }
}
