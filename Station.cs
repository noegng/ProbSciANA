using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ProbSciANA
{


public class Station
{
    public string Id { get; set; } // Identifiant unique de la station
    public string Nom { get; set; } // Nom de la station
    public decimal Longitude { get; set; } // Longitude de la station
    public decimal Latitude { get; set; } // Latitude de la station

    public int TempsChangement;

  
    public Station(string id, string nom, decimal longitude, decimal latitude)
    {
        Id = id;
        Nom = nom;
        Longitude = longitude;
        Latitude = latitude;
    }
}

}