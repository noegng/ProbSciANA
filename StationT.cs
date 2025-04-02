using System;
using System.Collections.Generic;

namespace ProbSciANA
{
    public class Station<TId, TNom>
    {
        public TId Id { get; set; } // Identifiant unique de la station
        public TNom Nom { get; set; } // Nom de la station
        public double Longitude { get; set; } // Longitude de la station
        public double Latitude { get; set; } // Latitude de la station
        public int TempsChangement { get; set; } // Temps de changement

        // Constructeur de la classe Station
        public Station(TId id, TNom nom, double longitude, double latitude, int temps)
        {
            Id = id;
            Nom = nom;
            Longitude = longitude;
            Latitude = latitude;
            TempsChangement = temps;
        }

        public override bool Equals(object obj)
        {
            if (obj is Station<TId, TNom> autre)
            {
                return EqualityComparer<TId>.Default.Equals(this.Id, autre.Id) &&
                       EqualityComparer<TNom>.Default.Equals(this.Nom, autre.Nom) &&
                       this.Longitude == autre.Longitude &&
                       this.Latitude == autre.Latitude;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nom, Longitude, Latitude, TempsChangement);
        }
    }
}