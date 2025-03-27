using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OfficeOpenXml;
using System.Globalization;

namespace ProbSciANA
{
    public static class ExcelHelper
    {
        /// <summary>
        /// Lit un fichier Excel afin de récupérer les positions (longitude, latitude) de chaque sommet.
        /// La colonne 1 correspond à l'identifiant du sommet,
        /// la colonne 3 correspond à la longitude,
        /// et la colonne 4 correspond à la latitude.
        /// La première ligne est supposée contenir les en-têtes.
        /// </summary>
        /// <param name="excelFilePath">Le chemin complet du fichier Excel.</param>
        /// <returns>Un dictionnaire associant l'identifiant du sommet à un tuple (longitude, latitude).</returns>
        public static Dictionary<int, (decimal Longitude, decimal Latitude)> GetVertexPositions(string excelFilePath)
        {
   
            var positions = new Dictionary<int, (decimal, decimal)>();

            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                // On considère la première feuille
                var worksheet = package.Workbook.Worksheets[1];
                // Les données commencent à la ligne 2 (la ligne 1 contient les titres)
                int row = 2;
                while (worksheet.Cells[row, 1].Value != null)
                {
                    int vertexId = Convert.ToInt32(worksheet.Cells[row, 1].Value);

                    // Utilisation de decimal.Parse pour les conversions
                    decimal longitude = decimal.Parse(worksheet.Cells[row, 3].Value.ToString());
                    decimal latitude = decimal.Parse(worksheet.Cells[row, 4].Value.ToString());

                    positions[vertexId] = (longitude, latitude);
                    row++;
                }
            }
            return positions;
        }
    }

public static class MetroGraphHelper
{
    /// <summary>
    /// Crée une matrice d'adjacence (int[,]) à partir d'un fichier Excel structuré par ID station et connexions par ligne.
    /// </summary>
    /// <param name="excelFilePath">Chemin complet vers le fichier Excel.</param>
    /// <returns>Une matrice d'adjacence binaire (1 = arête, 0 = aucune connexion)</returns>
    public static int[,] MatriceAdjacenceExcel(string excelFilePath)
   {
          
            // Lire le fichier Excel et extraire les stations et liaisons
            var stations = new HashSet<string>();
            var liens = new List<(string, string)>();

            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {

    // Accéder à la deuxième feuille
    var worksheet = package.Workbook.Worksheets[2]; // ou Worksheets["NomFeuille2"]

                int row = 2; // La première ligne contient les en-têtes

                while (worksheet.Cells[row, 1].Value != null)
                {
                    string station1 = worksheet.Cells[row, 2].Text.Trim();
                    string station2 = worksheet.Cells[row, 3].Text.Trim();

                    stations.Add(station1);
                    stations.Add(station2);

                    liens.Add((station1, station2));
                    row++;
                }
                
            }

            // Créer une correspondance entre nom de station et index de matrice
            var stationList = stations.OrderBy(s => s).ToList();
            var stationIndex = stationList.Select((station, index) => new { station, index })
                                          .ToDictionary(x => x.station, x => x.index);

            int n = stationList.Count;
            var matrice = new int[n, n];

            
            // Remplir la matrice d'adjacence
            foreach (var (s1, s2) in liens)
            {
                int i = stationIndex[s1];
                int j = stationIndex[s2];

                matrice[i, j] = 1;
                matrice[j, i] = 1; // Graphe non orienté
            }

            return matrice;
        }
}
    public static class Graphviz

    {
        /// <summary>
        /// Génère un fichier DOT en incluant la position de chaque sommet (si disponible),
        /// puis appelle Graphviz pour générer une image PNG.
        /// </summary>
        /// <param name="adjacence">Le dictionnaire représentant la liste d’adjacence.</param>
        /// <param name="dotFilePath">Chemin pour enregistrer le fichier DOT.</param>
        /// <param name="pngFilePath">Chemin pour générer l'image PNG.</param>

        public static void GenerateGraphImage(
    int [,] adjacence,
    string dotFilePath,
    string pngFilePath,
    Dictionary<int, (decimal Longitude, decimal Latitude)> vertexPositions = null)
{
    try
    {
        // Création du fichier DOT
        StringBuilder dot = new StringBuilder();
        dot.AppendLine("graph G {");
        dot.AppendLine("    layout=neato;"); // Utilise le moteur neato
        dot.AppendLine("    overlap=false;");

        // Pour chaque sommet, ajoutez l'attribut pos si disponible
        foreach (var vertex in vertexPositions)
        {
            var pos = vertex.Value;
            string longitude = pos.Longitude.ToString(CultureInfo.InvariantCulture);
            string latitude = pos.Latitude.ToString(CultureInfo.InvariantCulture);
            dot.AppendLine($"    \"{vertex.Key}\" [pos=\"{longitude},{latitude}!\"];");
        }

        // Ajout des arêtes sans doublons
       for (int i = 0; i < adjacence.GetLength(0); i++)
    {
    for (int j = i + 1; j < adjacence.GetLength(1); j++) // Parcourt uniquement la moitié supérieure pour éviter les doublons
    {
        if (adjacence[i, j] == 1) // Vérifie s'il existe une arête entre i et j
        {
            dot.AppendLine($"    \"{i + 1}\" -- \"{j + 1}\";");
        }
    }
}

        dot.AppendLine("}");
        File.WriteAllText(dotFilePath, dot.ToString());
        Console.WriteLine("Contenu du fichier DOT :");
        Console.WriteLine(dot.ToString());

        // Exécuter dot.exe pour générer l'image PNG
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\Noe\Documents\GitHub\ProbSciANA\Graphviz\bin\dot.exe",
                Arguments = $"-Tpng -o \"{pngFilePath}\" \"{dotFilePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            string errorOutput = process.StandardError.ReadToEnd();
    Console.WriteLine("Erreur lors de l'exécution de dot.exe :");
    Console.WriteLine(errorOutput);
    throw new Exception($"Le processus dot.exe a renvoyé le code {process.ExitCode}. Erreur : {errorOutput}");
        }

        Console.WriteLine($"Image PNG générée : {pngFilePath}");
        Process.Start(new ProcessStartInfo(pngFilePath) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
        throw new Exception("Une erreur est survenue lors de la génération de l'image du graphe.", ex);
    }
}

    }

}

