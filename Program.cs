using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using GraphSharp.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Data;


namespace ProbSciANA
{
    public class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            int mode = Initialisation();
            string[] tab = File.ReadAllLines("soc-karate.mtx");
            int noeudMax = 0;
            int nbLiens = 0;
            List<Lien> listeLien = new List<Lien>(78);
            int a = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i][0] != '%')
                {
                    if (a == 0) // Pour avoir seulement la 1ère ligne
                    {
                        noeudMax = Convert.ToInt32(tab[i].Substring(0, tab[i].IndexOf(' ')));
                        nbLiens = Convert.ToInt32(tab[i].Substring(tab[i].LastIndexOf(' ') + 1));
                        a++;
                    }
                    else
                    {
                        // Correction : récupérer la deuxième valeur avec Substring
                        Noeud noeud1 = new Noeud(Convert.ToInt32(tab[i].Substring(0, tab[i].IndexOf(' '))));
                        Noeud noeud2 = new Noeud(Convert.ToInt32(tab[i].Substring(tab[i].IndexOf(' ') + 1)));
                        Lien lien = new Lien(noeud1, noeud2);
                        listeLien.Add(lien);
                    }
                }
            }
            // Test(listeLien, noeudMax, nbLiens); // (facultatif)

            int depart = NoeudDepart(noeudMax);
            // Création de la liste d'adjacence (pour BFS/DFS)
            Dictionary<int, List<int>> adjacence = new Dictionary<int, List<int>>();
            ListeAdjacence(listeLien, noeudMax, adjacence);

            Graphe graphe1 = new Graphe(adjacence);
            graphe1.ParcoursLargeur(depart);      // BFS depuis le noeud de départ
            graphe1.ParcoursProfondeur(depart);     // DFS depuis le noeud de départ

            if (mode == 1)
                //graphe1.AfficherDansLordre();       // Affichage de la liste d'adjacence
            if (mode == 2)
            {
                // Matrice d'adjacence
                int[,] matrice1 = new int[noeudMax, noeudMax];
                foreach (Lien lien in listeLien)
                {
                    matrice1[lien.Noeud1.Noeuds - 1, lien.Noeud2.Noeuds - 1] = 1; // -1 car les noeuds commencent à 1
                    matrice1[lien.Noeud2.Noeuds - 1, lien.Noeud1.Noeuds - 1] = 1;
                }
                Graphe graphe2 = new Graphe(matrice1);
                graphe2.AfficherMatrice();         // Affichage de la matrice d'adjacence
            }

            if (EstConnexe(adjacence))
                Console.WriteLine("Le graphe est connexe.");
            else
                Console.WriteLine("Le graphe n'est pas connexe.");


            if (EstCycle(adjacence))
                Console.WriteLine("Le graphe est un cycle.");
            else
                Console.WriteLine("Le graphe n'est pas un cycle.");

            // Exemples de graphes pour des tests supplémentaires
            Dictionary<int, List<int>> grapheAvecCycle = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2, 3 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 1, 2 } }
            };
            if (EstConnexe(grapheAvecCycle))
                Console.WriteLine("Le graphe 2 est connexe.");
            else
                Console.WriteLine("Le graphe 2 n'est pas connexe.");
            if (EstCycle(grapheAvecCycle))
                Console.WriteLine("Le graphe 2 est un cycle.");
            else
                Console.WriteLine("Le graphe 2 n'est pas un cycle.");

            Dictionary<int, List<int>> grapheSansCycle = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 2 } }
            };
            if (EstCycle(grapheSansCycle))
                Console.WriteLine("Le graphe 3 est un cycle.");
            else
                Console.WriteLine("Le graphe 3 n'est pas un cycle.");
            if (EstConnexe(grapheSansCycle))
                Console.WriteLine("Le graphe 3 est connexe.");
            else
                Console.WriteLine("Le graphe 3 n'est pas connexe.");

// Chemin vers le fichier Excel contenant les positions des sommets.
            string excelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx"; 
// Appel de GetVertexPositions pour récupérer les positions
           var vertexPositions = ExcelHelper.GetVertexPositions(excelFilePath);

    // Lire la matrice d'adjacence
    int[,] matrice = MetroGraphHelper.MatriceAdjacenceExcel(excelFilePath);

    // Chemins pour le fichier DOT et l'image PNG
    string dotFile = "graphe.dot";
    string pngFile = "graphe.png";

    // Générer le fichier DOT et l'image PNG
    Graphviz.GenerateGraphImage(matrice, vertexPositions, dotFile, pngFile);

