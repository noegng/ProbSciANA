using QuickGraph;
using GraphSharp.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Net;



namespace ProbSciANA
{
    public class Program
    {
    static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            int mode = Initialisation();
            List<Lien> listeLien = new List<Lien>();
            (listeLien,int noeudMax,int nbLiens) = LectureFichier();
            //Test(listeLien, noeudMax, nbLiens);
            int départ = NoeudDépart(noeudMax);
            Dictionary<int, List<int>> adjacence = new Dictionary<int, List<int>>();    //Liste d'adjacence
            int[,] matrice = new int[noeudMax, noeudMax];    //Matrice d'adjacence
            Graphe graphe1 = null;
            if(mode == 1)
            {
                ListeAdjacence(listeLien, noeudMax,adjacence);  // Création d'un graph via une liste d'adjacence
                graphe1 = new Graphe(adjacence);
            }
            if (mode == 2)
            {
                MatriceAdjacence(listeLien, noeudMax, matrice); // Création d'un graph via une matrice d'adjacence
                graphe1 = new Graphe(matrice);
            }
            if(mode == 1)
            {
                graphe1.AfficherDansLordre(); // Affichage de la liste d'adjacence
            }
            if(mode == 2)
            {
                graphe1.AfficherMatrice(); // Affichage de la matrice d'adjacence
            }
          /*  graphe1.BFStoString(départ); // BFS depuis le sommet départ
            graphe1.DFStoString(départ); // DFS depuis le sommet départ
            graphe1.DFSRécursiftoString();
            graphe1.EstConnexe(); // Test de connexité
            graphe1.ContientCycle(); // Test de cycle
            
            // Exemple de graphe avec et sans cycle
            TestGraphe();
            */
         


// Chemin vers le fichier Excel contenant les positions des sommets.
            string excelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx"; 
// Appel de GetVertexPositions pour récupérer les positions
            (List<Station> stations, List<Arete> aretes) = ExcelHelper.GetVertexPositions(excelFilePath);
    // Chemins pour le fichier DOT et l'image PNG
    string dotFile = "graphe.dot";
    string pngFile = "graphe.png";

    // Générer le fichier DOT et l'image PNG
    Graphviz.GenerateGraphImage(stations, aretes, dotFile, pngFile);

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

        /// <summary>
        /// Choix du mode
        /// </summary>
        /// <returns></returns>
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
        static (List<Lien>,int,int) LectureFichier()
        {
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
            return (listeLien, noeudMax, nbLiens);
        }
        /// <summary>
        /// Choix du noeud de départ
        /// </summary>
        /// <param name="noeudMax"></param>
        /// <returns></returns>
        static int NoeudDépart(int noeudMax)
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
        static void ListeAdjacence(List<Lien> listeLien, int noeudMax,Dictionary<int, List<int>> adjacence)
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
        static void MatriceAdjacence(List<Lien> listeLien, int noeudMax, int[,] matrice)
        {
            foreach (Lien lien in listeLien)
                {
                    matrice[lien.Noeud1.Noeuds-1, lien.Noeud2.Noeuds-1] = 1; // -1 car les noeuds commencent à 1
                    matrice[lien.Noeud2.Noeuds-1, lien.Noeud1.Noeuds-1] = 1; // Pour un graphe non orienté car matrice symétrique
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
       
        }
        /// <summary>
        /// Test des méthodes EstConnexe & ContientCycle la classe Graphe
        /// </summary>
    /*    static void TestGraphe()
        {
            Dictionary<int, List<int>> grapheAvecCycle = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2, 3 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 1, 2 } }
            };
            Graphe graph2 = new Graphe(grapheAvecCycle);
            graph2.EstConnexe();
            graph2.ContientCycle();
            
            Dictionary<int, List<int>> grapheSansCycle = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2, 3 } },
                { 2, new List<int> { 1, 4 } },
                { 3, new List<int> { 1, 5 } },
                { 4, new List<int> { 2 } },
                { 5, new List<int> { 3, 6 } },
                { 6, new List<int> { 5 } }
            };
            Graphe graph3 = new Graphe(grapheSansCycle);
            graph3.EstConnexe();
            graph3.ContientCycle();
        }
    }*/
}