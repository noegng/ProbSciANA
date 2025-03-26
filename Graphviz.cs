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
        /// la colonne 4 correspond à la longitude,
        /// et la colonne 5 correspond à la latitude.
        /// La première ligne est supposée contenir les en-têtes.
        /// </summary>
        /// <param name="excelFilePath">Le chemin complet du fichier Excel.</param>
        /// <returns>Un dictionnaire associant l'identifiant du sommet à un tuple (longitude, latitude).</returns>
        public static Dictionary<int, (double Longitude, double Latitude)> GetVertexPositions(string excelFilePath)
        {
        

            var positions = new Dictionary<int, (double, double)>();

            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                // On considère la première feuille
                var worksheet = package.Workbook.Worksheets[1];
                // Les données commencent à la ligne 2 (la ligne 1 contient les titres)
                int row = 2;
                while (worksheet.Cells[row, 1].Value != null)
                {
                    var cellValue = worksheet.Cells[row, 1].Value.ToString();
                    if (string.IsNullOrWhiteSpace(cellValue))
                    {
                        break; // Arrêtez la boucle si la cellule est vide ou contient uniquement des espaces
                    }

                    int vertexId = Convert.ToInt32(cellValue);

                    // Vérifiez également les colonnes 3 et 4 pour longitude et latitude
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 4].Value == null)
                    {
                        throw new Exception($"Les colonnes 3 ou 4 sont vides à la ligne {row}. Vérifiez le fichier Excel.");
                    }

                    double longitude = double.Parse(worksheet.Cells[row, 3].Value.ToString(), CultureInfo.InvariantCulture);
                    double latitude = double.Parse(worksheet.Cells[row, 4].Value.ToString(), CultureInfo.InvariantCulture);

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
 
        var stations = new HashSet<int>();
        var adjacencyDict = new Dictionary<int, HashSet<int>>();

        using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
        {
            var worksheet = package.Workbook.Worksheets[1];
            int row = 2;

            // Identifier les colonnes de voisinage
            var totalColumns = worksheet.Dimension.Columns;
            var neighborColumns = new List<int>();
            for (int col = 1; col <= totalColumns; col++)
            {
                var header = worksheet.Cells[1, col].Text;
                if (header.StartsWith("LIGNE") && (header.Contains("PRECEDENT") || header.Contains("SUIVANT")))
                {
                    neighborColumns.Add(col);
                }
            }

            // Lire chaque ligne pour construire le dictionnaire d’adjacence
            while (worksheet.Cells[row, 1].Value != null)
            {
                int stationId = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                stations.Add(stationId);

                if (!adjacencyDict.ContainsKey(stationId))
                    adjacencyDict[stationId] = new HashSet<int>();

                foreach (var col in neighborColumns)
                {
                    var cell = worksheet.Cells[row, col].Value?.ToString();
                    if (int.TryParse(cell, out int neighborId))
                    {
                        stations.Add(neighborId);

                        // Ajout symétrique (graphe non orienté)
                        adjacencyDict[stationId].Add(neighborId);

                        if (!adjacencyDict.ContainsKey(neighborId))
                            adjacencyDict[neighborId] = new HashSet<int>();

                        adjacencyDict[neighborId].Add(stationId);
                    }
                }

                row++;
            }
        }

        // Mapping : stationId -> index dans la matrice
        var stationList = stations.OrderBy(x => x).ToList();
        var idToIndex = stationList
            .Select((id, idx) => new { id, idx })
            .ToDictionary(x => x.id, x => x.idx);

        int n = stationList.Count;
        int[,] matrix = new int[n, n];

        foreach (var kvp in adjacencyDict)
        {
            int fromIdx = idToIndex[kvp.Key];
            foreach (var neighbor in kvp.Value)
            {
                int toIdx = idToIndex[neighbor];
                matrix[fromIdx, toIdx] = 1;
                matrix[toIdx, fromIdx] = 1; // non orienté
            }
        }

        return matrix;
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
            Dictionary<int, List<int>> adjacence, 
            string dotFilePath, 
            string pngFilePath,
            Dictionary<int, (double Longitude, double Latitude)> vertexPositions = null)
        {
            try
            {
                // Création du fichier DOT
                StringBuilder dot = new StringBuilder();
                dot.AppendLine("graph G {");
                dot.AppendLine("    layout=neato;"); // Utilise le moteur neato
                dot.AppendLine("    overlap=false;");
                // Pour chaque sommet, s'il existe une position, on ajoute l'attribut pos.
                foreach (var vertex in adjacence.Keys)
                {
                    if (vertexPositions != null && vertexPositions.ContainsKey(vertex))
                    {
                        var pos = vertexPositions[vertex];
                        dot.AppendLine($"    \"{vertex}\" [pos=\"{pos.Longitude},{pos.Latitude}!\"];");
                    }
                    else
                    {
                        // Sommet sans position spécifiée
                         dot.AppendLine($"    \"{vertex}\";");
                    }
                }
                // Ajout des arêtes sans doublons (pour graphe non orienté, n'écrire que si vertex < voisin)
                foreach (var pair in adjacence)
                {
                    foreach (var voisin in pair.Value)
                    {
                        if (pair.Key < voisin)
                        {
                            dot.AppendLine($"    \"{pair.Key}\" -- \"{voisin}\";");
                        }
                    }
                }
                dot.AppendLine("}");
                File.WriteAllText(dotFilePath, dot.ToString());
                Console.WriteLine($"Fichier DOT généré : {dotFilePath}");

                // Préparer et démarrer le processus dot.exe (chemin complet requis)
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
                    throw new Exception($"Le processus dot.exe a renvoyé le code {process.ExitCode}. Erreur : {errorOutput}");
                }
                Console.WriteLine($"Image PNG générée : {pngFilePath}");
                // Ouvrir l'image PNG dans le visualiseur par défaut
                Process.Start(new ProcessStartInfo(pngFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la génération de l'image du graphe.", ex);
            }
        }

        public static void GenerateGraphImage(
            int[,] matrix,
            Dictionary<int, (double Longitude, double Latitude)> vertexPositions,
            string dotFilePath,
            string pngFilePath)
        {
            try
            {
                // Création du fichier DOT
                StringBuilder dot = new StringBuilder();
                dot.AppendLine("graph G {");
                dot.AppendLine("    layout=neato;"); // Utilise le moteur neato
                dot.AppendLine("    overlap=false;");

                // Ajout des sommets avec leurs positions
                foreach (var vertex in vertexPositions)
                {
                    var pos = vertex.Value;
                    dot.AppendLine($"    \"{vertex.Key}\" [pos=\"{pos.Longitude},{pos.Latitude}!\"];");
                }

                // Ajout des arêtes à partir de la matrice d'adjacence
                int size = matrix.GetLength(0);
                for (int i = 0; i < size; i++)
                {
                    for (int j = i + 1; j < size; j++) // Évite les doublons en ne parcourant que la moitié supérieure
                    {
                        if (matrix[i, j] == 1)
                        {
                            dot.AppendLine($"    \"{i + 1}\" -- \"{j + 1}\";");
                        }
                    }
                }

                dot.AppendLine("}");
                File.WriteAllText(dotFilePath, dot.ToString());
                Console.WriteLine($"Fichier DOT généré : {dotFilePath}");

                // Préparer et démarrer le processus dot.exe (chemin complet requis)
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
                    throw new Exception($"Le processus dot.exe a renvoyé le code {process.ExitCode}. Erreur : {errorOutput}");
                }
                Console.WriteLine($"Image PNG générée : {pngFilePath}");
                // Ouvrir l'image PNG dans le visualiseur par défaut
                Process.Start(new ProcessStartInfo(pngFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la génération de l'image du graphe.", ex);
            }
        }
    }
}

