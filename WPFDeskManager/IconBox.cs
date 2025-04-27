using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WPFDeskManager
{
    internal class IconBox
    {
        public string? TargetPath { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public required Image IconImage { get; set; }

        public required Path HexagonPath { get; set; }

        public List<SnapPoint> SnapPoints { get; set; } = new List<SnapPoint>();
    }

    internal class SnapPoint
    {
        public bool IsSnapped { get; set; }

        public Point Point { get; set; } = new Point();
    }
}
