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
        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public int IconType { get; set; }

        public string? SvgName { get; set; }

        public string? IconName { get; set; }

        public string? TargetPath { get; set; }

        public bool IsRoot { get; set; }

        public List<IconSerialization> Children { get; set; } = new();
    }
}
