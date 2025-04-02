using System;
using System.Collections.Generic;

namespace ProbSciANA
{
    public class Arete<T>
    {
        private T idPrevious;
        private T idNext;
        private string idLigne;
        private int temps;
#region Properties
        public T IdPrevious
        {
            get { return idPrevious; }
            set { idPrevious = value; }
        }

        public T IdNext
        {
            get { return idNext; }
            set { idNext = value; }
        }

        public string IdLigne
        {
            get { return idLigne; }
            set { idLigne = value; }
        }

        public int Temps
        {
            get { return temps; }
            set { temps = value; }
        }
#endregion
        public Arete(T idPrevious, T idNext, string idLigne)
        {
            IdPrevious = idPrevious;
            IdNext = idNext;
            IdLigne = idLigne;
        }
        public bool Equals(Arete<T> other)
        {
            if (other == null) return false;
            return EqualityComparer<T>.Default.Equals(IdPrevious, other.IdPrevious) &&
                   EqualityComparer<T>.Default.Equals(IdNext, other.IdNext) &&
                   IdLigne == other.IdLigne;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Arete<T>);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdPrevious, IdNext, IdLigne);
        }
        /*

        // Calculer la distance entre deux stations avec la formule de Haversine
        static double CalculerDistance(Station IdPrevious, Station IdNext) //obligé de le mettre dans Program car sinon IdPrevious et IdNext ne sont pas accessible dans la classe Arete
        {
        double R = 6371; // Rayon de la Terre en km
        double dLat = Convert.ToDouble((IdNext.Latitude - IdPrevious.Latitude) * Math.PI / 180.0);
        double dLon = Convert.ToDouble((IdNext.Longitude - IdPrevious.Longitude) * Math.PI / 180.0);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(Convert.ToDouble(IdPrevious.Latitude * Math.PI / 180.0)) * Math.Cos(Convert.ToDouble(IdNext.Latitude * Math.PI / 180.0)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance en km
        }


        // Calcul et met à jour la variable temps (temps de trajet entre deux stations)
        public void CalculerTempsTrajet(Dictionary<string, double> VitesseMoyenne)
        {
        // calcul de la distance entre idPrevious et idNext avec la formule de Haversine
        // puis conversion en temps de trajet (en minutes) en fonction de la vitesse moyenne du train
        // (ex: 80 km/h = 1.33 km/min)
        // (ex: 60 km/h = 1 km/min)

        double distance = CalculerDistance(IdPrevious, IdNext);
        double t = distance / VitesseMoyenne[idLigne];
        temps =  (int)t + 1; // +1 pour eviter d'avoir un temps de trajet nul et pour arrondir a l'entier superieur
        }
        */
    }
}