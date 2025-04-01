using System.Windows;

namespace ProbSciANA.Interface
{
    public partial class App : Application
    {
        private static App program = (App)Application.Current;
        public App()
        {
            InitializeComponent();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}