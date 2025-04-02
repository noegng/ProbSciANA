using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

 public static class SqlQueries{

            public static void SqlAddUser(string nom, string prenom, string email, string adresse, string role)
    {
        string connectionString = "server=localhost;user=root;password=ton_mot_de_passe;database=livinparis;";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = @"INSERT INTO utilisateurs (Nom, Prenom, Email, Adresse, Role)
                             VALUES (@Nom, @Prenom, @Email, @Adresse, @Role);";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Nom", nom);
                command.Parameters.AddWithValue("@Prenom", prenom);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Adresse", adresse);
                command.Parameters.AddWithValue("@Role", role);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }





    }