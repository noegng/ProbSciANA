using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciANA
{
    public class Noeud
    {
        private int noeud;
        private int noeudMax = 34;

        public Noeud(int noeud)
        {
            if (noeud > 0 && noeud <= noeudMax)
            {
                this.noeud = noeud;
            }
            else { Console.WriteLine("Erreur dans la saisie du noeud." + noeud); }
        }
        public int Noeuds
        {
            get { return noeud; }
            set { noeud = value; }
        }
        public override bool Equals(object obj)
        {
            if (obj is Noeud autre)
            {
                return this.noeud == autre.noeud;
            }
                
            return false;
        }

        public override int GetHashCode()
        {
            return noeud.GetHashCode();
        }

        public string toString()
        {
            return Convert.ToString(noeud);
        }
    }
}
