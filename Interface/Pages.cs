using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Net.Http;

namespace ProbSciANA.Interface
{
    
#region Page Accueil
    public partial class StartView : Page
    {
        public StartView()
        {
            InitializeComponent();
            Utilisateur.RefreshAll();
            _ = LoadStationsAsync(); /// Appel asynchrone pour charger les stations
        }

        public async Task LoadStationsAsync()
        {
            try
            {
                await Requetes.MàjStations();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Erreur de connexion : " + ex.Message);
            }
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

        private async void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            string nom = NomTextBox.Text;
            string prenom = PrenomTextBox.Text;
            string email = EmailTextBox.Text;
            string adresse = AdresseTextBox.Text;
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
            else if (!email.Contains("@"))
            {
                MessageBox.Show("Veuillez entrer une adresse e-mail valide.");
                return;
            }

            try
            {

                if (adresse == null)
                {
                    MessageBox.Show("Adresse non trouvée. Veuillez vérifier l'adresse saisie.");
                    return;
                }
                var Station = await Noeud<(int id, string nom)>.TrouverStationLaPlusProche(adresse); /// TODO : à revoir, car pas de station la plus proche dans le cas d'une adresse non trouvée
                /// recherche de la station la plus proche avec haversine
               
                var nouvelUtilisateur = new Utilisateur(
                    estClient: role == "Client",
                    estCuisinier: role == "Cuisinier",
                    nom,
                    prenom,
                    adresse,
                    "", // téléphone
                    email,
                    Station,
                    mdp,
                    estEntreprise: role == "Entreprise");

                MessageBox.Show($"Bienvenue {prenom} {nom} !\nRôle : {role}");

                if (role == "Client")
                    NavigationService?.Navigate(new UserDashboardView());
                else if (role == "Cuisinier")
                    NavigationService?.Navigate(new CuisinierDashboardView());
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Erreur réseau : " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur inattendue : " + ex.Message);
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
                string fullName = utilisateur.Prenom + " " + utilisateur.Nom;
                UserComboBox.Items.Add(fullName);
            }
        }


        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur et saisir le mot de passe.");
                return;
            }

            var nomUtilisateur = UserComboBox.SelectedItem.ToString();
            string motDePasseEntre = PasswordBox.Password;


            Utilisateur utilisateurTrouve = null;
            foreach (var utilisateur in Utilisateur.utilisateurs)
            {
        if ($"{utilisateur.Prenom} {utilisateur.Nom}" == nomUtilisateur)
        {
            utilisateurTrouve = utilisateur;
            break;
        }
            }

             if (utilisateurTrouve != null && motDePasseEntre == utilisateurTrouve.Mdp)
            {
        MessageBox.Show($"Connexion réussie : {nomUtilisateur} ");

        // Rediriger en fonction du rôle de l'utilisateur
        if (utilisateurTrouve.EstCuisinier)
            NavigationService?.Navigate(new CuisinierDashboardView());
        else if (utilisateurTrouve.EstClient)
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
        }
        private void LoadLivraisons()
    {

             /// Exemple de données de livraison

    }
       
       
      
        private void AjouterPlat_Click(object sender, RoutedEventArgs e)
        {
            /// Logique pour ajouter un plat
            MessageBox.Show("Ajouter un plat");

        }

        private async void BtnLivrer(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Livrer un plat");
            await Program.UtiliserGetCoordonnees();
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

#region Plat

    public partial class PlatView : Page
    {
        public PlatView()
        {
            InitializeComponent();
        }

        private void BtnAjouterPlat_Click(object sender, RoutedEventArgs e)
        {
            // string nomPlat = NomPlatTextBox.Text;
            // string typePlat = (TypePlatComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            // string nationalite = NationaliteTextBox.Text;
            // string regimeAlimentaire = RegimeTextBox.Text;
            // string ingredients = IngredientsTextBox.Text;
            // string prixParPersonne = PrixTextBox.Text;
            // string nombrePersonnes = NombrePersonnesTextBox.Text;
            // DateTime? dateFabrication = DateFabricationDatePicker.SelectedDate;
            // DateTime? datePeremption = DatePeremptionDatePicker.SelectedDate;

            // if (string.IsNullOrWhiteSpace(nomPlat) || string.IsNullOrWhiteSpace(typePlat) ||
            // string.IsNullOrWhiteSpace(nationalite) || string.IsNullOrWhiteSpace(regimeAlimentaire) ||
            // string.IsNullOrWhiteSpace(ingredients) || string.IsNullOrWhiteSpace(prixParPersonne) ||
            // string.IsNullOrWhiteSpace(nombrePersonnes) || !dateFabrication.HasValue || !datePeremption.HasValue)
            // {
            // MessageBox.Show("Veuillez remplir tous les champs.");
            // return;
            // }

            // try
            // {
            // var nouveauPlat = new Plat
            // {
            //     Nom = nomPlat,
            //     Type = typePlat,
            //     Nationalite = nationalite,
            //     RegimeAlimentaire = regimeAlimentaire,
            //     Ingredients = ingredients,
            //     PrixParPersonne = double.Parse(prixParPersonne),
            //     NombrePersonnes = int.Parse(nombrePersonnes),
            //     DateFabrication = dateFabrication.Value,
            //     DatePeremption = datePeremption.Value
            // };

            // Requetes.AjouterPlat(nouveauPlat);
            // MessageBox.Show("Plat ajouté avec succès !");
            // NavigationService?.GoBack();
            // }
            // catch (Exception ex)
            // {
            // MessageBox.Show("Erreur lors de l'ajout du plat : " + ex.Message);
            // }
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

        private void LoadCuisiniers()
        {
            // cuisiniers = Requetes.utilisateurs.FindAll(u => u.EstCuisinier);
            // if (orderBy == "adresse")
            //     cuisiniers.Sort((a, b) => a.Adresse.CompareTo(b.Adresse));
            // else
            //     cuisiniers.Sort((a, b) => a.Nom.CompareTo(b.Nom));

            // CuisiniersListView.ItemsSource = null;
            // CuisiniersListView.ItemsSource = cuisiniers;
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

        // private void BtnTrierNom_Click(object sender, RoutedEventArgs e) => LoadCuisiniers("nom");
        // private void BtnTrierAdresse_Click(object sender, RoutedEventArgs e) => LoadCuisiniers("adresse");
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