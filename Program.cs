namespace Pb_Sci_Etape_1
{
    internal class Program
    {
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
                        Lien lien = new Lien((noeud1, noeud2));
                        listeLien.Add(lien);
                    }
                }
            }
            Test(listeLien, noeudMax, nbLiens);

            if(mode == 1)
            {
                //Liste d'adjacence
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
                Graphe graphe = new Graphe(adjacence);
                graphe.ParcoursLargeur(1); // BFS depuis le sommet 1
                graphe.ParcoursProfondeur(1); // DFS depuis le sommet 1
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
                Graphe graphe = new Graphe(matrice);
                graphe.ParcoursLargeur(1); // BFS depuis le sommet 1
                graphe.ParcoursLargeurMatrice(1); // BFS depuis le sommet 1
                graphe.ParcoursProfondeur(1); // DFS depuis le sommet 1
                graphe.ParcoursProfondeurMatrice(1); // DFS depuis le sommet 1
                graphe.AfficherMatrice(); // Affichage de la matrice d'adjacence
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