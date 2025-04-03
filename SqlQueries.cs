using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProbSciANA
{
 public static class SqlQueries{

        public static void SqlAddUser(string connectionString, string nom, string prenom, string email, string adresse, string role, string mdp)
    {
        
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

        public static Dictionary<string, (int id, string motDePasse, string role)> ChargerUtilisateurs(string connectionString)
    {
        var utilisateurs = new Dictionary<string, (int, string, string)>();
       

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

    public static List<Client> GetAllClients(string connectionString, string orderBy = "nom")
{
    var clients = new List<Client>();

    using var connection = new MySqlConnection(connectionString);
    connection.Open();

    string query = $"SELECT id_utilisateur, nom, prenom, email, adresse FROM utilisateur WHERE role = 'Client' ORDER BY {orderBy}";
    using var cmd = new MySqlCommand(query, connection);
    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        clients.Add(new Client
        {
            Id = reader.GetInt32("id_utilisateur"),
            Nom = reader.GetString("nom"),
            Prenom = reader.GetString("prenom"),
            Email = reader.GetString("email"),
            Adresse = reader.GetString("adresse")
        });
    }

    return clients;
}


    public static void DeleteClient(string connectionString, int clientId)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();
        string query = "DELETE FROM utilisateur WHERE id_utilisateur = @Id";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", clientId);
        cmd.ExecuteNonQuery();
    }

    public static void UpdateClient(string connectionString, int id, string nom, string prenom, string email, string adresse)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = @"UPDATE utilisateur SET nom = @Nom, prenom = @Prenom, email = @Email, adresse = @Adresse WHERE id_utilisateur = @Id";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@Nom", nom);
        cmd.Parameters.AddWithValue("@Prenom", prenom);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Adresse", adresse);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }





}
}