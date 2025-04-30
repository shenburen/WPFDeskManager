using System.Windows.Controls;
using System.Windows.Shapes;

namespace WPFDeskManager
{
    internal class IconBoxInfo
    {
        /// <summary>
        /// 中心点X
        /// </summary>
        public double CenterX;

        /// <summary>
        /// 中心点Y
        /// </summary>
        public double CenterY;

        /// <summary>
        /// 图标类型
        /// 1: SVG 2: 来自文件
        /// </summary>
        public int IconType;

        /// <summary>
        /// SVG图标名称
        /// </summary>
        public string? SvgName;

        /// <summary>
        /// 实际指向的路径
        /// </summary>
        public string? TargetPath;

        /// <summary>
        /// 是否是根节点
        /// </summary>
        public bool IsRoot = false;

        /// <summary>
        /// 六边形
        /// </summary>
        public Path? Hexagon;

        /// <summary>
        /// 图标
        /// </summary>
        public Image? IconImage;

        /// <summary>
        /// 吸附点
        /// </summary>
        public List<SnapPoint> SnapPoints = new List<SnapPoint>();

        /// <summary>
        /// 实例对象
        /// </summary>
        public IconBox? Self;

        /// <summary>
        /// 父节点
        /// </summary>
        public IconBoxInfo? Parent;

        /// <summary>
        /// 兄弟节点
        /// </summary>
        public List<IconBoxInfo> Brothers = new List<IconBoxInfo>();

        /// <summary>
        /// 子节点
        /// </summary>
        public List<IconBoxInfo> Children = new List<IconBoxInfo>();
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
        public System.Windows.Point Point { get; set; } = new System.Windows.Point();

        /// <summary>
        /// 吸附对象
        /// </summary>
        public IconBoxInfo? IconBoxInfo;
    }
}
