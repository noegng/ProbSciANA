using System;
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Input; 
using System.Windows.Media; 
using System.Collections.Generic;
using System.Diagnostics;

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
           try
    {
        MessageBox.Show("Gestion des Clients");
        var stations = new List<Station>();
        var aretes = new List<Arete>();
        var VitessesMoyennes = new Dictionary<string, double>();
        (stations, aretes, VitessesMoyennes) = Program.LectureFichierExcel();
        Program.AffichageImage(stations, aretes);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
    }
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