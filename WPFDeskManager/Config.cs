namespace WPFDeskManager
{
    internal class Config
    {
        /// <summary>
        /// 图标尺寸
        /// </summary>
        public static int IconSize { get; set; } = 32;

        /// <summary>
        /// 六边形半径
        /// </summary>
        public static int HexagonRadius { get; set; } = 32;

        /// <summary>
        /// 吸附距离
        /// </summary>
        public static int SnapDistance { get; set; } = 15;

        /// <summary>
        /// 双击事件间隔
        /// </summary>
        public static int DoubleClickTime { get; set; } = 300;

        /// <summary>
        /// 解除吸附关系的移动距离
        /// </summary>
        public static int OffMapDistance { get; set; } = 10;
    }
}
