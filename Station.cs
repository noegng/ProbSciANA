using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ProbSciANA
{


public class Station
{
    public int id { get; set; } // Identifiant unique de la station
    public string nom { get; set; } // Nom de la station
    public double longitude { get; set; } // Longitude de la station
    public double latitude { get; set; } // Latitude de la station
    public int tempsChangement;

    // Constructeur de la classe Station

    public Station(int id)
    {
        this.id = id;
    }
    public Station(int id, string nom, double longitude, double latitude, int temps)
    {
        this.id = id;
        this.nom = nom;
        this.longitude = longitude;
        this.latitude = latitude;
        this.tempsChangement = temps;
    }
    public override bool Equals(object obj)
    {
        if (obj is Station autre)
        {
            return this.id == autre.id || this.nom == autre.nom ;
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
