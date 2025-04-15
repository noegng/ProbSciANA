using System.Collections.Generic;
using System.Windows;

namespace ProbSciANA.Interface
{
    public partial class App : Application
    {
        public static string ExcelFilePath { get; private set; }
        private static App program = (App)Application.Current;
        public static Graphe<(int id, string nom)> graphe { get;  set; }
        public static List<Noeud<(int id, string nom)>> Stations { get;  set; }
        public static List<Arc<(int id, string nom)>> Arcs { get;  set; }
        public App()
        {
            InitializeComponent();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ExcelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx";

            /// Initialiser les données
            Program.InitializeData(ExcelFilePath);

            /// Accéder aux variables du graphe présent dans program
            graphe = Program.GrapheMétro;
            Arcs = Program.Arcs;
            Stations = Program.Stations;

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}