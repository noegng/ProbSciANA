using System;
using System.Collections.Generic;

namespace ProbSciANA
{
    public class Arc<T>
    {
        private Noeud<T> idPrevious;
        private Noeud<T> idNext;
        private Noeud<T> idNoeudIsolé;
        private int poids;
        private bool sensUnique;
        private string idLigne;

        public Arc(Noeud<T> idPrevious, Noeud<T> idNext, bool sensUnique = false, string idLigne = "", int poids = 1)
        {
            this.idPrevious = idPrevious;
            this.idNext = idNext;
            this.sensUnique = sensUnique;
            this.poids = poids;
            this.idLigne = idLigne;
        }
        public Arc(Noeud<T> idNoeudIsolé)
        {
            this.idNoeudIsolé = idNoeudIsolé;
        }
        #region Properties
        public Noeud<T> IdPrevious
        {
            get { return idPrevious; }
            set { idPrevious = value; }
        }

        public Noeud<T> IdNext
        {
            get { return idNext; }
            set { idNext = value; }
        }
        public Noeud<T> IdNoeudIsolé
        {
            get { return IdNoeudIsolé; }
            set { IdNoeudIsolé = value; }
        }
        public int Poids
        {
            get { return poids; }
            set { poids = value; }
        }
        public bool SensUnique
        {
            get
            {
                return sensUnique;
            }
            set
            {
                sensUnique = value;
            }
        }
        public string IdLigne
        {
            get
            {
                return idLigne;
            }
            set
            {
                idLigne = value;
            }
        }
        #endregion
        public bool Equals(Arc<T> other)
        {
            if (other == null) return false;
            if (idLigne != null || idLigne != "")
            {
                return EqualityComparer<Noeud<T>>.Default.Equals(IdPrevious, other.IdPrevious) &&
                        EqualityComparer<Noeud<T>>.Default.Equals(IdNext, other.IdNext) &&
                        EqualityComparer<string>.Default.Equals(IdLigne, other.IdLigne);
            }
            return EqualityComparer<Noeud<T>>.Default.Equals(IdPrevious, other.IdPrevious) &&
                   EqualityComparer<Noeud<T>>.Default.Equals(IdNext, other.IdNext);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Arc<T>);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdPrevious, IdNext);
        }
        public double CalculerDistance()
        {
            double R = 6371; /// Rayon de la Terre en km
            double dLat = Convert.ToDouble((IdNext.Latitude - IdPrevious.Latitude) * Math.PI / 180.0);
            double dLon = Convert.ToDouble((IdNext.Longitude - IdPrevious.Longitude) * Math.PI / 180.0);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(Convert.ToDouble(IdPrevious.Latitude * Math.PI / 180.0)) * Math.Cos(Convert.ToDouble(IdNext.Latitude * Math.PI / 180.0)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; /// Distance en km
        }


        public static double CalculerDistanceHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; /// Rayon de la Terre en km
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; /// Distance en km
        }
        ///Calcul et met a jour la variabl temps ( temps de trajet entre deux stations)
        public int CalculerTempsTrajet(Dictionary<string, double> VitessesMoyennes, string idLigne)
        {
            /// calcul de la distance entre idPrevious et idNext avec la formule de Haversine
            double distance = CalculerDistance();
            double t = distance / VitessesMoyennes[idLigne];
            poids = (int)t + 1; /// +1 pour eviter d'avoir un temps de trajet nul et pour arrondir a l'entier superieur
            return poids;
        }

    }
}