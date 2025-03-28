using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Graphe
    {
        private Dictionary<int, List<int>> listeAdjacence;
        private int[,] matriceAdjacence;
        List<Noeud> noeuds = new List<Noeud>();


        public Graphe(Dictionary<int, List<int>> adjacence)
        {
            listeAdjacence = adjacence;
        }
        public Graphe(int[,] matriceAdjacence)
        {
            this.matriceAdjacence = matriceAdjacence;
            Dictionary<int, List<int>> listeAdjacencelocal = new Dictionary<int, List<int>>();
            for(int i = 1; i <= matriceAdjacence.GetLength(0); i++)
            {
                List<int> voisins = new List<int>();
                for (int j = 1; j <= matriceAdjacence.GetLength(1); j++)
                {
                    if (matriceAdjacence[i-1, j-1] == 1)
                    {
                        voisins.Add(j);
                    }
                }
                listeAdjacencelocal.Add(i, voisins);
            }
            listeAdjacence = listeAdjacencelocal;
        }
        /// <summary>
        /// Parcours en largeur (BFS) avec file
        /// 0 = blanc, 1 = jaune, 2 = rouge
        /// </summary>
        /// <param name="sommetDepart"></param>
        public HashSet<Noeud> BFS(int sommetDepart)
        {
            Dictionary<int, int> couleurs = new Dictionary<int, int>();
            Queue<int> file = new Queue<int>();
            HashSet<Noeud> visite = new HashSet<Noeud>();
            foreach (int sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }
            file.Enqueue(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune
            while (file.Count > 0)
            {
                int sommet = file.Dequeue();
                foreach (int voisin in listeAdjacence.Keys)
                {
                 if (couleurs[voisin] == 0) // blanc
                    {
                        file.Enqueue(voisin);
                        couleurs[voisin] = 1; // jaune
                    }
                }
             couleurs[sommet] = 2; // rouge
             visite.Add(new Noeud(sommet));
            }
            return visite;
        }   
        
        public HashSet<Noeud> DFS(int sommetDepart)
        {
            Dictionary<int, int> couleurs = new Dictionary<int, int>();
            Stack<int> pile = new Stack<int>();
            HashSet<Noeud> visite = new HashSet<Noeud>();
            foreach (int sommet in listeAdjacence.Keys)// Initialisation (tous les sommets sont blancs)
            {
                couleurs[sommet] = 0; // blanc
            }
            pile.Push(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune
            while (pile.Count > 0)
            {
                int sommet = pile.Peek();
                bool aExploréUnVoisin = false;
                foreach (int voisin in listeAdjacence[sommet].OrderBy(x => x)) // Pour explorer dans l'ordre et avoit le même résultat que le parcours en largeur
                {
                    if (couleurs[voisin] == 0)   // On cherche un voisin blanc
                    {
                        pile.Push(voisin);
                        couleurs[voisin] = 1; // jaune
                        aExploréUnVoisin = true;
                        break; // on explore un seul voisin à la fois
                    }
                }
                // Si aucun voisin blanc, on termine ce sommet
                if (!aExploréUnVoisin)
                {
                    couleurs[sommet] = 2; // rouge
                    pile.Pop();
                    visite.Add(new Noeud(sommet));
                }
            }
            return visite;
        }

        public HashSet<Noeud> DFSRécursif(bool rechercheCycle = false)
        {
            Dictionary<int, int> couleurs = new Dictionary<int, int>();
            HashSet<Noeud> sommetTerminé = new HashSet<Noeud>();

            // Initialisation : tous les sommets sont blancs
            foreach (int sommet in listeAdjacence.Keys)
            couleurs[sommet] = 0;

            foreach (int sommet in listeAdjacence.Keys)
            {
            if (couleurs[sommet] == 0)
                DFSrec(sommet,sommet, couleurs, sommetTerminé, rechercheCycle);
            }

            return sommetTerminé;
        }
        
        public void DFSrec(int sommet,int sommetPré, Dictionary<int, int> couleurs, HashSet<Noeud> sommetTerminé, bool rechercheCycle = false)
        {
            couleurs[sommet] = 1; // jaune : en traitement
            

            foreach (int successeur in listeAdjacence[sommet])
            {
                if (couleurs[successeur] == 0)
                {
                       // le sommet doit etre rouge ici pour ne pas être traité à nouveau dans la prochaine itération
                    DFSrec(successeur,sommet, couleurs, sommetTerminé, rechercheCycle);
                }
                else if (rechercheCycle && sommet != successeur && sommetPré != successeur && couleurs[successeur] == 1)
                {
                    // Si on trouve un sommet jaune, cela signifie qu'il y a un cycle
                    // On ne traite pas le sommet lui-même ni son prédécesseur pour éviter de détecter un cycle trivial
                    Console.WriteLine("Cycle détecté.");
                    return;
                }
                sommetTerminé.Add(new Noeud(sommet)); // Ajouter le sommet visité
                couleurs[sommet] = 2;
            }
        }
        public void DFStoString(int sommetDepart)
        {
            Console.Write("Parcours en Profondeur (DFS): ");
            foreach (Noeud sommet in DFS(sommetDepart))
            {
                Console.Write(sommet.toString() + " ");
            }
            Console.WriteLine();
        }
        public void BFStoString(int sommetDepart)
        {
            Console.Write("Parcours en Largeur (BFS):  ");
            foreach (Noeud sommet in BFS(sommetDepart))
            {
                Console.Write(sommet.toString() + " ");
            }
            Console.WriteLine();
        }
        public void DFSRécursiftoString()
        {
            Console.Write("Parcours en Profondeur (DFS récursif): ");
            foreach (Noeud sommet in DFSRécursif())
            {
                Console.Write(sommet.toString() + " ");
            }
            Console.WriteLine();
        }
        public void AfficherDansLordre()
        {
            Console.WriteLine("Liste d'adjacence: ");
            foreach (KeyValuePair<int, List<int>> sommet in listeAdjacence.OrderBy(x => x.Key)) // Pour afficher dans l'ordre
            {
                Console.Write(sommet.Key + ": ");
                foreach (int voisin in sommet.Value.OrderBy(x => x))
                {
                    Console.Write(voisin + " ");
                }
                Console.WriteLine();
            }
        }
        public void AfficherMatrice()
        {
            Console.WriteLine("Matrice d'adjacence: ");
            Console.WriteLine("     1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34");
            Console.WriteLine("   ------------------------------------------------------------------------------------------------------");
            for (int i = 0; i < matriceAdjacence.GetLength(0); i++)
            {
                Console.Write($"{i+1,2}" + "| ");
                for (int j = 0; j < matriceAdjacence.GetLength(1); j++)
                {
                    Console.Write($"{matriceAdjacence[i, j],2} ");
                }
                Console.WriteLine();
            }
        }
        public void AfficherNoeuds()
        {
            Console.WriteLine("Liste des noeuds: ");
            foreach (Noeud noeud in noeuds)
            {
                Console.Write(noeud.toString() + " ");
            }
            Console.WriteLine();
        }
        public void EstConnexe()
        {
            HashSet<Noeud> visite = BFS(1);
            if (visite.Count == listeAdjacence.Count)
            {
                Console.WriteLine("Le graphe est connexe.");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas connexe.");
            }
        }
        public void ContientCycle()
        {
            DFSRécursif(true);
        }
    }
    #region ancien code
    /* 
        // Parcours en Largeur (BFS)
        public HashSet<int> ParcoursLargeur(int sommetDepart)
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> visite = new HashSet<int>();

            queue.Enqueue(sommetDepart);
            visite.Add(sommetDepart);

            Console.Write("Parcours en Largeur (BFS):  ");

            while  (queue.Count > 0)
            {
                int sommet = queue.Dequeue();
                Console.Write(sommet + " ");
                foreach (int voisin in listeAdjacence[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                         queue.Enqueue(voisin);
                            visite.Add(voisin);
                        }
                    }
            }
            Console.WriteLine();
            return visite;
        }
        // Parcours en Profondeur (DFS) avec pile
        public void ParcoursProfondeur(int sommetDepart)
        {
            Stack<int> pile = new Stack<int>();
            HashSet<int> visite = new HashSet<int>();

            pile.Push(sommetDepart);

            Console.Write("Parcours en Profondeur (DFS): ");

            while (pile.Count > 0)
            {
                int sommet = pile.Pop();

                if (!visite.Contains(sommet))
                {
                    Console.Write(sommet + " ");
                    visite.Add(sommet);
                }
                foreach (int voisin in listeAdjacence[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                            pile.Push(voisin);
                        }
                    }
            }
            Console.WriteLine();
        }

        // Parcours en Profondeur (DFS) récursif BON
        public HashSet<Noeud> DFSRecursif(int sommetDepart, bool rechercheCycle = false)
        {
            Dictionary<int, int> couleurs = new Dictionary<int, int>();
            HashSet<Noeud> visite = new HashSet<Noeud>();
            bool cycle = false;
            // Initialisation des couleurs (tous les sommets sont blancs)
            foreach (int sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            void Explorer(int sommet)
            {
                if (rechercheCycle && couleurs[sommet] == 1)
                {
                    Console.WriteLine("Cycle détecté.");
                    cycle = true;
                    return;
                }

                couleurs[sommet] = 1; // jaune
                foreach (int voisin in listeAdjacence[sommet].OrderBy(x => x))
                {
                    if (couleurs[voisin] == 0) // blanc
                    {
                        Explorer(voisin);
                    }
                }
                couleurs[sommet] = 2; // rouge
                visite.Add(new Noeud(sommet));
            }

            // Lancer l'exploration à partir du sommet de départ
            Explorer(sommetDepart);

            if (!cycle && rechercheCycle)
            {
                Console.WriteLine("Pas de cycle détecté.");
            }

            return visite;
        }
        //Affichage du parcours en profondeur récursif
        public void DFSRecursiftoString()
        {
            Console.Write("Parcours en Profondeur (DFS recursif): ");
            foreach (Noeud sommet in DFSRecursif(1))
            {
                Console.Write(sommet.toString() + " ");
            }
            Console.WriteLine();
        }
    */
    #endregion
}
