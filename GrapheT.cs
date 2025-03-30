using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Graphe<T>
    {
        private Dictionary<T, List<T>> listeAdjacence;
        private Dictionary<T, int> couleurs;
        private List<T> stations;
        private Dictionary<T, int> poidsAretes;

        public Graphe(Dictionary<T, List<T>> adjacence) //Graphe non pondéré
        {
            listeAdjacence = adjacence;
        }
        public Graphe(Dictionary<T, int> poidsAretes) // Graphe pondéré
        {
            this.poidsAretes = poidsAretes;
        }
        public Graphe(List<T> stationss) // Graphe pondéré
        {
            this.stations = stations;           // On ne peut pas avoir poidsAretes et stations en même temps car ce sont deux types de graphes différents
        }
        #region Propriétés
        public Dictionary<T, List<T>> ListeAdjacence
        {
            get { return listeAdjacence; }
            set { listeAdjacence = value; }
        }
        public Dictionary<T, int> PoidsAretes
        {
            get { return poidsAretes; }
            set { poidsAretes = value; }
        }
        public Dictionary<T, int> Couleurs
        {
            get { return couleurs; }
        }
        #endregion
        public HashSet<T> BFS(T sommetDepart)
        {
            couleurs = new Dictionary<T, int>();
            Queue<T> file = new Queue<T>();
            HashSet<T> visite = new HashSet<T>();

            foreach (T sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            file.Enqueue(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune

            while (file.Count > 0)
            {
                T sommet = file.Dequeue();
                foreach (T voisin in listeAdjacence[sommet])
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

        public HashSet<T> DFS(T sommetDepart)
        {
            couleurs = new Dictionary<T, int>();
            Stack<T> pile = new Stack<T>();
            HashSet<T> visite = new HashSet<T>();

            foreach (T sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            pile.Push(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune

            while (pile.Count > 0)
            {
                T sommet = pile.Peek();
                bool aExploréUnVoisin = false;

                foreach (T voisin in listeAdjacence[sommet].OrderBy(x => x))
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

        public HashSet<T> DFSRécursif(bool rechercheCycle = false)
        {
            couleurs = new Dictionary<T, int>();
            HashSet<T> visite = new HashSet<T>();

            foreach (T sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            foreach (T sommet in listeAdjacence.Keys)
            {
                if (couleurs[sommet] == 0)
                {
                    DFSrec(sommet, visite, rechercheCycle);
                }
            }

            return visite;
        }

        private void DFSrec(T sommet, HashSet<T> visite, bool rechercheCycle)
        {
            couleurs[sommet] = 1; // jaune
            visite.Add(sommet);

            foreach (T voisin in listeAdjacence[sommet])
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
        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Dijkstra
        // 
        public Dictionary<T, int> Dijkstra(T sommetDepart, Dictionary<Arete, int> poidsAretes )   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
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
        }
        public Dictionary<T, int> Dijkstra2(T sommetDepart, Dictionary<string, double> VitesseMoyenne)   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
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
                        // On recalcule la distance entre sommetActuel et voisin avec la méthode CalculerTempsTrajet de la classe Arete
                        Arete arete = new Arete((Station)(object)sommetActuel, (Station)(object)voisin); // On cast les sommets en Station pour utiliser la classe Arete
                        arete.CalculerTempsTrajet(VitesseMoyenne); // On calcule le temps de trajet entre les deux stations
                        int nouvelleDistance = distances[sommetActuel] + arete.Temps; // On cast les sommets en Station pour utiliser la classe Arete
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
        }
        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Bellman-Ford
        public Dictionary<T, int> BellmanFord(T sommetDepart)
        {
            Dictionary<T, int> distances = new Dictionary<T, int>();
            HashSet<T> visites = new HashSet<T>();

            foreach (T sommet in listeAdjacence.Keys)
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
        public Dictionary<T, Dictionary<T, int>> FloydWarshall()
        {
            Dictionary<T, Dictionary<T, int>> distances = new Dictionary<T, Dictionary<T, int>>();

            foreach (T sommet in listeAdjacence.Keys)
            {
                distances[sommet] = new Dictionary<T, int>();
                foreach (T voisin in listeAdjacence.Keys)
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
    }
}