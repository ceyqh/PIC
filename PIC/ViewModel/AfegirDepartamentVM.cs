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

        // CONSTRUCTOR
        public AfegirDepartamentVM(DepartamentsVM departamentsVM)
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
        public ICommand AfegirDepartament_Click => new RelayCommand(async _ =>
        {
            await CrearNouDepartament();
        });

        // CREAR NOU CURS
        private async Task CrearNouDepartament()
        {
            Departament nouDepartament = new Departament();
            nouDepartament.Nom = Nom;

            await _departamentsApiClient.PostDepartamentAsync(nouDepartament);
            await _departamentsVM.MostrarDepartamentsAsync();

            EsVisible = Visibility.Collapsed;
        }
    }
}
