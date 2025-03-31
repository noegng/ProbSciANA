using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OfficeOpenXml;
using System.Globalization;

namespace ProbSciANA
{
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

