using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Graphe<T> // Graphe de noeuds
    {
        private Dictionary<Noeud<T>, List<Noeud<T>>> listeAdjacence;
        private int[,] matriceAdjacence;
        private Dictionary<Noeud<T>, int> couleurs;
        private List<Arc<T>> arcs;
        private int nbCycles = -1; //Détecter une erreur de cycle | on déclare la variable ici pour que l'incrémentation se fasse dans la méthode récursive DFS (sinon impossible de l'incrémenter)

        public Graphe(List<Arc<T>> arcs)
        {
            this.arcs = arcs;
            listeAdjacence = new Dictionary<Noeud<T>, List<Noeud<T>>>();
            RemplissageListeAdjacence(arcs);
            matriceAdjacence = new int[listeAdjacence.Count,listeAdjacence.Count]; // 248 Noeud<T>s
            RemplissageMatriceAdjacence();
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
        public int[,] MatriceAdjacence{
            get { return matriceAdjacence; }
            set { matriceAdjacence = value; }
        }
        public List<Arc<T>> Arcs{
            get { return arcs; }
            set { arcs = value; }
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
                couleurs[sommet] = 0; // blanc
            }

            file.Enqueue(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune

            while (file.Count > 0)
            {
                Noeud<T> sommet = file.Dequeue();
                foreach (Noeud<T> voisin in listeAdjacence[sommet])
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

        public HashSet<Noeud<T>> DFS(Noeud<T> sommetDepart)
        {
            couleurs = new Dictionary<Noeud<T>, int>();
            Stack<Noeud<T>> pile = new Stack<Noeud<T>>();
            HashSet<Noeud<T>> visite = new HashSet<Noeud<T>>();

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
            }

            pile.Push(sommetDepart);
            couleurs[sommetDepart] = 1; // jaune

            while (pile.Count > 0)
            {
                Noeud<T> sommet = pile.Peek();
                bool aExploréUnVoisin = false;

                foreach (Noeud<T> voisin in listeAdjacence[sommet])
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

        public HashSet<Noeud<T>> DFSRécursif(bool rechercheCycle = false)
        {
            couleurs = new Dictionary<Noeud<T>, int>();
            HashSet<Noeud<T>> visite = new HashSet<Noeud<T>>();
            nbCycles = 0;
            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                couleurs[sommet] = 0; // blanc
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
            couleurs[sommet] = 1; // jaune
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
        public void RemplissageListeAdjacence(List<Arc<T>> arcs)
        {
            foreach (Arc<T> arete in arcs)
            {
                if (listeAdjacence.Count == 0)
                {
                    listeAdjacence.Add(arete.IdPrevious, new List<Noeud<T>> { arete.IdNext }); //on fait arete.IdPrevious.Id pour avoir le nom de la Noeud<T> (après vérification toutes les Noeud<T>s se trouvent dans arete.IdPrevious.Id)
                }
                else if (listeAdjacence.ContainsKey(arete.IdPrevious))
                {
                    listeAdjacence[arete.IdPrevious].Add(arete.IdNext);
                }
                else
                {
                    listeAdjacence.Add(arete.IdPrevious, new List<Noeud<T>> { arete.IdNext });
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
//A TESTER vérifier si id s'incrémente a chaque création de noeud
        public void RemplissageMatriceAdjacence()
        {
            foreach (Arc<T> arc in arcs)
            {
                if ( arc.IdPrevious != null && arc.IdNext != null)
                {
                    matriceAdjacence[Convert.ToInt32(arc.IdPrevious.IdBrute)-1, Convert.ToInt32(arc.IdNext.IdBrute)-1] = 1; // -1 car les Noeud<T> commencent à 1                   
                }
            }
        }
        public void AfficherMatriceAdjacence()
        {
            Console.WriteLine("Matrice d'adjacence:");
            foreach (var sommet in listeAdjacence.Keys)
            {
                Console.Write($"{sommet.IdBrute,3} ");
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

        public Dictionary<Noeud<T>, int> Dijkstra3(Noeud<T> sommetDepart)   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
        {
            Dictionary<Noeud<T>, int> distances = new Dictionary<Noeud<T>, int>();
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            PriorityQueue<Noeud<T>, int> filePriorite = new PriorityQueue<Noeud<T>, int>(); // On utilise une priority queue pour gérer les sommets à explorer
            string idLignePrécédent = ""; // On initialise l'id de la ligne précédente à une chaîne vide

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue; // On initialise les distances à l'infini
            }

            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                Noeud<T> sommetActuel = filePriorite.Dequeue(); // On prend le sommet avec la distance la plus courte
                visites.Add(sommetActuel);  
                int i = 0;
                foreach (Arc<T> arcVoisin in arcs) // On parcourt les voisins du sommet actuel
                {
                    if (arcVoisin.IdPrevious == sommetActuel) // On vérifie si le voisin est bien un voisin du sommet actuel
                    { 
                        int tempsChangement = 0;
                        if (idLignePrécédent != arcVoisin.IdLigne && i != 0) // On vérifie si on change de ligne
                        {
                            tempsChangement = arcVoisin.IdPrevious.TempsChangement; // On met à jour le temps de changement
                        }

                        if (!visites.Contains(arcVoisin.IdNext)) // On vérifie si le voisin n'a pas déjà été visité
                        {
                            // On met à jour la distance si on trouve un chemin plus court
                            int nouvelleDistance = distances[sommetActuel] + arcVoisin.Poids + tempsChangement; // On cast les sommets en Noeud<T> pour utiliser la classe Arc<T>
                            if (nouvelleDistance < distances[arcVoisin.IdNext])
                            {
                                distances[arcVoisin.IdNext] = nouvelleDistance;
                                filePriorite.Enqueue(arcVoisin.IdNext, nouvelleDistance);
                            }
                        }
                        idLignePrécédent = arcVoisin.IdLigne; // On mémorise l'id de la ligne pour le prochain sommet
                    }
                }
            }

            return distances;
        }
        public (Dictionary<Noeud<T>, int >, Dictionary<Noeud<T>, Arc<T>>) DijkstraTest(Noeud<T> sommetDepart)
        {
            Dictionary<Noeud<T>, int> distances = new Dictionary<Noeud<T>, int>();
            Dictionary<Noeud<T>, Arc<T>> predecesseurs = new Dictionary<Noeud<T>, Arc<T>>(); // Pour reconstruire le chemin
            List<Noeud<T>> noeudsNonVisités = new List<Noeud<T>>();
            string idLignePrécédent = "";
            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue; // On initialise les distances à l'infini
                predecesseurs[sommet] = null; // On initialise les prédécesseurs à null
                noeudsNonVisités.Add(sommet);
            }

            distances[sommetDepart] = 0;

            while (noeudsNonVisités.Count > 0)
            {
                Noeud<T> sommetActuel = null;
                int distanceMin = int.MaxValue;

                foreach(Noeud<T> noeud in listeAdjacence.Keys){
                    if(distances[noeud] < distanceMin){
                        distanceMin = distances[noeud];
                        sommetActuel = noeud;
                    }
                }
                noeudsNonVisités.Remove(sommetActuel);
                int  i = 0;
                foreach (Arc<T> voisin in arcs) // On parcourt les voisins du sommet actuel
                {
                    if (voisin.IdPrevious == sommetActuel) // On vérifie si le voisin est bien un voisin du sommet actuel
                    { 
                        int tempsChangement = 0;
                        if (idLignePrécédent != voisin.IdLigne && i != 0) // On vérifie si on change de ligne et i != 0 pour éviter la 1ère occcurence car forcément vrai 
                        {
                            tempsChangement = voisin.IdPrevious.TempsChangement; // On met à jour le temps de changement
                        }

                        
                            // On met à jour la distance si on trouve un chemin plus court
                            int nouvelleDistance = distances[sommetActuel] + voisin.Poids + tempsChangement; // On cast les sommets en Noeud<T> pour utiliser la classe Arete
                            if (nouvelleDistance < distances[voisin.IdNext])
                            {
                                distances[voisin.IdNext] = nouvelleDistance;
                                predecesseurs[voisin.IdNext] = voisin; // On met à jour le prédécesseur
                            }
                        idLignePrécédent = voisin.IdLigne; // On mémorise l'id de la ligne pour le prochain sommet
                        i++;
                    }
                    
                }
            }
            // Reconstruire le chemin
            foreach(var pre in predecesseurs)
            {
            List<Arc<T>> cheminAretes = new List<Arc<T>>();
            Noeud<T> courant = sommetDepart;
            while (predecesseurs[courant] != null)
            {
                    Arc<T> arc = predecesseurs[courant];
                    cheminAretes.Add(arc);
                    courant = arc.IdPrevious;
            }
            cheminAretes.Reverse(); // Important pour avoir le chemin dans le bon sens
            }
            return (distances, predecesseurs);

        }


        //Déterminer le chemin le plus court entre deux sommets avec l'algorithme de Dijkstra
        public (List<Arc<T>>, int) DijkstraChemin(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)   //Renvoie un dictionnaire avec les distances entre le sommet de départ et tous les autres sommets
        {
            Dictionary<Noeud<T>, int> distances = new Dictionary<Noeud<T>, int>();
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            PriorityQueue<Noeud<T>, int> filePriorite = new PriorityQueue<Noeud<T>, int>(); // On utilise une priority queue pour gérer les sommets à explorer
            Dictionary<Noeud<T>, Arc<T>> predecesseurs = new Dictionary<Noeud<T>, Arc<T>>(); // Pour reconstruire le chemin
            string idLignePrécédent = ""; // On initialise l'id de la ligne précédente à une chaîne vide

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue; // On initialise les distances à l'infini
                predecesseurs[sommet] = null; // On initialise les prédécesseurs à null
            }

            distances[sommetDepart] = 0;
            filePriorite.Enqueue(sommetDepart, 0);

            while (filePriorite.Count > 0)
            {
                Noeud<T> sommetActuel = filePriorite.Dequeue(); // On prend le sommet avec la distance la plus courte
                visites.Add(sommetActuel); 

                if (sommetActuel.Equals(sommetArrivee)) // Si on a atteint le sommet d'arrivée
                {
                    break;
                }
                int  i = 0;
                foreach (Arc<T> voisin in arcs) // On parcourt les voisins du sommet actuel
                {
                    if (voisin.IdPrevious == sommetActuel) // On vérifie si le voisin est bien un voisin du sommet actuel
                    { 
                        int tempsChangement = 0;
                        if (idLignePrécédent != voisin.IdLigne && i != 0) // On vérifie si on change de ligne et i != 0 pour éviter la 1ère occcurence car forcément vrai 
                        {
                            tempsChangement = voisin.IdPrevious.TempsChangement; // On met à jour le temps de changement
                        }

                        if (!visites.Contains(voisin.IdNext)) // On vérifie si le voisin n'a pas déjà été visité
                        {
                            // On met à jour la distance si on trouve un chemin plus court
                            int nouvelleDistance = distances[sommetActuel] + voisin.Poids + tempsChangement; // On cast les sommets en Noeud<T> pour utiliser la classe Arete
                            if (nouvelleDistance < distances[voisin.IdNext])
                            {
                                distances[voisin.IdNext] = nouvelleDistance;
                                predecesseurs[voisin.IdNext] = voisin; // On met à jour le prédécesseur
                                filePriorite.Enqueue(voisin.IdNext, nouvelleDistance);
                            }
                        }
                        idLignePrécédent = voisin.IdLigne; // On mémorise l'id de la ligne pour le prochain sommet
                        i++;
                    }
                    
                }
            }
            // Reconstruire le chemin
            List<Arc<T>> cheminAretes = new List<Arc<T>>();
            Noeud<T> courant = sommetArrivee;

            while (predecesseurs[courant] != null)
            {
                    Arc<T> arc = predecesseurs[courant];
                    cheminAretes.Add(arc);
                    courant = arc.IdPrevious;
            }
            cheminAretes.Reverse(); // Important pour avoir le chemin dans le bon sens
            return (cheminAretes, distances[sommetArrivee]);
        }
        public (List<Arc<T>>, int) DijkstraChemin2(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
{
    var distances = new Dictionary<Noeud<T>, int>();
    var predecesseurs = new Dictionary<Noeud<T>, Arc<T>>();
    var lignePrecedente = new Dictionary<Noeud<T>, string>();
    var filePriorite = new PriorityQueue<Noeud<T>, int>();
    var visites = new HashSet<Noeud<T>>();

    foreach (var sommet in listeAdjacence.Keys)
    {
        distances[sommet] = int.MaxValue;
        predecesseurs[sommet] = null;
    }

    distances[sommetDepart] = 0;
    lignePrecedente[sommetDepart] = "";
    filePriorite.Enqueue(sommetDepart, 0);

    while (filePriorite.Count > 0)
    {
        var sommetActuel = filePriorite.Dequeue();
        visites.Add(sommetActuel);

        if (sommetActuel.Equals(sommetArrivee))
            break;

        foreach (var arc in arcs.Where(a => a.IdPrevious.Equals(sommetActuel)))
        {
            if (arc.IdPrevious == sommetActuel){


            int tempsChangement = 0;
            if (lignePrecedente.TryGetValue(sommetActuel, out string ligneActuelle))
            {
                if (ligneActuelle != arc.IdLigne)
                    tempsChangement = arc.IdPrevious.TempsChangement;
            }

                int nouvelleDistance = distances[sommetActuel] + arc.Poids + tempsChangement;

            if (nouvelleDistance < distances[arc.IdNext])
            {
                distances[arc.IdNext] = nouvelleDistance;
                predecesseurs[arc.IdNext] = arc;
                lignePrecedente[arc.IdNext] = arc.IdLigne;
                filePriorite.Enqueue(arc.IdNext, nouvelleDistance);
            }
            }
        }
    }

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
        public (List<Arc<T>>, int) DijkstraCheminG(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
{
    var distances = new Dictionary<Noeud<T>, int>();
    var visites = new HashSet<Noeud<T>>();
    var filePriorite = new PriorityQueue<Noeud<T>, int>();
    var predecesseurs = new Dictionary<Noeud<T>, Arc<T>>();

    // Initialisation
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
            break;

        // Parcours des arcs sortants du sommet actuel
        foreach (Arc<T> arc in arcs)
        {
            if (arc.IdPrevious.Equals(sommetActuel))// On vérifie si le voisin est bien un voisin du sommet actuel
            {
                Noeud<T> voisin = arc.IdNext;

                if (visites.Contains(voisin))
                    continue;
    
                // Ligne précédente = ligne de l'arc ayant permis d'arriver au sommet actuel
                string idLignePrecedente = null;
                if (predecesseurs.ContainsKey(sommetActuel) && predecesseurs[sommetActuel] != null)
                {
                    idLignePrecedente = predecesseurs[sommetActuel].IdLigne;
                }
                int tempsChangement = 0;
                if (idLignePrecedente != null && idLignePrecedente != arc.IdLigne)
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

    // Reconstruction du chemin
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

public (List<Arc<T>>, int) BellmanFordChemin2(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
{
    var distances = new Dictionary<Noeud<T>, int>();
    var predecesseurs = new Dictionary<Noeud<T>, Arc<T>>();
    var lignePrecedente = new Dictionary<Noeud<T>, string>();

    foreach (var sommet in listeAdjacence.Keys)
    {
        distances[sommet] = int.MaxValue;
        predecesseurs[sommet] = null;
    }

    distances[sommetDepart] = 0;
    lignePrecedente[sommetDepart] = "";

    for (int i = 0; i < listeAdjacence.Count - 1; i++)
    {
        foreach (var arc in arcs)
        {
            if (distances[arc.IdPrevious] == int.MaxValue)
                continue;

            int tempsChangement = 0;
            if (lignePrecedente.TryGetValue(arc.IdPrevious, out string ligneActuelle))
            {
                if (ligneActuelle != arc.IdLigne)
                    tempsChangement = arc.IdPrevious.TempsChangement;
            }

            int nouvelleDistance = distances[arc.IdPrevious] + arc.Poids + tempsChangement;

            if (nouvelleDistance < distances[arc.IdNext])
            {
                distances[arc.IdNext] = nouvelleDistance;
                predecesseurs[arc.IdNext] = arc;
                lignePrecedente[arc.IdNext] = arc.IdLigne;
            }
        }
    }

    // Détection de cycle négatif (facultatif ici si tu sais qu’il n’y en a pas)
    foreach (var arc in arcs)
    {
        int tempsChangement = 0;
        if (lignePrecedente.TryGetValue(arc.IdPrevious, out string ligneActuelle))
        {
            if (ligneActuelle != arc.IdLigne)
                tempsChangement = arc.IdPrevious.TempsChangement;
        }

        if (distances[arc.IdPrevious] != int.MaxValue &&
            distances[arc.IdPrevious] + arc.Poids + tempsChangement < distances[arc.IdNext])
        {
            throw new InvalidOperationException("Cycle de poids négatif détecté.");
        }
    }

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


        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Bellman-Ford
        public Dictionary<Noeud<T>, int> BellmanFord(Noeud<T> sommetDepart)
        {
            Dictionary<Noeud<T>, int> distances = new Dictionary<Noeud<T>, int>();
            string idLignePrécédent = ""; // On initialise l'id de la ligne précédente à une chaîne vide

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue;
            }

            distances[sommetDepart] = 0;

            for (int i = 0; i < listeAdjacence.Count - 1; i++)
            {
                foreach (Arc<T> arc in arcs)
                {
                    if (arc.IdPrevious.IdBrute == i) // On vérifie si le voisin est bien un voisin du sommet actuel (L'id d'une Noeud)
                    {
                    int tempsChangement = 0;
                    if (idLignePrécédent != arc.IdLigne && i != 0) // On vérifie si on change de ligne
                    {
                        tempsChangement = arc.IdPrevious.TempsChangement; // On met à jour le temps de changement
                    }
                    if ((distances[arc.IdPrevious] != int.MaxValue) && (distances[arc.IdPrevious] + arc.Poids + arc.IdPrevious.TempsChangement < distances[arc.IdNext]))
                    {
                        distances[arc.IdNext] = distances[arc.IdPrevious] + arc.Poids + arc.IdPrevious.TempsChangement;
                    }
                    }
                    idLignePrécédent = arc.IdLigne; // On mémorise l'id de la ligne pour le prochain sommet
                }
            }

            return distances;
        }
        //Calculer et Renvoie le chemin le plus court entre deux sommets avec l'algorithme de Bellman-Ford
        public (List<Arc<T>>, int) BellmanFordChemin(Noeud<T> sommetDepart, Noeud<T> sommetArrivee)
        {
            Dictionary<Noeud<T>, int> distances = new Dictionary<Noeud<T>, int>();
            Dictionary<Noeud<T>, Arc<T>> predecesseurs = new Dictionary<Noeud<T>, Arc<T>>(); // Pour reconstruire le chemin
            string idLignePrécédent = ""; // On initialise l'id de la ligne précédente à une chaîne vide

            foreach (Noeud<T> sommet in listeAdjacence.Keys)
            {
                distances[sommet] = int.MaxValue;
                predecesseurs[sommet] = null; // On initialise les prédécesseurs à null
            }

            distances[sommetDepart] = 0;


            for (int i = 0; i < listeAdjacence.Count - 1; i++)
            {
                foreach (Arc<T> arc in arcs)
                {
                    if (arc.IdPrevious.IdBrute == i) // On vérifie si le voisin est bien un voisin du sommet actuel (L'id d'une Noeud)
                    {
                    int tempsChangement = 0;
                    if (idLignePrécédent != arc.IdLigne && i != 0) // On vérifie si on change de ligne
                    {
                        tempsChangement = arc.IdPrevious.TempsChangement; // On met à jour le temps de changement
                    }
                    if ((distances[arc.IdPrevious] != int.MaxValue) && (distances[arc.IdPrevious] + arc.Poids + arc.IdPrevious.TempsChangement < distances[arc.IdNext]))
                    {
                        distances[arc.IdNext] = distances[arc.IdPrevious] + arc.Poids + arc.IdPrevious.TempsChangement;
                        predecesseurs[arc.IdNext] = arc; // On met à jour le prédécesseur
                    }
                    }
                    idLignePrécédent = arc.IdLigne; // On mémorise l'id de la ligne pour le prochain sommet
                }
            }
            // Reconstruire le chemin
            List<Arc<T>> cheminAretes = new List<Arc<T>>();
            Noeud<T> courant = sommetArrivee;

            while (predecesseurs[courant] != null)
            {
                    Arc<T> arete = predecesseurs[courant];
                    cheminAretes.Add(arete);
                    courant = arete.IdPrevious;
            }
            cheminAretes.Reverse(); // Important pour avoir le chemin dans le bon sens
            return (cheminAretes, distances[sommetArrivee]);
        }
        //Calculer le chemin le plus court entre deux sommets avec l'algorithme de Floyd-Warshall
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
        #endregion
    }
    
}