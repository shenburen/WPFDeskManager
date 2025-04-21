using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    public class IconInfo
    {
        public required string Name { get; set; }

        public required string TargetPath { get; set; }

        public required BitmapSource Icon { get; set; }
    }
}
