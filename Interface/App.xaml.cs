using System;
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

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                // Configuration de l'encodage
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                // Initialisation des données
                string excelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx";
                Program.InitializeData(excelFilePath);

                // Rafraîchissement et recalcul des stations
                Utilisateur.RefreshList();
                Console.WriteLine("Rafraîchissement de la liste des stations...");
                await Utilisateur.RecalculerStationsAsync(); /// Attend la fin du recalcul
                Console.WriteLine("Rafraîchissement terminé.");
                /// Lance l'interface utilisateur une fois que tout est prêt
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du démarrage : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}