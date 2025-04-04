using System;
using System.Collections.Generic;

namespace ProbSciANA
{
    public class Station<T>
    {
        private T info;  // Identifiant unique de la station
        private double longitude; // Longitude de la station
        private double latitude;  // Latitude de la station
        private int tempsChangement; // Temps de changement

        // Constructeur de la classe Station
        public Station(T info, double longitude, double latitude, int temps)
        {
            this.info = info;
            this.longitude = longitude;
            this.latitude = latitude;
            tempsChangement = temps;
        }
        #region Propriétés
        public T Info
        {
            get { return info; }
            set { info = value; }
        }
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        public int TempsChangement
        {
            get { return tempsChangement; }
            set { tempsChangement = value; }
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Station<T> autre)
            {
                return EqualityComparer<T>.Default.Equals(this.Info, autre.Info);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Info, Longitude, Latitude, TempsChangement);
        }
        public string ToStringLong()
        {
            return $"Station: {Info}, Longitude: {Longitude}, Latitude: {Latitude}, Temps de changement: {TempsChangement} minutes";
        }
    }
}