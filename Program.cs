using QuickGraph;
using GraphSharp.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ProbSciAna
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Créez un graphe
            var graph = new BidirectionalGraph<object, IEdge<object>>();

            // Ajoutez des sommets et des arêtes
            var v1 = "1";
            var v2 = "2";
            var v3 = "3";
            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v3);
            graph.AddEdge(new Edge<object>(v1, v2));
            graph.AddEdge(new Edge<object>(v2, v3));
            graph.AddEdge(new Edge<object>(v3, v1));

            // Créez une fenêtre WPF pour afficher le graphe
            var app = new Application();
            var window = new Window
            {
                Title = "Graph Visualization",
                Width = 800,
                Height = 600
            };

            // Créez le contrôle GraphSharp avec l'algorithme de layout "Circular"
            // et désactivez la suppression des chevauchements
            var graphLayout = new GraphLayout<object, IEdge<object>, BidirectionalGraph<object, IEdge<object>>>()
            {
                Graph = graph,
                LayoutAlgorithmType = "Circular",
                HighlightAlgorithmType = "Simple"
                // OverlapRemovalAlgorithmType non défini pour éviter l'erreur
            };

            // Ajoutez le contrôle GraphSharp à la fenêtre
            var grid = new Grid();
            grid.Children.Add(graphLayout);
            window.Content = grid;

            // Affichez la fenêtre
            app.Run(window);
        }
    }
}


/* VERSION AVEC LES STRING + LES TABLEAUX DE SOMMETS

using QuickGraph;
using GraphSharp.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProbSciAna
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Exemple de liste de sommets (ici des nombres entiers)
            var sommets = new List<string> { "1", "2", "3", "4", "5" };

            // Exemple de définitions d'arêtes sous forme de chaînes "source target"
            var definitionsAretes = new List<string>
            {
                "1 2",
                "2 3",
                "3 1",
                "4 5"
            };

            // Création du graphe orienté
            var graph = new BidirectionalGraph<string, Edge<string>>();

            // Ajout des sommets au graphe
            foreach (var s in sommets)
            {
                graph.AddVertex(s);
            }

            // Parcours de la liste des définitions d'arêtes pour les ajouter au graphe
            foreach (var def in definitionsAretes)
            {
                // Séparer la chaîne en utilisant l'espace comme séparateur
                var parts = def.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length == 2)
                {
                    string source = parts[0];
                    string cible = parts[1];
                    // Vérifier que les deux sommets existent dans le graphe
                    if (graph.ContainsVertex(source) && graph.ContainsVertex(cible))
                    {
                        graph.AddEdge(new Edge<string>(source, cible));
                    }
                }
            }

            // Création de l'application WPF et de la fenêtre
            var app = new Application();
            var window = new Window
            {
                Title = "Visualisation du Graphe",
                Width = 800,
                Height = 600
            };

            // Création du contrôle GraphSharp pour afficher le graphe
            var graphLayout = new GraphLayout<string, Edge<string>, BidirectionalGraph<string, Edge<string>>>()
            {
                Graph = graph,
                LayoutAlgorithmType = "Circular",  // Layout adapté aux graphes cycliques
                HighlightAlgorithmType = "Simple"
                // On peut omettre OverlapRemovalAlgorithmType pour éviter des problèmes éventuels
            };

            // Ajout du contrôle dans une grille et affectation à la fenêtre
            var grid = new Grid();
            grid.Children.Add(graphLayout);
            window.Content = grid;

            // Lancement de l'application WPF
            app.Run(window);
        }
    }
}
    
    */