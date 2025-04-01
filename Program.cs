using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Net;
using OfficeOpenXml;


namespace ProbSciANA
{
    public class Program
    {
        static void Main(string[] args)
        {   
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);  // Initialisation de la bibliothèque EPPlus pour lire les fichiers Excel  
            //Etape1(); // Appel de la méthode principale
            string excelFilePath = "Metro_Arcs_Par_Station_IDs.xlsx"; // Chemin vers le fichier Excel contenant les positions des sommets.
            var stations = new List<Station>();
            var aretes = new List<Arete>(); 
            (stations, aretes) = LectureFichierExcel(excelFilePath); // Lecture du fichier Excel
            


            //Dictionary<Arete, int> poidsAretes = new Dictionary<Arete, int>(); // Dictionnaire pour stocker les poids des arêtes
            //poidsAretes = PoidsAretes(aretes, VitessesMoyennes); // Calcul des poids des arêtes


            TestDistanceTemps(aretes); // Test de la distance et du temps de trajet entre deux stations
            //Graphe2 graphePondéré = new Graphe2(aretes); // Création d'un graphe à partir des stations
            //graphePondéré.AfficherListeAdjacence(); // Affichage de la liste d'adjacence
            //graphePondéré.AfficherMatriceAdjacence(); // Affichage de la matrice d'adjacence
            //TestDijkstra(graphePondéré, stations, poidsAretes); // Test de l'algorithme de Dijkstra
            //TestDijkstra2(graphePondéré, stations, VitessesMoyennes); // Test de l'algorithme de Dijkstra avec vitesses moyennes

            //AffichageImage(stations, aretes); // Affichage de l'image du graphe
            Console.WriteLine("Appuyez sur une touche pour quitter...");
            Console.ReadKey();
        }
        
        static Dictionary<Arete, int> PoidsAretes(List<Arete> aretes, Dictionary<string, double> VitessesMoyennes)
        {
            Dictionary<Arete, int> poidsAretes = new Dictionary<Arete, int>();
            foreach (Arete arete in aretes) // Calcul des poids des arêtes
            {
                if (arete.IdPrevious != null && arete.IdNext != null) // Ignore les arêtes sans stations
                {
                    arete.CalculerTempsTrajet(VitessesMoyennes); // Calcul du temps de trajet entre deux stations  

                    //Il faut faire attention a la ligne de metro car 2 stations peuvent etre sur 2 lignes de metro differentes
                    // et donc avoir des vitesses moyennes differentes
                    // Le if eles fait fonctionner le code mais a modifier car il faut que 2 aretes puissent relier 2 stations avec des poids differents

                    if (poidsAretes.ContainsKey(arete)) // Si l'arête existe déjà dans le dictionnaire, on met à jour son poids
                    {
                        poidsAretes[arete] = arete.Temps; // Met à jour le poids de l'arête
                    }
                    else // Sinon, on l'ajoute au dictionnaire
                    {
                        poidsAretes.Add(arete, arete.Temps); // Calcul du temps de trajet entre deux stations  
                    }
                     // Ajout de l'arête et de son poids au dictionnaire
                    
                }
            }
            return poidsAretes;
        }
        static (List<Station>, List<Arete>) LectureFichierExcel(string excelFilePath){
            var stations = new List<Station>();
            var aretes = new List<Arete>(); 
            var VitessesMoyennes = new Dictionary<string, double>();
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                // On considère la première feuille
                var worksheet = package.Workbook.Worksheets[2]; // On prend la deuxième feuille
                // Les données commencent à la ligne 2 (la ligne 1 contient les titres)
                int i=2;
                while(worksheet.Cells[i, 5].Value != null)
                {
                    string IdLigne = worksheet.Cells[i, 5].Value.ToString();
                    double VitesseMoyenne = double.Parse(worksheet.Cells[i, 6].Value.ToString());
                    VitessesMoyennes.Add(IdLigne, VitesseMoyenne);
                    i++;
                }
                Arete.VitesseMoyenne = VitessesMoyennes; // Initialisation de la vitesse moyenne obligatoire pour le calcul du temps de trajet et la création de l'arête
                worksheet = package.Workbook.Worksheets[1]; // On considère la premiere feuille
                while (worksheet.Cells[i, 1].Value != null)
                {
                    string Id = worksheet.Cells[i, 1].Value.ToString();
                    string Nom = worksheet.Cells[i, 2].Value.ToString();
                    double Longitude = double.Parse(worksheet.Cells[i, 3].Value.ToString());
                    double Latitude = double.Parse(worksheet.Cells[i, 4].Value.ToString());
                    int tempsChamgement=0;
                if (worksheet.Cells[i, 5].Value != null)
                    {
                        tempsChamgement = int.Parse(worksheet.Cells[i, 5].Value.ToString());
                    }
                    Station station = new Station(Id, Nom, Longitude, Latitude, tempsChamgement);
                    stations.Add(station);
                    i++;
                }
                worksheet = package.Workbook.Worksheets[2]; // On considère la deuxième feuille
                
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
            }
            return (stations, aretes);
        }
        static void AffichageImage(List<Station> stations, List<Arete> aretes)
        {
            // Chemins pour le fichier DOT et l'image PNG
            string dotFile = "graphe.dot";
            string pngFile = "graphe.png";                        
            // Générer le fichier DOT et l'image PNG
            Graphviz.GenerateGraphImage(stations, aretes, dotFile, pngFile);
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
        static void TestDijkstra(Graphe2 graphePondéré, List<Station> stations, Dictionary<Arete, int> poidsAretes)
        {
            // Test de l'algorithme de Dijkstra
            Station depart = stations[0]; // Station de départ
            Station arrivee = stations[10]; // Station d'arrivée
            int plusPetitTemps = graphePondéré.Dijkstra(depart, poidsAretes)[arrivee]; // Calcul du chemin le plus court
            Console.WriteLine("Le temps le plus court entre " + depart.Nom + " et " + arrivee.Nom + " est de " + plusPetitTemps + " min.");
        }
        static void TestDijkstra2(Graphe2 graphePondéré, List<Station> stations, Dictionary<string, double> VitessesMoyennes)
        {
            // Test de l'algorithme de Dijkstra
            Station depart = stations[0]; // Station de départ
            Station arrivee = stations[10]; // Station d'arrivée
            int plusPetiteDistance = graphePondéré.Dijkstra2(depart)[arrivee]; // Calcul du chemin le plus court
            Console.WriteLine("Le temps le plus court entre " + depart.Nom + " et " + arrivee.Nom + " est de " + plusPetiteDistance + " min.");
        }

        static void TestDistanceTemps(List<Arete> aretes)
        // Test de la distance et du temps de trajet entre deux stations
        {
            foreach (Arete arete in aretes)
            {
                if (arete.IdPrevious == null || arete.IdNext == null)
                {
                    continue;   // Ignore les arêtes sans stations
                }
                // Calcul de la distance entre deux stations
                double distance = arete.CalculerDistance();
                // Affichage de la distance et du temps de trajet
                Console.WriteLine($"Distance 1 entre {arete.IdPrevious.Nom} et {arete.IdNext.Nom} : {distance} km et temps de trajet : {arete.Temps} min");
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