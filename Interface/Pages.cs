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
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace ProbSciANA.Interface
{
    #region Page Accueil
    public partial class StartView : Page
    {
        public StartView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                UpdateNavButtons();
                UpdateAuthButtons();
            };


        }


        private void BtnModeTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Test());
        }
        private void UpdateAuthButtons()
        {
            bool loggedIn = SessionManager.IsLoggedIn();

            BtnProfil.Visibility = loggedIn ? Visibility.Visible : Visibility.Collapsed;
            BtnLogout.Visibility = loggedIn ? Visibility.Visible : Visibility.Collapsed;

            BtnConnexion.Visibility = loggedIn ? Visibility.Collapsed : Visibility.Visible;
            BtnInscription.Visibility = loggedIn ? Visibility.Collapsed : Visibility.Visible;
        }
        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // 1. Supprimer la session utilisateur
            SessionManager.Logout();

            // 2. Réinitialiser les boutons
            UpdateAuthButtons();

            // 3. Revenir à l'accueil
            NavigationService?.Navigate(new StartView());

            MessageBox.Show("Vous avez été déconnecté.");
        }
        private void BtnModeTest2_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Test2());
        }
        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConnexionView());
        }
        private void BtnInscription_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Register());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e) { }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Page Register
    public partial class Register : Page
    {
        public Register()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
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
                string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(mdp))
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
                    mdp,
                    Station,
                    estEntreprise: role == "Entreprise");
                SessionManager.CurrentUser = nouvelUtilisateur;

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

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConnexionView());
        }

        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
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
            Utilisateur.RefreshList();

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
                SessionManager.CurrentUser = utilisateurTrouve;

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

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }

        private void BtnInscription_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Register());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
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

        private void Commander_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommanderView());
        }
        private void AfficherCommandes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
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
            Loaded += (s, e) => UpdateNavButtons();
        }
        private void AfficherClients_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AfficherCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandeView());
        }
        private void AfficherPlats_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PlatView());
        }
        private void AfficherAvis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AvisView());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Add Plat

    public partial class AddPlat : Page
    {
        public AddPlat()
        {
            InitializeComponent();
            //Loaded += (s, e) => UpdateNavButtons();
        }

        private void BtnAjouterPlat_Click(object sender, RoutedEventArgs e)
        {
            string nomPlat = NomPlatTextBox.Text;
            string prixPlat = PrixTextBox.Text;
            string typePlat = (TypePlatComboBox.SelectedItem as ComboBoxItem)?.Content.ToString().ToLower();
            string nationalitePlat = NationaliteTextBox.Text;
            string regimeAlimentaire = RegimeTextBox.Text;
            string nbPortions = NombrePersonnesTextBox.Text;

            DateTime? dateperemption = DatePeremptionDatePicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(nomPlat) || string.IsNullOrWhiteSpace(typePlat) ||
            string.IsNullOrWhiteSpace(nationalitePlat) || string.IsNullOrWhiteSpace(regimeAlimentaire)
            || prixPlat == null || nbPortions == null || !dateperemption.HasValue)
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            try
            {
                double prix = double.Parse(prixPlat);
                int nbPortionsInt = int.Parse(nbPortions);
                DateTime datePeremption = dateperemption.Value;
                var nouveauPlat = new Plat
                (
                    nomPlat,
                    prix,
                    nbPortionsInt,
                    typePlat,
                    regimeAlimentaire,
                    nationalitePlat,
                    datePeremption
                );

                MessageBox.Show("Plat ajouté avec succès !");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout du plat : " + ex.Message);
            }
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

    #region Page Vue Commander
    public partial class CommanderView : Page
    {
        public List<Utilisateur> Cuisiniers { get; set; }
        public CommanderView()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Utilisateur.RefreshList();
            Cuisiniers = Utilisateur.cuisiniers;
            DataContext = this;
        }

        private void CuisinierCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Utilisateur cuisinier)
            {
                NavigationService.Navigate(new RestoView(cuisinier));
            }
        }
        private void AfficherCommandes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Page Vue Resto
    public partial class RestoView : Page
    {
        public Utilisateur Cuisinier { get; set; }
        public List<Plat> Plats { get; set; } = new();
        public List<Plat> Panier { get; set; } = new();
        public Plat? SelectedPlat { get; set; }
        public double Prix
        {
            get
            {
                double result = 0;
                foreach (Plat p in Panier)
                {
                    result += p.Prix;
                }
                return result;
            }
        }

        public RestoView(Utilisateur cuisinier_select)
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Cuisinier = cuisinier_select;
            Cuisine.RefreshList();
            foreach (Cuisine c in Cuisinier.Cuisines)
            {
                Plats.Add(c.Plat);
            }
            DataContext = this;
        }

        private void AfficherCommandes_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PlatCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.DataContext is Plat plat)
            {
                SelectedPlat = plat;
                DataContext = null;
                DataContext = this;
            }
        }
        private void AjouterAuPanier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Plat plat)
            {
                Panier.Add(plat);
                DataContext = null;
                DataContext = this;
            }
        }
        private void ValiderPanier_Click(object sender, RoutedEventArgs e)
        {
            new Commande("Commande " + Commande.commandes.Count + 1, Prix, "en attente", SessionManager.CurrentUser.Id_utilisateur, Cuisinier.Id_utilisateur);
            MessageBox.Show($"Commande de {Panier.Count} plat(s) validée !");
        }
        private void AnnulerPanier_Click(object sender, RoutedEventArgs e)
        {
            Panier.Clear();
            DataContext = null;
            DataContext = this;
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Page Vue Commande
    public partial class CommandeView : Page
    {
        public CommandeView()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Utilisateur.RefreshList();
            dataGridCommandes.ItemsSource = null;
            dataGridCommandes.ItemsSource = SessionManager.CurrentUser.Cuisines;
        }
        private async void dataGridCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridCommandes.SelectedItem is Plat selected)
            {
                DataContext = selected;
            }
        }
        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddPlat());
        }

        private void AfficherClients_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AfficherPlats_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PlatView());
        }
        private void AfficherAvis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AvisView());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Page Vue Avis
    public partial class AvisView : Page
    {
        public AvisView()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Requetes.RefreshAllLists();
            dataGridAvis.ItemsSource = null;
            dataGridAvis.ItemsSource = SessionManager.CurrentUser.Cuisines;
        }
        private async void dataGridAvis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridAvis.SelectedItem is Cuisine selected)
            {
                DataContext = selected;
            }
        }
        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddPlat());
        }
        private void AfficherClients_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AfficherCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandeView());
        }
        private void AfficherPlats_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PlatView());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Page Vue Plat
    public partial class PlatView : Page
    {
        public PlatView()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Requetes.RefreshAllLists();
            dataGridPlats.ItemsSource = null;
            dataGridPlats.ItemsSource = SessionManager.CurrentUser.Cuisines;
            Console.WriteLine(SessionManager.CurrentUser.Cuisines.Count);
            Console.WriteLine(SessionManager.CurrentUser.Cuisines[0].Plat.Composes.Count);
        }
        private void dataGridPlats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridPlats.SelectedItem is Cuisine selected)
            {
                DataContext = selected;
            }
        }
        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddPlat());
        }
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridPlats.SelectedItem is Cuisine selected)
            {
                selected.Delete();
                Utilisateur.RefreshList();
                dataGridPlats.ItemsSource = null;
                dataGridPlats.ItemsSource = SessionManager.CurrentUser.Cuisines;
            }
        }

        private void AfficherClients_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AfficherCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandeView());
        }
        private void AfficherAvis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AvisView());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Page Vue Admin
    public partial class AdminDashboardView : Page
    {
        public AdminDashboardView()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsViewAdmin());
        }
        private void BtnCuisiniers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CuisiniersViewAdmin());
        }
        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandesViewAdmin());
        }
        private void BtnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatistiquesViewAdmin());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e) { }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Page Clients Admin
    public partial class ClientsViewAdmin : Page
    {
        //public var SelectedItem;
        public ClientsViewAdmin()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Requetes.RefreshAllLists();
            dataGridClients.ItemsSource = null;
            dataGridClients.ItemsSource = Utilisateur.clients;
        }
        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // Masquer la fiche client, afficher le bandeau
            FicheClient.Visibility = Visibility.Collapsed;
            AddPane.Visibility = Visibility.Visible;

            // Réinitialiser les champs
            TxtPrenom.Text = TxtNom.Text = TxtAdresse.Text = TxtTel.Text = TxtEmail.Text = "";
        }
        private void BtnAnnulerAjout_Click(object sender, RoutedEventArgs e)
        {
            AddPane.Visibility = Visibility.Collapsed;
            FicheClient.Visibility = Visibility.Visible;
        }
        private async void BtnValiderAjout_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPrenom.Text) ||
                string.IsNullOrWhiteSpace(TxtNom.Text) ||
                string.IsNullOrWhiteSpace(TxtAdresse.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("Tous les champs obligatoires doivent être remplis.");
                return;
            }

            try
            {
                bool estEntreprise = (CmbStatut.SelectedItem as ComboBoxItem)?
                     .Content?.ToString() == "Entreprise";

                var station = await Noeud<(int id, string nom)>.TrouverStationLaPlusProche(TxtAdresse.Text);

                var nouveauClient = new Utilisateur(
                    estClient: true,
                    estCuisinier: false,
                    nom: TxtNom.Text,
                    prenom: TxtPrenom.Text,
                    adresse: TxtAdresse.Text,
                    telephone: TxtTel.Text,
                    email: TxtEmail.Text,
                    mdp: "mdp1234",
                    station: station,
                    nom_referent: estEntreprise ? TxtReferent.Text : "",
                    estEntreprise: estEntreprise);

                // Rafraîchir la liste
                Utilisateur.RefreshList();
                dataGridClients.ItemsSource = null;
                dataGridClients.ItemsSource = Utilisateur.clients;

                // Fermer le bandeau
                AddPane.Visibility = Visibility.Collapsed;
                FicheClient.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout : " + ex.Message);
            }
        }
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridClients.SelectedItem is Utilisateur selectedUtilisateur)
            {
                selectedUtilisateur.Delete();
                dataGridClients.ItemsSource = null;
                dataGridClients.ItemsSource = Utilisateur.clients;
            }
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsViewAdmin());
        }
        private void BtnCuisiniers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CuisiniersViewAdmin());
        }
        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandesViewAdmin());
        }
        private void BtnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatistiquesViewAdmin());
        }
        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }

        private void dataGridClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridClients.SelectedItem is Utilisateur selected)
            {
                DataContext = selected;
            }
        }
        private void CmbStatut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LblReferent == null)      // appelé trop tôt pendant InitializeComponent
                return;

            bool isEntreprise = (CmbStatut.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Entreprise";

            LblReferent.Visibility = isEntreprise ? Visibility.Visible : Visibility.Collapsed;
            TxtReferent.Visibility = isEntreprise ? Visibility.Visible : Visibility.Collapsed;
        }
        private void OnListCommandes_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListCommandes.SelectedItem != null)
            {
                ListAvis.SelectedItem = null;
                ListCuisiniers.SelectedItem = null;
            }
        }
        private void OnListAvis_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListAvis.SelectedItem != null)
            {
                ListCommandes.SelectedItem = null;
                ListCuisiniers.SelectedItem = null;
            }
        }
        private void OnListCuisiniers_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListCuisiniers.SelectedItem != null)
            {
                ListCommandes.SelectedItem = null;
                ListAvis.SelectedItem = null;
            }
        }
        private async void dataGridClients_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header?.ToString() == "Adresse" &&
                e.Row.Item is Utilisateur utilisateur &&
                e.EditingElement is TextBox tb)
            {
                var nouvelleAdresse = tb.Text;
                utilisateur.Adresse = nouvelleAdresse;

                try
                {
                    utilisateur.Station = await Noeud<(int id, string nom)>.TrouverStationLaPlusProche(nouvelleAdresse);
                    dataGridClients.Items.Refresh(); // Mise à jour de l'affichage
                }
                catch
                {
                    MessageBox.Show("Adresse invalide ou station introuvable.");
                }
            }
        }

    }
    #endregion

    #region Page Cuisiniers Admin
    public partial class CuisiniersViewAdmin : Page
    {
        public object SelectedElement { get; set; }

        public CuisiniersViewAdmin()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Requetes.RefreshAllLists();
            dataGridCuisiniers.ItemsSource = null;
            dataGridCuisiniers.ItemsSource = Utilisateur.cuisiniers;
        }
        private async void dataGridCuisiniers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridCuisiniers.SelectedItem is Utilisateur selected)
            {
                DataContext = selected;
            }
        }
        private async void dataGridCuisiniers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header?.ToString() == "Adresse" &&
                e.Row.Item is Utilisateur utilisateur &&
                e.EditingElement is TextBox tb)
            {
                var nouvelleAdresse = tb.Text;
                utilisateur.Adresse = nouvelleAdresse;

                try
                {
                    utilisateur.Station = await Noeud<(int id, string nom)>.TrouverStationLaPlusProche(nouvelleAdresse);
                    dataGridCuisiniers.Items.Refresh(); // Mise à jour de l'affichage
                }
                catch
                {
                    MessageBox.Show("Adresse invalide ou station introuvable.");
                }
            }
        }
        private void OnListCommandes_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListCommandes.SelectedItem != null)
            {
                ListAvis.SelectedItem = null;
                ListClients.SelectedItem = null;
            }
        }
        private void OnListAvis_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListAvis.SelectedItem != null)
            {
                ListCommandes.SelectedItem = null;
                ListClients.SelectedItem = null;
            }
        }
        private void OnListClients_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListClients.SelectedItem != null)
            {
                ListCommandes.SelectedItem = null;
                ListAvis.SelectedItem = null;
            }
        }
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridCuisiniers.SelectedItem is Utilisateur selectedUtilisateur)
            {
                selectedUtilisateur.Delete();
                dataGridCuisiniers.ItemsSource = null;
                dataGridCuisiniers.ItemsSource = Utilisateur.cuisiniers;
            }
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Register());
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsViewAdmin());
        }
        private void BtnCuisiniers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CuisiniersViewAdmin());
        }
        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandesViewAdmin());
        }
        private void BtnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatistiquesViewAdmin());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }

    #endregion

    #region Page Commandes Admin

    public partial class CommandesViewAdmin : Page
    {
        public CommandesViewAdmin()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Commande.RefreshList();
            dataGridCommandes.ItemsSource = null;
            dataGridCommandes.ItemsSource = Commande.commandes;
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridCommandes.SelectedItem is Commande selectedCommande)
            {
                selectedCommande.Delete();
                dataGridCommandes.ItemsSource = null;
                dataGridCommandes.ItemsSource = Commande.commandes;
            }
        }
        private void dataGridCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridCommandes.SelectedItem is Commande selected)
            {
                DataContext = selected;
            }
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsViewAdmin());
        }
        private void BtnCuisiniers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CuisiniersViewAdmin());
        }
        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandesViewAdmin());
        }
        private void BtnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatistiquesViewAdmin());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }

    #endregion

    #region Page Statistiques Admin

    public partial class StatistiquesViewAdmin : Page
    {
        public StatistiquesViewAdmin()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsViewAdmin());
        }
        private void BtnCuisiniers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CuisiniersViewAdmin());
        }
        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandesViewAdmin());
        }
        private void BtnStatistiques_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StatistiquesViewAdmin());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDashboardView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }

    #endregion

    #region SessionManager
    public static class SessionManager
    {
        public static Utilisateur CurrentUser { get; set; }

        static SessionManager()
        {
            CurrentUser = null; // Important pour éviter des valeurs fantômes
        }

        public static bool IsLoggedIn()
        {
            if (CurrentUser == null)
            {
                return false;
            }
            return true;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
    #endregion

    #region Test
    public partial class Test : Page
    {
        public Test()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
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
            Utilisateur.RefreshList();
            Console.WriteLine(Utilisateur.clients[1].Avis_laisses[0].Commentaire);
        }

        private void Commander_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la page de commande
            MessageBox.Show("Page Commander à implémenter.");
        }

        private void SuiviCommandes_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la page de suivi
            MessageBox.Show("Page Suivi des commandes à implémenter.");
        }
        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Changer de mode (sombre / clair) à implémenter !");
        }
        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConnexionView());
        }
        private void BtnInscription_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Register());
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
    }
    #endregion

    #region Test2

    public partial class Test2 : Page
    {
        public Test2()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
        }
        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Changer de mode (sombre / clair) à implémenter !");
        }
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            UpdateNavButtons();
        }
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                NavigationService.GoForward();
            UpdateNavButtons();
        }
        private void BtnAffichage_Djikstra(object sender, RoutedEventArgs e)
        {
            int tempsTrajet = Program.GrapheMétro.AffichageDijkstra(Program.Stations[1], Program.Stations[100]);
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[100].Valeur.nom} : {tempsTrajet} minutes.");
        }

        private void BtnAffichage_BF(object sender, RoutedEventArgs e)
        {
            int tempsTrajet = Program.GrapheMétro.AffichageBellmanFord(Program.Stations[1], Program.Stations[100]);
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[100].Valeur.nom} : {tempsTrajet} minutes.");
        }
        private void BtnAffichage_Floyd(object sender, RoutedEventArgs e)
        {
            int tempsTrajet = Program.GrapheMétro.AffichageFloydWarshall(Program.Stations[1], Program.Stations[100]);
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[100].Valeur.nom} : {tempsTrajet} minutes.");
        }
        private void BtnCheminOptimal(object sender, RoutedEventArgs e)
        {
            List<Noeud<(int id, string nom)>> Liste = new List<Noeud<(int id, string nom)>>();
            Liste.Add(Program.Stations[1]);
            Liste.Add(Program.Stations[20]);
            Liste.Add(Program.Stations[40]);
            Liste.Add(Program.Stations[60]);
            Liste.Add(Program.Stations[80]);
            Liste.Add(Program.Stations[100]);
            Liste.Add(Program.Stations[120]);
            var programInstance = new Program();
            int tempsTrajet = programInstance.CheminOptimal(Program.GrapheMétro, Liste);
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[80].Valeur.nom} en passant par {Program.Stations[20].Valeur.nom}; {Program.Stations[40].Valeur.nom} ;{Program.Stations[60].Valeur.nom} est de : {tempsTrajet} minutes.");
        }
    }
    #endregion
}