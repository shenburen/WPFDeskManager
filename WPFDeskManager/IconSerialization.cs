using System.IO;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    internal class IconSerialization
    {
        public double CenterX;
        public double CenterY;
        public int IconType;
        public string? SvgName;
        public string? TargetPath;
        public string? ImageBase64;

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

        public List<SnapSerialization> SnapPoints = new List<SnapSerialization>();
        public List<IconSerialization> Children = new List<IconSerialization>();
    }

    internal class SnapSerialization
    {
        public bool IsSnapped { get; set; }

        public Point Point { get; set; } = new Point();

        [JsonIgnore]
        public IconSerialization? IconBoxInfo;
    }
}
