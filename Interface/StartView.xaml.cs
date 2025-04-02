using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;

namespace ProbSciANA.Interface
{

    public partial class StartView : Page
    {
        public StartView()
        {
            InitializeComponent();
        }

        private void BtnModeUtilisateur_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginView());
        }

        private void BtnModeAdmin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
    }

    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            // Récupération des champs d'identité
            string nom = NomTextBox.Text;
            string prenom = PrenomTextBox.Text;
            string email = EmailTextBox.Text;
            string adresse = AdresseTextBox.Text;
            string mdp = MdpTextBox.Password;
            var selectedItem = RoleComboBox.SelectedItem as ComboBoxItem;
            string role = selectedItem?.Content.ToString();

    if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(prenom) ||
        string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(adresse) || string.IsNullOrWhiteSpace(role))
    {
        MessageBox.Show("Veuillez remplir tous les champs et sélectionner un rôle.");
        return;
    }

    try
    {
        SqlQueries.SqlAddUser(nom, prenom, email, adresse, role, mdp);
        MessageBox.Show($"Bienvenue {prenom} {nom} !\nRôle : {role}");
        if (role == "Client")  // Redirection selon le rôle
            {
                NavigationService?.Navigate(new UserDashboardView());
            }
            else if (role == "Cuisinier")
            {
                NavigationService?.Navigate(new CuisinierDashboardView());
            }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erreur lors de l'enregistrement : " + ex.Message);
    }

            
            
        }
      
      private void BtnSeConnecter_Click(object sender, RoutedEventArgs e)
        {
    NavigationService?.Navigate(new ConnexionView());
        }
        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
    }

public partial class ConnexionView : Page
{
private Dictionary<string, (int id, string motDePasse, string role)> utilisateurs;

    public ConnexionView()
    {
        InitializeComponent();
        utilisateurs = SqlQueries.ChargerUtilisateurs();
        foreach (var utilisateur in utilisateurs.Keys)
    {
        UserComboBox.Items.Add(utilisateur);
    }
    }

    
    private void BtnConnexion_Click(object sender, RoutedEventArgs e)
    {
        if (UserComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(PasswordBox.Password))
        {
            MessageBox.Show("Veuillez sélectionner un utilisateur et saisir le mot de passe.");
            return;
        }

        string nomUtilisateur = UserComboBox.SelectedItem.ToString();
        string motDePasseEntre = PasswordBox.Password;

        if (utilisateurs.TryGetValue(nomUtilisateur, out var infos)
            && motDePasseEntre == infos.motDePasse)
        {
            MessageBox.Show($"Connexion réussie : {nomUtilisateur} ({infos.role})");

            if (infos.role == "Cuisinier")
                NavigationService?.Navigate(new CuisinierDashboardView());
            else
                NavigationService?.Navigate(new UserDashboardView());
        }
        else
        {
            MessageBox.Show("Mot de passe incorrect.");
        }
    }

    private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new StartView());
    }
}
    public partial class UserDashboardView : Page
    {
        public UserDashboardView()
        {
            InitializeComponent();
        }

        private void BtnCommander_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandesView());
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
    }


    public partial class CuisinierDashboardView : Page
    {


        public CuisinierDashboardView()
        {
            InitializeComponent();
        }

        private void AjouterPlat_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour ajouter un plat
            MessageBox.Show("Ajouter un plat");
        }

        private void VoirLivraisons_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour voir les livraisons
            MessageBox.Show("Voir les livraisons");
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }

    }
        public partial class AdminDashboardView : Page
    {
        public AdminDashboardView()
        {
            InitializeComponent();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsView());
        }

        private void BtnCuisiniers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CuisiniersView());
        }

        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandesView());
        }

        private void BtnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatistiquesView());
        }

        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
    }

    public partial class ClientsView : Page
    {
        public ClientsView()
        {
            InitializeComponent();
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }

    public partial class CuisiniersView : Page
    {
        public CuisiniersView()
        {
            InitializeComponent();
        }


        
        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }

    public partial class CommandesView : Page
    {
        public CommandesView()
        {
            InitializeComponent();
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }

    public partial class StatistiquesView : Page
    {
        public StatistiquesView()
        {
            InitializeComponent();
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}