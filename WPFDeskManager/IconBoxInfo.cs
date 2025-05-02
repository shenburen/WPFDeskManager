using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WPFDeskManager
{
    internal class IconBoxInfo
    {
        /// <summary>
        /// 中心点X
        /// </summary>
        public double CenterX { get; set; }

        /// <summary>
        /// 中心点Y
        /// </summary>
        public double CenterY { get; set; }

        /// <summary>
        /// 图标类型
        /// 1: SVG 2: 来自文件
        /// </summary>
        public int IconType { get; set; }

        /// <summary>
        /// SVG图标名称
        /// </summary>
        public string? SvgName { get; set; }

        /// <summary>
        /// 图标名称
        /// </summary>
        public string? IconName { get; set; }

        /// <summary>
        /// 实际指向的路径
        /// </summary>
        public string? TargetPath { get; set; }

        /// <summary>
        /// 是否是根节点
        /// </summary>
        public bool IsRoot { get; set; } = false;

        /// <summary>
        /// 六边形
        /// </summary>
        public Path? Hexagon { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public Image? IconImage { get; set; }

        /// <summary>
        /// 吸附点
        /// </summary>
        public List<SnapPoint> SnapPoints { get; set; } = new();

        /// <summary>
        /// 实例对象
        /// </summary>
        public IconBox? Self { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public IconBoxInfo? Parent { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<IconBoxInfo> Children { get; set; } = new();
    }

    /// <summary>
    /// 吸附点对象
    /// </summary>
    internal class SnapPoint
    {
        /// <summary>
        /// 是否吸附
        /// </summary>
        public bool IsSnapped { get; set; }

        /// <summary>
        /// 吸附点
        /// </summary>
        public Point Point { get; set; } = new Point();

        /// <summary>
        /// 吸附对象
        /// </summary>
        public IconBoxInfo? IconBoxInfo { get; set; }
    }
}
