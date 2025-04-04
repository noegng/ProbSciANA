using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Net;
using OfficeOpenXml;
using MySql.Data.MySqlClient;
using System.Net.Sockets;
using System.Diagnostics;
using OfficeOpenXml.Utils;
using Org.BouncyCastle.Asn1.Misc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;


namespace ProbSciANA
{
     public class Program<T>
    {
         // Chaîne de connexion SQL
         
        public static string ConnectionString { get; } = "server=localhost;port=3306;user=root;password=root;database=pbsciana;";

        // Liste des stations et des arêtes
        public static List<Noeud<T>> Stations { get; private set; }
        public static List<Arc<T>> Aretes { get; private set; }

        // Méthode pour initialiser les données
        public static void InitializeData(string excelFilePath)
        {
            // Charger les données depuis le fichier Excel
            (var Stations,var  Aretes) = LectureFichierExcel(excelFilePath);
        }


        
      /* static void Main(string[] args)
        {   
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);  // Initialisation de la bibliothèque EPPlus pour lire les fichiers Excel  
            //Etape1(); // Appel de la méthode principale
            string excelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx"; // Chemin vers le fichier Excel contenant les positions des sommets.
            (List<Noeud<(int id,string nom)>> noeuds , List<Arc<(int id,string nom)>> arcs) = LectureFichierExcel(excelFilePath); // Lecture du fichier Excel
            Graphe<(int id,string nom)> graphePondéré = new Graphe<(int id,string nom)>(arcs); // Création d'un graphe à partir des arêtes
            //graphePondéré.BFStoString(noeuds[0]); 
            //graphePondéré.DFStoString(noeuds[0]);
            //graphePondéré.DFSRécursiftoString();
            //graphePondéré.EstConnexe();
            //graphePondéré.ContientCycle();
            //TestDistanceTemps(arcs); // Test de la distance et du temps de trajet entre deux noeuds
            //TestListeEtMatrice(graphePondéré); // Test de la liste d'adjacence et de la matrice d'adjacence
            //TestDijkstra(graphePondéré, noeuds); // Test de l'algorithme de Dijkstra
            //TestBellmanFord(graphePondéré, noeuds);
            TestDijkstraChemin(graphePondéré, noeuds); // Test de l'algorithme de Dijkstra avec vitesses moyennes
            //TestBellmanFordChemin(graphePondéré, noeuds);

            //AffichageImage(noeuds, arcs); // Affichage de l'image du graphe
            Console.WriteLine("Appuyez sur une touche pour quitter...");
            Console.ReadKey();
        }
      */
        static (List<Noeud<(int id,string nom)>>, List<Arc<(int id,string nom)>>) LectureFichierExcel(string excelFilePath){
            var noeuds = new List<Noeud<(int id,string nom)>>();
            var arcs = new List<Arc<(int id,string nom)>>(); 
            var VitessesMoyennes = new Dictionary<string, double>();

            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                // On considère la première feuille
                var worksheet = package.Workbook.Worksheets[2]; // On prend la deuxième feuille
                // Les données commencent à la ligne 2 (la ligne 1 contient les titres)
                int i=2;
                while(worksheet.Cells[i, 5].Value != null) //On commence par les vitesses moyennes
                {
                    string IdLigne = worksheet.Cells[i, 5].Value.ToString();
                    double VitesseMoyenne = double.Parse(worksheet.Cells[i, 6].Value.ToString());
                    VitessesMoyennes.Add(IdLigne, VitesseMoyenne);
                    i++;
                }
                worksheet = package.Workbook.Worksheets[1]; // On considère la premiere feuille
                i = 2;
                while (worksheet.Cells[i, 1].Value != null)
                {
                    int Id = int.Parse(worksheet.Cells[i, 1].Value.ToString());
                    string Nom = worksheet.Cells[i, 2].Value.ToString();
                    double Longitude = double.Parse(worksheet.Cells[i, 3].Value.ToString());
                    double Latitude = double.Parse(worksheet.Cells[i, 4].Value.ToString());
                    int tempsChamgement=0;
                    if (worksheet.Cells[i, 5].Value != null)
                    {
                        tempsChamgement = int.Parse(worksheet.Cells[i, 5].Value.ToString());
                    }
                    Noeud<(int, string)> noeud = new Noeud<(int, string)>((Id, Nom), tempsChamgement, Longitude, Latitude);
                    noeuds.Add(noeud);
                    i++;    
                }
                noeuds.Sort((s1, s2) => s1.Valeur.id.CompareTo(s2.Valeur.id)); // Tri des arcs par Id pour que les Id correspondent aux indices de la liste ( Ou par IdBrute)
                worksheet = package.Workbook.Worksheets[2]; // On considère la deuxième feuille
                i = 2;
                while(worksheet.Cells[i, 1].Value != null)
                {
                    string IdPrevious = worksheet.Cells[i, 2].Value.ToString();
                    string IdNext = worksheet.Cells[i, 3].Value.ToString();
                    string IdLigne = worksheet.Cells[i, 1].Value.ToString();
                    bool sensUnique = false;
                    if (worksheet.Cells[i, 4].Value != null)
                    {
                        sensUnique = true;
                    }
                    int idStationPrevious = 0;
                    int idStationNext = 0;
                    foreach(Noeud<(int id, string nom)>var in noeuds)
                    {
                        if(var.Valeur.nom == IdPrevious)
                        {
                            idStationPrevious = var.Valeur.id;
                        }
                        if (var.Valeur.nom== IdNext)
                        {
                            idStationNext = var.Valeur.id;
                        }

                    }
                    if (idStationPrevious != 0 && idStationNext != 0) // Aucune station a un id = 0 donc on ne peut pas créer l'arête
                    {
                        Arc<(int id,string nom)> arcAllé = new Arc<(int id,string nom)>(noeuds[idStationPrevious-1], noeuds[idStationNext-1], sensUnique, IdLigne); // Création de l'arête avec les arcs correspondantes (on faut cela pour conserver toutes les informations des arcs dans arete et les -1 car les id commencent à 1)
                        int poids = arcAllé.CalculerTempsTrajet(VitessesMoyennes,IdLigne ); // On met a jour le poids
                        if (!sensUnique) // Si l'arête n'est pas sens unique, on crée l'arête retour
                        {
                            Arc<(int id,string nom)> arcRetour = new Arc<(int id,string nom)>(noeuds[idStationNext-1], noeuds[idStationPrevious-1], sensUnique, IdLigne, poids); // Création de l'arête retour
                            arcs.Add(arcRetour); // Ajout de l'arête retour à la liste des arêtes
                        }
                        arcs.Add(arcAllé); // Ajout de l'arête à la liste des arêtes
                    }
                    i++;
                }
            }
            return (noeuds, arcs);
        }
        static void AffichageImage(List<Noeud<(int id, string nom)>> noeuds, List<Arc<(int id,string nom)>> arcs)
        {
            Graphviz<(int id, string nom)>.GenerateGraphImage(noeuds, arcs);
        }
        
        
        #region Etape 1
        static void Etape1()
        {
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
            graphe1.BFStoString(départ); // BFS depuis le sommet départ
            graphe1.DFStoString(départ); // DFS depuis le sommet départ
            graphe1.DFSRécursiftoString();
            graphe1.EstConnexe(); // Test de connexité
            graphe1.ContientCycle(); // Test de cycle
            
            // Exemple de graphe avec et sans cycle
            TestGraphe();
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
        #endregion
        #region Test
        static void TestDijkstra(Graphe<(int id,string nom)> graphePondéré, List<Noeud<(int id,string nom)>> arcs)
        {
            var sw = Stopwatch.StartNew();
            // Test de l'algorithme de Dijkstra
            Noeud<(int id, string nom)> depart = arcs[0]; // Noeud de départ
            Noeud<(int id, string nom)> arrivee = arcs[10]; // Noeud d'arrivée
            int plusPetitTemps = graphePondéré.Dijkstra(depart)[arrivee]; // Calcul du chemin le plus court
            sw.Stop();
            Console.WriteLine($"Temps écoulé : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetitTemps + " min.");
            sw = Stopwatch.StartNew();
            depart = arcs[0]; // Noeud de départ
            arrivee = arcs[246]; // Noeud d'arrivée
            plusPetitTemps = graphePondéré.Dijkstra(depart)[arrivee]; // Calcul du chemin le plus court
            sw.Stop();
            Console.WriteLine($"Temps écoulé : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetitTemps + " min.");
        }
        static void TestDijkstraChemin(Graphe<(int id,string nom)> graphePondéré, List<Noeud<(int id,string nom)>> arcs)
        {
            // Test de l'algorithme de Dijkstra
            var sw = Stopwatch.StartNew();
            Noeud<(int id, string nom)> depart = arcs[0]; // Noeud<(int id, string nom)> de départ
            Noeud<(int id, string nom)> arrivee = arcs[174]; // Noeud<(int id, string nom)> d'arrivée
            (List<Arc<(int id, string nom)>> chemin , int plusPetiteDistance) = graphePondéré.DijkstraChemin2(depart, arrivee); // Calcul du chemin le plus court
            sw.Stop();
            Console.WriteLine($"Temps écoulé : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetiteDistance + " min.");
            Console.Write("Chemin : ");
            foreach (Arc<(int id, string nom)> arete in chemin)
            {
                Console.Write(arete.IdPrevious.Valeur.nom + " -> "); // Affichage du chemin
            }
            Console.WriteLine(arrivee.Valeur.nom);
            Graphviz<(int id, string nom)>.GenerateChemin(chemin, arcs);
            Console.WriteLine("--------------------------------------------------");
            depart = arcs[0]; // Noeud<(int id, string nom)> de départ
            arrivee = arcs[246]; // Noeud<(int id, string nom)> d'arrivée
            sw = Stopwatch.StartNew();
            (chemin , plusPetiteDistance) = graphePondéré.DijkstraChemin2(depart, arrivee); // Calcul du chemin le plus court
            sw.Stop();
            Console.WriteLine($"Temps écoulé : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetiteDistance + " min.");
            Console.Write("Chemin : ");
            foreach (Arc<(int id, string nom)> arete in chemin)
            {
                Console.Write(arete.IdPrevious.Valeur.nom + " -> "); // Affichage du chemin
            }
            Console.WriteLine(arrivee.Valeur.nom);
            Graphviz<(int id, string nom)>.GenerateChemin(chemin, arcs);
        }

        static void TestBellmanFord(Graphe<(int id,string nom)> graphePondéré,List<Noeud<(int id,string nom)>> arcs)
        {
            Noeud<(int id, string nom)> depart = arcs[0]; // Noeud<(int id, string nom)> de départ
            Noeud<(int id, string nom)> arrivee = arcs[10]; // Noeud<(int id, string nom)> d'arrivée
            int plusPetitTemps = graphePondéré.BellmanFord(depart)[arrivee]; // Calcul du chemin le plus court
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetitTemps + " min.");
            depart = arcs[0]; // Noeud<(int id, string nom)> de départ
            arrivee = arcs[246]; // Noeud<(int id, string nom)> d'arrivée
            plusPetitTemps = graphePondéré.BellmanFord(depart)[arrivee]; // Calcul du chemin le plus court
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetitTemps + " min.");
        }
        static void TestBellmanFordChemin(Graphe<(int id,string nom)> graphePondéré,List<Noeud<(int id,string nom)>> arcs)
        {
            Noeud<(int id, string nom)> depart = arcs[0]; // Noeud<(int id, string nom)> de départ
            Noeud<(int id, string nom)> arrivee = arcs[174]; // Noeud<(int id, string nom)> d'arrivée
            var sw = Stopwatch.StartNew();
            (List<Arc<(int id, string nom)>> chemin , int plusPetiteDistance) = graphePondéré.BellmanFordChemin2(depart, arrivee); // Calcul du chemin le plus court
            sw.Stop();
            Console.WriteLine($"Temps écoulé : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetiteDistance + " min.");
            Console.Write("Chemin : ");
            foreach (Arc<(int id, string nom)> arete in chemin)
            {
                Console.Write(arete.IdPrevious.Valeur.nom + " -> "); // Affichage du chemin
            }
            Graphviz<(int id, string nom)>.GenerateChemin(chemin, arcs);
            Console.WriteLine("--------------------------------------------------");
            depart = arcs[0]; // Noeud<(int id, string nom)> de départ
            arrivee = arcs[246]; // Noeud<(int id, string nom)> d'arrivée
            sw = Stopwatch.StartNew();
            (chemin , plusPetiteDistance) = graphePondéré.BellmanFordChemin2(depart, arrivee); // Calcul du chemin le plus court
            sw.Stop();
            Console.WriteLine($"Temps écoulé : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("Le temps le plus court entre " + depart.Valeur.nom + " et " + arrivee.Valeur.nom + " est de " + plusPetiteDistance + " min.");
            Console.Write("Chemin : ");
            foreach (Arc<(int id, string nom)> arete in chemin)
            {
                Console.Write(arete.IdPrevious.Valeur.nom + " -> "); // Affichage du chemin
            }
            Graphviz<(int id, string nom)>.GenerateChemin(chemin, arcs);
        }
        static void TestListeEtMatrice(Graphe<(int id,string nom)> graphePondéré)
        {
            graphePondéré.AfficherListeAdjacence(); // Affichage de la liste d'adjacence
            graphePondéré.AfficherMatriceAdjacence(); // Affichage de la matrice d'adjacence
        }

        static void TestDistanceTemps(List<Arc<(int id,string nom)>> arcs)
        // Test de la distance et du temps de trajet entre deux arcs
        {
            foreach (Arc<(int id,string nom)> arete in arcs)
            {
                if (arete.IdPrevious == null || arete.IdNext == null)
                {
                    continue;   // Ignore les arêtes sans arcs
                }
                // Calcul de la distance entre deux arcs
                double distance = arete.CalculerDistance();
                // Affichage de la distance et du temps de trajet
                Console.WriteLine($"Distance entre {arete.IdPrevious.ToString()} et {arete.IdNext.ToString()} : {distance} km et temps de trajet : {arete.Poids} min");
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
        /// Test des méthodes EstConnexe & ContientCycle la classe Graphe
        /// </summary>
        static void TestGraphe()
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
        #endregion
    }

   
}
