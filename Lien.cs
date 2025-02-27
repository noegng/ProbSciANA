using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbSciAna
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
        public string toString()
        {
            return $"{noeud1.toString(),2 } {noeud2.toString(),2}";
        }
    }
}