// Exemple : afficher les 10 premières lignes
for (int i = 0; i < Math.Min(10, matrice.GetLength(0)); i++)
{
    for (int j = 0; j < Math.Min(10, matrice.GetLength(1)); j++)
    {
        Console.Write(matrice[i, j] + " ");
    }
    Console.WriteLine();
}


            Console.ReadKey();
        }

        static int Initialisation()
        {
            Console.WriteLine("Quel mode voulez-vous utiliser ? \n1 - Listes d’adjacence \n2 - Matrice d’adjacence");
            string s = Console.ReadLine();
            int mode = 0;
            while (!int.TryParse(s, out mode) || (mode != 1 && mode != 2))
            {
                Console.WriteLine("Saisie inadaptée veuillez rentrer 1 ou 2.");
                s = Console.ReadLine();
            }
            Console.WriteLine($"Mode {mode} sélectionné");
            return mode;
        }

        static int NoeudDepart(int noeudMax)
        {
            Console.WriteLine("Quel noeud de départ voulez-vous choisir ?");
            string s = Console.ReadLine();
            int depart = -1;
            while (!int.TryParse(s, out depart) || (depart <= 0 || depart > noeudMax))
            {
                Console.WriteLine("Saisie inadaptée. Veuillez rentrer un nombre entre 1 et " + noeudMax + ".");
                s = Console.ReadLine();
            }
            return depart;
        }

        static void ListeAdjacence(List<Lien> listeLien, int noeudMax, Dictionary<int, List<int>> adjacence)
        {
            foreach (Lien lien in listeLien)
            {
                if (adjacence.ContainsKey(lien.Noeud1.Noeuds))
                    adjacence[lien.Noeud1.Noeuds].Add(lien.Noeud2.Noeuds);
                else
                    adjacence.Add(lien.Noeud1.Noeuds, new List<int> { lien.Noeud2.Noeuds });
                
                if (adjacence.ContainsKey(lien.Noeud2.Noeuds))
                    adjacence[lien.Noeud2.Noeuds].Add(lien.Noeud1.Noeuds);
                else
                    adjacence.Add(lien.Noeud2.Noeuds, new List<int> { lien.Noeud1.Noeuds });
            }
        }

        static void Test(List<Lien> listeLien, int noeudMax, int nbLiens)
        {
            Console.WriteLine("Nombre de noeuds max : " + noeudMax);
            Console.WriteLine("Nombre de liens : " + nbLiens);
            Console.WriteLine("Liste des liens : ");
            foreach (Lien lien in listeLien)
            {
                Console.WriteLine(lien.ToString());
            }
        }
/*
        static void AfficherGraph(int noeudMax, List<Lien> listeLien)
        {
            var graph = new BidirectionalGraph<string, Edge<string>>();
            for (int i = 0; i < noeudMax; i++)
            {
                graph.AddVertex((i + 1).ToString());
            }

            foreach (Lien lien in listeLien)
            {
                // Utilisation de ToString() sur vos objets Noeud (vérifiez l’implémentation de ToString dans la classe Noeud)
                graph.AddEdge(new Edge<string>(lien.Noeud1.toString(), lien.Noeud2.toString()));
              //  graph.AddEdge(new Edge<string>(lien.Noeud2.toString(), lien.Noeud1.toString())); // Pour un graphe non orienté
            }

            var app = new Application();
            var window = new Window
            {
                Title = "Visualisation du Graphe",
                Width = 800,
                Height = 600
            };

            var graphLayout = new GraphLayout<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>()
            {
                Graph = graph,
                LayoutAlgorithmType = "Circular",  // Layout adapté aux graphes cycliques
                HighlightAlgorithmType = "Simple"
            };

            var grid = new Grid();
            grid.Children.Add(graphLayout);
            window.Content = grid;

            app.Run(window);
        }
        */

        static bool EstConnexe(Dictionary<int, List<int>> adjacence)
        {
            HashSet<int> visite = new HashSet<int>();
            Graphe graphe = new Graphe(adjacence);
            visite = graphe.ParcoursLargeur(1);
            int nbNoeuds = adjacence.Count;
            return (visite.Count == nbNoeuds);
        }

        static bool EstCycle(Dictionary<int, List<int>> adjacence)
        {
            foreach (var sommet in adjacence)
            {
                if (sommet.Value.Count < 2)
                    return false;
            }
            return EstConnexe(adjacence);
        }
    }
}