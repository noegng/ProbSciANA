using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pb_Sci_Etape_1
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
            else { Console.WriteLine("Erreur dans la saisie du noeud."); }
        }
        public int Noeuds
        {
            get { return noeud; }
            set { noeud = value; }
        }
        public string toString()
        {
            return Convert.ToString(noeud);
        }
    }
}
