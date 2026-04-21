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

        // CONSTRUCTOR
        public EditarDepartamentVM(DepartamentsVM departamentsVM)
        {
            _departamentsVM = departamentsVM;
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
        public async Task Mostrar(Departament departament)
        {
            _departamentEnEdicio = departament;
            Nom = departament.Nom;

            EsVisible = Visibility.Visible;
        }

        // GUARDAR CURS
        public ICommand GuardarCurs_Click => new RelayCommand(async _ =>
        {
            _departamentEnEdicio.Nom = Nom;

            //MessageBox.Show($"{_cursEnEdicio.Id} + {_cursEnEdicio.Nom}");
            await _departamentsApiClient.UpdateDepartamentAsync(_departamentEnEdicio);
            await _departamentsVM.MostrarDepartamentsAsync();

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
