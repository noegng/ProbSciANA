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
        SqlQueries.SqlAddUser(Program.ConnectionString, nom, prenom, email, adresse, role, mdp);
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

#endregion

#region Page Connexion
public partial class ConnexionView : Page
{
private Dictionary<string, (int id, string motDePasse, string role)> utilisateurs;

    public ConnexionView()
    {
        InitializeComponent();
        utilisateurs = SqlQueries.ChargerUtilisateurs(Program.ConnectionString);
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

        public ObservableCollection<Livraison> Livraisons { get; set; }
        public ICommand LivrerCommandeCommand { get; }

        public CuisinierDashboardView()
        {
            InitializeComponent();

             Livraisons = new ObservableCollection<Livraison>
            {
                new Livraison { NomPlat = "Pizza", NomClient = "Jean Dupont", AdresseLivraison = "123 Rue de Paris" },
                new Livraison { NomPlat = "Sushi", NomClient = "Marie Curie", AdresseLivraison = "456 Avenue Einstein" },
                new Livraison { NomPlat = "Burger", NomClient = "Albert Einstein", AdresseLivraison = "789 Boulevard Newton" }
            };

            LivrerCommandeCommand = new RelayCommand<Livraison>(LivrerCommande);
            DataContext = this;
        }

        private void LivrerCommande(Livraison livraison)
        {
            if (livraison == null) return;

            // Appeler votre algorithme de plus court chemin ici
            MessageBox.Show($"Calcul du trajet pour livrer {livraison.NomPlat} à {livraison.NomClient} ({livraison.AdresseLivraison})");

          
            Program.AffichageImage(Program.Stations,Program.Aretes);
        }
        private void AjouterPlat_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour ajouter un plat
            MessageBox.Show("Ajouter un plat");
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

        private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StartView());
        }
    }
#endregion

#region Page Gestion Clients (admin)
    public partial class ClientsView : Page
    {
    public ClientsView()
    {
        InitializeComponent();
        LoadClients();
    }

    private void LoadClients(string orderBy = "nom")
    {
        var clients = SqlQueries.GetAllClients(Program.ConnectionString, orderBy);
        ClientsListView.ItemsSource = clients;

    }

    private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
    {
        if (ClientsListView.SelectedItem is Client client)
    {
        SqlQueries.DeleteClient(Program.ConnectionString, client.Id);
        LoadClients();
    }
    }

    private void BtnModifier_Click(object sender, RoutedEventArgs e)
    {
        
        if (ClientsListView.SelectedItem is Client client)
    {
        SqlQueries.UpdateClient(Program.ConnectionString, client.Id, client.Nom, client.Prenom, client.Email, client.Adresse);
        LoadClients();
    }
    }

    private void BtnAjouter_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new LoginView());
    }

    private void BtnTrierNom_Click(object sender, RoutedEventArgs e) => LoadClients("nom");
    private void BtnTrierAdresse_Click(object sender, RoutedEventArgs e) => LoadClients("adresse");

    private void BtnRetourAccueil_Click(object sender, RoutedEventArgs e)
{
    NavigationService?.Navigate(new StartView());
}
}

#endregion

#region Page Gestion Cuisiniers (admin)
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

public class Livraison
    {
        public string NomPlat { get; set; }
        public string NomClient { get; set; }
        public string AdresseLivraison { get; set; }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

}