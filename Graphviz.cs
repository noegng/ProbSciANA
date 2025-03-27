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
        public static (List<Station> stations, List<Arete> aretes) GetVertexPositions(string excelFilePath)
        {
   
            var stations = new List<Station>();

            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                // On considère la première feuille
                var worksheet = package.Workbook.Worksheets[1];
                // Les données commencent à la ligne 2 (la ligne 1 contient les titres)
                int i = 2;
                while (worksheet.Cells[i, 1].Value != null)
                {
                    Station station = new Station();
                    station.Id = Convert.ToInt32(worksheet.Cells[i, 1].Value);

                    station.Nom = worksheet.Cells[i, 2].Value.ToString();
                    station.Longitude = decimal.Parse(worksheet.Cells[i, 3].Value.ToString());
                    station.Latitude = decimal.Parse(worksheet.Cells[i, 4].Value.ToString());
                    stations.Add(station);
                    i++;
                }
                worksheet = package.Workbook.Worksheets[2];
                i = 2;
                var aretes = new List<Arete>();
                while(worksheet.Cells[i, 1].Value != null)
                {
                    Arete arete = new Arete();
                    arete.IdPrevious = Convert.ToInt32(worksheet.Cells[i, 2].Value);
                    arete.IdNext = Convert.ToInt32(worksheet.Cells[i, 3].Value);
                    arete.IdLigne = Convert.ToInt32(worksheet.Cells[i, 1].Value);
                    //arete.Temps = Convert.ToInt32(worksheet.Cells[i, 4].Value);
                    aretes.Add(arete);
                    i++;

                }


            }
            return (stations, aretes);
        }
    }

public static class MetroGraphHelper
{
    /// <summary>
    /// Crée une matrice d'adjacence (int[,]) à partir d'un fichier Excel structuré par ID station et connexions par ligne.
    /// </summary>
    /// <param name="excelFilePath">Chemin complet vers le fichier Excel.</param>
    /// <returns>Une matrice d'adjacence binaire (1 = arête, 0 = aucune connexion)</returns>
    public static int[,] GetArete(string excelFilePath)
   {
          
            // Lire le fichier Excel et extraire les stations et liaisons
            var stations = new HashSet<string>();
            var liens = new List<(string, string)>();

            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {

    // Accéder à la deuxième feuille
    var worksheet = package.Workbook.Worksheets[2]; // ou Worksheets["NomFeuille2"]

                int i = 2; // La première ligne contient les en-têtes

                while (worksheet.Cells[i, 1].Value != null)
                {
                    string station1 = worksheet.Cells[i, 2].Text.Trim();
                    string station2 = worksheet.Cells[i, 3].Text.Trim();

                    stations.Add(station1);
                    stations.Add(station2);

                    liens.Add((station1, station2));
                    i++;
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
    List <Stations> stations,
    List <Arete> aretes,
    string dotFilePath,
    string pngFilePath,
    )
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
    thi new Exception($"Le processus dot.exe a renvoyé le code {process.ExitCode}. Erreur : {errorOutput}");
        }

        Console.WriteLine($"Image PNG générée : {pngFilePath}");
        Process.Start(new ProcessStartInfo(pngFilePath) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
        thi new Exception("Une erreur est survenue lors de la génération de l'image du graphe.", ex);
    }
}

    }

}

