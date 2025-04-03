using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace ProbSciANA
{
 public static class SqlQueries{

        public static void SqlAddUser(string nom, string prenom,string adresse, string email, string station, string role, string mdp)
    {
        
        using (MySqlConnection connection = new MySqlConnection(Program.ConnectionString))
        {
            connection.Open();

            string query = @"INSERT INTO utilisateur (Nom, Prenom, adresse, email, station, mdp)
                             VALUES (@Nom, @Prenom, @Adresse, @Email, @Station, @Mdp);";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Nom", nom);
                command.Parameters.AddWithValue("@Prenom", prenom);
                command.Parameters.AddWithValue("@Adresse", adresse);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Station", station);
                command.Parameters.AddWithValue("@Mdp", mdp);
                command.ExecuteNonQuery();

                if(role == "Client")
                {
                    string getIdQuery = "INSERT INTO Client_ (id_utilisateur) VALUES (LAST_INSERT_ID())"; 
                    using (MySqlCommand getIdCommand = new MySqlCommand(getIdQuery, connection))
                    {
                        getIdCommand.ExecuteNonQuery();
                    }
                }
                else if(role == "Cuisinier")
                {
                    string getIdQuery = "INSERT INTO Cuisinier (id_utilisateur) VALUES (LAST_INSERT_ID())"; 
                    using (MySqlCommand getIdCommand = new MySqlCommand(getIdQuery, connection))
                    {
                        getIdCommand.ExecuteNonQuery();
                    }
                }
            }
            connection.Close();
        }
    }

        public static Dictionary<string, (int id, string motDePasse, string role)> ChargerUtilisateurs()
    {
        var utilisateurs = new Dictionary<string, (int, string, string)>();
       

        using var connection = new MySqlConnection(Program.ConnectionString);
        {
        connection.Open();

        string query = "SELECT id_utilisateur, nom, prenom, Mdp FROM utilisateur";
        using var cmd = new MySqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id_utilisateur");
            string nom = reader.GetString("nom");
            string prenom = reader.GetString("prenom");
            string motDePasse = reader.GetString("mdp");

            string display = $"{prenom} {nom}";
            utilisateurs[display] = (id, motDePasse, "Client"); // Par défaut, on suppose que c'est un client
        }
        }
        return utilisateurs;
    }

    public static List<Client> GetAllClients(string orderBy = "nom")
{
    var clients = new List<Client>();

    using var connection = new MySqlConnection(Program.ConnectionString);
    connection.Open();

    string query = $"SELECT id_utilisateur, nom, prenom, email, adresse FROM utilisateur WHERE role = 'Client' ORDER BY {orderBy}";
    using var cmd = new MySqlCommand(query, connection);
    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        clients.Add(new Client
        {
            reader.GetInt32("id_utilisateur"),
            reader.GetString("nom"),
            reader.GetString("prenom"),
            reader.GetString("email"),
            reader.GetString("adresse")
    });
    }

    return clients;
}


    public static void DeleteClient(int clientId)
    {
        using var connection = new MySqlConnection(Program.ConnectionString);
        connection.Open();
        string query = "DELETE FROM utilisateur WHERE id_utilisateur = @Id";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", clientId);
        cmd.ExecuteNonQuery();
    }

    public static void UpdateClient(int id, string nom, string prenom, string email, string adresse)
    {
        using var connection = new MySqlConnection(Program.ConnectionString);
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