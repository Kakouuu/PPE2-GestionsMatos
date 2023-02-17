using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PPE_ver0._1
{
    internal class Intervention
    {
        public int ID;
        public string nom, NoSerie, MTBF, type, marque;
        public DateTime dateInstall;

        public Intervention(int id, string nom, string NoSerie, string MTBF, string type, string marque, DateTime dateInstall)
        {
            this.ID = id;
            this.nom = nom;
            this.NoSerie = NoSerie;
            this.MTBF = MTBF;
            this.type = type;
            this.marque = marque;
            this.dateInstall = dateInstall;
        }
        

        public override string ToString()
        {
            return this.nom;
        }
    }
}
