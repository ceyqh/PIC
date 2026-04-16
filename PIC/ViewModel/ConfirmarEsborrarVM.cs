using PIC.APIClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.ViewModel
{
    internal class ConfirmarEsborrarVM: Utilities.ViewModelBase
    {
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly ProfessorsApiClient _professorsApiClient;
        private readonly CursosApiClient _cursosApiClient;
        private readonly DepartamentsApiClient _departamentsApiClient;
        
        private readonly UsuarisVM _usuarisVM;

        // CONSTRUCTOR
        public ConfirmarEsborrarVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            _usuarisApiClient = new UsuarisApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();
            _cursosApiClient = new CursosApiClient();
            _departamentsApiClient = new DepartamentsApiClient();            
        }
    }
}
