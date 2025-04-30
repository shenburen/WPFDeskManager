using System.Windows;

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
        /// 实例对象
        /// </summary>
        public IconBox? Self;

        /// <summary>
        /// 父节点
        /// </summary>
        public IconBoxInfo? Parent;

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
        public Point Point { get; set; } = new Point();
    }
}
