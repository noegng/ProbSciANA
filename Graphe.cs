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
        public Graphe(int[,] adjacence)
        {
            matriceAdjacence = adjacence;
        }
        public void Afficher()
        {
            foreach (KeyValuePair<int, List<int>> sommet in listeAdjacence)
            {
                Console.Write(sommet.Key + ": ");
                foreach (int voisin in sommet.Value)
                {
                    Console.Write(voisin + " ");
                }
                Console.WriteLine();
            }
        }
        public void AfficherDansLordre()
        {
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
            for (int i = 0; i < matriceAdjacence.GetLength(0); i++)
            {
                for (int j = 0; j < matriceAdjacence.GetLength(1); j++)
                {
                    Console.Write(matriceAdjacence[i, j] + " ");
                }
                Console.WriteLine();
            }
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
    }
}
