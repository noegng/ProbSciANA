using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pb_Sci_Etape_1
{
    public class Lien
    {
        private (Noeud noeud1, Noeud noeud2) lien;
        private Noeud noeud1;
        private Noeud noeud2;

        public Lien ((Noeud neud1, Noeud neud2) lien)
        {
            this.lien = lien;
            this.noeud1 = lien.neud1;
            this.noeud2 = lien.neud2;
        }
        public (Noeud noeud1, Noeud noeud2) Lien1
        {
            get { return lien; }
            set { lien = value; }
        }
        public Noeud Noeud1
        {
            get { return noeud1; }
            set { noeud1 = value; }
        }
        public Noeud Noeud2
        {
            get { return noeud2; }
            set { noeud2 = value; }
        }
        public string toString()
        {
            return $"{noeud1.toString(),2 } {noeud2.toString(),2}";
        }
    }
}
