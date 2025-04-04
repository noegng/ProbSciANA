using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Noeud<T>
    {
        private T valeur;  // VAleur unique du noeud
        private int tempsChangement; // Temps de changement (si exitant)
        private static int idBrute = 0;

        // Constructeur de la classe Station
        public Noeud(T valeur, int temps = 0 ) // valeur par défaut
        {
            this.valeur = valeur;
            tempsChangement = temps;
            idBrute++;
        }
        #region Propriétés
        public T Valeur
        {
            get { return valeur; }
            set { valeur = value; }
        }
        public int TempsChangement
        {
            get { return tempsChangement; }
            set { tempsChangement = value; }
        }
        public int IdBrute
        {
            get { return idBrute; }
            set { idBrute = value; }
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Station<T> autre)
            {
                return EqualityComparer<T>.Default.Equals(this.Valeur, autre.Info);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Valeur, TempsChangement);
        }
        public string ToStringLong()
        {
            return $"Noeud: {Valeur}, Temps de changement: {TempsChangement} minutes";
        }
    }
}