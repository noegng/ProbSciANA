using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace LivinParis
{
    public static class Requetes
    {
        public static string connectionString = "server=localhost;user=root;password=ton_mot_de_passe;database=livinparis;";
        public static List<Utilisateur> utilisateurs = new List<Utilisateur>();
        public static void SelectUtilisateurs()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string queryUtilisateur = @"SELECT id_utilisateur, nom, prenom, adresse, telephone, email, station, date_inscription, mdp
                                          FROM Utilisateur;";

                using (MySqlCommand command = new MySqlCommand(queryUtilisateur, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            utilisateurs.Add(new Utilisateur(
                            false,
                            false,
                            reader.GetString("nom"),
                            reader.GetString("prenom"),
                            reader.GetString("adresse"),
                            reader.GetString("telephone"),
                            reader.GetString("email"),
                            reader.GetString("station"),
                            reader.GetString("mdp")
                            ));
                        }
                    }
                }

                string queryClient = @"SELECT u.id_utilisateur
                                    FROM utilisateur u
                                    JOIN Client_ c ON u.id_utilisateur = c.id_utilisateur;";

                using (MySqlCommand command = new MySqlCommand(queryClient, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        int i = 0;
                        while(reader.Read())
                        {
                            utilisateurs[reader.GetInt32("id_utilisateur")].EstClient = true;
                            i++;
                        }
                    }
                }

                string queryCuisinier = @"SELECT u.id_utilisateur
                                        FROM utilisateur u
                                        JOIN Cuisinier c ON u.id_utilisateur = c.id_utilisateur;";

                using (MySqlCommand command = new MySqlCommand(queryCuisinier, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        int i = 0;
                        while(reader.Read())
                        {
                            utilisateurs[reader.GetInt32("id_utilisateur")].EstClient = true;
                            i++;
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
        private bool estClient;
        private bool estCuisinier;
        private string nom;
        private string prenom;
        private string adresse;
        private string telephone;
        private string email;
        private string station;
        private DateTime date_inscription;
        private string mdp;
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
            InsertMaj();
            date_inscription = DateTime.Now; // Pas ouf car pas forcément égal à celle de la BDD
            id_utilisateur = Requetes.utilisateurs.Count+1;
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
                    estClient = value; Delete("Client_");
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
                    estCuisinier = value; Delete("Cuisinier");
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
        private void Delete(string champ)
        {
            if(!estClient)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = $"DELETE FROM Client_ WHERE id_utilisateur = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id_utilisateur);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if(!estCuisinier)
            {
                using (MySqlConnection connection = new MySqlConnection(Requetes.connectionString))
                {
                    connection.Open();
                    string query = $"DELETE FROM Cuisinier WHERE id_utilisateur = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id_utilisateur);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private void Update(string champ, string value)
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
    }
}