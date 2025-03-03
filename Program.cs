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
        public static void Main(string[] args)
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
            //Test(listeLien, noeudMax, nbLiens);

            int départ = NoeudDépart(noeudMax);
            //Liste d'adjacence     (obligatoire pour le BFS et DFS)
            Dictionary<int, List<int>> adjacence = new Dictionary<int, List<int>>();
            ListeAdjacence(listeLien, noeudMax,adjacence);
            Graphe graphe1 = new Graphe(adjacence);
            graphe1.ParcoursLargeur(départ); // BFS depuis le sommet 1
            graphe1.ParcoursProfondeur(départ); // DFS depuis le sommet 1

            if(mode == 1)
            {
                graphe1.AfficherDansLordre(); // Affichage des listes d'adjacence
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

            if (EstConnexe(adjacence))     // Test de connexité
            {
                Console.WriteLine("Le graphe est connexe.");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas connexe.");
            }
            if (EstCycle(adjacence))      // Test de cycle
            {
                Console.WriteLine("Le graphe est un cycle.");
            }else
            {
                Console.WriteLine("Le graphe n'est pas un cycle.");
            }
            // Exemple de graphe avec et sans cycle
            Dictionary<int, List<int>> grapheAvecCycle = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2, 3 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 1, 2 } }
            };
            if(EstConnexe(grapheAvecCycle))
            {
                Console.WriteLine("Le graphe 2 est connexe.");
            }
            else
            {
                Console.WriteLine("Le graphe 2 n'est pas connexe.");
            }
            if(EstCycle(grapheAvecCycle))
            {
                Console.WriteLine("Le graphe 2 est un cycle.");
            }
            else
            {
                Console.WriteLine("Le graphe 2 n'est pas un cycle.");
            }
            Dictionary<int, List<int>> grapheSansCycle = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 2 } }
            };
            if (EstCycle(grapheSansCycle))
            {
                Console.WriteLine("Le graphe 3 est un cycle.");
            }
            else
            {
                Console.WriteLine("Le graphe 3 n'est pas un cycle.");
            }
            if (EstConnexe(grapheSansCycle))
            {
                Console.WriteLine("Le graphe 3 est connexe.");
            }
            else
            {
                Console.WriteLine("Le graphe 3 n'est pas connexe.");
            }

            // Création du graphe orienté
            AfficherGraph(noeudMax, listeLien);
            Console.ReadKey();
        }

        /// <summary>
        /// Choix du mode
        /// </summary>
        /// <returns></returns>
        public static int Initialisation()
        {
            Console.WriteLine("Quel mode voulez-vous utiliser ? \n1 - Listes d’adjacence \n2 - Matrice d’adjacence");
            string s = Console.ReadLine();
            int mode = 0;
            while (!int.TryParse(s, out mode) || (mode != 1 && mode != 2))
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
        /// <summary>
        /// Choix du noeud de départ
        /// </summary>
        /// <param name="noeudMax"></param>
        /// <returns></returns>
        public static int NoeudDépart(int noeudMax)
        {
            Console.WriteLine("Quel noeud de départ voulez-vous choisir ?");
            string s = Console.ReadLine();
            int départ = -1;
            while (!int.TryParse(s, out départ) || (départ <= 0 || départ > noeudMax))
            {
                Console.WriteLine("Saisie inadaptée veuillez rentrer un nombre entre 1 et " + noeudMax + ".");
                s = Console.ReadLine();
            }
            return départ;
        }
        /// <summary>
        /// Création de la liste d'adjacence
        /// </summary>
        /// <param name="listeLien"></param>
        /// <param name="noeudMax"></param>
        /// <param name="adjacence"></param>
        public static void ListeAdjacence(List<Lien> listeLien, int noeudMax,Dictionary<int, List<int>> adjacence)
        {
            
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
        /// <summary>
        /// Affichage du graphe
        /// </summary>
        /// <param name="noeudMax"></param>
        /// <param name="listeLien"></param>
        public static void AfficherGraph(int noeudMax, List<Lien> listeLien)
        {
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
        }
        /// <summary>
        /// Test de connexité
        /// </summary>
        /// <param name="adjacence"></param>
        /// <param name="noeudMax"></param>
        /// <param name="graphe"></param>
        public static bool EstConnexe(Dictionary<int, List<int>> adjacence)
        {
            // Est connexe si le parcours en largeur (BFS) partant de n'importe quel sommet atteint tous les sommets
            bool estConnexe = false;
            HashSet<int> visite = new HashSet<int>();
            Graphe graphe = new Graphe(adjacence);
            visite = graphe.ParcoursLargeur(1);
            int noeudMax = adjacence.Count;
            if (visite.Count == noeudMax)
            {
                estConnexe = true;
            }

            return estConnexe;
        } 
        /// <summary>
        /// Test de cycle
        /// Pour savoir si un graphe est un cycle, il faut que tous les sommets aient un degré de 2 et que le graphe soit connexe  
        /// </summary>
        /// <param name="adjacence"></param>
        /// <param name="graphe"></param>
        /// <returns></returns>
        public static bool EstCycle(Dictionary<int, List<int>> adjacence)
        {
        // Vérifier que tous les sommets ont un degré de 2
        foreach (var sommet in adjacence)
        {
            if (sommet.Value.Count < 2)
                return false;
        }
        // Vérifier que le graphe est connexe
        return EstConnexe(adjacence);
        }        
    }
}