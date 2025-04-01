using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ProbSciANA
{


public class Station
{
    public int Id { get; set; } // Identifiant unique de la station
    public string Nom { get; set; } // Nom de la station
    public double Longitude { get; set; } // Longitude de la station
    public double Latitude { get; set; } // Latitude de la station
    public int TempsChangement;

    // Constructeur de la classe Station

    public Station(int id, string nom, double longitude, double latitude, int temps)
    {
        Id = id;
        Nom = nom;
        Longitude = longitude;
        Latitude = latitude;
        TempsChangement = temps;
    }
    public override bool Equals(object obj)
    {
        if (obj is Station autre)
        {
            return this.Id == autre.Id && this.Nom == autre.Nom && this.Longitude == autre.Longitude && this.Latitude == autre.Latitude;
        }
                
        return false;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ Nom.GetHashCode() ^ Longitude.GetHashCode() ^ Latitude.GetHashCode();
    }
    public string ToStringLong()
    {
        return $"Station: {Nom} (ID: {Id}), Longitude: {Longitude}, Latitude: {Latitude}, Temps de changement: {TempsChangement} minutes";
    }
    public string toString()
    {
        return "Station : " + this.Nom;
    }
        
}
}
