using QuickGraph;
using GraphSharp.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;



namespace Pb_Sci_Etape_1
{
    public class Program
    {
         [STAThread]

        static void Main(string[] args)
        {
            
            int mode = Initialisation();
            string[] tab = new string[102];
            tab = File.ReadAllLines("soc-karate.mtx");
            int noeudMax = 0;
            int nbLiens = 0;
            List <Lien> listeLien = new List<Lien> (78);
            int a = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i][0] != '%')
                {
                    if (a == 0)         //Pour avoir seulement la 1ere ligne
                    {
                        noeudMax = Convert.ToInt32(tab[i].Substring(0,tab[i].IndexOf(' ')));
                        nbLiens = Convert.ToInt32(tab[i].Substring(tab[i].LastIndexOf(' ')+1));
                        a++;
                    }
                    else
                    {
                        Noeud noeud1 = new Noeud(Convert.ToInt32(tab[i].Substring(0, tab[i].IndexOf(' '))));
                        Noeud noeud2 = new Noeud(Convert.ToInt32(tab[i].Substring(tab[i].IndexOf(' ')+1)));
                        Lien lien = new Lien(noeud1, noeud2);
                        listeLien.Add(lien);
                    }
                }
            }
            Test(listeLien, noeudMax, nbLiens);
            int départ = NoeudDépart(noeudMax);
            //Liste d'adjacence     (obligatoire pour le BFS et DFS)
            Dictionary<int, List<int>> adjacence = new Dictionary<int, List<int>>();
            foreach (Lien lien in listeLien)
            {
                if (adjacence.ContainsKey(lien.Noeud1.Noeuds))
                {
                    adjacence[lien.Noeud1.Noeuds].Add(lien.Noeud2.Noeuds);
                }
                else
                {
                    adjacence.Add(lien.Noeud1.Noeuds, new List<int> { lien.Noeud2.Noeuds });
                }
                if (adjacence.ContainsKey(lien.Noeud2.Noeuds))
                {
                    adjacence[lien.Noeud2.Noeuds].Add(lien.Noeud1.Noeuds);
                }
                else
                {
                    adjacence.Add(lien.Noeud2.Noeuds, new List<int> { lien.Noeud1.Noeuds });
                }
            }
            
            Graphe graphe1 = new Graphe(adjacence);
            graphe1.ParcoursLargeur(départ); // BFS depuis le sommet 1
            graphe1.ParcoursProfondeur(départ); // DFS depuis le sommet 1
            /*
            if(mode == 1)
            {
                graphe.AfficherDansLordre(); // Affichage des listes d'adjacence
            }
            if (mode == 2)
            {
                //Matrice d'adjacence
                int[,] matrice = new int[noeudMax, noeudMax];
                foreach (Lien lien in listeLien)
                {
                    matrice[lien.Noeud1.Noeuds-1, lien.Noeud2.Noeuds-1] = 1; // -1 car les noeuds commencent à 1
                    matrice[lien.Noeud2.Noeuds-1, lien.Noeud1.Noeuds-1] = 1;
                }
                Graphe graphe2 = new Graphe(matrice);
                graphe2.AfficherMatrice(); // Affichage de la matrice d'adjacence
            }     
            */
            
            // Création du graphe orienté
            var graph = new BidirectionalGraph<string, Edge<string>>();

            for(int i = 0; i < noeudMax; i++)
            {
                graph.AddVertex(Convert.ToString(i+1));
            }

            foreach (Lien lien in listeLien)
            {
                graph.AddEdge(new Edge<string>(lien.Noeud1.toString(), lien.Noeud2.toString()));
                graph.AddEdge(new Edge<string>(lien.Noeud2.toString(), lien.Noeud1.toString())); // Pour un graphe non orienté
            }

            var app = new Application();
            var window = new Window
            {
                Title = "Visualisation du Graphe",
                Width = 800,
                Height = 600
            };

            // Création du contrôle GraphSharp pour afficher le graphe
            var graphLayout = new GraphLayout<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>()
            {
                Graph = graph,
                LayoutAlgorithmType = "Circular",  // Layout adapté aux graphes cycliques
                HighlightAlgorithmType = "Simple"
                // On peut omettre OverlapRemovalAlgorithmType pour éviter des problèmes éventuels
            };

            // Ajout du contrôle dans une grille et affectation à la fenêtre
            var grid = new Grid();
            grid.Children.Add(graphLayout);
            window.Content = grid;

            // Lancement de l'application WPF
            app.Run(window);
        
            Console.ReadKey();
        }

        /// <summary>
        /// Choix du mode
        /// </summary>
        /// <returns></returns>
        static int Initialisation()
        {
            Console.WriteLine("Quel mode voulez-vous utiliser ? \n1 - Listes d’adjacence \n2 - Matrice d’adjacence");
            string s = Console.ReadLine();
            int mode = 0;
            while (!int.TryParse(s, out mode) && (mode != 1 || mode != 2))
            {
                Console.WriteLine("Saisie inadaptée veuillez rentrer 1 ou 2.");
                s = Console.ReadLine();
            }
            if (mode == 1)
            {
                Console.WriteLine("Mode 1 sélectionné");
            }
            if (mode == 2)
            {
                Console.WriteLine("Mode 2 sélectionné");
            }
            return mode;
        }
        static int NoeudDépart(int noeudMax)
        {
            Console.WriteLine("Quel noeud de départ voulez-vous choisir ?");
            string s = Console.ReadLine();
            int départ = 0;
            while (!int.TryParse(s, out départ) && (départ > 0 || départ <= noeudMax))
            {
                Console.WriteLine("Saisie inadaptée veuillez rentrer un nombre entre 1 et " + noeudMax + ".");
                s = Console.ReadLine();
            }
            return départ;
        }
        /// <summary>
        /// Test tabLien et noeudMax et nbLien
        /// </summary>
        /// <param name="listeLien"></param>
        /// <param name="noeudMax"></param>
        /// <param name="nbLiens"></param>
        static void Test(List <Lien> listeLien, int noeudMax, int nbLiens)
        {
            Console.WriteLine("nombre de noeuds max : " + noeudMax);
            Console.WriteLine("nombre de liens : " + nbLiens);
            Console.WriteLine("Liste des liens : ");
            foreach (Lien lien in listeLien)
            {
                Console.WriteLine(lien.toString());
            }
        }
    }
}

