using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.Model
{
    internal class Registre
    {
        public long Id { get; set; }
        public int IdPrestec { get; set; }
        public string Accio { get; set; }
        public string NomUsuari { get; set; }
        public int IdUsuari { get; set; }
        public string NomDispositiu { get; set; }
        public int IdDispositiu { get; set; }
        public string NomGrup { get; set; }
        public int IdGrup { get; set; }
        public DateTime DataAccio { get; set; }
        public DateTime DataRetorn { get; set; }

        public string DataAccioFormatat => $"A: {DataAccio}";
        public string DataRetornFormatat => $"R: {DataRetorn}";
    }
}
