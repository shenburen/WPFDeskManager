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

namespace WPFDeskManager
{
    internal class Common
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
        /// <param name="iconBox">图标</param>
        /// <param name="locNow">新位置</param>
        /// <param name="locOld">旧位置</param>
        public static void ChangeIconBoxLoc(IconBox iconBox, System.Windows.Point locNow, System.Windows.Point locOld)
        {
            double offsetY = iconBox.IconBoxInfo.CenterY - locOld.Y;
            double offsetX = iconBox.IconBoxInfo.CenterX - locOld.X;

            iconBox.IconBoxInfo.CenterY = locNow.Y + offsetY;
            iconBox.IconBoxInfo.CenterX = locNow.X + offsetX;

            Canvas.SetTop(iconBox.IconBoxInfo.Hexagon, iconBox.IconBoxInfo.CenterY);
            Canvas.SetLeft(iconBox.IconBoxInfo.Hexagon, iconBox.IconBoxInfo.CenterX);
            if (iconBox.IconBoxInfo.IconImage != null)
            {
                Canvas.SetTop(iconBox.IconBoxInfo.IconImage, iconBox.IconBoxInfo.CenterY - iconBox.IconBoxInfo.IconImage.Height / 2);
                Canvas.SetLeft(iconBox.IconBoxInfo.IconImage, iconBox.IconBoxInfo.CenterX - iconBox.IconBoxInfo.IconImage.Width / 2);
            }
        }

        /// <summary>
        /// 计算六边形的吸附点
        /// </summary>
        /// <param name="iconBox">图标</param>
        public static void CreateHexagonSnap(IconBox iconBox)
        {
            if (iconBox.IconBoxInfo.SnapPoints.Count == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    double angle = Math.PI / 3 * i + Math.PI / 6;
                    double y = iconBox.IconBoxInfo.CenterY + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Sin(angle);
                    double x = iconBox.IconBoxInfo.CenterX + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Cos(angle);
                    iconBox.IconBoxInfo.SnapPoints.Add(new SnapPoint { Point = new System.Windows.Point(x, y) });
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    double angle = Math.PI / 3 * i + Math.PI / 6;
                    double y = iconBox.IconBoxInfo.CenterY + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Sin(angle);
                    double x = iconBox.IconBoxInfo.CenterX + (Config.HexagonRadius + 1) * Math.Cos(Math.PI / 6) * 2 * Math.Cos(angle);
                    iconBox.IconBoxInfo.SnapPoints[i].Point = new System.Windows.Point(x, y);
                }
            }

            // 更新自身关于父节点的吸附关系
            if (!iconBox.IconBoxInfo.IsRoot && iconBox.IconBoxInfo?.Parent?.Self != null)
            {
                foreach (SnapPoint snap in iconBox.IconBoxInfo.Parent.SnapPoints)
                {
                    if (snap.IconBoxInfo == iconBox.IconBoxInfo)
                    {
                        int index = iconBox.IconBoxInfo.Parent.SnapPoints.IndexOf(snap);
                        if (index > 2)
                        {
                            index -= 3;
                        }
                        else
                        {
                            index += 3;
                        }

                        iconBox.IconBoxInfo.SnapPoints[index].IconBoxInfo = snap.IconBoxInfo;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 判断六边形是否有其它可吸附的六边形
        /// </summary>
        /// <param name="iconBox">图标</param>
        /// <param name="point">鼠标位置</param>
        /// <returns>是否存在可吸附的六边形</returns>
        public static bool SnapToIconBox(IconBox iconBox, out System.Windows.Point point)
        {
            foreach (var item in Global.IconBoxes)
            {
                if (item.Value == iconBox)
                {
                    continue;
                }
                if (iconBox.IconBoxInfo.Children.Contains(item.Value.IconBoxInfo))
                {
                    continue;
                }

                double snapDistance = Math.Sqrt(Math.Pow(iconBox.IconBoxInfo.CenterX - item.Value.IconBoxInfo.CenterX, 2) +
                                                Math.Pow(iconBox.IconBoxInfo.CenterY - item.Value.IconBoxInfo.CenterY, 2));

                double minDistance = Config.HexagonRadius * Math.Cos(Math.PI / 6) * 2 - Config.SnapDistance;
                double maxDistance = Config.HexagonRadius * Math.Cos(Math.PI / 6) * 2 + Config.SnapDistance;

                if (snapDistance < minDistance || snapDistance > maxDistance)
                {
                    continue;
                }

                bool isFinished = false;
                double nearestDistance = double.MaxValue;
                foreach (SnapPoint snap in item.Value.IconBoxInfo.SnapPoints)
                {
                    if (snap.IsSnapped)
                    {
                        continue;
                    }

                    double distance = Math.Sqrt(Math.Pow(iconBox.IconBoxInfo.CenterX - snap.Point.X, 2) +
                                                Math.Pow(iconBox.IconBoxInfo.CenterY - snap.Point.Y, 2));

                    if (distance < nearestDistance)
                    {
                        point = snap.Point;
                        isFinished = true;
                        nearestDistance = distance;
                    }
                }

                if (!isFinished)
                {
                    continue;
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
