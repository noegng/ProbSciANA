using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pb_Sci_Etape_1
{
    public class Graphe
    {
        private Dictionary<int, List<int>> listeAdjacence;
        private int[,] matriceAdjacence;

        public Graphe(Dictionary<int, List<int>> adjacence)
        {
            listeAdjacence = adjacence;
        }
        public Graphe(int[,] matriceAdjacence)
        {
            this.matriceAdjacence = matriceAdjacence;
        }
        public void ParcoursLargeurMatrice(int sommetDepart)
        {
            Queue<int> file = new Queue<int>();
            HashSet<int> visite = new HashSet<int>();

            file.Enqueue(sommetDepart);
            visite.Add(sommetDepart);

            Console.Write("Parcours en Largeur (BFS):  ");

            while (file.Count > 0)
            {
                int sommet = file.Dequeue();
                Console.Write(sommet + " ");
                for (int i = 0; i < matriceAdjacence.GetLength(1); i++)
                {
                    if (matriceAdjacence[sommet, i] == 1 && !visite.Contains(i))
                    {
                        file.Enqueue(i);
                        visite.Add(i);
                    }
                }
            }
            Console.WriteLine();
        }
        public void ParcoursProfondeurMatrice(int sommetDepart)
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

                for (int i = 0; i < matriceAdjacence.GetLength(1); i++)
                {
                    if (matriceAdjacence[sommet, i] == 1 && !visite.Contains(i))
                    {
                        pile.Push(i);
                    }
                }
            }
            Console.WriteLine();
        }
        
        // Parcours en Largeur (BFS)
        public void ParcoursLargeur(int sommetDepart)
        {
            Queue<int> file = new Queue<int>();
            HashSet<int> visite = new HashSet<int>();

            file.Enqueue(sommetDepart);
            visite.Add(sommetDepart);

            Console.Write("Parcours en Largeur (BFS):  ");

            while (file.Count > 0)
            {
                int sommet = file.Dequeue();
                Console.Write(sommet + " ");
                if(listeAdjacence != null)
                {
                    foreach (int voisin in listeAdjacence[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                            file.Enqueue(voisin);
                            visite.Add(voisin);
                        }
                    }
                }else{
                    for (int i = 0; i < matriceAdjacence.GetLength(1); i++)
                    {
                        if (matriceAdjacence[sommet, i] == 1 && !visite.Contains(i))
                        {
                            file.Enqueue(i);
                            visite.Add(i);
                        }
                    }
                }
            }
            Console.WriteLine();
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
                if(listeAdjacence != null)
                {
                    foreach (int voisin in listeAdjacence[sommet])
                    {
                        if (!visite.Contains(voisin))
                        {
                            pile.Push(voisin);
                        }
                    }
                }else{
                    for (int i = matriceAdjacence.GetLength(1)-1; i >= 0; i--)
                    {
                        if (matriceAdjacence[sommet, i] == 1 && !visite.Contains(i))
                        {
                            pile.Push(i);
                        }
                    }
                }
            }
            Console.WriteLine();
        }
        public void AfficherDansLordre()
        {
            Console.WriteLine("Liste d'adjacence: ");
            foreach (KeyValuePair<int, List<int>> sommet in listeAdjacence.OrderBy(x => x.Key))
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
    }
}
