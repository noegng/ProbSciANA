using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;

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
            Requetes.GetUtilisateurs();
            utilisateurs = new Dictionary<string, Utilisateur>();

            foreach (var utilisateur in Requetes.utilisateurs)
            {
                string display = utilisateur.Prenom + " " + utilisateur.Nom;
                utilisateurs[display] = utilisateur;
                UserComboBox.Items.Add(display);
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

            if (utilisateurs.TryGetValue(nomUtilisateur, out var utilisateur)
                && motDePasseEntre == utilisateur.Mdp)
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
        
private List<Livraison> livraisons;

        public CuisinierDashboardView()
        {
            InitializeComponent();
            LoadLivraisons();
        }
        private void LoadLivraisons()
    {
        // Exemple de données de livraison
        livraisons = new List<Livraison>
        {
            new Livraison
            {
                NomPlat = "Pizza",
                NomClient = "Jean Dupont",
                AdresseLivraison = "123 Rue de Paris",
                IdStationDepart = Noeud<(int id, string nom)>,
                IdStationArrivee = 5
            },
            new Livraison
            {
                NomPlat = "Sushi",
                NomClient = "Marie Curie",
                AdresseLivraison = "456 Avenue Einstein",
                IdStationDepart = 2,
                IdStationArrivee = 8
            },
            new Livraison
            {
                NomPlat = "Burger",
                NomClient = "Albert Einstein",
                AdresseLivraison = "789 Boulevard Newton",
                IdStationDepart = 3,
                IdStationArrivee = 10
            }
        };

        // Lier la liste à une ListView (si nécessaire)
        LivraisonsListView.ItemsSource = livraisons;
    }
      
        private void AjouterPlat_Click(object sender, RoutedEventArgs e)
        {
            /// Logique pour ajouter un plat
            MessageBox.Show("Ajouter un plat");
        }

        private void BtnLivrer(object sender, RoutedEventArgs e)
        {
            string excelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx"; // Chemin vers le fichier Excel
            var graphe = Program<(int id, string nom)>.InitializeData(excelFilePath);
            if (LivraisonsListView.SelectedItem is Livraison livraison)
            {
                /// Logique pour livrer le plat
                MessageBox.Show($"Livraison de {livraison.NomPlat} à {livraison.NomClient}");
                graphe.DijkstraChemin(livraison.IdStationDepart, livraison.IdStationArrivee);
                
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une livraison.");
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
            Requetes.GetUtilisateurs();
            LoadClients();
        }

        private void LoadClients(string orderBy = "u.nom")
        {
           
        /// Récupérer tous les clients
    clients = Requetes.utilisateurs.FindAll(u => u.EstClient);

    /// Appliquer le tri en fonction de `orderBy`
    switch (orderBy)
    {
        case "u.nom":
            clients.Sort((a, b) => a.Nom.CompareTo(b.Nom));
            break;

        case "u.adresse":
            clients.Sort((a, b) => a.Adresse.CompareTo(b.Adresse));
            break;

        case "achats":
            /// Récupérer les clients triés par leurs achats
            Requetes.GetClientsByAchats(orderBy);
            clients.Sort((a, b) =>
            {
                double achatsA = Requetes.clients.ContainsKey(a) ? Requetes.clients[a] : 0;
                double achatsB = Requetes.clients.ContainsKey(b) ? Requetes.clients[b] : 0;
                return achatsB.CompareTo(achatsA); /// Tri décroissant par montant des achats
            });
            break;

        default:
            break;
    }

    /// Mettre à jour la source de données de la ListView
    ClientsListView.ItemsSource = null;
    ClientsListView.ItemsSource = clients;
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsListView.SelectedItem is Utilisateur client)
            {
                client.EstClient = false; /// Déclenche suppression automatique
                LoadClients();
            }
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsListView.SelectedItem is Utilisateur client)
            {
                /// Exemple : mise à jour du nom pour test
                client.Nom = client.Nom + " (modifié)";
                LoadClients();
            }
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginView());
        }

        private void BtnTrierNom_Click(object sender, RoutedEventArgs e) => LoadClients("u.nom");
        private void BtnTrierAdresse_Click(object sender, RoutedEventArgs e) => LoadClients("u.adresse");

        private void BtnTrierAchats_Click(object sender, RoutedEventArgs e) => LoadClients("achats");
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
            Requetes.GetUtilisateurs();
            LoadCuisiniers();
        }

        private void LoadCuisiniers(string orderBy = "u.nom")
        {
            cuisiniers = Requetes.utilisateurs.FindAll(u => u.EstCuisinier);
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
            if (CuisiniersListView.SelectedItem is Utilisateur cuisinier)
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



   

}