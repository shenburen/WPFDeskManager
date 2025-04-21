using System.Configuration;
using System.Data;
using System.Windows;

namespace WPFDeskManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Hexagon hexagon =  new Hexagon();
            hexagon.Show();
        }
    }
}
