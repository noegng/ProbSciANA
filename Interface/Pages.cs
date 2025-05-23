using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

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


        private void BtnTest_Click(object sender, RoutedEventArgs e)
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
            if (SessionManager.CurrentUser.EstCuisinier)
                NavigationService?.Navigate(new CuisinierDashboardView());
            else if (SessionManager.CurrentUser.EstClient)
                NavigationService?.Navigate(new UserDashboardView());
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
                var Station = await Noeud<(int id, string nom)>.TrouverStationLaPlusProche(adresse);
                if (!Program.Stations.Contains(Station))
                {
                    MessageBox.Show("Adresse non trouvée. Veuillez vérifier l'adresse saisie.");
                    return;
                }

                var nouvelUtilisateur = new Utilisateur(
                    estClient: role == "Client" || role == "Client et Cuisinier",
                    estCuisinier: role == "Cuisinier" || role == "Client et Cuisinier",
                    nom,
                    prenom,
                    adresse,
                    "", /// téléphone
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
                else if (role == "Client et Cuisinier")
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

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentUser.EstCuisinier)
            {
                NavigationService?.Navigate(new CuisinierDashboardView());
            }
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
            if (SessionManager.CurrentUser.EstClient)
            {
                NavigationService?.Navigate(new UserDashboardView());
            }
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
        public Dictionary<Ingredient, int> Ingredients_selectionnes { get; set; } = new();
        public AddPlat()
        {
            InitializeComponent();
            RefreshIngredients();
            //Loaded += (s, e) => UpdateNavButtons();
        }

        private void RefreshIngredients()
        {
            foreach (Ingredient i in Ingredient.ingredients)
            {
                Ingredients_selectionnes[i] = 0;
            }
            DataContext = null;
            DataContext = this;
        }
        private void BtnValiderPlat_Click(object sender, RoutedEventArgs e)
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
                new Cuisine(SessionManager.CurrentUser, nouveauPlat, false, DateTime.Now, "à faire");
                if (Ingredients_selectionnes != null && Ingredients_selectionnes.Count > 0)
                {
                    foreach (Ingredient i in Ingredients_selectionnes.Keys)
                    {
                        if (Ingredients_selectionnes[i] != 0)
                        {
                            new Compose(nouveauPlat, i, Ingredients_selectionnes[i]);
                        }
                    }
                }
                MessageBox.Show("Plat ajouté avec succès !");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout du plat : " + ex.Message);
            }
        }
        private void BtnAjouterAuPlat_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    Ingredient ingredient = (Ingredient)button.Tag;
                    Ingredients_selectionnes[ingredient]++;
                    DataContext = null;
                    DataContext = this;
                    CollectionViewSource.GetDefaultView(Ingredients_selectionnes).Refresh();
                }
                catch (InvalidCastException ex)
                {
                    MessageBox.Show($"Erreur de cast : {ex.Message}");
                }
            }
        }
        private void BtnAnnulerPanier_Click(object sender, RoutedEventArgs e)
        {
            Ingredients_selectionnes.Clear();
            RefreshIngredients();
            DataContext = null;
            DataContext = this;
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
            if (SessionManager.CurrentUser.EstClient)
            {
                NavigationService?.Navigate(new UserDashboardView());
            }
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

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentUser.EstCuisinier)
            {
                NavigationService?.Navigate(new CuisinierDashboardView());
            }
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
        public string? PanierSelectionne { get; set; }
        public Plat? SelectedPlat { get; set; }

        public double PrixTotalPanierSelectionne
        {
            get
            {
                double result = 0;
                if (PanierSelectionne != null)
                {
                    foreach (Plat p in Paniers[PanierSelectionne].plats.Keys)
                    {
                        result += p.Prix * Paniers[PanierSelectionne].plats[p];
                    }
                }
                return result;
            }
        }
        public double PrixTotal
        {
            get
            {
                double result = 0;
                foreach (string s in Paniers.Keys)
                {
                    foreach (Plat p in Paniers[s].plats.Keys)
                    {
                        result += p.Prix * Paniers[s].plats[p];
                    }
                }
                return result;
            }
        }
        public Dictionary<string, (Dictionary<Plat, int> plats, DateTime date)> Paniers { get; set; } = new();

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
            DataContext = null;
            DataContext = this;
        }

        private void RefreshPanier()
        {
            if (PanierSelectionne == null)
            {
                return;
            }
            foreach (Plat p in Plats)
            {
                Paniers[PanierSelectionne].plats[p] = 0;
            }
            DataContext = null;
            DataContext = this;
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
        private async void BtnAjouterPanier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AdresseInput.Text) || DateInput.SelectedDate == null)
            {
                MessageBox.Show("Renseigne l’adresse et la date.");
                return;
            }
            else
            {
                var adresse = AdresseInput.Text;
                var date = DateInput.SelectedDate.Value;
                var station = await Noeud<(int id, string nom)>.TrouverStationLaPlusProche(adresse);

                if (!Program.Stations.Contains(station))
                {
                    MessageBox.Show("Adresse non trouvée.");
                    return;
                }
                if (!Paniers.ContainsKey(adresse))
                {
                    Paniers.Add(adresse, (new Dictionary<Plat, int>(), date));
                    PanierSelectionne = adresse;
                    RefreshPanier();
                }

                else
                {
                    MessageBox.Show("Il existe déjà un panier pour cette adresse.");
                    return;
                }
            }
            // Réinitialise les champs et force l’UI à se rafraîchir
            AdresseInput.Text = "";
            DateInput.SelectedDate = null;
            DataContext = null;
            DataContext = this;
            CollectionViewSource.GetDefaultView(Paniers).Refresh();
        }
        private void BtnAjouterAuPanier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Plat plat)
            {
                if (PanierSelectionne == null)
                {
                    MessageBox.Show("Aucun panier sélectionné !");
                    return;
                }

                Paniers[PanierSelectionne].plats[plat]++;
                DataContext = null;
                DataContext = this;
            }
        }
        private void BtnValiderPanier_Click(object sender, RoutedEventArgs e)
        {
            if (Paniers != null && Paniers.Count > 0)
            {
                Commande c = new Commande("Commande " + (Commande.commandes.Count + 1), PrixTotal, SessionManager.CurrentUser.Id_utilisateur, Cuisinier.Id_utilisateur);
                foreach (string adresse in Paniers.Keys)
                {
                    if (Paniers[adresse].plats != null && Paniers[adresse].plats.Count > 0)
                    {
                        Livraison l = new Livraison(c, adresse, Paniers[adresse].date);

                        foreach (Plat p in Paniers[adresse].plats.Keys)
                        {
                            if (Paniers[adresse].plats[p] > 0)
                            {
                                new Requiert(p, l, Paniers[adresse].plats[p]);
                            }
                        }
                        Paniers.Remove(adresse);
                    }
                }
                CollectionViewSource.GetDefaultView(Paniers).Refresh();
                PanierSelectionne = null;
                DataContext = null;
                DataContext = this;
            }
        }
        private void BtnAnnulerPanier_Click(object sender, RoutedEventArgs e)
        {
            if (PanierSelectionne != null && Paniers.ContainsKey(PanierSelectionne))
            {
                Paniers.Remove(PanierSelectionne);
                CollectionViewSource.GetDefaultView(Paniers).Refresh();
                PanierSelectionne = null;
                DataContext = null;
                DataContext = this;
            }
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Expander exp && exp.DataContext is KeyValuePair<string, (Dictionary<Plat, int>, DateTime)> kvp)
            {
                PanierSelectionne = kvp.Key;
                DataContext = null;
                DataContext = this;
            }
        }
        private void ListePaniers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is KeyValuePair<string, (Dictionary<Plat, int>, DateTime)> kvp)
            {
                PanierSelectionne = kvp.Key;
                DataContext = null;
                DataContext = this;
            }
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentUser.EstCuisinier)
            {
                NavigationService?.Navigate(new CuisinierDashboardView());
            }
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
        public string cheminAcces = "Interface'\'Images'\'graphes'\'grapheChemin1.png";
        public List<Livraison> Livraisons_a_effectuer
        {
            get
            {
                List<Livraison> result = new();
                if (dataGridCommandes.SelectedItem is Commande selected)
                {
                    foreach (Livraison l in selected.Livraisons)
                    {
                        result.Add(l);
                    }
                }
                return result;
            }
        }
        public List<Plat> Plats_a_cuisiner
        {
            get
            {
                List<Plat> result = new();
                if (dataGridCommandes.SelectedItem is Commande selected)
                {
                    foreach (Livraison l in selected.Livraisons)
                    {
                        foreach (Requiert r in l.Requierts)
                        {
                            result.Add(r.Plat);
                        }
                    }
                }
                return result;
            }
        }

        public CommandeView()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateNavButtons();
            Commande.RefreshList();
            Livraison.RefreshList();
            Requiert.RefreshList();
            dataGridCommandes.ItemsSource = null;
            dataGridCommandes.ItemsSource = SessionManager.CurrentUser.Commandes_effectuees;
        }
        private void dataGridCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridCommandes.SelectedItem is Commande selected)
            {
                DataContext = this;
                ListLivraisons.ItemsSource = null;
                ListLivraisons.ItemsSource = Livraisons_a_effectuer;

                ListPlats.ItemsSource = null;
                ListPlats.ItemsSource = Plats_a_cuisiner;
            }
        }
        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridCommandes.SelectedItem is Commande selectedCommande && selectedCommande.Statut == "en attente")
            {
                selectedCommande.Statut = "en cours";
                dataGridCommandes.ItemsSource = null;
                dataGridCommandes.ItemsSource = SessionManager.CurrentUser.Commandes_effectuees;
            }
        }
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridCommandes.SelectedItem is Commande selectedCommande && selectedCommande.Statut == "en attente")
            {
                selectedCommande.Delete();
                dataGridCommandes.ItemsSource = null;
                dataGridCommandes.ItemsSource = SessionManager.CurrentUser.Commandes_effectuees;
            }
        }

        private void AfficherPlats_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PlatView());
        }

        private void OnListLivraisons_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListLivraisons.SelectedItem != null)
            {
                ListPlats.SelectedItem = null;
            }
        }
        private void OnListPlats_Selected(object s, SelectionChangedEventArgs e)
        {
            if (ListPlats.SelectedItem != null)
            {
                ListLivraisons.SelectedItem = null;
            }
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentUser.EstClient)
            {
                NavigationService?.Navigate(new UserDashboardView());
            }
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
        private async void BtnAffichage_Djikstra(object sender, RoutedEventArgs e)
        {
            Noeud<(int id, string nom)> stationArrivee = null;
            if (ListLivraisons.SelectedItem is Livraison selectedLivraison)
            {
                stationArrivee = await Program.GetNoeud(selectedLivraison.Adresse);
            }
            else if (dataGridCommandes.SelectedItem is Commande selected)
            {
                stationArrivee = selected.Client.Station;
            }
            (int tempsTrajet, cheminAcces) = Program.GrapheMétro.AffichageDijkstra(SessionManager.CurrentUser.Station, stationArrivee);
            MessageBox.Show($"Temps de trajet entre {SessionManager.CurrentUser.Station.Valeur.nom} et {stationArrivee.Valeur.nom} : {tempsTrajet} minutes.");
        }
        private async void BtnCheminOptimal(object sender, RoutedEventArgs e)
        {
            if (dataGridCommandes.SelectedItem is Commande selected)
            {
                var listeLieux = new List<Noeud<(int id, string nom)>> { SessionManager.CurrentUser.Station };
                foreach (var livraison in selected.Livraisons)
                {
                    var noeud = await Program.GetNoeud(livraison.Adresse);
                    listeLieux.Add(noeud);
                }
                (int tempsTrajet, cheminAcces) = Program.GrapheMétro.AffichageCheminOptimal(listeLieux);
                CollectionViewSource.GetDefaultView(cheminAcces).Refresh();
                string listStations = "";
                foreach (var station in listeLieux)
                {
                    listStations += station.Valeur.nom + ", ";
                }
                MessageBox.Show($"Temps de trajet depuis {SessionManager.CurrentUser.Station.Valeur.nom} vers {listStations} est de : {tempsTrajet} minutes.");
            }
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

        private void AfficherCommandes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CommandeView());
        }

        private void UpdateNavButtons()
        {
            BtnBack.IsEnabled = NavigationService?.CanGoBack == true;
            BtnForward.IsEnabled = NavigationService?.CanGoForward == true;
        }
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentUser.EstClient)
            {
                NavigationService?.Navigate(new UserDashboardView());
            }
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
        private void BtnStat_Click(object sender, RoutedEventArgs e)
        {
            Utilisateur.RefreshList();
            Commande.RefreshList();
            double prixMoyenParCommande = 0;
            foreach (var commande in Commande.commandes)
            {
                prixMoyenParCommande += commande.Prix;
            }
            prixMoyenParCommande = (int)prixMoyenParCommande * 1000 / 1000;  ///Pour arrondire au 0,001 près
            string commandeParCuisinier = "";
            foreach (var cuisinier in Utilisateur.cuisiniers)
            {
                commandeParCuisinier += cuisinier.Prenom + " " + cuisinier.Nom + " : ";
                string s = "";
                foreach (var commande in cuisinier.Commandes_effectuees)
                {
                    if (!s.Contains(commande.NomClient))
                    {
                        s += commande.NomClient + ", ";
                    }
                }
                commandeParCuisinier += s + "\n";
            }
            string pxMoyen = "Le prix moyen par commande est de : " + prixMoyenParCommande / Convert.ToDouble(Commande.commandes.Count());
            string nbCuisinier = "Nombre de cuisinier : " + Utilisateur.cuisiniers.Count();
            string nbClient = "Nombre de client : " + Utilisateur.clients.Count();
            MessageBox.Show($"Le nombre totale de commande est de : {Commande.commandes.Count()}\n{pxMoyen}\n{nbCuisinier}\n{nbClient}\nListe des cuisiniers avec les personnes qui leur ont fait une commande :\n{commandeParCuisinier}");

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
        private void BtnStat_Click(object sender, RoutedEventArgs e)
        {
            Utilisateur.RefreshList();
            Commande.RefreshList();
            double prixMoyenParCommande = 0;
            foreach (var commande in Commande.commandes)
            {
                prixMoyenParCommande += commande.Prix;
            }
            prixMoyenParCommande = (int)prixMoyenParCommande * 1000 / 1000;  ///Pour arrondire au 0,001 près
            string commandeParCuisinier = "";
            foreach (var cuisinier in Utilisateur.cuisiniers)
            {
                commandeParCuisinier += cuisinier.Prenom + " " + cuisinier.Nom + " : ";
                string s = "";
                foreach (var commande in cuisinier.Commandes_effectuees)
                {
                    if (!s.Contains(commande.NomClient))
                    {
                        s += commande.NomClient + ", ";
                    }
                }
                commandeParCuisinier += s + "\n";
            }
            string pxMoyen = "Le prix moyen par commande est de : " + prixMoyenParCommande / Convert.ToDouble(Commande.commandes.Count());
            string nbCuisinier = "Nombre de cuisinier : " + Utilisateur.cuisiniers.Count();
            string nbClient = "Nombre de client : " + Utilisateur.clients.Count();
            MessageBox.Show($"Le nombre totale de commande est de : {Commande.commandes.Count()}\n{pxMoyen}\n{nbCuisinier}\n{nbClient}\nListe des cuisiniers avec les personnes qui leur ont fait une commande :\n{commandeParCuisinier}");

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
        //public object SelectedElement { get; set; }

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
            // Masquer la fiche client, afficher le bandeau
            FicheCuisinier.Visibility = Visibility.Collapsed;
            AddPane.Visibility = Visibility.Visible;

            // Réinitialiser les champs
            TxtPrenom.Text = TxtNom.Text = TxtAdresse.Text = TxtTel.Text = TxtEmail.Text = "";
        }
        private void BtnAnnulerAjout_Click(object sender, RoutedEventArgs e)
        {
            AddPane.Visibility = Visibility.Collapsed;
            FicheCuisinier.Visibility = Visibility.Visible;
        }
        private async void BtnValiderAjout_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPrenom.Text) ||
                string.IsNullOrWhiteSpace(TxtNom.Text) ||
                string.IsNullOrWhiteSpace(TxtAdresse.Text) ||
                string.IsNullOrWhiteSpace(TxtTel.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("Tous les champs obligatoires doivent être remplis.");
                return;
            }

            try
            {
                var station = await Noeud<(int id, string nom)>.TrouverStationLaPlusProche(TxtAdresse.Text);

                var nouveauClient = new Utilisateur(
                    estClient: false,
                    estCuisinier: true,
                    nom: TxtNom.Text,
                    prenom: TxtPrenom.Text,
                    adresse: TxtAdresse.Text,
                    telephone: TxtTel.Text,
                    email: TxtEmail.Text,
                    mdp: "mdp1234",
                    station: station);

                dataGridCuisiniers.ItemsSource = null;
                dataGridCuisiniers.ItemsSource = Utilisateur.cuisiniers;

                AddPane.Visibility = Visibility.Collapsed;
                FicheCuisinier.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout : " + ex.Message);
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
        private void BtnStat_Click(object sender, RoutedEventArgs e)
        {
            Utilisateur.RefreshList();
            Commande.RefreshList();
            double prixMoyenParCommande = 0;
            foreach (var commande in Commande.commandes)
            {
                prixMoyenParCommande += commande.Prix;
            }
            prixMoyenParCommande = (int)prixMoyenParCommande * 1000 / 1000;  ///Pour arrondire au 0,001 près
            string commandeParCuisinier = "";
            foreach (var cuisinier in Utilisateur.cuisiniers)
            {
                commandeParCuisinier += cuisinier.Prenom + " " + cuisinier.Nom + " : ";
                string s = "";
                foreach (var commande in cuisinier.Commandes_effectuees)
                {
                    if (!s.Contains(commande.NomClient))
                    {
                        s += commande.NomClient + ", ";
                    }
                }
                commandeParCuisinier += s + "\n";
            }
            string pxMoyen = "Le prix moyen par commande est de : " + prixMoyenParCommande / Convert.ToDouble(Commande.commandes.Count());
            string nbCuisinier = "Nombre de cuisinier : " + Utilisateur.cuisiniers.Count();
            string nbClient = "Nombre de client : " + Utilisateur.clients.Count();
            MessageBox.Show($"Le nombre totale de commande est de : {Commande.commandes.Count()}\n{pxMoyen}\n{nbCuisinier}\n{nbClient}\nListe des cuisiniers avec les personnes qui leur ont fait une commande :\n{commandeParCuisinier}");

        }
    }

    #endregion

    #region Page Commandes Admin
    public partial class CommandesViewAdmin : Page
    {
        public double Moyenne
        {
            get
            {
                double result = 0;
                foreach (Commande c in Commande.commandes)
                {
                    result += c.Prix;
                }
                return result;
            }
        }
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
        private void BtnStat_Click(object sender, RoutedEventArgs e)
        {
            Utilisateur.RefreshList();
            Commande.RefreshList();
            double prixMoyenParCommande = 0;
            foreach (var commande in Commande.commandes)
            {
                prixMoyenParCommande += commande.Prix;
            }
            prixMoyenParCommande = (int)prixMoyenParCommande * 1000 / 1000;  ///Pour arrondire au 0,001 près
            string commandeParCuisinier = "";
            foreach (var cuisinier in Utilisateur.cuisiniers)
            {
                commandeParCuisinier += cuisinier.Prenom + " " + cuisinier.Nom + " : ";
                string s = "";
                foreach (var commande in cuisinier.Commandes_effectuees)
                {
                    if (!s.Contains(commande.NomClient))
                    {
                        s += commande.NomClient + ", ";
                    }
                }
                commandeParCuisinier += s + "\n";
            }
            string pxMoyen = "Le prix moyen par commande est de : " + prixMoyenParCommande / Convert.ToDouble(Commande.commandes.Count());
            string nbCuisinier = "Nombre de cuisinier : " + Utilisateur.cuisiniers.Count();
            string nbClient = "Nombre de client : " + Utilisateur.clients.Count();
            MessageBox.Show($"Le nombre totale de commande est de : {Commande.commandes.Count()}\n{pxMoyen}\n{nbCuisinier}\n{nbClient}\nListe des cuisiniers avec les personnes qui leur ont fait une commande :\n{commandeParCuisinier}");

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
        Graphe<Utilisateur> graphU = Program.CreationGrapheU();

        public Test()
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
        private void BtnGraphMétro(object sender, RoutedEventArgs e)
        {
            Program.GrapheMétro.AffichageGrapheOrienté();
        }
        private void BtnGraphMétroAncien(object sender, RoutedEventArgs e)
        {
            Program.GrapheMétro.AffichageAncienGraphe();
        }

        private void BtnAffichage_Djikstra(object sender, RoutedEventArgs e)
        {
            (int tempsTrajet, string cheminAcces) = Program.GrapheMétro.AffichageDijkstra(Program.Stations[1], Program.Stations[100]);
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[100].Valeur.nom} : {tempsTrajet} minutes.");
        }

        private void BtnAffichage_BF(object sender, RoutedEventArgs e)
        {
            (int tempsTrajet, string cheminAcces) = Program.GrapheMétro.AffichageBellmanFord(Program.Stations[1], Program.Stations[100]);
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[100].Valeur.nom} : {tempsTrajet} minutes.");
        }
        private void BtnAffichage_Floyd(object sender, RoutedEventArgs e)
        {
            (int tempsTrajet, string cheminAcces) = Program.GrapheMétro.AffichageFloydWarshall(Program.Stations[1], Program.Stations[100]);
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[100].Valeur.nom} : {tempsTrajet} minutes.");

        }
        private void BtnCheminOptimal(object sender, RoutedEventArgs e)
        {
            List<Noeud<(int id, string nom)>> Liste =
            [
                Program.Stations[1],
                Program.Stations[20],
                Program.Stations[40],
                Program.Stations[60],
                Program.Stations[80],
                Program.Stations[100],
                Program.Stations[120],
            ];
            int tempsTrajet = Program.GrapheMétro.AffichageCheminOptimal(Liste).tempsMinimal;
            MessageBox.Show($"Temps de trajet entre {Program.Stations[1].Valeur.nom} et {Program.Stations[120].Valeur.nom} en passant par {Program.Stations[20].Valeur.nom}; {Program.Stations[40].Valeur.nom} ;{Program.Stations[60].Valeur.nom}; {Program.Stations[80].Valeur.nom}; {Program.Stations[100].Valeur.nom} est de : {tempsTrajet} minutes.");
        }
        private void BtnTri(object sender, RoutedEventArgs e)
        {
            /*
            var A = new Noeud<string>("A", 1);
            var B = new Noeud<string>("B", 2);
            var C = new Noeud<string>("C", 3);
            var D = new Noeud<string>("D", 4);
            var E = new Noeud<string>("E", 5);
            var F = new Noeud<string>("F", 6);

            var arcsTest = new List<Arc<string>>
            {
                new Arc<string>(A, B),
                new Arc<string>(A, C),
                new Arc<string>(A, D),
                new Arc<string>(A, E),
                new Arc<string>(A, F), // A a 5 voisins (B, C, D, E, F)

                new Arc<string>(B, C),
                new Arc<string>(B, D),
                new Arc<string>(B, E), // B a 4 voisins (A, C, D, E)

                new Arc<string>(C, E), // C a 3 voisins (A, B, E)

                new Arc<string>(D, E)  // D a 2 voisins (A, B), E a 1 voisin (A)
            };
            Graphe<string> Test = new Graphe<string>(arcsTest);
            */
            var trié = graphU.TriListeAdjacence();
            foreach (var a in trié)
            {
                Console.WriteLine(a.noeud + " : " + a.successeur.Count);
            }
        }
        private void BtnAffichageColoriationDeGraph(object sender, RoutedEventArgs e)
        {
            int couleurMin = graphU.WelshPowell();
            graphU.AffichageGrapheNonOrienté();
            MessageBox.Show($"Coloration du graphe avec {couleurMin} couleurs.");
        }
        private void BtnPropriétéGraphe(object sender, RoutedEventArgs e)
        {
            /// Afficher les propriétés
            string propriétés = graphU.PropriétésGraphe();
            MessageBox.Show(propriétés);
        }
        private void Serialisation(object sender, RoutedEventArgs e)
        {
            /// Sérialiser le graphe en JSON et XML
            graphU.ExporterVersJSON(graphU, "donnees_graphe.json");
            graphU.ExporterVersXML(graphU, "donnees_graphe.xml");
            MessageBox.Show("Sérialisation en JSON et XML terminée.");
        }
    }
    #endregion
}