using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OfficeOpenXml;
using System.Globalization;

namespace ProbSciANA
{
    public static class Graphviz<T>
    {
        private static int numéroImage =  0; ///Pour pouvoir créer plusieurs images en meme temps si nécessaire
        private static int numéroImageChemin =  0;
        /// <summary>
        /// Génère un fichier DOT en incluant la position de chaque sommet (si disponible),
        /// puis appelle Graphviz pour générer une image PNG.
        /// </summary>
        /// <param name="adjacence">Le dictionnaire représentant la liste d’adjacence.</param>
        /// <param name="dotFilePath">Chemin pour enregistrer le fichier DOT.</param>
        /// <param name="pngFilePath">Chemin pour générer l'image PNG.</param>

        private static string GetColorForLine(string idLigne)
        {
            // Définir les couleurs pour chaque ligne
            return idLigne switch
            {
            "1" => "#F2C931", /// Bouton d'Or
            "2" => "#216EB4", /// Azur
            "3" => "#9A9940", /// Olive
            "3bis" => "#89C7D6", /// Pervenche
            "4" => "#BB4D98", /// Parme
            "5" => "#DE5D29", /// Orange
            "6" => "#79BB92", /// Menthe
            "7" => "#DF9AB1", /// Rose
            "7bis" => "#79BB92", /// Menthe
            "8" => "#C5A3CA", /// Lilas
            "9" => "#CDC83F", /// Acacia
            "10" => "#DFB039", /// Ocre
            "11" => "#8E6538", /// Marron
            "12" => "#328E5B", /// Sapin
            "13" => "#89C7D6", /// Pervenche
            "14" => "#67328E", /// Iris
            "A" => "#D35E3C", /// Coquelicot
            _ => "black", /// Couleur par défaut
            };
        }

        public static void GenerateGraphImage( List <Noeud<T>> noeuds, List <Arc<T>> arcs)
    {
    numéroImage++;
    /// Chemins pour le fichier DOT et l'image PNG
    string dotFilePath = "graphe"+numéroImage+".dot";
    string pngFilePath = "graphe"+numéroImage+".png";                        
    /// Générer le fichier DOT et l'image PNG
    try
    {
        /// Création du fichier DOT
        StringBuilder dot = new StringBuilder();
        dot.AppendLine("digraph G {");
        dot.AppendLine("    layout=neato;"); /// Utilise le moteur neato
        dot.AppendLine("    overlap=false;");
        dot.AppendLine("    graph [dpi=400];");

        /// Creation de chaque sommet avec sa position   
        foreach (Noeud<T> vertex in noeuds)
        {
            var pos = vertex.Valeur.ToString();
            string longitude = vertex.Longitude.ToString(CultureInfo.InvariantCulture);
            string latitude = vertex.Latitude.ToString(CultureInfo.InvariantCulture);
            dot.AppendLine($"    \"{pos}\" [pos=\"{longitude},{latitude}!\",label=\"{pos}\", fontsize=12];");
        }
        for (int i = 0; i < arcs.Count; i= i+2) 
        {
            var idPrevious = arcs[i].IdPrevious.Valeur.ToString();
            var idNext = arcs[i].IdNext.Valeur.ToString();
            var color = GetColorForLine(arcs[i].IdLigne);
            string nonSensUnique = "";
            if (!arcs[i].SensUnique)
            {
                nonSensUnique = "dir=\"both\",";
            }
            dot.AppendLine($"    \"{idPrevious}\" -> \"{idNext}\" [{nonSensUnique} color=\"{color}\", penwidth=3, style=bold];");
            if(arcs[i].SensUnique){
                i--;
            }
        }


        
       

        dot.AppendLine("}");
        File.WriteAllText(dotFilePath, dot.ToString());
        ///  Pour déboguer, afficher le contenu du fichier DOT
        /// Console.WriteLine("Contenu du fichier DOT :");
        /// Console.WriteLine(dot.ToString());
        

       
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
    public static void GenerateChemin(List <Arc<T>> arcsChemin, List <Noeud<T>> noeuds)
{
    numéroImageChemin++;
    string dotFilePath = "grapheChemin"+numéroImageChemin+".dot";
    string pngFilePath = "grapheChemin"+numéroImageChemin+".png";
    try
    {
        /// Création du fichier DOT
        StringBuilder dot = new StringBuilder();
        dot.AppendLine("digraph G {");
        dot.AppendLine("    layout=neato;"); /// Utilise le moteur neato
        dot.AppendLine("    overlap=false;");
        dot.AppendLine("    graph [dpi=400];");
        ///Creation du premier sommet (impossible dans la boucle foreach)
        foreach (Noeud<T> vertex in noeuds)
        {
            var pos = vertex.Valeur.ToString();
            string longitude = vertex.Longitude.ToString(CultureInfo.InvariantCulture);
            string latitude = vertex.Latitude.ToString(CultureInfo.InvariantCulture);
            var color = "";
            var style ="";
            if (arcsChemin[0].IdPrevious.Valeur.ToString() == pos || arcsChemin[arcsChemin.Count - 1].IdNext.Valeur.ToString() == pos)
            {
                style = "filled";
                color = "red";
            }else
            {
                for (int i = 1; i < arcsChemin.Count ; i++){
                    if(vertex == arcsChemin[i].IdPrevious){
                        style = "bold";
                        color = "red";
                    }
                }
            }
            dot.AppendLine($"    \"{pos}\" [pos=\"{longitude},{latitude}!\", color=\"{color}\",label=\"{pos}\",style=\"{style}\", fontsize=12];");
        }

        for (int i = 0; i < arcsChemin.Count; i++) 
        {
            var idPrevious = arcsChemin[i].IdPrevious.Valeur.ToString();
            var idNext = arcsChemin[i].IdNext.Valeur.ToString();
            var color = GetColorForLine(arcsChemin[i].IdLigne);
            string nonSensUnique = "";
            dot.AppendLine($"    \"{idPrevious}\" -> \"{idNext}\" [{nonSensUnique} color=\"{color}\", penwidth=3, style=bold];");
        }


        
       

        dot.AppendLine("}");
        File.WriteAllText(dotFilePath, dot.ToString());
        ///Pour déboguer, afficher le contenu du fichier DOT
        ///Console.WriteLine("Contenu du fichier DOT :");
        ///Console.WriteLine(dot.ToString());
        

        /// Exécuter dot.exe pour générer l'image PNG
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

