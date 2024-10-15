using drawingApp.ViewModels;
using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;

namespace drawingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;
        protected void ApplicationStart(object sender, EventArgs e)
        {
            mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

}
