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
    internal class AfegirDepartamentVM: Utilities.ViewModelBase
    {
        private readonly DepartamentsApiClient _departamentsApiClient;
        private readonly DepartamentsVM _departamentsVM;

        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotAfegir = true;

        // CONSTRUCTOR
        public AfegirDepartamentVM(DepartamentsVM departamentsVM)
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
        public void Mostrar()
        {
            Nom = "";

            esPotAfegir = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR NOU DEPARTAMENT
        public ICommand AfegirDepartament_Click => new RelayCommand(async _ =>
        {
            if (esPotAfegir)
            {
                // Si hi ha camps buits
                if (string.IsNullOrWhiteSpace(Nom))
                {
                    MissatgeError.Mostrar("No hi poden haver camps buits.");
                }
                else
                {
                    Departament nouDepartament = new Departament();
                    nouDepartament.Nom = Nom;

                    Departament departamentCreat = await _departamentsApiClient.PostDepartamentAsync(nouDepartament);
                    
                    // Si crear el departament falla
                    if (departamentCreat == null)
                    {
                        MissatgeError.Mostrar("Hi ha hagut un problema al crear el departament.");
                    }
                    // Si crear el departament funciona
                    else
                    {
                        esPotAfegir = false;
                        await _departamentsVM.MostrarDepartamentsAsync();
                        EsVisible = Visibility.Collapsed;
                    }                        
                }
            }                     
        });
    }
}
