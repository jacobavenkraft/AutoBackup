using FrameworkLibrary;
using System.Windows;

namespace AutoBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Initialize<AppServiceConfigurator>();
        }
    }
}
