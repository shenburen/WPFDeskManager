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
