using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProbSciANA
{
    public static class Requetes
    {
        public static string connectionString = "SERVER=localhost;PORT=3306;user=root;password=root;database=pbsciana;";
        public static List<Utilisateur> utilisateurs = new List<Utilisateur>();
        public static Dictionary<Utilisateur,double> clients = new Dictionary<Utilisateur,double>();
        public static void GetUtilisateurs()
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
                        int i = 0;
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
                            i++;
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
        public static void GetClientsByAchats(string order)
        {
            GetUtilisateurs();
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
                    estClient = value; Delete("Client_", this.id_utilisateur);
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
                    estCuisinier = value; Delete("Cuisinier", this.id_utilisateur);
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
        public static void Delete(string table, int id_utilisateur)
        {
            using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
            {
                connection.Open();
                string query = $"DELETE FROM {table} WHERE id_utilisateur = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id_utilisateur);
                    cmd.ExecuteNonQuery();
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
    
}