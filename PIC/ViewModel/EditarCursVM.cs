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

        // CONSTRUCTOR
        public EditarCursVM(CursosVM cursosVM)
        {
            _cursosVM = cursosVM;

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
        public async Task Mostrar(Curs curs)
        {
            _cursEnEdicio = curs;
            Nom = curs.Nom;

            EsVisible = Visibility.Visible;
        }

        // GUARDAR CURS
        public ICommand GuardarCurs_Click => new RelayCommand(async _ =>
        {
            _cursEnEdicio.Nom = Nom;

            //MessageBox.Show($"{_cursEnEdicio.Id} + {_cursEnEdicio.Nom}");
            await _cursosApiClient.UpdateCursAsync(_cursEnEdicio);
            await _cursosVM.MostrarCursosAsync();

            EsVisible = Visibility.Collapsed;
        });

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
    }
}
