using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.IO;

namespace ProbSciANA
{
    public class Graphe<T> /// Graphe de noeuds
    {
        private Dictionary<Noeud<T>, List<Noeud<T>>> listeAdjacence;
        private int[,] matriceAdjacence;
        private List<Noeud<T>> noeuds;
        private List<Noeud<T>> noeudsIsolés;
        private Dictionary<Noeud<T>, int> couleurs;
        private List<Arc<T>> arcs;
        private int nbCycles = -1; /// étecter une erreur de cycle | on déclare la variable ici pour que l'incrémentation se fasse dans la méthode récursive DFS (sinon impossible de l'incrémenter)

        private static readonly string ProjectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));


        public Graphe(List<Arc<T>> arcs, List<Noeud<T>> noeudsIsolés = null)
        {
            this.arcs = arcs;
            this.noeudsIsolés = noeudsIsolés;
            listeAdjacence = new Dictionary<Noeud<T>, List<Noeud<T>>>();
            noeuds = new List<Noeud<T>>();
            RemplissageListeAdjacence();
            int nbNoeudsIsolés = 0;
            if (noeudsIsolés != null)
            {
                nbNoeudsIsolés = noeudsIsolés.Count;
            }
            matriceAdjacence = new int[listeAdjacence.Count + nbNoeudsIsolés, listeAdjacence.Count + nbNoeudsIsolés];
            RemplissageMatriceAdjacence();
            couleurs = new Dictionary<Noeud<T>, int>();
            noeuds.AddRange(listeAdjacence.Keys); /// Remplie la liste des noeuds
            foreach (Noeud<T> noeud in noeuds)
            {
                couleurs.Add(noeud, 0);     /// On remplie les couleurs 0 = couleur initiale.
            }
        }
        #region Propriétés
        public Dictionary<Noeud<T>, List<Noeud<T>>> ListeAdjacence
        {
            get { return listeAdjacence; }
            set { listeAdjacence = value; }
        }
        public Dictionary<Noeud<T>, int> Couleurs
        {
            get { return couleurs; }
        }
        public List<Noeud<T>> NoeudsIsolés
        {
            get { return noeudsIsolés; }
            set { noeudsIsolés = value; }
        }
        public int[,] MatriceAdjacence
        {
            get { return matriceAdjacence; }
            set { matriceAdjacence = value; }
        }
        public List<Arc<T>> Arcs
        {
            get { return arcs; }
            set { arcs = value; }
        }
        public List<Noeud<T>> Noeuds
        {
            get { return noeuds; }
            set { noeuds = value; }
        }
        #endregion
        #region Méthodes de parcours
        public HashSet<Noeud<T>> BFS(Noeud<T> sommetDepart)
        {
            couleurs = new Dictionary<Noeud<T>, int>();
            Queue<Noeud<T>> file = new Queue<Noeud<T>>();
            HashSet<Noeud<T>> visite = new HashSet<Noeud<T>>();

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; /// blanc
            }

            file.Enqueue(sommetDepart);
            couleurs[sommetDepart] = 1; /// jaune

            while (file.Count > 0)
            {
                Noeud<T> sommet = file.Dequeue();
                foreach (Noeud<T> voisin in listeAdjacence[sommet])
                {
                    if (couleurs[voisin] == 0) /// blanc
                    {
                        file.Enqueue(voisin);
                        couleurs[voisin] = 1; /// jaune
                    }
                }
                couleurs[sommet] = 2; /// rouge
                visite.Add(sommet);
            }

            return visite;
        }

        public HashSet<Noeud<T>> DFS(Noeud<T> sommetDepart)
        {
            couleurs = new Dictionary<Noeud<T>, int>();
            Stack<Noeud<T>> pile = new Stack<Noeud<T>>();
            HashSet<Noeud<T>> visite = new HashSet<Noeud<T>>();

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; /// blanc
            }

            pile.Push(sommetDepart);
            couleurs[sommetDepart] = 1; /// jaune

            while (pile.Count > 0)
            {
                Noeud<T> sommet = pile.Peek();
                bool aExploréUnVoisin = false;

                foreach (Noeud<T> voisin in listeAdjacence[sommet])
                {
                    if (couleurs[voisin] == 0) /// blanc
                    {
                        pile.Push(voisin);
                        couleurs[voisin] = 1; /// jaune
                        aExploréUnVoisin = true;
                        break;
                    }
                }

                if (!aExploréUnVoisin)
                {
                    couleurs[sommet] = 2; /// rouge
                    pile.Pop();
                    visite.Add(sommet);
                }
            }

            return visite;
        }

        public HashSet<Noeud<T>> DFSRécursif(bool rechercheCycle = false)
        {
            couleurs = new Dictionary<Noeud<T>, int>();
            HashSet<Noeud<T>> visite = new HashSet<Noeud<T>>();
            nbCycles = 0;
            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; /// blanc
            }

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                if (couleurs[sommet] == 0)
                {
                    DFSrec(sommet, visite, rechercheCycle);
                }
            }

            return visite;
        }

        private void DFSrec(Noeud<T> sommet, HashSet<Noeud<T>> visite, bool rechercheCycle)
        {
            couleurs[sommet] = 1; /// jaune
            visite.Add(sommet);

            foreach (Noeud<T> voisin in listeAdjacence[sommet])
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

            couleurs[sommet] = 2; /// rouge
        }

        public bool EstConnexe()
        {
            var visite = BFS(listeAdjacence.Keys.First());
            bool estConnexe = false;
            if (visite.Count == listeAdjacence.Count)
            {
                Console.WriteLine("Le graphe est connexe.");
                estConnexe = true;
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas connexe.");
            }
            return estConnexe;
        }

        public bool ContientCycle()
        {
            bool contientCycle = false;
            DFSRécursif(true);
            Console.WriteLine(nbCycles + " cycles trouvés dans le graphe.");
            if (nbCycles > 0)
            {
                contientCycle = true;
            }
            return contientCycle;
        }
        public void DFStoString(Noeud<T> sommetDepart)
        {
            Console.Write("Parcours en Profondeur (DFS): ");
            foreach (Noeud<T> sommet in DFS(sommetDepart))
            {
                Console.Write(sommet.ToString() + " ; ");
            }
            Console.WriteLine();
        }
        public void BFStoString(Noeud<T> sommetDepart)
        {
            Console.Write("Parcours en Largeur (BFS):  ");
            foreach (Noeud<T> sommet in BFS(sommetDepart))
            {
                Console.Write(sommet.ToString() + " ; ");
            }
            Console.WriteLine();
        }
        public void DFSRécursiftoString()
        {
            Console.Write("Parcours en Profondeur (DFS récursif): ");
            foreach (Noeud<T> sommet in DFSRécursif())
            {
                Console.Write(sommet.ToString() + " ; ");
            }
            Console.WriteLine();
        }
        public void RemplissageListeAdjacence()
        {
            foreach (Arc<T> arc in arcs)
            {
                if (!listeAdjacence.ContainsKey(arc.IdPrevious))
                {
                    listeAdjacence.Add(arc.IdPrevious, new List<Noeud<T>>());
                }
                if (!listeAdjacence[arc.IdPrevious].Contains(arc.IdNext))
                {
                    listeAdjacence[arc.IdPrevious].Add(arc.IdNext);
                }

                if (!arc.SensUnique)
                {
                    if (!listeAdjacence.ContainsKey(arc.IdNext))
                    {
                        listeAdjacence.Add(arc.IdNext, new List<Noeud<T>>());
                    }
                    if (!listeAdjacence[arc.IdNext].Contains(arc.IdPrevious))
                    {
                        listeAdjacence[arc.IdNext].Add(arc.IdPrevious);
                    }
                }
            }
            if (noeudsIsolés != null)
            {
                foreach (Noeud<T> noeud in noeudsIsolés)
                {
                    listeAdjacence.Add(noeud, new List<Noeud<T>>());
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
            foreach (Arc<T> arc in arcs)
            {
                matriceAdjacence[Convert.ToInt32(arc.IdPrevious.Id) - 1, Convert.ToInt32(arc.IdNext.Id) - 1] = 1; /// -1 car les Noeud<T> commencent à 1                   
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
                Console.Write($"{i + 1,3} | ");
                for (int j = 0; j < matriceAdjacence.GetLength(1); j++)
                {
                    Console.Write($"{matriceAdjacence[i, j],3}");
                }
                Console.WriteLine();
            }
        }
        #endregion
        #region Méthodes de recherche de chemin
        /// <summary>
        /// Renvoie le temps minimal d'un noeud par rapport au sommet de départ.
        /// </summary>
        /// <param name="sommetDepart"></param>
        /// <returns></returns>
        public Dictionary<Noeud<T>, int> Dijkstra(Noeud<T> sommetDepart)
        {
            var distances = new Dictionary<Noeud<T>, int>();
            var visites = new HashSet<Noeud<T>>();
            var filePriorite = new PriorityQueue<Noeud<T>, int>(); /// On utilise une priority queue pour gérer les sommets à explorer
            var predecesseurs = new Dictionary<Noeud<T>, Arc<T>>(); /// Pour suivre les arcs utilisés

            /// On initialise les distances à l'infini
            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue;
            }

            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                Noeud<T> sommetActuel = filePriorite.Dequeue(); /// On prend le sommet avec la distance la plus courte
                visites.Add(sommetActuel);

                /// Parcours des arcs sortants du sommet actuel
                foreach (Arc<T> arc in arcs)
                {
                    if (arc.IdPrevious.Equals(sommetActuel)) /// On vérifie si le voisin est bien un voisin du sommet actuel
                    {
                        Noeud<T> voisin = arc.IdNext;
                        if (visites.Contains(voisin))
                        {
                            continue;
                        }

                        /// Ligne précédente = ligne de l'arc ayant permis d’arriver au sommet actuel
                        string idLignePrecedente = null;
                        if (predecesseurs.ContainsKey(sommetActuel) && predecesseurs[sommetActuel] != null)
                        {
                            idLignePrecedente = predecesseurs[sommetActuel].IdLigne;
                        }

                        int tempsChangement = 0;
                        if (idLignePrecedente != null && idLignePrecedente != arc.IdLigne) /// On vérifie si on change de ligne
                        {
                            tempsChangement = sommetActuel.TempsChangement; /// On met à jour le temps de changement
                        }

                        /// On met à jour la distance si on trouve un chemin plus court
                        int nouvelleDistance = distances[sommetActuel] + arc.Poids + tempsChangement;
                        if (nouvelleDistance < distances[voisin])
                        {
                            distances[voisin] = nouvelleDistance;
                            predecesseurs[voisin] = arc;
                            filePriorite.Enqueue(voisin, nouvelleDistance);
                        }
                    }
                }
            }

            return distances;
        }

        /// Déterminer le chemin le plus court entre deux sommets avec l'algorithme de Dijkstra
        public (List<Arc<T>>, int) DijkstraChemin(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
        {
            var distances = new Dictionary<Noeud<T>, int>();
            var visites = new HashSet<Noeud<T>>();
            var filePriorite = new PriorityQueue<Noeud<T>, int>();
            var predecesseurs = new Dictionary<Noeud<T>, Arc<T>>();

            /// Initialisation des sommet a l'infini.
            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue;
            }
            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                Noeud<T> sommetActuel = filePriorite.Dequeue();
                visites.Add(sommetActuel);
                if (sommetActuel.Equals(sommetArrivee))
                {
                    break;
                }
                /// Parcours des arcs sortants du sommet actuel
                foreach (Arc<T> arc in arcs) /// On parcourt les voisins du sommet actuel
                {
                    if (arc.IdPrevious.Equals(sommetActuel))/// On vérifie si le voisin est bien un voisin du sommet actuel
                    {
                        Noeud<T> voisin = arc.IdNext;
                        if (visites.Contains(voisin))
                        {
                            continue;
                        }
                        /// Ligne précédente = ligne de l'arc ayant permis d'arriver au sommet actuel
                        string idLignePrecedente = null;
                        if (predecesseurs.ContainsKey(sommetActuel) && predecesseurs[sommetActuel] != null)
                        {
                            idLignePrecedente = predecesseurs[sommetActuel].IdLigne;
                        }
                        int tempsChangement = 0;
                        if (idLignePrecedente != null && idLignePrecedente != arc.IdLigne) /// On vérifie si on change de ligne
                        {
                            tempsChangement = sommetActuel.TempsChangement;
                        }
                        int nouvelleDistance = distances[sommetActuel] + arc.Poids + tempsChangement;
                        if (nouvelleDistance < distances[voisin])
                        {
                            distances[voisin] = nouvelleDistance;
                            predecesseurs[voisin] = arc;
                            filePriorite.Enqueue(voisin, nouvelleDistance);
                        }
                    }
                }
            }

            /// Reconstruction du chemin
            List<Arc<T>> cheminAretes = new List<Arc<T>>();
            Noeud<T> courant = sommetArrivee;
            while (predecesseurs.ContainsKey(courant) && predecesseurs[courant] != null)
            {
                Arc<T> arc = predecesseurs[courant];
                cheminAretes.Add(arc);
                courant = arc.IdPrevious.Equals(courant) ? arc.IdNext : arc.IdPrevious;
            }
            cheminAretes.Reverse();
            return (cheminAretes, distances[sommetArrivee]);
        }

        /// Calculer le chemin le plus court entre un sommet de départ et tous les autres sommets avec l'algorithme de Bellman-Ford
        public Dictionary<Noeud<T>, int> BellmanFord(Noeud<T> sommetDepart)
        {
            var distances = new Dictionary<Noeud<T>, int>();
            var predecesseurs = new Dictionary<Noeud<T>, Arc<T>>();

            /// Initialisation des distances et prédécesseurs à l'infini
            foreach (var sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue;
                predecesseurs[sommet] = null;
            }
            distances[sommetDepart] = 0;

            for (int i = 0; i < listeAdjacence.Count - 1; i++)
            {
                foreach (Arc<T> arc in arcs)
                {
                    var noeudPrécédent = arc.IdPrevious;
                    var noeudSuivant = arc.IdNext;
                    if (distances[noeudPrécédent] == int.MaxValue)
                    {
                        continue;
                    }

                    /// Ligne précédente = ligne de l'arc ayant permis d'arriver au sommet actuel
                    string idLignePrecedente = null;
                    if (predecesseurs.ContainsKey(noeudPrécédent) && predecesseurs[noeudPrécédent] != null)
                    {
                        idLignePrecedente = predecesseurs[noeudPrécédent].IdLigne;
                    }
                    int tempsChangement = 0;
                    if (idLignePrecedente != null && idLignePrecedente != arc.IdLigne)
                    {
                        tempsChangement = noeudPrécédent.TempsChangement;
                    }
                    int nouvelleDistance = distances[noeudPrécédent] + arc.Poids + tempsChangement;
                    if (nouvelleDistance < distances[noeudSuivant])
                    {
                        distances[noeudSuivant] = nouvelleDistance;
                        predecesseurs[noeudSuivant] = arc;
                    }
                }
            }
            return distances;
        }
        /// Calculer et Renvoie le chemin le plus court entre deux sommets avec l'algorithme de Bellman-Ford
        public (List<Arc<T>>, int) BellmanFordChemin(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
        {
            var distances = new Dictionary<Noeud<T>, int>();
            var predecesseurs = new Dictionary<Noeud<T>, Arc<T>>();

            /// Initialisation des distances et prédécesseurs à l'infini
            foreach (var sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue;
                predecesseurs[sommet] = null;
            }
            distances[sommetDepart] = 0;

            for (int i = 0; i < listeAdjacence.Count - 1; i++)
            {
                foreach (Arc<T> arc in arcs)
                {
                    var noeudPrécédent = arc.IdPrevious;
                    var noeudSuivant = arc.IdNext;
                    if (distances[noeudPrécédent] == int.MaxValue)
                    {
                        continue;
                    }

                    /// Ligne précédente = ligne de l'arc ayant permis d'arriver au sommet actuel
                    string idLignePrecedente = null;
                    if (predecesseurs.ContainsKey(noeudPrécédent) && predecesseurs[noeudPrécédent] != null)
                    {
                        idLignePrecedente = predecesseurs[noeudPrécédent].IdLigne;
                    }
                    int tempsChangement = 0;
                    if (idLignePrecedente != null && idLignePrecedente != arc.IdLigne)
                    {
                        tempsChangement = noeudPrécédent.TempsChangement;
                    }
                    int nouvelleDistance = distances[noeudPrécédent] + arc.Poids + tempsChangement;
                    if (nouvelleDistance < distances[noeudSuivant])
                    {
                        distances[noeudSuivant] = nouvelleDistance;
                        predecesseurs[noeudSuivant] = arc;
                    }
                }
            }

            /// Reconstruction du chemin
            var chemin = new List<Arc<T>>();
            var courant = sommetArrivee;
            while (predecesseurs[courant] != null)
            {
                var arc = predecesseurs[courant];
                chemin.Add(arc);
                courant = arc.IdPrevious;
            }
            chemin.Reverse();
            return (chemin, distances[sommetArrivee]);
        }

        /// Calculer le chemin le plus court entre deux sommets avec l'algorithme de Floyd-Warshall
        public Dictionary<Noeud<T>, Dictionary<Noeud<T>, int>> FloydWarshall()
        {
            Dictionary<Noeud<T>, Dictionary<Noeud<T>, int>> distances = new Dictionary<Noeud<T>, Dictionary<Noeud<T>, int>>();

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = new Dictionary<Noeud<T>, int>();
                foreach (Noeud<T> voisin in listeAdjacence.Keys)
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
                    distances[sommet][voisin] = 1; /// On suppose que chaque arête a un poids de 1
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
        public Dictionary<Noeud<T>, Dictionary<Noeud<T>, int>> FloydWarshall2()
        {
            var distances = new Dictionary<Noeud<T>, Dictionary<Noeud<T>, int>>();
            var lignes = new Dictionary<(Noeud<T>, Noeud<T>), string>();

            /// Initialisation des distances et lignes
            foreach (var i in listeAdjacence.Keys)
            {
                distances[i] = new Dictionary<Noeud<T>, int>();
                foreach (var j in listeAdjacence.Keys)
                {
                    if (i.Equals(j))
                    {
                        distances[i][j] = 0;
                    }
                    else
                    {
                        distances[i][j] = int.MaxValue;
                    }
                }
            }

            /// Mise à jour avec les arcs existants
            foreach (var arc in arcs)
            {
                distances[arc.IdPrevious][arc.IdNext] = arc.Poids;
                lignes[(arc.IdPrevious, arc.IdNext)] = arc.IdLigne;

                /// Si le graphe est non orienté
                if (!arc.SensUnique)
                {
                    distances[arc.IdNext][arc.IdPrevious] = arc.Poids;
                    lignes[(arc.IdNext, arc.IdPrevious)] = arc.IdLigne;
                }
            }

            /// Algorithme de Floyd-Warshall avec gestion du changement de ligne
            foreach (var k in listeAdjacence.Keys)
            {
                foreach (var i in listeAdjacence.Keys)
                {
                    foreach (var j in listeAdjacence.Keys)
                    {
                        if (distances[i][k] == int.MaxValue || distances[k][j] == int.MaxValue)
                            continue;

                        int tempsChangement = 0;

                        /// On récupère les lignes i → k et k → j si elles existent
                        lignes.TryGetValue((i, k), out string ligneIK);
                        lignes.TryGetValue((k, j), out string ligneKJ);

                        if (ligneIK != null && ligneKJ != null && ligneIK != ligneKJ)
                        {
                            tempsChangement = k.TempsChangement;
                        }

                        int nouvelleDistance = distances[i][k] + distances[k][j] + tempsChangement;

                        if (nouvelleDistance < distances[i][j])
                        {
                            distances[i][j] = nouvelleDistance;
                            lignes[(i, j)] = ligneIK ?? ligneKJ; /// On mémorise la ligne dominante
                        }
                    }
                }
            }

            return distances;
        }
        public (List<Arc<T>>, int) FloydWarshallChemin(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
        {
            var distances = new Dictionary<Noeud<T>, Dictionary<Noeud<T>, int>>();
            var predecesseurs = new Dictionary<Noeud<T>, Dictionary<Noeud<T>, Noeud<T>>>();
            var lignes = new Dictionary<(Noeud<T>, Noeud<T>), string>();

            /// Initialisation
            foreach (var i in listeAdjacence.Keys)
            {
                distances[i] = new Dictionary<Noeud<T>, int>();
                predecesseurs[i] = new Dictionary<Noeud<T>, Noeud<T>>();

                foreach (var j in listeAdjacence.Keys)
                {
                    if (i.Equals(j))
                    {
                        distances[i][j] = 0;
                    }
                    else
                    {
                        distances[i][j] = int.MaxValue;
                    }
                    predecesseurs[i][j] = null;
                }
            }

            /// Initialisation avec les arcs (graphe orienté : un seul sens)
            foreach (var arc in arcs)
            {
                distances[arc.IdPrevious][arc.IdNext] = arc.Poids;
                predecesseurs[arc.IdPrevious][arc.IdNext] = arc.IdPrevious;
                lignes[(arc.IdPrevious, arc.IdNext)] = arc.IdLigne;
            }

            foreach (var k in listeAdjacence.Keys)
            {
                foreach (var i in listeAdjacence.Keys)
                {
                    foreach (var j in listeAdjacence.Keys)
                    {
                        if (distances[i][k] == int.MaxValue || distances[k][j] == int.MaxValue)
                            continue;

                        lignes.TryGetValue((i, k), out string ligneIK);
                        lignes.TryGetValue((k, j), out string ligneKJ);

                        int tempsChangement = 0;
                        if (ligneIK != null && ligneKJ != null && ligneIK != ligneKJ)
                        {
                            tempsChangement = k.TempsChangement;
                        }

                        int nouvelleDistance = distances[i][k] + distances[k][j] + tempsChangement;

                        if (nouvelleDistance < distances[i][j])
                        {
                            distances[i][j] = nouvelleDistance;
                            predecesseurs[i][j] = predecesseurs[k][j];
                            lignes[(i, j)] = ligneIK ?? ligneKJ;
                        }
                    }
                }
            }

            /// Reconstruction du chemin
            List<Noeud<T>> cheminNoeuds = new List<Noeud<T>>();
            Noeud<T> courant = sommetArrivee;

            while (courant != null && !courant.Equals(sommetDepart))
            {
                cheminNoeuds.Insert(0, courant);
                courant = predecesseurs[sommetDepart][courant];
            }

            if (courant == null)
                return (new List<Arc<T>>(), int.MaxValue); /// Pas de chemin remontable

            cheminNoeuds.Insert(0, sommetDepart); ///On met le sommet de départ au début

            /// Traduction en arcs
            List<Arc<T>> cheminArcs = new List<Arc<T>>();
            for (int i = 0; i < cheminNoeuds.Count - 1; i++)
            {
                Noeud<T> from = cheminNoeuds[i];
                Noeud<T> to = cheminNoeuds[i + 1];

                Arc<T> arc = arcs.FirstOrDefault(a =>
                    a.IdPrevious.Equals(from) && a.IdNext.Equals(to)); /// Graphe orienté : un seul sens

                if (arc != null)
                    cheminArcs.Add(arc);
                else
                    return (new List<Arc<T>>(), int.MaxValue); /// Arc manquant (cas anormal)
            }

            return (cheminArcs, distances[sommetDepart][sommetArrivee]);
        }
        public (List<Arc<T>>, int) FloydWarshallCheminNonOrienté(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
        {
            var distances = new Dictionary<Noeud<T>, Dictionary<Noeud<T>, int>>();
            var predecesseurs = new Dictionary<Noeud<T>, Dictionary<Noeud<T>, Noeud<T>>>();
            var lignes = new Dictionary<(Noeud<T>, Noeud<T>), string>();

            /// Initialisation des distances et prédecesseurs
            foreach (var i in listeAdjacence.Keys)
            {
                distances[i] = new Dictionary<Noeud<T>, int>();
                predecesseurs[i] = new Dictionary<Noeud<T>, Noeud<T>>();
                foreach (var j in listeAdjacence.Keys)
                {
                    if (i.Equals(j))
                    {
                        distances[i][j] = 0;
                    }
                    else
                    {
                        distances[i][j] = int.MaxValue;
                    }
                    predecesseurs[i][j] = null;
                }
            }
            /// Initialisation avec les arcs
            foreach (var arc in arcs)
            {
                distances[arc.IdPrevious][arc.IdNext] = arc.Poids;
                predecesseurs[arc.IdPrevious][arc.IdNext] = arc.IdPrevious;
                lignes[(arc.IdPrevious, arc.IdNext)] = arc.IdLigne;
                if (!arc.SensUnique) /// NON ORIENTE
                {
                    distances[arc.IdNext][arc.IdPrevious] = arc.Poids;
                    predecesseurs[arc.IdNext][arc.IdPrevious] = arc.IdNext;
                    lignes[(arc.IdNext, arc.IdPrevious)] = arc.IdLigne;
                }
            }

            foreach (var k in listeAdjacence.Keys)
            {
                foreach (var i in listeAdjacence.Keys)
                {
                    foreach (var j in listeAdjacence.Keys)
                    {
                        if (distances[i][k] == int.MaxValue || distances[k][j] == int.MaxValue)
                        {
                            continue;
                        }
                        lignes.TryGetValue((i, k), out string ligneIK);
                        lignes.TryGetValue((k, j), out string ligneKJ);

                        int tempsChangement = 0;
                        if (ligneIK != null && ligneKJ != null && ligneIK != ligneKJ)
                        {
                            tempsChangement = k.TempsChangement;
                        }
                        int nouvelleDistance = distances[i][k] + distances[k][j] + tempsChangement;
                        if (nouvelleDistance < distances[i][j])
                        {
                            distances[i][j] = nouvelleDistance;
                            predecesseurs[i][j] = predecesseurs[k][j];  /// On garde l'avant-dernier nœud avant j
                            lignes[(i, j)] = ligneIK ?? ligneKJ;
                        }
                    }
                }
            }

            /// Reconstruction du chemin        
            var cheminNoeuds = new List<Noeud<T>>();
            var courant = sommetArrivee;

            /// Remontée depuis l’arrivée jusqu’au départ
            while (courant != null && !courant.Equals(sommetDepart))
            {
                cheminNoeuds.Insert(0, courant);
                courant = predecesseurs[sommetDepart][courant];
            }

            if (courant == null)
                return (new List<Arc<T>>(), int.MaxValue);

            cheminNoeuds.Insert(0, sommetDepart);///On met le sommet de départ au début

            /// Traduction Noeuds → Arcs
            var cheminArcs = new List<Arc<T>>();
            for (int i = 0; i < cheminNoeuds.Count - 1; i++)
            {
                Noeud<T> from = cheminNoeuds[i];
                Noeud<T> to = cheminNoeuds[i + 1];

                Arc<T> arc = arcs.FirstOrDefault(a =>
                    a.IdPrevious.Equals(from) && a.IdNext.Equals(to) ||
                    (!a.SensUnique && a.IdPrevious.Equals(to) && a.IdNext.Equals(from)));

                if (arc != null)
                    cheminArcs.Add(arc);
            }

            return (cheminArcs, distances[sommetDepart][sommetArrivee]);
        }

        #endregion
        #region Affichage
        public string AffichageGrapheOrienté()
        {
            string cheminAcces = Graphviz<T>.GenerateGraphImage(Noeuds, Arcs);
            return cheminAcces;
        }
        public string AffichageAncienGraphe()
        {
            string cheminAcces = Graphviz<T>.GenerateGraphImageOG(Noeuds, Arcs);
            return cheminAcces;

        }
        public string AffichageGrapheNonOrienté()
        {
            string cheminAcces = Graphviz<T>.GenerateGraphNonOrienté(Noeuds, Arcs, Couleurs);
            return cheminAcces;

        }
        public (int tempsMinimal, string cheminAcces) AffichageDijkstra(Noeud<T> depart, Noeud<T> arrivee)
        {
            (List<Arc<T>> chemin, int tempsMinimal) = DijkstraChemin(depart, arrivee); /// Calcul du chemin le plus court
            string cheminAcces = Graphviz<T>.GenerateChemin(chemin, noeuds);
            return (tempsMinimal, cheminAcces);
        }
        public (int tempsMinimal, string cheminAcces) AffichageBellmanFord(Noeud<T> depart, Noeud<T> arrivee)
        {
            (List<Arc<T>> chemin, int tempsMinimal) = BellmanFordChemin(depart, arrivee); /// Calcul du chemin le plus court
            string cheminAcces = Graphviz<T>.GenerateChemin(chemin, noeuds);
            return (tempsMinimal, cheminAcces);
        }
        public (int tempsMinimal, string cheminAcces) AffichageFloydWarshall(Noeud<T> depart, Noeud<T> arrivee)
        {
            (List<Arc<T>> chemin, int tempsMinimal) = FloydWarshallChemin(depart, arrivee); /// Calcul du chemin le plus court
            string cheminAcces = Graphviz<T>.GenerateChemin(chemin, noeuds);
            return (tempsMinimal, cheminAcces);
        }
        public (int tempsMinimal, string cheminAcces) AffichageCheminOptimal(List<Noeud<T>> stationAVisité)
        {
            (int tempsMinimal, List<Noeud<T>> cheminLePlusCourt) = CheminOptimal(stationAVisité); /// Calcul du chemin le plus court entre ttes les stations
            List<Arc<T>> cheminOptiArcs = new List<Arc<T>>();
            for (int i = 0; i < cheminLePlusCourt.Count - 1; i++)
            {
                (var listArc, int temps) = DijkstraChemin(cheminLePlusCourt[i], cheminLePlusCourt[i + 1]);
                cheminOptiArcs.AddRange(listArc);
            }
            string cheminAcces = Graphviz<T>.GenerateCheminOptimal(cheminOptiArcs, noeuds, cheminLePlusCourt);
            return (tempsMinimal, cheminAcces);
        }
        #endregion
        public List<(Noeud<T> noeud, List<Noeud<T>> successeur)> TriListeAdjacence()
        {
            List<(Noeud<T> noeud, List<Noeud<T>> successeur)> listeAdjacenceATriée = new List<(Noeud<T>, List<Noeud<T>>)>();
            foreach (Noeud<T> noeuds in listeAdjacence.Keys)
            {
                listeAdjacenceATriée.Add((noeuds, listeAdjacence[noeuds]));
            }

            return TriListeAdjacenceDecroissant(listeAdjacenceATriée, 0, listeAdjacenceATriée.Count - 1);
        }
        #region Chemin optimal
        public (int valeurMin, List<Noeud<T>> cheminLePlusCourt) CheminOptimal(List<Noeud<T>> stations)
        {
            int valeurMin = int.MaxValue;
            Noeud<T> stationDépart = stations[0];    /// La première station est forcément celle de départ
            List<Noeud<T>> cheminLePlusCourt = new List<Noeud<T>>();
            stations.RemoveAt(0);
            List<List<Noeud<T>>> listCheminPossible = Permutations(stations);
            foreach (List<Noeud<T>> chemin in listCheminPossible)
            {
                int tempsTraj = Dijkstra(stationDépart)[chemin[0]];
                for (int j = 0; j < chemin.Count - 1; j++)
                {
                    tempsTraj += Dijkstra(chemin[j])[chemin[j + 1]];
                }
                if (tempsTraj < valeurMin)
                {
                    valeurMin = tempsTraj;
                    cheminLePlusCourt = chemin;
                }
                Console.WriteLine("Temps :" + valeurMin);   /// Visualiser la progression et pour le débug
            }
            cheminLePlusCourt.Insert(0, stationDépart); /// On remet la station de départ au début du chemin
            foreach (Noeud<T> station in cheminLePlusCourt)
            {
                Console.Write(" -> " + station);
            }
            return (valeurMin, cheminLePlusCourt);
        }
        private static List<List<Noeud<T>>> Permutations(List<Noeud<T>> liste)
        {
            var resultats = new List<List<Noeud<T>>>();
            Permuter(liste, 0, resultats);
            return resultats;
        }
        private static void Permuter(List<Noeud<T>> liste, int index, List<List<Noeud<T>>> resultats)
        {
            if (index == liste.Count)
            {
                resultats.Add(new List<Noeud<T>>(liste));
                return;
            }

            for (int i = index; i < liste.Count; i++)
            {
                (liste[index], liste[i]) = (liste[i], liste[index]); /// change de place
                Permuter(liste, index + 1, resultats);
                (liste[index], liste[i]) = (liste[i], liste[index]); /// inverse le changement
            }
        }
        #endregion
        private List<(Noeud<T> noeud, List<Noeud<T>> successeur)> TriListeAdjacenceDecroissant(List<(Noeud<T> noeud, List<Noeud<T>> successeur)> listeAdjacenceTriée, int début, int fin)
        {
            if (début >= fin)
                return listeAdjacenceTriée;

            int pivot = listeAdjacenceTriée[début].successeur.Count;
            int i = début + 1;
            int j = fin;

            while (i <= j)
            {
                while (i <= fin && listeAdjacenceTriée[i].successeur.Count > pivot)
                {
                    i++;
                }

                while (j >= début && listeAdjacenceTriée[j].successeur.Count < pivot)
                {
                    j--;
                }

                if (i <= j)
                {
                    var temp = listeAdjacenceTriée[i];
                    listeAdjacenceTriée[i] = listeAdjacenceTriée[j];
                    listeAdjacenceTriée[j] = temp;
                    i++;
                    j--;
                }
            }

            var tempPivot = listeAdjacenceTriée[début];
            listeAdjacenceTriée[début] = listeAdjacenceTriée[j];
            listeAdjacenceTriée[j] = tempPivot;

            TriListeAdjacenceDecroissant(listeAdjacenceTriée, début, j - 1);
            TriListeAdjacenceDecroissant(listeAdjacenceTriée, j + 1, fin);
            return listeAdjacenceTriée;
        }
        private void ClearCouleurs()
        {
            foreach (var couleur in couleurs.Keys)
            {
                couleurs[couleur] = 0;
            }
        }
        public int WelshPowell()
        {
            List<(Noeud<T> noeud, List<Noeud<T>> successeur)> listeAdjacenceTriée = TriListeAdjacence();
            int couleur = 0;
            ClearCouleurs();    /// Obligé de réinitalisté les couleurs pour que l'algo se déroule bien.
            while (listeAdjacenceTriée.Count != 0)
            {
                couleur++;
                couleurs[listeAdjacenceTriée[0].noeud] = couleur;
                var successeur = listeAdjacenceTriée[0].successeur;
                listeAdjacenceTriée.RemoveAt(0);
                foreach (var s in listeAdjacenceTriée)
                {
                    if (!successeur.Contains(s.noeud))
                    {
                        couleurs[s.noeud] = couleur;
                        successeur.AddRange(listeAdjacence[s.noeud]);
                    }
                }
                var listeASupprimer = new List<(Noeud<T> noeud, List<Noeud<T>> successeur)>();
                foreach (var s in listeAdjacenceTriée)  ///On retire tt les sommets coloriés
                {
                    if (couleurs[s.noeud] != 0)
                    {
                        listeASupprimer.Add(s);
                    }
                }
                foreach (var s in listeASupprimer)
                {
                    listeAdjacenceTriée.Remove(s);
                }
            }
            return couleur;
        }
        public bool EstBiparti()
        {
            int couleurMinimale = WelshPowell();
            bool estBiparti = false;
            if (couleurMinimale == 2)
            {
                estBiparti = true;
            }
            return estBiparti;
        }
        public bool EstPlanaire()
        {
            var listeTrié = TriListeAdjacence();
            bool estPlanaire = false;
            if (EstConnexe())
            {
                if ((listeTrié[0].successeur.Count >= 4 && !ContientCycle()) || listeTrié[0].successeur.Count - 1 >= 4)
                {
                    estPlanaire = true;
                }
            }
            return estPlanaire;
        }
        public string PropriétésGraphe()
        {
            string result = "";
            result += "Propriétés du graphe :\n";
            result += "Nombre de sommets : " + listeAdjacence.Count + "\n";
            result += "Nombre d'arcs : " + arcs.Count + "\n";
            result += "Nombre de sommets isolés : " + noeudsIsolés.Count + "\n";
            int couleurMinimale = WelshPowell();
            result += "Nombre de couleurs : " + couleurMinimale + "\n";

            if (EstBiparti())
            {
                result += "Le graphe est biparti.\n";
            }
            else
            {
                result += "Le graphe n'est pas biparti.\n";
            }
            if (EstPlanaire())
            {
                result += "Le graphe est probablement planaire.\n";
            }
            else
            {
                result += "Le graphe n'est probablement pas planaire.\n";
            }
            return result;
        }
        #region Classes pour la sérialisation
        [Serializable]
        public class NoeudExport
        {
            public int Id { get; set; }
            public string Nom { get; set; }
        }

        [Serializable]
        public class ArcExport
        {
            public int Source { get; set; }
            public int Destination { get; set; }
        }

        [Serializable]
        public class PropriétésExport
        {
            public bool EstConnexe { get; set; }
            public bool ContientCycle { get; set; }
            public int NombreCouleurs { get; set; }
        }

        [Serializable]
        public class DonnéesGraphe
        {
            public List<NoeudExport> Noeuds { get; set; }
            public List<ArcExport> Arcs { get; set; }
            public List<NoeudExport> NoeudsIsolés { get; set; }
            public PropriétésExport Propriétés { get; set; }
        }
        #endregion

        #region Méthodes d'exportation
        /// <summary>
        /// Exporte les données du graphe au format JSON
        /// </summary>
        public void ExporterVersJSON(Graphe<Utilisateur> graphU, string nomFichier)
        {
            Utilisateur.RefreshList();
            var donnees = new DonnéesGraphe
            {
                Propriétés = new PropriétésExport
                {
                    EstConnexe = EstConnexe(),
                    ContientCycle = ContientCycle(),
                    NombreCouleurs = graphU.WelshPowell()
                }
            };

            /// Conversion des noeuds
            donnees.Noeuds = new List<NoeudExport>();
            foreach (var utilisateur in Utilisateur.utilisateurs)
            {
                var noeudExport = new NoeudExport
                {
                    Id = utilisateur.Id_utilisateur,
                    Nom = utilisateur.Nom
                };
                donnees.Noeuds.Add(noeudExport);
            }

            /// Conversion des arcs
            donnees.Arcs = new List<ArcExport>();
            foreach (var arc in graphU.Arcs)
            {
                var arcExport = new ArcExport
                {
                    Source = arc.IdPrevious.Id,
                    Destination = arc.IdNext.Id
                };
                donnees.Arcs.Add(arcExport);
            }

            /// Conversion des noeuds isolés
            donnees.NoeudsIsolés = new List<NoeudExport>();
            if (graphU.NoeudsIsolés != null)
            {
                foreach (var noeud in graphU.NoeudsIsolés)
                {
                    var noeudIsoléExport = new NoeudExport
                    {
                        Id = noeud.Id,
                        Nom = noeud.Valeur.Nom
                    };
                    donnees.NoeudsIsolés.Add(noeudIsoléExport);
                }
            }

            /// Sérialisation en JSON
            string json = System.Text.Json.JsonSerializer.Serialize(donnees,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

            System.IO.File.WriteAllText(nomFichier, json);
            Console.WriteLine($"Données exportées avec succès vers {nomFichier}");
        }

        /// <summary>
        /// Exporte les données du graphe au format XML
        /// </summary>
        public void ExporterVersXML(Graphe<Utilisateur> graphU, string nomFichier)
        {
            Utilisateur.RefreshList();
            var donnees = new DonnéesGraphe();

            /// Propriétés du graphe
            donnees.Propriétés = new PropriétésExport
            {
                EstConnexe = EstConnexe(),
                ContientCycle = ContientCycle(),
                NombreCouleurs = graphU.WelshPowell()
            };

            /// Conversion des noeuds
            donnees.Noeuds = new List<NoeudExport>();
            foreach (var utilisateur in Utilisateur.utilisateurs)
            {
                var noeudExport = new NoeudExport
                {
                    Id = utilisateur.Id_utilisateur,
                    Nom = utilisateur.Nom
                };
                donnees.Noeuds.Add(noeudExport);
            }

            /// Conversion des arcs
            donnees.Arcs = new List<ArcExport>();
            foreach (var arc in graphU.Arcs)
            {
                var arcExport = new ArcExport
                {
                    Source = arc.IdPrevious.Id,
                    Destination = arc.IdNext.Id
                };
                donnees.Arcs.Add(arcExport);
            }

            /// Conversion des noeuds isolés
            donnees.NoeudsIsolés = new List<NoeudExport>();
            if (graphU.NoeudsIsolés != null)
            {
                foreach (var noeud in graphU.NoeudsIsolés)
                {
                    var noeudIsoléExport = new NoeudExport
                    {
                        Id = noeud.Id,
                        Nom = noeud.Valeur.Nom
                    };
                    donnees.NoeudsIsolés.Add(noeudIsoléExport);
                }
            }

            /// Sérialisation en XML
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DonnéesGraphe));
            using (var writer = new StreamWriter(nomFichier))
            {
                serializer.Serialize(writer, donnees);
            }
            Console.WriteLine($"Données exportées avec succès vers {nomFichier}");
        }
        #endregion
    }
}