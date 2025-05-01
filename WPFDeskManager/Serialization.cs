using System.IO;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    internal class Serialization
    {
        public DateTime LastSaveTime { get; set; } = DateTime.Now;

        public List<IconSerialization> Icons { get; set; } = new();
    }

    internal class IconSerialization
    {
        public int id { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public int IconType { get; set; }

        public string? SvgName { get; set; }

        public string? TargetPath { get; set; }

        public bool IsRoot { get; set; }

        [JsonIgnore]
        public ImageSource? Image
        {
            get
            {
                if (string.IsNullOrEmpty(ImageBase64))
                {
                    return null;
                }

                using MemoryStream memStream = new MemoryStream(Convert.FromBase64String(this.ImageBase64));
                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memStream;
                bitmap.EndInit();

                return bitmap;
            }
            set
            {
                if (value is BitmapSource bmp)
                {
                    using MemoryStream memStream = new MemoryStream();

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmp));
                    encoder.Save(memStream);

                    ImageBase64 = Convert.ToBase64String(memStream.ToArray());
                }
            }
        }

        public string? ImageBase64 { get; set; }

        public List<SnapSerialization> SnapPoints { get; set; } = new();

        public List<IconSerialization> Children { get; set; } = new();
    }

    internal class SnapSerialization
    {
        public bool IsSnapped { get; set; }

        public Point Point { get; set; } = new Point();

        public int id { get; set; }
    }
}
