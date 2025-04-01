using System.Windows;
using System.Windows.Controls; 
using System.Windows.Input; 
using System.Windows.Media; 

namespace ProbSciANA
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gestion des Clients");
            // Appeler la méthode GestionClients() ici
        }

        private void Cuisiniers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gestion des Cuisiniers");
            // Appeler la méthode GestionCuisiniers() ici
        }

        private void Commandes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gestion des Commandes");
            // Appeler la méthode GestionCommandes() ici
        }

        private void Trajets_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gestion des Trajets");
            // Appeler la méthode GestionTrajets() ici
        }

        private void Statistiques_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Statistiques");
            // Appeler la méthode AfficherStatistiques() ici
        }

        private void Quitter_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}