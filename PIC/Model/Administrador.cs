using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.Model
{
    internal class Administrador
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public string Contrasenya { get; set; }
        public string Permisos { get; set; }
    }
}
