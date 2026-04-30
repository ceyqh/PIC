using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace PIC.ViewModel
{
    internal class EditarDepartamentVM: Utilities.ViewModelBase
    {
        private readonly DepartamentsApiClient _departamentsApiClient;
        private readonly DepartamentsVM _departamentsVM;

        private Departament _departamentEnEdicio;
        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotEditar = true;

        // CONSTRUCTOR
        public EditarDepartamentVM(DepartamentsVM departamentsVM)
        {
            _departamentsVM = departamentsVM;

            MissatgeError = new MissatgeErrorVM();
            _departamentsApiClient = new DepartamentsApiClient();
        }

        // NOM
        private string _nom;
        public string Nom
        {
            get => _nom;
            set
            {
                _nom = value;
                OnPropertyChanged();
            }
        }

        // VISIBILITAT MENU
        private Visibility _esVisble = Visibility.Collapsed;
        public Visibility EsVisible
        {
            get => _esVisble;
            set
            {
                _esVisble = value;
                OnPropertyChanged();
            }
        }

        // OBRIR FINESTRA
        public void Mostrar(Departament departament)
        {
            _departamentEnEdicio = departament;
            Nom = departament.Nom;

            esPotEditar = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // GUARDAR DEPARTAMENT
        public ICommand GuardarDepartament_Click => new RelayCommand(async _ =>
        {
            if (!esPotEditar)
            {
                return;
            }

            esPotEditar = false;

            // Si hi ha camps buits
            if (string.IsNullOrWhiteSpace(Nom))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
                esPotEditar = true;
                return;
            }                

            // Actualitzar departament
            _departamentEnEdicio.Nom = Nom;
            int departamentActualitzat = await _departamentsApiClient.UpdateDepartamentAsync(_departamentEnEdicio);

            // Si actualitzar el departament falla
            if (departamentActualitzat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar el departament.");
                esPotEditar = true;
                return;
            }

            await _departamentsVM.MostrarDepartamentsAsync();
            EsVisible = Visibility.Collapsed;
        });
    }
}
