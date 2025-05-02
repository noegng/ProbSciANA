using System.Windows;
using System.Threading.Tasks;

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

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string excelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx"; // Chemin vers le fichier Excel
            Program.InitializeData(excelFilePath);
            Utilisateur.RefreshList();
            Task.Run(() => Utilisateur.RecalculerStationsAsync());

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}