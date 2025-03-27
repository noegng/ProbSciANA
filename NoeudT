using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Noeud<T>
    {
        public T Valeur { get; }

        public Noeud(T valeur)
        {
            Valeur = valeur;
        }

        public override bool Equals(object obj)
        {
            return obj is Noeud<T> autre && EqualityComparer<T>.Default.Equals(Valeur, autre.Valeur);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Valeur);
        }

        public override string ToString()
        {
            return Valeur.ToString();
        }
    }
}