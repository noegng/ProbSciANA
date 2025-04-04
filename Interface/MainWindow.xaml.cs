using System.Windows;


namespace ProbSciANA.Interface
{
    public partial class App : Application { }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Interface.StartView());
        }
    }
}