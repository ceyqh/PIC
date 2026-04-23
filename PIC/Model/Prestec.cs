using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.Model
{
    internal class Prestec
    {
        public int Id { get; set; }
        public string NomUsuari { get; set; }
        public int IdUsuari { get; set; }
        public string NomDispositiu { get; set; }
        public int IdDispositiu { get; set; }
        public DateTime DataEntrega { get; set; }
        public DateTime DataRetorn { get; set; }
    }
}
