using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Graphe<T> // Graphe non orienté
    {
        private Dictionary<T, List<T>> listeAdjacence;
        private int[,] matriceAdjacence;
        private Dictionary<T, int> couleurs;
        private List<Arete> aretes;
        private int nbCycles = -1; //Détecter une erreur de cycle | on déclare la variable ici pour que l'incrémentation se fasse dans la méthode récursive DFS (sinon impossible de l'incrémenter)
        //private Dictionary<T, int> poidsAretes;
        public Graphe(List<Arete> aretes) //Graphe non pondéré
        {
            this.aretes = aretes;
            listeAdjacence = new Dictionary<T, List<T>>();
            RemplissageListeAdjacence(aretes);
            matriceAdjacence = new int[listeAdjacence.Count,listeAdjacence.Count]; // 248 stations
            RemplissageMatriceAdjacence();
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
        public int[,] MatriceAdjacence{
            get { return matriceAdjacence; }
            set { matriceAdjacence = value; }
        }
        public List<Arete> Aretes{
            get { return aretes; }
            set { aretes = value; }
        }
        #endregion
        #region Méthodes de parcours
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
            nbCycles = 0;
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
                    nbCycles = nbCycles + 1;
                    return;
                }
            }

            couleurs[sommet] = 2; // rouge
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
            Console.WriteLine(nbCycles + " cycles trouvés dans le graphe.");
        }
        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Dijkstra
        // 
        
        public void RemplissageListeAdjacence(List<Arete> aretes)
        {
            foreach (Arete arete in aretes)
            {
                if (listeAdjacence.Count == 0)
                {
                    listeAdjacence.Add(arete.IdPrevious, new List<Station> { arete.IdNext }); //on fait arete.IdPrevious.Id pour avoir le nom de la station (après vérification toutes les stations se trouvent dans arete.IdPrevious.Id)
                }
                else if (listeAdjacence.ContainsKey(arete.IdPrevious))
                {
                    listeAdjacence[arete.IdPrevious].Add(arete.IdNext);
                }
                else
                {
                    listeAdjacence.Add(arete.IdPrevious, new List<Station> { arete.IdNext });
                }
            }
        }
        public void AfficherListeAdjacence()
        {
            Console.WriteLine("Liste d'adjacence:");
            foreach (var sommet in listeAdjacence)
            {
                Console.Write($"{sommet.Key.ToString() + " -> ",36} ");
                int i = 0;
                foreach (var voisin in sommet.Value)
                {
                    if (i != 0)
                    {
                        Console.Write(", ");
                    }
                    i++;
                    Console.Write(voisin.ToString());
                }
                Console.WriteLine();
            }
        }
        public void RemplissageMatriceAdjacence()
        {
            foreach (Arete arete in aretes)
            {
                if ( arete.IdPrevious != null && arete.IdNext != null)
                {
                    matriceAdjacence[Convert.ToInt32(arete.IdPrevious.Id)-1, Convert.ToInt32(arete.IdNext.Id)-1] = 1; // -1 car les station commencent à 1                   
                }
            }
        }
        public void AfficherMatriceAdjacence()
        {
            Console.WriteLine("Matrice d'adjacence:");
            foreach (var sommet in listeAdjacence.Keys)
            {
                Console.Write($"{sommet.Id,3} ");
            }
            foreach (var sommet in listeAdjacence.Keys)
            {
                Console.Write($"{"---",3} ");
            }
            Console.WriteLine();
            for (int i = 0; i < matriceAdjacence.GetLength(0); i++)
            {
                Console.Write($"{i + 1,3} | " );
                for (int j = 0; j < matriceAdjacence.GetLength(1); j++)
                {
                    Console.Write($"{matriceAdjacence[i, j],3}");
                }
                Console.WriteLine();
            }
        }
        #endregion
        #region Méthodes de recherche de chemin
        public Dictionary<T, int> Dijkstra(T sommetDepart, Dictionary<Arete, int> poidsAretes )   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
        {
            Dictionary<T, int> distances = new Dictionary<T, int>();
            HashSet<T> visites = new HashSet<T>();
            PriorityQueue<T, int> filePriorite = new PriorityQueue<T, int>(); // On utilise une priority queue pour gérer les sommets à explorer
            string idLignePrécédent = ""; // On initialise l'id de la ligne précédente à une chaîne vide

            foreach (T sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue; // On initialise les distances à l'infini
            }

            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                T sommetActuel = filePriorite.Dequeue(); // On prend le sommet avec la distance la plus courte
                visites.Add(sommetActuel); 

                foreach (Arete voisin in aretes) // On parcourt les voisins du sommet actuel
                {
                    /*
                    if (!visites.Contains(voisin))
                    {
                        // On met à jour la distance si on trouve un chemin plus court
                        // On suppose que les poids des arêtes sont stockés dans un dictionnaire avec la clé étant le couple (sommetActuel, voisin)
                        // et la valeur étant le poids de l'arête entre ces deux sommets
                        int nouvelleDistance = distances[sommetActuel] + poidsAretes[new Arete((Station)(object)sommetActuel, (Station)(object)voisin, "1")]; // On cast les sommets en Station pour utiliser la classe Arete
                        // On peut aussi utiliser la méthode CalculerDistance() de la classe Arete si on a besoin de calculer la distance entre deux stations
                        if (nouvelleDistance < distances[voisin])
                        {
                            distances[voisin] = nouvelleDistance;
                            filePriorite.Enqueue(voisin, nouvelleDistance);
                        }
                    }
                    */
                    if (voisin.IdPrevious == sommetActuel) // On vérifie si le voisin est bien un voisin du sommet actuel
                    { 
                        int tempsChangement = 0;
                        if (idLignePrécédent != voisin.IdLigne) // On vérifie si on change de ligne
                        {
                            tempsChangement = voisin.IdPrevious.TempsChangement; // On met à jour le temps de changement
                        }

                        if (!visites.Contains(voisin.IdNext)) // On vérifie si le voisin n'a pas déjà été visité
                        {
                            // On met à jour la distance si on trouve un chemin plus court
                            int nouvelleDistance = distances[sommetActuel] + voisin.Temps + tempsChangement; // On cast les sommets en Station pour utiliser la classe Arete
                            if (nouvelleDistance < distances[voisin.IdNext])
                            {
                                distances[voisin.IdNext] = nouvelleDistance;
                                filePriorite.Enqueue(voisin.IdNext, nouvelleDistance);
                            }
                        }
                        idLignePrécédent = voisin.IdLigne; // On mémorise l'id de la ligne pour le prochain sommet
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
                        Arete arete = new Arete((Station)(object)sommetActuel, (Station)(object)voisin, "1"); // On cast les sommets en Station pour utiliser la classe Arete
                        arete.CalculerTempsTrajet(); // On calcule le temps de trajet entre les deux stations
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