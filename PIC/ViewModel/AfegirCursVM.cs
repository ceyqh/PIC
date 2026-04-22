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
    internal class AfegirCursVM: Utilities.ViewModelBase
    {
        private readonly CursosApiClient _cursosApiClient;
        private readonly CursosVM _cursosVM;
        public MissatgeErrorVM MissatgeError { get; set; }

        // CONSTRUCTOR
        public AfegirCursVM(CursosVM cursosVM)
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
        public void Mostrar()
        {
            Nom = "";
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR NOU CURS
        public ICommand AfegirCurs_Click => new RelayCommand(async _ =>
        {
            if (string.IsNullOrWhiteSpace(Nom))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
            }
            else
            {
                await CrearNouCurs();
            }
        });

        // CREAR NOU CURS
        private async Task CrearNouCurs()
        {
            Curs nouCurs = new Curs();
            nouCurs.Nom = Nom;

            await _cursosApiClient.PostCursAsync(nouCurs);
            await _cursosVM.MostrarCursosAsync();

            EsVisible = Visibility.Collapsed;
        }
    }
}
