using System.Windows.Media.Imaging;

namespace WPFDeskManager
{
    public class HexagonInfo
    {
        public string? Name { get; set; }

        public BitmapSource? Icon { get; set; }

        public string? TargetPath { get; set; }

        public bool IsRoot { get; set; } = false;
    }
}
