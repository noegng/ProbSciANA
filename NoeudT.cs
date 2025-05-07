using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProbSciANA
{
    public class Noeud<T>
    {
        private T valeur;  /// Valeur unique du noeud
        private int tempsChangement; /// Temps de changement (si exitant)
        private int id;
        public double Longitude { get; set; } /// Longitude de la station
        public double Latitude { get; set; } /// Latitude de la station

        /// Constructeur de la classe Station
        public Noeud(T valeur, int id, int temps = 0) // valeur par défaut
        {
            this.valeur = valeur;
            this.id = id;
            tempsChangement = temps;
        }
        public Noeud(T valeur, int id, int temps, double longitude, double latitude) /// valeur par défaut
        {
            this.valeur = valeur;
            this.id = id;
            tempsChangement = temps;
            Longitude = longitude;
            Latitude = latitude;
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
        public int Id
        {
            get { return id; }
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Noeud<T> autre)
            {
                return Id == autre.Id; // Comparaison basée sur l'Id
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public string ToStringLong()
        {
            return $"Noeud: {Valeur}, Temps de changement: {TempsChangement} minutes";
        }
        public override string ToString()
        {
            return Valeur.ToString();
        }

        public async static Task<Noeud<(int, string)>> TrouverStationLaPlusProche(string adresse)
        {
            var Adresse = await Program.GetCoordonnees<string>(adresse);
            if (Adresse == null)
            {
                Noeud<(int, string)> station = new Noeud<(int, string)>((0, "Aucune station trouvée"), 0);
                return station;
            }
            Noeud<(int, string)> stationLaPlusProche = null;
            double distanceMinimale = double.MaxValue;

            foreach (var station in Program.Stations)
            {
                double distance = Arc<T>.CalculerDistanceHaversine(
                    Adresse.Latitude, Adresse.Longitude,
                    station.Latitude, station.Longitude
                );

                if (distance < distanceMinimale)
                {
                    distanceMinimale = distance;
                    stationLaPlusProche = station;
                }
            }
            return stationLaPlusProche;
        }

    }
}