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
