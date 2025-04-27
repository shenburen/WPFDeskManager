namespace WPFDeskManager
{
    internal class Global
    {
        // public static NotifyIcon? AppNotify = null;

        public static MainWindow? MainWindow = null;

        public static Dictionary<int, IconBox> IconBoxes = new Dictionary<int, IconBox>();

        public static void Init()
        {
            CreateAppNotify();
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        public static void Dispose()
        {

        }

        public static void CreateAppNotify()
        {

        }
    }
}
