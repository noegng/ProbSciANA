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
    private bool sensUnique;
    private static Dictionary<string, double> vitesseMoyenne = new Dictionary<string, double>();
    
        public Arete(Station idPrevious, Station idNext, string idLigne, bool sensUnique = false) {
        this.idPrevious = idPrevious;
        this.idNext = idNext;
        this.idLigne = idLigne;
        this.sensUnique = sensUnique;
        temps = CalculerTempsTrajet();
    }

    #region Propriétés
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
    public static Dictionary<string, double> VitesseMoyenne {
        get {
            return vitesseMoyenne;
        }
        set {
            vitesseMoyenne = value;
        }
    }
    public bool SensUnique {
        get {
            return sensUnique;
        }
        set {
            sensUnique = value;
        }
    }
    #endregion
    public bool Equals(Arete other)
    {
        if (other == null) return false;
        return IdPrevious == other.IdPrevious && IdNext == other.IdNext && IdLigne == other.IdLigne;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Arete);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IdPrevious, IdNext);
    }
    public string ToStringLong()
    {
        return $"Arete: {IdPrevious.Nom} -> {IdNext.Nom}, Ligne: {IdLigne}, Temps: {temps} minutes";
    }
    public string toString()
    {
        return $"Arete: {IdPrevious.Nom} -> {IdNext.Nom}, Ligne: {IdLigne}";
    }
    /*
    public override bool Equals(object obj)
    {
        if (obj is Arete autre)
        {
            return this.IdPrevious.Equals(autre.IdPrevious) && this.IdNext.Equals(autre.IdNext);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return IdPrevious.GetHashCode() ^ IdNext.GetHashCode();
    }
    */
    //Calculer la distance entre deux stations avec la formule de Haversine
    public double CalculerDistance()
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
    //Calcul et met a jour la variabl temps ( temps de trajet entre deux stations)
    public int CalculerTempsTrajet()
    {
        // calcul de la distance entre idPrevious et idNext avec la formule de Haversine
        // puis conversion en temps de trajet (en minutes) en fonction de la vitesse moyenne du train
        // (ex: 80 km/h = 1.33 km/min)
        // (ex: 60 km/h = 1 km/min)

        double distance = CalculerDistance();
        double t = distance / VitesseMoyenne[idLigne];
        temps =  (int)t + 1; // +1 pour eviter d'avoir un temps de trajet nul et pour arrondir a l'entier superieur
        return temps;
    }



}
}