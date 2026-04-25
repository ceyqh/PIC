using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.Model
{
    internal class Usuari
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public string Cognom { get; set; }
        public string Tipus { get; set; }
        public string Grup { get; set; }
        public long IdGrup { get; set; }

        public string GrupFormatat => $"{IdGrup} | {Grup}";
    }
}
