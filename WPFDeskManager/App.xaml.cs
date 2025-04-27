using System.Windows;

namespace WPFDeskManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Global.Init();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Global.Dispose();
            base.OnExit(e);
        }
    }
}
