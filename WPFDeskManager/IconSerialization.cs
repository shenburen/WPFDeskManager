using System.IO;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    internal class IconSerialization
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public int IconType { get; set; }
        public string? SvgName { get; set; }
        public string? TargetPath { get; set; }
        public string? ImageBase64 { get; set; }

        [JsonIgnore]
        public ImageSource? image
        {
            get
            {
                if (string.IsNullOrEmpty(ImageBase64)) return null;
                byte[] bytes = Convert.FromBase64String(ImageBase64);
                using var ms = new MemoryStream(bytes);
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                img.Freeze();
                return img;
            }
            set
            {
                if (value is BitmapSource bmp)
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmp));
                    using var ms = new MemoryStream();
                    encoder.Save(ms);
                    ImageBase64 = Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public List<SnapSerialization> SnapPoints { get; set; } = new();
        public List<IconSerialization> Children { get; set; } = new();
    }

    internal class SnapSerialization
    {
        public bool IsSnapped { get; set; }

        public Point Point { get; set; } = new Point();

        [JsonIgnore]
        public IconSerialization? IconBoxInfo { get; set; }
    }
}
