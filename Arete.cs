using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProbSciANA
{

public class Arete {


    Station idPrevious;
    Station idNext;
    int idLigne;

    int temps;

    
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
    public int IdLigne {
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

  
    public Arete(int idPrevious, int idNext, int idLigne) {
        IdPrevious = idPrevious;
        IdNext = idNext;
        IdLigne = idLigne;
    }
}
}