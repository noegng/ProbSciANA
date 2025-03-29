using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;


namespace ProbSciANA
{

public class Arete {


    private Station idPrevious;
    private Station idNext;
    private string idLigne;
    private int temps;

   
    public int CalculTemps(Dictionary<string, int> VitesseMoyenne)
    {
        // calcul de la distance entre idPrevious et idNext avec la formule de Haversine
        // puis conversion en temps de trajet (en minutes) en fonction de la vitesse moyenne du train
        // (ex: 80 km/h = 1.33 km/min)
        // (ex: 60 km/h = 1 km/min)

        int R= 6371;
        double distance = 2*R*Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(Convert.ToDouble(IdNext.Latitude - IdPrevious.Latitude / 2)), 2) + Math.Cos(Convert.ToDouble(IdPrevious.Latitude)*Math.PI/180.0) * Math.Cos(Convert.ToDouble(IdNext.Latitude)*Math.PI/180) * Math.Pow(Math.Sin(Convert.ToDouble(IdNext.Longitude - IdPrevious.Longitude) / 2), 2)));
        double temps = distance / VitesseMoyenne[idLigne];
        return (int)temps;
    }


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


}
}