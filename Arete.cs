using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Org.BouncyCastle.Asn1.Tsp;


namespace ProbSciANA
{

public class Arete {


    private Station idPrevious;
    private Station idNext;
    private string idLigne;
    private int temps;
    
    
    public Station IdPrevious {
        get {
            return idPrevious;
        }
        set {
            idPrevious = value;
        }
    }
   
    public Station IdNext {
        get {
            return idNext;
        }
        set {
            idNext = value;
        }
    }
    public string IdLigne {
        get {
            return idLigne;
        }
        set {
            idLigne = value;
        }
    }
    public int Temps {
        get {
            return temps;
        }
        set {
            temps = value;
        }
    }

  
    public Arete(Station idPrevious, Station idNext, string idLigne) {
        IdPrevious = idPrevious;
        IdNext = idNext;
        IdLigne = idLigne;
    }
    //Calculer la distance entre deux stations avec la formule de Haversine
    public double CalculerDistance() {
        double R = 6371; // Rayon de la Terre en km
        double dLat = Convert.ToDouble((idNext.Latitude - idPrevious.Latitude) * Convert.ToDecimal(Math.PI) / (decimal)180.0);
        double dLon = Convert.ToDouble((idNext.Longitude - idPrevious.Longitude) * Convert.ToDecimal(Math.PI) / (decimal)180.0);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(Convert.ToDouble(idPrevious.Latitude * Convert.ToDecimal(Math.PI) / (decimal)180.0)) * Math.Cos(Convert.ToDouble(idNext.Latitude * Convert.ToDecimal(Math.PI)/ (decimal)180.0)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance en km
    }
    //Calcul du temps de trajet entre deux stations
    public void CalculerTemps(){

    }



}
}