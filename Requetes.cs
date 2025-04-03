using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
public static class Requetes
{
    static string connectionString = "server=localhost;user=root;password=ton_mot_de_passe;database=livinparis;";
    static List<Utilisateur> utilisateurs = new List<Utilisateur>();
    

    public static void SqlAddUser(string nom, string prenom, string email, string adresse, string role)
    {
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

    public static List<Utilisateur> SqlGetUsersByRole(string role)
    {
        List<Utilisateur> utilisateurs = new List<Utilisateur>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = @"SELECT Nom, Prenom, Email, Adresse, Role
                             FROM utilisateurs
                             WHERE Role = @Role;";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Role", role);

                // Exécution et lecture encapsulées
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        utilisateurs.Add(new Utilisateur
                        {
                            Nom = reader.GetString("Nom"),
                            Prenom = reader.GetString("Prenom"),
                            Email = reader.GetString("Email"),
                            Adresse = reader.GetString("Adresse"),
                            Role = reader.GetString("Role")
                        });
                    }
                }
            }

            connection.Close();
        }

        return utilisateurs;
    }
}