using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OfficeOpenXml;
using System.Globalization;

namespace ProbSciANA
{

    public class ExcelHelper
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
        public static (List<Station> stations, List<Arete> aretes, Dictionary<string,int> VitessesMoyennes) GetVertexPositions(string excelFilePath)
        {
   
            var stations = new List<Station>();
            var aretes = new List<Arete>(); 
            var VitessesMoyennes = new Dictionary<string, int>();
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                // On considère la première feuille
                var worksheet = package.Workbook.Worksheets[1];
                // Les données commencent à la ligne 2 (la ligne 1 contient les titres)
                int i = 2;
                while (worksheet.Cells[i, 1].Value != null)
                {
                    string Id = worksheet.Cells[i, 1].Value.ToString();
                    string Nom = worksheet.Cells[i, 2].Value.ToString();
                    decimal Longitude = decimal.Parse(worksheet.Cells[i, 3].Value.ToString());
                    decimal Latitude = decimal.Parse(worksheet.Cells[i, 4].Value.ToString());
                    int temps=0;
                if (worksheet.Cells[i, 5].Value != null)
                    {
                        temps = int.Parse(worksheet.Cells[i, 5].Value.ToString());
                    }
                    Station station = new Station(Id, Nom, Longitude, Latitude, temps);
                    stations.Add(station);
                    i++;
                }
                worksheet = package.Workbook.Worksheets[2];
                i = 2;
                
                while(worksheet.Cells[i, 1].Value != null)
                {
                    string IdPrevious = worksheet.Cells[i, 2].Value.ToString();
                    string IdNext = worksheet.Cells[i, 3].Value.ToString();
                    string IdLigne = worksheet.Cells[i, 1].Value.ToString();
                    Station memoire1 = null;
                    Station memoire2 = null;
                    foreach(Station var in stations)
                    {
                        if(var.Nom == IdPrevious)
                        {
                            memoire1 = var;
                        }
                        if (var.Nom == IdNext)
                        {
                            memoire2 = var;
                        }
                    }
                    Arete arete = new Arete(memoire1, memoire2, IdLigne);
                    aretes.Add(arete);
                    i++;
                }
                i=2;
                while(worksheet.Cells[i, 5].Value != null)
                {
                    string IdLigne = worksheet.Cells[i, 5].Value.ToString();
                    int VitesseMoyenne = int.Parse(worksheet.Cells[i, 6].Value.ToString());
                    VitessesMoyennes.Add(IdLigne, VitesseMoyenne);
                    i++;
                }

            }
            return (stations, aretes, VitessesMoyennes);
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
    List <Station> stations,
    List <Arete> aretes,
    string dotFilePath,
    string pngFilePath
    )
{
    try
    {
        // Création du fichier DOT
        StringBuilder dot = new StringBuilder();
        dot.AppendLine("graph G {");
        dot.AppendLine("    layout=neato;"); // Utilise le moteur neato
        dot.AppendLine("    overlap=false;");

        // Creation de chaque sommet avec sa position   
        foreach (Station vertex in stations)
        {
            var pos = vertex.Nom;
            string longitude = vertex.Longitude.ToString(CultureInfo.InvariantCulture);
            string latitude = vertex.Latitude.ToString(CultureInfo.InvariantCulture);
            dot.AppendLine($"    \"{pos}\" [pos=\"{longitude},{latitude}!\"];");
        }

        foreach (Arete edge in aretes)
        {
            if (edge.IdPrevious == null || edge.IdNext == null)
            {
                continue;
            }
            var idPrevious = edge.IdPrevious.Nom;
            var idNext = edge.IdNext.Nom;
            dot.AppendLine($"    \"{idPrevious}\" -- \"{idNext}\";");
        }
       

        dot.AppendLine("}");
        File.WriteAllText(dotFilePath, dot.ToString());
        /*  Pour déboguer, afficher le contenu du fichier DOT
        Console.WriteLine("Contenu du fichier DOT :");
        Console.WriteLine(dot.ToString());
        */

        // Exécuter dot.exe pour générer l'image PNG
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Graphviz", "bin", "dot.exe"),
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

