using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
        public string Estat
        {
            get
            {
                if (DataRetorn < DateTime.Now)
                {
                    return "Caducat";
                }
                return "En curs"; // O l'estat que tinguis per defecte
            }
        }

        public string DataEntregaFormatat => $"E: {DataEntrega}";
        public string DataRetornFormatat => $"R: {DataRetorn}";
    }
}
