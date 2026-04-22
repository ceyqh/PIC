using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.Model
{
    internal class Dispositiu
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public long IdCategoria { get; set; }
        public string Categoria { get; set; }
        public string Estat { get; set; }
    }
}
