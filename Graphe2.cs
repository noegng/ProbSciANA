using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Graphe2
    {
        private Dictionary<Station, List<Station>> listeAdjacence;
        
        private Dictionary<Station, int> couleurs;
        private List<Arete> aretes;

        public Graphe2(List<Arete> aretes) //Graphe non pondéré
        {
            this.aretes = aretes;
            listeAdjacence = new Dictionary<Station, List<Station>>();
            RemplissageListeAdjacence(aretes);
        }
        #region Propriétés
        public Dictionary<Station, List<Station>> ListeAdjacence
        {
            get { return listeAdjacence; }
            set { listeAdjacence = value; }
        }
        public Dictionary<Station, int> Couleurs
        {
            get { return couleurs; }
        }
        #endregion
        public HashSet<Station> BFS(Station sommetDepart)
        {
            couleurs = new Dictionary<Station, int>();
            Queue<Station> file = new Queue<Station>();
            HashSet<Station> visite = new HashSet<Station>();

            foreach (Station sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            file.Enqueue(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune

            while (file.Count > 0)
            {
                Station sommet = file.Dequeue();
                foreach (Station voisin in listeAdjacence[sommet])
                {
                    if (couleurs[voisin] == 0) // blanc
                    {
                        file.Enqueue(voisin);
                        couleurs[voisin] = 1; // jaune
                    }
                }
                couleurs[sommet] = 2; // rouge
                visite.Add(sommet);
            }

            return visite;
        }

        public HashSet<Station> DFS(Station sommetDepart)
        {
            couleurs = new Dictionary<Station, int>();
            Stack<Station> pile = new Stack<Station>();
            HashSet<Station> visite = new HashSet<Station>();

            foreach (Station sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            pile.Push(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune

            while (pile.Count > 0)
            {
                Station sommet = pile.Peek();
                bool aExploréUnVoisin = false;

                foreach (Station voisin in listeAdjacence[sommet].OrderBy(x => x))
                {
                    if (couleurs[voisin] == 0) // blanc
                    {
                        pile.Push(voisin);
                        couleurs[voisin] = 1; // jaune
                        aExploréUnVoisin = true;
                        break;
                    }
                }

                if (!aExploréUnVoisin)
                {
                    couleurs[sommet] = 2; // rouge
                    pile.Pop();
                    visite.Add(sommet);
                }
            }

            return visite;
        }

        public HashSet<Station> DFSRécursif(bool rechercheCycle = false)
        {
            couleurs = new Dictionary<Station, int>();
            HashSet<Station> visite = new HashSet<Station>();

            foreach (Station sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            foreach (Station sommet in listeAdjacence.Keys)
            {
                if (couleurs[sommet] == 0)
                {
                    DFSrec(sommet, visite, rechercheCycle);
                }
            }

            return visite;
        }

        private void DFSrec(Station sommet, HashSet<Station> visite, bool rechercheCycle)
        {
            couleurs[sommet] = 1; // jaune
            visite.Add(sommet);

            foreach (Station voisin in listeAdjacence[sommet])
            {
                if (couleurs[voisin] == 0)
                {
                    DFSrec(voisin, visite, rechercheCycle);
                }
                else if (rechercheCycle && couleurs[voisin] == 1)
                {
                    Console.WriteLine("Cycle détecté.");
                    return;
                }
            }

            couleurs[sommet] = 2; // rouge
        }

        public void AfficherListeAdjacence()
        {
            Console.WriteLine("Liste d'adjacence:");
            foreach (var sommet in listeAdjacence.OrderBy(x => x.Key))
            {
                Console.Write($"{sommet.Key}: ");
                foreach (var voisin in sommet.Value.OrderBy(x => x))
                {
                    Console.Write($"{voisin} ");
                }
                Console.WriteLine();
            }
        }

        public void EstConnexe()
        {
            var visite = BFS(listeAdjacence.Keys.First());
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
        
        public void RemplissageListeAdjacence(List<Arete> aretes)
        {
            foreach (Arete arete in aretes)
            {
                if (listeAdjacence.ContainsKey(arete.IdPrevious))
                {
                    listeAdjacence[arete.IdPrevious].Add(arete.IdNext);
                }
                else
                {
                    listeAdjacence.Add(arete.IdPrevious, new List<Station> { arete.IdNext });
                }
            }
        }
        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Dijkstra
        public Dictionary<Station, int> Dijkstra(Station sommetDepart, Dictionary<Arete, int> poidsAretes)   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
        {
            Dictionary<Station, int> distances = new Dictionary<Station, int>();
            HashSet<Station> visites = new HashSet<Station>();
            PriorityQueue<Station, int> filePriorite = new PriorityQueue<Station, int>(); // On utilise une priority queue pour gérer les sommets à explorer

            foreach (Station sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue; // On initialise les distances à l'infini
            }

            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                Station sommetActuel = filePriorite.Dequeue(); // On prend le sommet avec la distance la plus courte
                visites.Add(sommetActuel); 

                foreach (Station voisin in listeAdjacence[sommetActuel]) // On parcourt les voisins du sommet actuel
                {
                    if (!visites.Contains(voisin))
                    {
                        // On met à jour la distance si on trouve un chemin plus court
                        // On suppose que les poids des arêtes sont stockés dans un dictionnaire avec la clé étant le couple (sommetActuel, voisin)
                        // et la valeur étant le poids de l'arête entre ces deux sommets

                        int nouvelleDistance = distances[sommetActuel] + poidsAretes[new Arete(sommetActuel, voisin)]; // On cast les sommets en Station pour utiliser la classe Arete
                        // On peut aussi utiliser la méthode CalculerDistance() de la classe Arete si on a besoin de calculer la distance entre deux stations
                        // Il faut identifier le poids de l'arête entre sommetActuel et voisin
                        //int nouvelleDistance = distances[sommetActuel] + poidsAretes[new Arete(sommetActuel, voisin)]; // On cast les sommets en Station pour utiliser la classe Arete
                        if (nouvelleDistance < distances[voisin])
                        {
                            distances[voisin] = nouvelleDistance;
                            filePriorite.Enqueue(voisin, nouvelleDistance);
                        }
                    }
                }
            }

            return distances;
        }
        public Dictionary<Station, int> Dijkstra2(Station sommetDepart)   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
        {
            Dictionary<Station, int> distances = new Dictionary<Station, int>();
            HashSet<Station> visites = new HashSet<Station>();
            PriorityQueue<Station, int> filePriorite = new PriorityQueue<Station, int>(); // On utilise une priority queue pour gérer les sommets à explorer

            foreach (Station sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue; // On initialise les distances à l'infini
            }

            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                Station sommetActuel = filePriorite.Dequeue(); // On prend le sommet avec la distance la plus courte
                visites.Add(sommetActuel); 

                foreach (Arete voisin in aretes) // On parcourt les voisins du sommet actuel
                {
                    if (!visites.Contains(voisin.IdNext))
                    {
                        // On met à jour la distance si on trouve un chemin plus court
                        // On suppose que les poids des arêtes sont stockés dans un dictionnaire avec la clé étant le couple (sommetActuel, voisin)
                        // et la valeur étant le poids de l'arête entre ces deux sommets

                        int nouvelleDistance = distances[sommetActuel] + voisin.Temps; // On cast les sommets en Station pour utiliser la classe Arete
                        // On peut aussi utiliser la méthode CalculerDistance() de la classe Arete si on a besoin de calculer la distance entre deux stations
                        if (nouvelleDistance < distances[voisin.IdNext])
                        {
                            distances[voisin.IdNext] = nouvelleDistance;
                            filePriorite.Enqueue(voisin.IdNext, nouvelleDistance);
                        }
                    }
                }
            }

            return distances;
        }
        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Bellman-Ford
        public Dictionary<Station, int> BellmanFord(Station sommetDepart)
        {
            Dictionary<Station, int> distances = new Dictionary<Station, int>();
            HashSet<Station> visites = new HashSet<Station>();

            foreach (Station sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue;
            }

            distances[sommetDepart] = 0;

            for (int i = 0; i < listeAdjacence.Count - 1; i++)
            {
                foreach (var sommet in listeAdjacence.Keys)
                {
                    foreach (var voisin in listeAdjacence[sommet])
                    {
                        if (distances[sommet] != int.MaxValue && distances[sommet] + 1 < distances[voisin])
                        {
                            distances[voisin] = distances[sommet] + 1;
                        }
                    }
                }
            }

            return distances;
        }
        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Floyd-Warshall
        public Dictionary<Station, Dictionary<Station, int>> FloydWarshall()
        {
            Dictionary<Station, Dictionary<Station, int>> distances = new Dictionary<Station, Dictionary<Station, int>>();

            foreach (Station sommet in listeAdjacence.Keys)
            {
                distances[sommet] = new Dictionary<Station, int>();
                foreach (Station voisin in listeAdjacence.Keys)
                {
                    if (sommet.Equals(voisin))
                    {
                        distances[sommet][voisin] = 0;
                    }
                    else
                    {
                        distances[sommet][voisin] = int.MaxValue;
                    }
                }
            }

            foreach (var sommet in listeAdjacence.Keys)
            {
                foreach (var voisin in listeAdjacence[sommet])
                {
                    distances[sommet][voisin] = 1; // On suppose que chaque arête a un poids de 1
                }
            }

            foreach (var k in listeAdjacence.Keys)
            {
                foreach (var i in listeAdjacence.Keys)
                {
                    foreach (var j in listeAdjacence.Keys)
                    {
                        if (distances[i][k] != int.MaxValue && distances[k][j] != int.MaxValue && distances[i][k] + distances[k][j] < distances[i][j])
                        {
                            distances[i][j] = distances[i][k] + distances[k][j];
                        }
                    }
                }
            }

            return distances;
        }
        /*
        public Dictionary<T, int> DijkstraAlex(T sommetDepart)   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
        {
            Dictionary<T, int> distances = new Dictionary<T, int>();
            HashSet<T> visites = new HashSet<T>();
            PriorityQueue<T, int> filePriorite = new PriorityQueue<T, int>(); // On utilise une priority queue pour gérer les sommets à explorer

            foreach (T sommet in stations)
            {
                distances[sommet] = int.MaxValue; // On initialise les distances à l'infini
            }

            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                T sommetActuel = filePriorite.Dequeue(); // On prend le sommet avec la distance la plus courte
                visites.Add(sommetActuel); 

                foreach (T voisin in listeAdjacence[sommetActuel]) // On parcourt les voisins du sommet actuel
                {
                    if (!visites.Contains(voisin))
                    {
                        // On met à jour la distance si on trouve un chemin plus court
                        // On suppose que les poids des arêtes sont stockés dans un dictionnaire avec la clé étant le couple (sommetActuel, voisin)
                        // et la valeur étant le poids de l'arête entre ces deux sommets
                        int nouvelleDistance = distances[sommetActuel] + poidsAretes[new Arete((Station)(object)sommetActuel, (Station)(object)voisin)]; // On cast les sommets en Station pour utiliser la classe Arete
                        // On peut aussi utiliser la méthode CalculerDistance() de la classe Arete si on a besoin de calculer la distance entre deux stations
                        if (nouvelleDistance < distances[voisin])
                        {
                            distances[voisin] = nouvelleDistance;
                            filePriorite.Enqueue(voisin, nouvelleDistance);
                        }
                    }
                }
            }
            return distances;
        }*/
    }
}