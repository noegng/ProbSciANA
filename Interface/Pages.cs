using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProbSciANA.Interface
{
    
#region Page Accueil
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
        private void BtnModeTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Test());
        }
    }
#endregion

#region Page Login
    public partial class LoginView : Page
   {
        public LoginView()
        {
            InitializeComponent();
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            string nom = NomTextBox.Text;
            string prenom = PrenomTextBox.Text;
            string email = EmailTextBox.Text;
            string adresse = AdresseTextBox.Text;
            string station = StationTextBox.Text;
            string mdp = MdpTextBox.Password;
            var selectedItem = RoleComboBox.SelectedItem as ComboBoxItem;
            string role = selectedItem?.Content.ToString();

            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(prenom) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(adresse) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Veuillez remplir tous les champs et sélectionner un rôle.");
                return;
            }

            try
            {
                var nouvelUtilisateur = new Utilisateur(
                    estClient: role == "Client",
                    estCuisinier: role == "Cuisinier",
                    nom,
                    prenom,
                    adresse,
                    "", // téléphone
                    email,
                    station,
                    mdp);

                MessageBox.Show($"Bienvenue {prenom} {nom} !\nRôle : {role}");

                if (role == "Client")
                    NavigationService?.Navigate(new UserDashboardView());
                else if (role == "Cuisinier")
                    NavigationService?.Navigate(new CuisinierDashboardView());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement : " + ex.Message);
            }*/
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

#endregion

#region Page Connexion
public partial class ConnexionView : Page
{
        private Dictionary<string, Utilisateur> utilisateurs;

        public ConnexionView()
        {
            InitializeComponent();
            Utilisateur.RefreshAll();

            foreach (var utilisateur in Utilisateur.utilisateurs)
            {
                UserComboBox.Items.Add(utilisateur.Prenom + " " + utilisateur.Nom);
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

            if (utilisateurs.TryGetValue(nomUtilisateur, out var utilisateur) && motDePasseEntre == utilisateur.Mdp)
            {
                MessageBox.Show($"Connexion réussie : {nomUtilisateur}");

                if (utilisateur.EstCuisinier)
                    NavigationService?.Navigate(new CuisinierDashboardView());
                else if (utilisateur.EstClient)
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

#endregion

#region Page Vue Client
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

#endregion

#region Page Vue Cuisinier
    public partial class CuisinierDashboardView : Page
    {
        public CuisinierDashboardView()
        {
            InitializeComponent();
            string excelFilePath = App.ExcelFilePath;
        }

      
        private void AjouterPlat_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour ajouter un plat
            MessageBox.Show("Ajouter un plat");

        }

        private void AfficherGraphe_Click(object sender, RoutedEventArgs e)
        {
            var graphe = Program.GrapheMétro;
            var stations = Program.Stations;
            var arcs = Program.Arcs;

            Graphviz<(int id, string nom)>.GenerateGraphImage(stations, arcs);
            graphe.AffichageDijkstra(stations[10],stations[80]);
            graphe.AffichageBellmanFord(stations[10],stations[80]);
            graphe.AffichageFloydWarshall(stations[10],stations[80]);
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
#endregion

#region Page Vue Admin
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

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
    }
#endregion

#region Page Gestion Clients (admin)
    public partial class ClientsView : Page
    {
        private List<Utilisateur> clients;

        public ClientsView()
        {
            InitializeComponent();
            LoadClients();
        }

        private bool _triAscendant = true;
        private string _colonneTriee = "";

        private void SetSorting(string colonne)
        {
            if (_colonneTriee == colonne)
            {
                _triAscendant = !_triAscendant;
            }
            else
            {
                _colonneTriee = colonne;
                _triAscendant = true;
            }
            LoadClients();
        }

        public void LoadClients()
        {
            var achats = Requetes.GetAchatsUtilisateursSQL();

            var table = new DataTable();
            table.Columns.Add("Id_utilisateur", typeof(int));
            table.Columns.Add("Statut", typeof(string));
            table.Columns.Add("Nom", typeof(string));
            table.Columns.Add("Prenom", typeof(string));
            table.Columns.Add("Adresse", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Telephone", typeof(string));
            table.Columns.Add("Station", typeof(string));
            table.Columns.Add("Achats", typeof(double));
            table.Columns.Add("Date_inscription", typeof(DateTime));

            foreach (KeyValuePair<Utilisateur, double> kv in achats)
            {
                string statut;
                Utilisateur u = kv.Key;
                if(u.EstEntreprise)
                {
                    statut = "Entreprise";
                }
                else
                {
                    statut = "Particulier";
                }
                string stationNom = "Inconnu";
                if (u.Station != null && u.Station.Valeur.nom != null)
                {
                    stationNom = u.Station.Valeur.nom;
                }
                table.Rows.Add(
                    u.Id_utilisateur,
                    statut,
                    u.Nom,
                    u.Prenom,
                    u.Adresse,
                    u.Email,
                    u.Telephone,
                    stationNom,
                    kv.Value,
                    u.Date_inscription
                );
            }

            DataView view = table.DefaultView;
            string direction = "";
            if (!string.IsNullOrEmpty(_colonneTriee) && table.Columns.Contains(_colonneTriee))
            {
                if(_triAscendant)
                {
                    direction = "ASC";
                }
                else
                {
                    direction = "DESC";
                }
                view.Sort = $"{_colonneTriee} {direction}";
            }

            ClientsListView.ItemsSource = view;
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsListView.SelectedItem is DataRowView row)
            {
                int id = (int)row["Id_utilisateur"];
                Utilisateur client = Utilisateur.utilisateurs.Find(u => u.Id_utilisateur == id);

                if (client != null && !client.Nom.Contains("(modifié)"))
                {
                    client.Nom += " (modifié)";
                    LoadClients();
                }
            }
        }
        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginView());
        }
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Header_ID_Click(object sender, RoutedEventArgs e) {SetSorting("Id_utilisateur");}
        private void Header_Statut_Click(object sender, RoutedEventArgs e) {SetSorting("Statut");}
        private void Header_Nom_Click(object sender, RoutedEventArgs e) {SetSorting("Nom");}
        private void Header_Prenom_Click(object sender, RoutedEventArgs e) {SetSorting("Prenom");}
        private void Header_Adresse_Click(object sender, RoutedEventArgs e) {SetSorting("Adresse");}
        private void Header_Email_Click(object sender, RoutedEventArgs e) {SetSorting("Email");}
        private void Header_Telephone_Click(object sender, RoutedEventArgs e) {SetSorting("Telephone");}
        private void Header_Station_Click(object sender, RoutedEventArgs e) {SetSorting("Station");}
        private void Header_Achats_Click(object sender, RoutedEventArgs e) {SetSorting("Achats");}
        private void Header_Date_Click(object sender, RoutedEventArgs e) {SetSorting("Date_inscription");}
        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
    }

#endregion

#region Page Gestion Cuisiniers (admin)
    public partial class CuisiniersView : Page
    {
        private List<Utilisateur> cuisiniers;

        public CuisiniersView()
        {
            InitializeComponent();
            Utilisateur.RefreshAll();
            LoadCuisiniers();
        }

        private void LoadCuisiniers(string orderBy = "u.nom")
        {
            cuisiniers = Utilisateur.utilisateurs.FindAll(u => u.EstCuisinier);
            if (orderBy == "adresse")
                cuisiniers.Sort((a, b) => a.Adresse.CompareTo(b.Adresse));
            else
                cuisiniers.Sort((a, b) => a.Nom.CompareTo(b.Nom));

            CuisiniersListView.ItemsSource = null;
            CuisiniersListView.ItemsSource = cuisiniers;
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (CuisiniersListView.SelectedItem is Utilisateur cuisinier)
            {
                cuisinier.EstCuisinier = false; // Déclenche suppression automatique
                LoadCuisiniers();
            }
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (CuisiniersListView.SelectedItem is Utilisateur cuisinier && !cuisinier.Nom.Contains("(modifié)"))
            {
                cuisinier.Nom = cuisinier.Nom + " (modifié)";
                LoadCuisiniers();
            }
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginView());
        }

        private void BtnTrierNom_Click(object sender, RoutedEventArgs e) => LoadCuisiniers("nom");
        private void BtnTrierAdresse_Click(object sender, RoutedEventArgs e) => LoadCuisiniers("adresse");
        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
    }

#endregion

#region Page Gestion Commandes (admin)

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

#endregion

#region Page Statistiques (admin)

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

#endregion

#region Test
    public partial class Test : Page
    {
        public Test()
        {
            InitializeComponent();
        }
        private void Btn1(object sender, RoutedEventArgs e)
        {
            // Logique pour le bouton 1
            MessageBox.Show("Bouton 1 cliqué !");
        }
        private void Btn2(object sender, RoutedEventArgs e)
        {
            // Logique pour le bouton 2
            MessageBox.Show("Bouton 2 cliqué !");
        }
        private void Btn3(object sender, RoutedEventArgs e)
        {
            // Logique pour le bouton 3
            MessageBox.Show("Bouton 3 cliqué !");
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
#endregion
}