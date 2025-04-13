using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProbSciANA
{
    public static class Requetes
    {
        public static string connectionString = "SERVER=localhost;PORT=3306;user=root;password=root;database=pbsciana;";
        //Program.IntializeData();
        //public static Graphe<(int id, string nom)> graphe = new Graphe<(int id, string nom)>(Program.Arcs);
        public static List<Utilisateur> GetClientsByAchat() //Trie les utilisateurs par nombre d'achats
        {
            Commande.Refreshes();
            List<Utilisateur> clients = new List<Utilisateur>();
            foreach(Commande commande in Commande.commandes)
            {
                if(commande.Client != null && !clients.Contains(commande.Client))
                {
                    clients.Add(commande.Client);
                }
            }
            Utilisateur.Refreshes();
            foreach(Utilisateur u in Utilisateur.utilisateurs)
            {
                if(!clients.Contains(u) && u.EstClient)
                {
                    clients.Add(u);
                }
            }
            return clients;
        }
    }
    public class Utilisateur
    {
        public static List<Utilisateur> utilisateurs = new List<Utilisateur>();
        private int id_utilisateur;
        private bool estClient;
        private bool estCuisinier;
        private bool estEntreprise;
        private string nom = null;
        private string prenom = null;
        private string adresse;
        private string telephone;
        private string email;
        private Noeud<(int id,string nom)> station;
        private DateTime date_inscription;
        private string mdp;
        private string nom_referent;
        public Utilisateur(int id_utilisateur)
        {
            this.id_utilisateur = id_utilisateur;
            Refresh();
        }
        public Utilisateur(bool estClient, bool estCuisinier, bool estEntreprise, string nom, string prenom, string adresse, string telephone, string email, Noeud<(int id, string nom)> station, string mdp, string nom_referent="")
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string queryInsert = @"INSERT INTO Utilisateur (nom, prenom, adresse, telephone, email, station, mdp)
                                VALUES (@nom, @prenom, @adresse, @telephone, @email, @station, @mdp);";
                
                //station = Graphe.getStation(adresse, graphe); 

                using (MySqlCommand command = new MySqlCommand(queryInsert, connection))
                {
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@prenom", prenom);
                    command.Parameters.AddWithValue("@adresse", adresse);
                    command.Parameters.AddWithValue("@telephone", telephone);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@station", station.Valeur.nom);
                    command.Parameters.AddWithValue("@mdp", mdp);
                    command.ExecuteNonQuery();
                }
                string queryGetId = "SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(queryGetId, connection))
                {
                    id_utilisateur = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
                connection.Close();
            }
            this.station = station;
            this.estClient = estClient;
            this.estCuisinier = estCuisinier;
            this.estEntreprise = estEntreprise;
            this.nom_referent = nom_referent;
            InsertMaj();
            Refresh();
        }

        #region GetSet
        public int Id_utilisateur
        {
            get { return id_utilisateur; }
        }
        public bool EstClient
        {
            get { return estClient; }
            set 
            { 
                if(value && !estClient)
                {
                    estClient = value; InsertMaj();
                }
                if(!value && estClient)
                {
                    estClient = value; DeleteMaj();
                }
            }
        }
        public bool EstCuisinier
        {
            get { return estCuisinier; }
            set 
            {
                if(value && !estCuisinier)
                {
                    estCuisinier = value; InsertMaj();
                }
                if(!value && estCuisinier)
                {
                    estCuisinier = value; DeleteMaj();
                }
            }
        }
        public bool EstEntreprise
        {
            get { return estEntreprise; }
            set 
            {
                if(value && !estEntreprise)
                {
                    estEntreprise = value; InsertMaj();
                }
                if(!value && estEntreprise)
                {
                    estEntreprise = value; DeleteMaj(); nom_referent = null;
                }
            }
        }
        public string Nom
        {
            get { return nom; }
            set { nom = value; Update("nom", value); }
        }
        public string Prenom
        {
            get { return prenom; }
            set { prenom = value; Update("prenom", value); }
        }
        public string Adresse
        {
            get { return adresse; }
            set { adresse = value; Update("adresse", value); }
        }
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; Update("telephone", value); }
        }
        public string Email
        {
            get { return email; }
            set { email = value; Update("email", value); }
        }
        public Noeud<(int id, string nom)> Station
        {
            get { return station; }
            set { station = value; Update("station", value.Valeur.nom); }
        }
        public DateTime Date_inscription
        {
            get { return date_inscription; }
            set { date_inscription = value; Update("date_inscription", value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }
        public string Mdp
        {
            get { return mdp; }
            set { mdp = value; Update("mdp", value); }
        }
        public string Nom_referent
        {
            get { return nom_referent; }
            set { nom_referent = value; Update("nom_referent", value); }
        }
        #endregion
        private void InsertMaj()
        {
            if (estClient)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Client_ (id_utilisateur)
                                    VALUES (@id_utilisateur);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }

            if (estCuisinier)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Cuisinier (id_utilisateur)
                                    VALUES (@id_utilisateur);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }

            if (!estEntreprise)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Particulier (id_utilisateur)
                                    VALUES (@id_utilisateur);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }

            if (estEntreprise)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Entreprise (id_utilisateur, nom_referent)
                                    VALUES (@id_utilisateur, @nom_referent);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        command.Parameters.AddWithValue("@nom_referent", nom_referent);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
        }
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Utilisateur SET {champ} = @valeur WHERE id_utilisateur = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id", id_utilisateur);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Utilisateur WHERE id_utilisateur = @id_utilisateur;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteMaj()
        {
            if(!estClient)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Client_ WHERE id_utilisateur = @id_utilisateur;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if(!estCuisinier)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Cuisinier WHERE id_utilisateur = @id_utilisateur;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if(!estEntreprise)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Entreprise WHERE id_utilisateur = @id_utilisateur;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if(estEntreprise)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Particuulier WHERE id_utilisateur = @id_utilisateur;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public void Refresh() // Refreshes the information of the utilisateur
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT u.*, " +
                               "CASE WHEN cl.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estClient," +
                               "CASE WHEN cu.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estCuisinier, " +
                               "CASE WHEN e.id_utilisateur IS NOT NULL THEN TRUE ELSE FALSE END AS estEntreprise, " +
                               "e.nom_referent " +
                               "FROM Utilisateur u " +
                               "LEFT JOIN Client_ cl ON cl.id_utilisateur = u.id_utilisateur " +
                               "LEFT JOIN Cuisinier cu ON cu.id_utilisateur = u.id_utilisateur " +
                               "LEFT JOIN Entreprise e ON e.id_utilisateur = u.id_utilisateur " +
                               "WHERE u.id_utilisateur = @id_utilisateur;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            nom = reader.GetString("nom");
                            prenom = reader.GetString("prenom");
                            adresse = reader.GetString("adresse");
                            telephone = reader.GetString("telephone");
                            email = reader.GetString("email");
                            for(int i = 0; i < Program.Stations.Count; i++)
                            {
                                if (Program.Stations[i].Valeur.nom == reader.GetString("station"))
                                {
                                    station = Program.Stations[i];
                                }
                            }
                            date_inscription = reader.GetDateTime("date_inscription");
                            mdp = reader.GetString("mdp");
                            estClient = reader.GetBoolean("estClient");
                            estCuisinier = reader.GetBoolean("estCuisinier");
                            estEntreprise = reader.GetBoolean("estEntreprise");
                            nom_referent = reader.GetString("nom_referent");
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of utilisateurs
        {
            utilisateurs.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_utilisateur FROM Utilisateur;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            utilisateurs.Add(new Utilisateur(reader.GetInt32("id_utilisateur")));
                        }
                    }
                }
                connection.Close();
            }
        }
    }

    public class Commande
    {
        public static List<Commande> commandes = new List<Commande>();
        private int id_commande;
        private string nom;
        private double prix;
        private string statut;
        private DateTime date_commande;
        private Utilisateur client;
        private Utilisateur cuisinier;
        public Commande(int id_commande)
        {
            this.id_commande = id_commande;
            Refresh();
        }
        public Commande(string nom, double prix, string statut, int id_client, int id_cuisinier)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Commande (nom, prix, statut, id_client, id_cuisinier)
                                VALUES (@nom, @prix, @statut, @id_client, @id_cuisinier);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@prix", prix);
                    command.Parameters.AddWithValue("@statut", statut);
                    command.Parameters.AddWithValue("@id_client", id_client);
                    command.Parameters.AddWithValue("@id_cuisinier", id_cuisinier);
                    command.ExecuteNonQuery();
                }
                string queryGetId = "SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(queryGetId, connection))
                {
                    id_commande = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
                connection.Close();
            }
            Refresh();
        }

        #region GetSet
        public int Id_commande
        {
            get { return id_commande; }
        }
        public string Nom
        {
            get { return nom; }
            set { nom = value; Update("nom", value); }
        }
        public double Prix
        {
            get { return prix; }
            set { prix = value; Update("prix", value.ToString()); }
        }
        public string Statut
        {
            get { return statut; }
            set { statut = value; Update("statut", value); }
        }
        public DateTime Date_commande
        {
            get { return date_commande; }
            set { date_commande = value; Update("date_commande", value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }
        public Utilisateur Client
        {
            get { return client; }
            set { client = value; Update("id_client", value.Id_utilisateur.ToString()); }
        }
        public Utilisateur Cuisinier
        {
            get { return cuisinier; }
            set { cuisinier = value; Update("id_cuisinier", value.Id_utilisateur.ToString()); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Commande SET {champ} = @valeur WHERE id_commande = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id", id_commande);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Commande WHERE id_commande = @id_commande;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_commande", id_commande);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the commande
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Commande WHERE id_commande = @id_commande;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_commande", id_commande);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            nom = reader.GetString("nom");
                            prix = reader.GetDouble("prix");
                            statut = reader.GetString("statut");
                            date_commande = reader.GetDateTime("date_commande");
                            client = new Utilisateur(reader.GetInt32("id_client"));
                            cuisinier = new Utilisateur(reader.GetInt32("id_cuisinier"));
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of commandes
        {
            commandes.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_commande FROM Commande;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            commandes.Add(new Commande(reader.GetInt32("id_commande")));
                        }
                    }
                }
                connection.Close();
            }
        }
    }

    public class Livraison
    {
        public static List<Livraison> livraisons = new List<Livraison>();
        private int id_livraison;
        private DateTime date_livraison;
        private string statut;
        private Utilisateur cuisinier;
        private Commande commande;
        
        public Livraison(int id_livraison)
        {
            this.id_livraison = id_livraison;
            Refresh();
        }

        public Livraison(DateTime date_livraison, string statut, Utilisateur cuisinier, Commande commande)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Livraison (date_livraison, statut, id_utilisateur, id_commande)
                                VALUES (@date_livraison, @statut, @id_utilisateur, @id_commande);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@date_livraison", date_livraison.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@statut", statut);
                    command.Parameters.AddWithValue("@id_utilisateur", cuisinier.Id_utilisateur);
                    command.Parameters.AddWithValue("@id_commande", commande.Id_commande);
                    command.ExecuteNonQuery();
                }
                string queryGetId = "SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(queryGetId, connection))
                {
                    id_livraison = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
                connection.Close();
            }
            this.date_livraison = date_livraison;
            this.statut = statut;
            this.cuisinier = cuisinier;
            this.commande = commande;
        }

        #region GetSet
        public int Id_livraison
        {
            get { return id_livraison; }
        }
        public DateTime Date_livraison
        {
            get { return date_livraison; }
            set { date_livraison = value; Update("date_livraison", value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }
        public string Statut
        {
            get { return statut; }
            set { statut = value; Update("statut", value); }
        }
        public Utilisateur Cuisinier
        {
            get { return cuisinier; }
            set { cuisinier = value; Update("id_trajet", value.Id_utilisateur.ToString()); }
        }
        public Commande Commande
        {
            get { return commande; }
            set { commande = value; Update("id_commande", value.Id_commande.ToString()); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Livraison SET {champ} = @valeur WHERE id_livraison = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id", id_livraison);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Livraison WHERE id_livraison = @id_livraison;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_livraison", id_livraison);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the livraison
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Livraison WHERE id_livraison = @id_livraison;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_livraison", id_livraison);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            date_livraison = reader.GetDateTime("date_livraison");
                            statut = reader.GetString("statut");
                            cuisinier = new Utilisateur(reader.GetInt32("id_utilisateur"));
                            commande = new Commande(reader.GetInt32("id_commande"));
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of livraisons
        {
            livraisons.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_livraison FROM Livraison;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            livraisons.Add(new Livraison(reader.GetInt32("id_livraison")));
                        }
                    }
                }
                connection.Close();
            }
        }
    }

    public class Plat
    {
        public static List<Plat> plats = new List<Plat>();
        private int id_plat;
        private string nom;
        private double prix;
        private int nbPortions;
        private string type;
        private string regime;
        private string nationalite;
        private DateTime date_peremption;
        private string photo;
        
        public Plat(int id_plat)
        {
            this.id_plat = id_plat;
            Refresh();
        }
        public Plat(string nom, double prix, int nbPortions, string type, string regime, string nationalite, DateTime date_peremption, string photo)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Plat (nom, prix, nbPortions, type, regime, nationalite, date_peremption, photo)
                                VALUES (@nom, @prix, @nbPortions, @type, @regime, @nationalite, @date_peremption, @photo);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@prix", prix);
                    command.Parameters.AddWithValue("@nbPortions", nbPortions);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@regime", regime);
                    command.Parameters.AddWithValue("@nationalite", nationalite);
                    command.Parameters.AddWithValue("@date_peremption", date_peremption.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@photo", photo);
                    command.ExecuteNonQuery();
                }
                string queryGetId = "SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(queryGetId, connection))
                {
                    id_plat = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
                connection.Close();
            }
            this.nom = nom;
            this.prix = prix;
            this.nbPortions = nbPortions;
            this.type = type;
            this.regime = regime;
            this.nationalite = nationalite;
            this.date_peremption = date_peremption;
            this.photo = photo;
        }

        #region GetSet
        public int Id_plat
        {
            get { return id_plat; }
        }
        public string Nom
        {
            get { return nom; }
            set { nom = value; Update("nom", value); }
        }
        public double Prix
        {
            get { return prix; }
            set { prix = value; Update("prix", value.ToString()); }
        }
        public int NbPortions
        {
            get { return nbPortions; }
            set { nbPortions = value; Update("nbPortions", value.ToString()); }
        }
        public string Type
        {
            get { return type; }
            set { type = value; Update("type", value); }
        }
        public string Regime
        {
            get { return regime; }
            set { regime = value; Update("regime", value); }
        }
        public string Nationalite
        {
            get { return nationalite; }
            set { nationalite = value; Update("nationalite", value); }
        }
        public DateTime Date_peremption
        {
            get { return date_peremption; }
            set { date_peremption = value; Update("date_peremption", value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }
        public string Photo
        {
            get { return photo; }
            set { photo = value; Update("photo", value); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Plat SET {champ} = @valeur WHERE id_plat = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id", id_plat);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Plat WHERE id_plat = @id_plat;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_plat", id_plat);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the plat
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Plat WHERE id_plat = @id_plat;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_plat", id_plat);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            nom = reader.GetString("nom");
                            prix = reader.GetDouble("prix");
                            nbPortions = reader.GetInt32("nbPortions");
                            type = reader.GetString("type");
                            regime = reader.GetString("regime");
                            nationalite = reader.GetString("nationalite");
                            date_peremption = reader.GetDateTime("date_peremption");
                            photo = reader.GetString("photo");
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of plats
        {
            plats.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_plat FROM Plat;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            plats.Add(new Plat(reader.GetInt32("id_plat")));
                        }
                    }
                }
                connection.Close();
            }
        }
    }

    public class Ingredient
    {
        public static List<Ingredient> ingredients = new List<Ingredient>();
        private int id_ingredient;
        private string nom;
        private string regime;

        public Ingredient(int id_ingredient)
        {
            this.id_ingredient = id_ingredient;
            Refresh();
        }
        public Ingredient(string nom, string regime)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Ingredient (nom, regime)
                                VALUES (@nom, @regime);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@regime", regime);
                    command.ExecuteNonQuery();
                }
                string queryGetId = "SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(queryGetId, connection))
                {
                    id_ingredient = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
                connection.Close();
            }
            this.nom = nom;
        }

        #region GetSet
        public int Id_ingredient
        {
            get { return id_ingredient; }
        }
        public string Nom
        {
            get { return nom; }
            set { nom = value; Update("nom", value); }
        }
        public string Regime
        {
            get { return regime; }
            set { regime = value; Update("regime", value); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Ingredient SET {champ} = @valeur WHERE id_ingredient = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id", id_ingredient);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Ingredient WHERE id_ingredient = @id_ingredient;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_ingredient", id_ingredient);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the ingredient
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Ingredient WHERE id_ingredient = @id_ingredient;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_ingredient", id_ingredient);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            nom = reader.GetString("nom");
                            regime = reader.GetString("regime");
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of ingredients
        {
            ingredients.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_ingredient FROM Ingredient;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            ingredients.Add(new Ingredient(reader.GetInt32("id_ingredient")));
                        }
                    }
                }
                connection.Close();
            }
        }
    }

    public class Avis
    {
        public static List<Avis> avis = new List<Avis>();
        private int id_avis;
        private int note;
        private string commentaire;
        private DateTime date_avis;
        private Utilisateur client;
        private Utilisateur cuisinier;

        public Avis(int id_avis)
        {
            this.id_avis = id_avis;
            Refresh();
        }
        public Avis(int note, string commentaire, int id_client, int id_cuisinier)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Avis (note, commentaire, id_client, id_cuisinier)
                                VALUES (@note, @commentaire, @id_client, @id_cuisinier);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@note", note);
                    command.Parameters.AddWithValue("@commentaire", commentaire);
                    command.Parameters.AddWithValue("@id_client", id_client);
                    command.Parameters.AddWithValue("@id_cuisinier", id_cuisinier);
                    command.ExecuteNonQuery();
                }
                string queryGetId = "SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(queryGetId, connection))
                {
                    id_avis = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
                connection.Close();
            }
            Refresh();
        }

        #region GetSet
        public int Id_avis
        {
            get { return id_avis; }
        }
        public int Note
        {
            get { return note; }
            set { note = value; Update("note", value.ToString()); }
        }
        public string Commentaire
        {
            get { return commentaire; }
            set { commentaire = value; Update("commentaire", value); }
        }
        public DateTime Date_avis
        {
            get { return date_avis; }
            set { date_avis = value; Update("date_avis", value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }
        public Utilisateur Client
        {
            get { return client; }
            set { client = value; Update("id_client", value.Id_utilisateur.ToString()); }
        }
        public Utilisateur Cuisinier
        {
            get { return cuisinier; }
            set { cuisinier = value; Update("id_cuisinier", value.Id_utilisateur.ToString()); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Avis SET {champ} = @valeur WHERE id_avis = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id", id_avis);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Avis WHERE id_avis = @id_avis;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_avis", id_avis);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the avis
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Avis WHERE id_avis = @id_avis;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_avis", id_avis);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            note = reader.GetInt32("note");
                            commentaire = reader.GetString("commentaire");
                            date_avis = reader.GetDateTime("date_avis");
                            client = new Utilisateur(reader.GetInt32("id_client"));
                            cuisinier = new Utilisateur(reader.GetInt32("id_cuisinier"));
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of avis
        {
            avis.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_avis FROM Avis;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            avis.Add(new Avis(reader.GetInt32("id_avis")));
                        }
                    }
                }
                connection.Close();
            }
        }
    }

    public class Cuisine
    {
        public static List<Cuisine> cuisines = new List<Cuisine>();
        private Utilisateur cuisinier {get; set;}
        private Plat plat {get; set;}
        private bool plat_du_jour {get; set;}
        private DateTime date_cuisine {get; set;}
        private string statut {get; set;}

        public Cuisine(Utilisateur cuisinier, Plat plat)
        {
            this.cuisinier = cuisinier;
            this.plat = plat;
            Refresh();
        }
        public Cuisine(Utilisateur cuisinier, Plat plat, bool plat_du_jour, DateTime date_cuisine, string statut)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Cuisine (id_cuisinier, id_plat, plat_du_jour, date_cuisine, statut)
                                VALUES (@id_cuisinier, @id_plat, @plat_du_jour, @date_cuisine, @statut);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_cuisinier", cuisinier.Id_utilisateur);
                    command.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    command.Parameters.AddWithValue("@plat_du_jour", plat_du_jour);
                    command.Parameters.AddWithValue("@date_cuisine", date_cuisine.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@statut", statut);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            this.cuisinier = cuisinier;
            this.plat = plat;
            this.plat_du_jour = plat_du_jour;
            this.date_cuisine = date_cuisine;
            this.statut = statut;
        }

        #region GetSet
        public Utilisateur Cuisinier
        {
            get { return cuisinier; }
            set { cuisinier = value; Update("id_cuisinier", value.Id_utilisateur.ToString()); }
        }
        public Plat Plat
        {
            get { return plat; }
            set { plat = value; Update("id_plat", value.Id_plat.ToString()); }
        }
        public bool Plat_du_jour
        {
            get { return plat_du_jour; }
            set { plat_du_jour = value; Update("plat_du_jour", value.ToString()); }
        }
        public DateTime Date_cuisine
        {
            get { return date_cuisine; }
            set { date_cuisine = value; Update("date_cuisine", value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }
        public string Statut
        {
            get { return statut; }
            set { statut = value; Update("statut", value); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Cuisine SET {champ} = @valeur WHERE id_cuisinier = @id_cuisinier AND id_plat = @id_plat";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id_cuisinier", cuisinier.Id_utilisateur);
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Cuisine WHERE id_cuisinier = @id_cuisinier AND id_plat = @id_plat;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_cuisinier", cuisinier.Id_utilisateur);
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the cuisine
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Cuisine WHERE id_cuisinier = @id_cuisinier AND id_plat = @id_plat;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_cuisinier", cuisinier.Id_utilisateur);
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            plat_du_jour = reader.GetBoolean("plat_du_jour");
                            date_cuisine = reader.GetDateTime("date_cuisine");
                            statut = reader.GetString("statut");
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of cuisines
        {
            cuisines.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_cuisinier, id_plat FROM Cuisine;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            cuisines.Add(new Cuisine(new Utilisateur(reader.GetInt32("id_cuisinier")), new Plat(reader.GetInt32("id_plat"))));
                        }
                    }
                }
                connection.Close();
            }
        }
    }

    public class Requiert
    {
        public static List <Requiert> requierts = new List<Requiert>();
        private Plat plat {get; set;}
        private Livraison livraison {get; set;}
        private int quantite {get; set;}

        public Requiert(Plat plat, Livraison livraison)
        {
            this.plat = plat;
            this.livraison = livraison;
            Refresh();
        }
        public Requiert(Plat plat, Livraison livraison, int quantite)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Requiert (id_plat, id_livraison, quantite)
                                VALUES (@id_plat, @id_livraison, @quantite);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    command.Parameters.AddWithValue("@id_livraison", livraison.Id_livraison);
                    command.Parameters.AddWithValue("@quantite", quantite);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            this.plat = plat;
            this.livraison = livraison;
            this.quantite = quantite;
        }

        #region GetSet
        public Plat Plat
        {
            get { return plat; }
            set { plat = value; Update("id_plat", value.Id_plat.ToString()); }
        }
        public Livraison Livraison
        {
            get { return livraison; }
            set { livraison = value; Update("id_livraison", value.Id_livraison.ToString()); }
        }
        public int Quantite
        {
            get { return quantite; }
            set { quantite = value; Update("quantite", value.ToString()); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Requiert SET {champ} = @valeur WHERE id_plat = @id_plat AND id_livraison = @id_livraison";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.Parameters.AddWithValue("@id_livraison", livraison.Id_livraison);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Requiert WHERE id_plat = @id_plat AND id_livraison = @id_livraison;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.Parameters.AddWithValue("@id_livraison", livraison.Id_livraison);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the requiert
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Requiert WHERE id_plat = @id_plat AND id_livraison = @id_livraison;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.Parameters.AddWithValue("@id_livraison", livraison.Id_livraison);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            quantite = reader.GetInt32("quantite");
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of requierts
        {
            requierts.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_plat, id_livraison FROM Requiert;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            requierts.Add(new Requiert(new Plat(reader.GetInt32("id_plat")), new Livraison(reader.GetInt32("id_livraison"))));
                        }
                    }
                }
                connection.Close();
            }
        }
    }
    
    public class Compose
    {
        public static List<Compose> composes = new List<Compose>();
        private Plat plat {get; set;}
        private Ingredient ingredient {get; set;}
        private int quantite {get; set;}

        public Compose(Plat plat, Ingredient ingredient)
        {
            this.plat = plat;
            this.ingredient = ingredient;
            Refresh();
        }
        public Compose(Plat plat, Ingredient ingredient, int quantite)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Compose (id_plat, id_ingredient, quantite)
                                VALUES (@id_plat, @id_ingredient, @quantite);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    command.Parameters.AddWithValue("@id_ingredient", ingredient.Id_ingredient);
                    command.Parameters.AddWithValue("@quantite", quantite);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            this.plat = plat;
            this.ingredient = ingredient;
            this.quantite = quantite;
        }

        #region GetSet
        public Plat Plat
        {
            get { return plat; }
            set { plat = value; Update("id_plat", value.Id_plat.ToString()); }
        }
        public Ingredient Ingredient
        {
            get { return ingredient; }
            set { ingredient = value; Update("id_ingredient", value.Id_ingredient.ToString()); }
        }
        public int Quantite
        {
            get { return quantite; }
            set { quantite = value; Update("quantite", value.ToString()); }
        }
        #endregion
        public void Update(string champ, string value)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"UPDATE Compose SET {champ} = @valeur WHERE id_plat = @id_plat AND id_ingredient = @id_ingredient";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@valeur", value);
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.Parameters.AddWithValue("@id_ingredient", ingredient.Id_ingredient);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Compose WHERE id_plat = @id_plat AND id_ingredient = @id_ingredient;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.Parameters.AddWithValue("@id_ingredient", ingredient.Id_ingredient);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Refresh() // Refreshes the information of the compose
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Compose WHERE id_plat = @id_plat AND id_ingredient = @id_ingredient;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_plat", plat.Id_plat);
                    cmd.Parameters.AddWithValue("@id_ingredient", ingredient.Id_ingredient);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            quantite = reader.GetInt32("quantite");
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void Refreshes() // Refreshes the list of composes
        {
            composes.Clear();
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT id_plat, id_ingredient FROM Compose;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            composes.Add(new Compose(new Plat(reader.GetInt32("id_plat")), new Ingredient(reader.GetInt32("id_ingredient"))));
                        }
                    }
                }
                connection.Close();
            }
        }
    }
}