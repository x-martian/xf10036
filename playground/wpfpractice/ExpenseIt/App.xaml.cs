using System; // Uri, UriKind, EventArgs, Console
using System.Windows; // Application, StartupEventArgs
using System.Windows.Navigation; // NavigationWindow
using System.Windows.Threading; // DispatcherUnhandledExceptionEventArgs

namespace ExpenseIt
{
    public partial class App : Application
    {        
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Open a window
	    NavigationWindow window = new NavigationWindow();
	    MainWindow.Show();

	    // Navigate to the home page
            ((NavigationWindow)this.MainWindow).Navigate(new Uri("3DAnimation.xaml", UriKind.Relative));
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(e.Exception.Message); //, MessageBoxButton.YesNo, MessageBoxImage.Error);
	    this.Shutdown(-1);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
