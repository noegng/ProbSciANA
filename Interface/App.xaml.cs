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

    /// Accéder à la variable graphe
    var graphe1 = Program.graphe;
    graphe = graphe1;
    

    var arcs = Program.Arcs;
    Arcs = arcs;
    /// Exemple : Afficher les noeuds du graphe
  var stations = Program.Stations;
    Stations = stations;

    MainWindow mainWindow = new MainWindow();
    mainWindow.Show();
        }
    }
}