/* static void Main(string[] args)
        {
            // Exemple de liste de sommets (ici des nombres entiers)
            var sommets = new List<string> { "1", "2", "3", "4", "5" };

            // Exemple de définitions d'arêtes sous forme de chaînes "source target"
            var definitionsAretes = new List<string>
            {
                "1 2",
                "2 3",
                "3 1",
                "4 5"
            };

            // Création du graphe orienté
            var graph = new BidirectionalGraph<string, Edge<string>>();

            // Ajout des sommets au graphe
            foreach (var s in sommets)
            {
                graph.AddVertex(s);
            }

            // Parcours de la liste des définitions d'arêtes pour les ajouter au graphe
            foreach (var def in definitionsAretes)
            {
                // Séparer la chaîne en utilisant l'espace comme séparateur
                var parts = def.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length == 2)
                {
                    string source = parts[0];
                    string cible = parts[1];
                    // Vérifier que les deux sommets existent dans le graphe
                    if (graph.ContainsVertex(source) && graph.ContainsVertex(cible))
                    {
                        graph.AddEdge(new Edge<string>(source, cible));
                    }
                }
            }

            // Création de l'application WPF et de la fenêtre
            var app = new Application();
            var window = new Window
            {
                Title = "Visualisation du Graphe",
                Width = 800,
                Height = 600
            };

            // Création du contrôle GraphSharp pour afficher le graphe
            var graphLayout = new GraphLayout<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>()
            {
                Graph = graph,
                LayoutAlgorithmType = "Circular",  // Layout adapté aux graphes cycliques
                HighlightAlgorithmType = "Simple"
                // On peut omettre OverlapRemovalAlgorithmType pour éviter des problèmes éventuels
            };

            // Ajout du contrôle dans une grille et affectation à la fenêtre
            var grid = new Grid();
            grid.Children.Add(graphLayout);
            window.Content = grid;

            // Lancement de l'application WPF
            app.Run(window);

            */