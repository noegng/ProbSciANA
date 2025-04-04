using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Noeud<T>
    {
        private T valeur;  /// VAleur unique du noeud
        private int tempsChangement; /// Temps de changement (si exitant)
        private int idBrute = 0;
        public static int Compteur = 0;
        public double Longitude { get; set; } /// Longitude de la station
        public double Latitude { get; set; } /// Latitude de la station

        /// Constructeur de la classe Station
        public Noeud(T valeur, int temps = 0 ) // valeur par défaut
        {
            this.valeur = valeur;
            tempsChangement = temps;
            idBrute++;

        }
        public Noeud(T valeur, int temps, double longitude, double latitude) /// valeur par défaut
        {
            this.valeur = valeur;
            tempsChangement = temps;
            Longitude = longitude;
            Latitude = latitude;
            Compteur++;
            this.idBrute = Compteur;
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
        public  int IdBrute
        {
            get { return idBrute; }
            set { idBrute = value; }
        }
        #endregion
        public override bool Equals(object obj)
        {
            return obj is Noeud<T> autre && EqualityComparer<T>.Default.Equals(Valeur, autre.Valeur);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Valeur);
        }
        public string ToStringLong()
        {
            return $"Noeud: {Valeur}, Temps de changement: {TempsChangement} minutes";
        }
        public override string ToString()
        {
            return Valeur.ToString();
        }
    }
}