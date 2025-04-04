using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProbSciANA
{
    public static class Requetes
    {
        public static string connectionString = "SERVER=localhost;PORT=3306;user=root;password=root;database=pbsciana;";
        public static List<Utilisateur> utilisateurs = new List<Utilisateur>();
        public static List<Commande> commandes = new List<Commande>();
        public static List<Livraison> livraisons = new List<Livraison>();
        public static Dictionary<Utilisateur,double> clients = new Dictionary<Utilisateur,double>();
        public static void RefreshUtilisateurs()
        {
            utilisateurs.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string queryUtilisateur = @"SELECT id_utilisateur, nom, prenom, adresse, telephone, email, station, date_inscription, mdp
                                        FROM Utilisateur;";

                string queryClient      = @"SELECT u.id_utilisateur
                                        FROM utilisateur u
                                        JOIN Client_ c ON u.id_utilisateur = c.id_utilisateur;";

                string queryCuisinier   = @"SELECT u.id_utilisateur
                                        FROM utilisateur u
                                        JOIN Cuisinier c ON u.id_utilisateur = c.id_utilisateur;";

                using (MySqlCommand command = new MySqlCommand(queryUtilisateur, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            utilisateurs.Add(new Utilisateur(
                            reader.GetInt32("id_utilisateur"),
                            false,
                            false,
                            reader.GetString("nom"),
                            reader.GetString("prenom"),
                            reader.GetString("adresse"),
                            reader.GetString("telephone"),
                            reader.GetString("email"),
                            reader.GetString("station"),
                            reader.GetDateTime("date_inscription"),
                            reader.GetString("mdp")
                            ));
                        }
                    }
                }

                using (MySqlCommand command = new MySqlCommand(queryClient, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            for(int i = 0; i < utilisateurs.Count; i++)
                            {
                                if(utilisateurs[i].Id_utilisateur == reader.GetInt32("id_utilisateur"))
                                {
                                    utilisateurs[i].estClient = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                using (MySqlCommand command = new MySqlCommand(queryCuisinier, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            for(int i = 0; i < utilisateurs.Count; i++)
                            {
                                if(utilisateurs[i].Id_utilisateur == reader.GetInt32("id_utilisateur"))
                                {
                                    utilisateurs[i].estCuisinier = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void RefreshClientsByAchats(string order)
        {
            RefreshUtilisateurs();
            clients.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = $"SELECT u.id_utilisateur, SUM(cmd.prix) AS achats " +
                               $"FROM client_ c " +
                               $"JOIN utilisateur u ON c.id_utilisateur = u.id_utilisateur " +
                               $"LEFT JOIN commande cmd ON cmd.id_client = c.id_utilisateur " +
                               $"GROUP BY u.id_utilisateur " +
                               $"ORDER BY {order} DESC;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            for(int i = 0; i < utilisateurs.Count; i++)
                            {
                                if(utilisateurs[i].Id_utilisateur == reader.GetInt32("id_utilisateur"))
                                {
                                    clients.Add(utilisateurs[i], reader.GetDouble("achats"));
                                    break;
                                }
                            }
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void RefreshCommandes()
        {
            commandes.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM Commande;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        int i = 0;
                        while(reader.Read())
                        {
                            commandes.Add(new Commande(
                            reader.GetInt32("id_commande"),
                            reader.GetString("nom"),
                            reader.GetDouble("prix"),
                            reader.GetString("statut"),
                            reader.GetDateTime("date_commande"),
                            reader.GetInt32("id_client"),
                            reader.GetInt32("id_cuisinier")
                            ));
                            i++;
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void RefreshLivraisons()
        {
            livraisons.Clear();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM Livraison;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            livraisons.Add(new Livraison(
                            reader.GetInt32("id_livraison"),
                            reader.GetString("station"),
                            reader.GetDateTime("date_livraison"),
                            reader.GetString("statut"),
                            reader.GetInt32("id_trajet"),
                            reader.GetInt32("id_commande")
                            ));
                        }
                    }
                }
                connection.Close();
            }
        }
        public static void DeleteById(string table, int id)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"DELETE FROM {table} WHERE id_utilisateur = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class Utilisateur
    {
        private int id_utilisateur;
        public bool estClient;
        public bool estCuisinier;
        private string nom;
        private string prenom;
        private string adresse;
        private string telephone;
        private string email;
        private string station;
        private DateTime date_inscription;
        private string mdp;

        public Utilisateur(int id_utilisateur, bool estClient, bool estCuisinier, string nom, string prenom, string adresse, string telephone, string email, string station, DateTime date_inscription, string mdp)
        {
            this.id_utilisateur = id_utilisateur;
            this.estClient = estClient;
            this.estCuisinier = estCuisinier;
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.telephone = telephone;
            this.email = email;
            this.station = station;
            this.date_inscription = date_inscription;
            this.mdp = mdp;
        }
        public Utilisateur(bool estClient, bool estCuisinier, string nom, string prenom, string adresse, string telephone, string email, string station, string mdp)
        {
            this.estClient = estClient;
            this.estCuisinier = estCuisinier;
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.telephone = telephone;
            this.email = email;
            this.station = station;
            this.mdp = mdp;
            InsertUtilisateur();
            Getid();
            InsertMaj();
            Getdate();
            Requetes.utilisateurs.Add(this);
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
                    estClient = value; Requetes.DeleteById("Client_", this.id_utilisateur);
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
                    estCuisinier = value; Requetes.DeleteById("Cuisinier", this.id_utilisateur);
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
        public string Station
        {
            get { return station; }
            set { station = value; Update("station", value); }
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
        #endregion
        private void InsertUtilisateur()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Utilisateur (nom, prenom, adresse, telephone, email, station, mdp)
                                VALUES (@nom, @prenom, @adresse, @telephone, @email, @station, @mdp);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@prenom", prenom);
                    command.Parameters.AddWithValue("@adresse", adresse);
                    command.Parameters.AddWithValue("@telephone", telephone);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@station", station);
                    command.Parameters.AddWithValue("@mdp", mdp);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
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
        private void Getid()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT LAST_INSERT_ID();";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    id_utilisateur = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
            }
        }
        private void Getdate()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT date_inscription FROM Utilisateur WHERE id_utilisateur = @id_utilisateur;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id_utilisateur", id_utilisateur);
                    date_inscription = Convert.ToDateTime(cmd.ExecuteScalar().ToString());
                }
            }
        }
    }

    public class Commande
    {
        private int id_commande {get; set;}
        private string nom {get; set;}
        private double prix {get; set;}
        private string statut {get; set;}
        private DateTime date_commande {get; set;}
        private int id_client {get; set;}
        private int id_cuisinier {get; set;}
        public Commande(int id_commande, string nom, double prix, string statut, DateTime date_commande, int id_client, int id_cuisinier)
        {
            this.id_commande = id_commande;
            this.id_client = id_client;
            this.id_cuisinier = id_cuisinier;
            this.nom = nom;
            this.prix = prix;
            this.statut = statut;
            this.date_commande = date_commande;
        }

        public Commande(string nom, double prix, string statut, DateTime date_commande, int id_client, int id_cuisinier)
        {
            this.nom = nom;
            this.prix = prix;
            this.statut = statut;
            this.date_commande = date_commande;
            this.id_client = id_client;
            this.id_cuisinier = id_cuisinier;
            InsertCommande();
            Getid();
            Requetes.commandes.Add(this);
        }

        public void InsertCommande()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Commande (nom, prix, statut, date_commande, id_client, id_cuisinier)
                                VALUES (@nom, @prix, @statut, @date_commande, @id_client, @id_cuisinier);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@prix", prix);
                    command.Parameters.AddWithValue("@statut", statut);
                    command.Parameters.AddWithValue("@date_commande", date_commande.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@id_client", id_client);
                    command.Parameters.AddWithValue("@id_cuisinier", id_cuisinier);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        private void Getid()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT LAST_INSERT_ID();";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    id_commande = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
            }
        }
    }

    public class Livraison
    {
        private int id_livraison {get; set;}
        private string station {get; set;}
        private DateTime date_livraison {get; set;}
        private string statut {get; set;}
        private int id_trajet {get; set;}
        private int id_commande {get; set;}
        
        public Livraison(int id_livraison, string station, DateTime date_livraison, string statut, int id_trajet, int id_commande)
        {
            this.id_livraison = id_livraison;
            this.station = station;
            this.date_livraison = date_livraison;
            this.statut = statut;
            this.id_trajet = id_trajet;
            this.id_commande = id_commande;
        }

        public Livraison(string station, DateTime date_livraison, string statut, int id_trajet, int id_commande)
        {
            this.station = station;
            this.date_livraison = date_livraison;
            this.statut = statut;
            this.id_trajet = id_trajet;
            this.id_commande = id_commande;
            InsertLivraison();
            Getid();
            Requetes.livraisons.Add(this);
        }

        public void InsertLivraison()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Livraison (station, date_livraison, statut, id_trajet, id_commande)
                                VALUES (@station, @date_livraison, @statut, @id_trajet, @id_commande);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@station", station);
                    command.Parameters.AddWithValue("@date_livraison", date_livraison.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@statut", statut);
                    command.Parameters.AddWithValue("@id_trajet", id_trajet);
                    command.Parameters.AddWithValue("@id_commande", id_commande);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        private void Getid()
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();

                string query = "SELECT LAST_INSERT_ID();";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    id_livraison = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                }
            }
        }
    }
}