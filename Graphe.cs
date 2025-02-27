using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciAna
{
    internal class Graphe
    {
        private Dictionary<int, List<int>> listeAdjacence;

        public Graphe(Dictionary<int, List<int>> adjacence)
        {
            listeAdjacence = adjacence;
        }

        // Parcours en Largeur (BFS)
        public void ParcoursLargeur(int sommetDepart)
        {
            Queue<int> file = new Queue<int>();
            HashSet<int> visite = new HashSet<int>();

            file.Enqueue(sommetDepart);
            visite.Add(sommetDepart);

            Console.Write("BFS: ");

            while (file.Count > 0)
            {
                int sommet = file.Dequeue();
                Console.Write(sommet + " ");

                foreach (int voisin in listeAdjacence[sommet])
                {
                    if (!visite.Contains(voisin))
                    {
                        file.Enqueue(voisin);
                        visite.Add(voisin);
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

            Console.Write("DFS: ");

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
    }
}
