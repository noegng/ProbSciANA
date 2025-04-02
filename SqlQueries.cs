using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

 public static class SqlQueries{

            public static void SqlAddUser(string nom, string prenom, string email, string adresse, string role, string mdp)
    {
        string connectionString = "server=localhost;port=3306;user=root;password=root;database=pbsciana;";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = @"INSERT INTO utilisateur (Nom, Prenom, Email, Adresse, Role, MotDePasse)
                             VALUES (@Nom, @Prenom, @Email, @Adresse, @Role, @Mdp);";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Nom", nom);
                command.Parameters.AddWithValue("@Prenom", prenom);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Adresse", adresse);
                command.Parameters.AddWithValue("@Role", role);
                command.Parameters.AddWithValue("@Mdp", mdp);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }





    }