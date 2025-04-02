using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProbSciANA
{
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

        public static Dictionary<string, (int id, string motDePasse, string role)> ChargerUtilisateurs()
    {
        var utilisateurs = new Dictionary<string, (int, string, string)>();
        string connectionString = "server=localhost;port=3306;user=root;password=root;database=pbsciana;";

        using var connection = new MySqlConnection(connectionString);
        {
        connection.Open();

        string query = "SELECT id_utilisateur, nom, prenom, MotDePasse, Role FROM utilisateur";
        using var cmd = new MySqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id_utilisateur");
            string nom = reader.GetString("nom");
            string prenom = reader.GetString("prenom");
            string motDePasse = reader.GetString("MotDePasse");
            string role = reader.GetString("Role");
            string display = $"{prenom} {nom}";
            utilisateurs[display] = (id, motDePasse, role);
        }
        }
        return utilisateurs;
    }





}
}