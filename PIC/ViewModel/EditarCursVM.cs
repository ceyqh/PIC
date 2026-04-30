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
    internal class EditarCursVM: Utilities.ViewModelBase
    {
        private readonly CursosApiClient _cursosApiClient;
        private readonly CursosVM _cursosVM;

        private Curs _cursEnEdicio;
        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotEditar = true;

        // CONSTRUCTOR
        public EditarCursVM(CursosVM cursosVM)
        {
            _cursosVM = cursosVM;

            MissatgeError = new MissatgeErrorVM();
            _cursosApiClient = new CursosApiClient();
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
        public void Mostrar(Curs curs)
        {
            _cursEnEdicio = curs;
            Nom = curs.Nom;

            esPotEditar = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // GUARDAR CURS
        public ICommand GuardarCurs_Click => new RelayCommand(async _ =>
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
                
            // Actualitzar curs
            _cursEnEdicio.Nom = Nom;
            int cursActualitzat = await _cursosApiClient.UpdateCursAsync(_cursEnEdicio);

            // Si actualitzar el curs falla
            if (cursActualitzat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar el curs.");
                esPotEditar = true;
                return;
            }

            await _cursosVM.MostrarCursosAsync();
            EsVisible = Visibility.Collapsed;
        });
    }
}
