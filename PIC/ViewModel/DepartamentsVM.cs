using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    public enum DepartamentsTipusCerca
    {
        PerId,
        UsuarisPerDepartament
    }

    internal class DepartamentsVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Departament> Departaments { get; set; }
        public ObservableCollection<Usuari> Usuaris { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirDepartamentVM AfegirDepartament { get; set; }
        public EditarDepartamentVM EditarDepartament { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly DepartamentsApiClient _departamentsApiClient;
        private readonly UsuarisApiClient _usuarisApiClient;

        public DepartamentsVM()
        {
            Departaments = new ObservableCollection<Departament>();
            Usuaris = new ObservableCollection<Usuari>();

            _departamentsApiClient = new DepartamentsApiClient();
            _usuarisApiClient = new UsuarisApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirDepartament = new AfegirDepartamentVM(this);
            EditarDepartament= new EditarDepartamentVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarDepartamentsAsync();
        }

        // PROPIETATS DE LA UI
        private Visibility _cercaVisibility = Visibility.Collapsed;
        public Visibility CercaVisibility
        {
            get => _cercaVisibility;
            set { _cercaVisibility = value; OnPropertyChanged(); }
        }

        private string _titolPantalla = "DEPARTAMENTS: TOTS";
        public string TitolPantalla
        {
            get => _titolPantalla;
            set { _titolPantalla = value; OnPropertyChanged(); }
        }

        // CURS SEL·LECCIONAT
        private Departament _departamentSeleccionat;
        public Departament DepartamentSeleccionat
        {
            get => _departamentSeleccionat;
            set
            {
                _departamentSeleccionat = value;
                OnPropertyChanged();

                if (_departamentSeleccionat != null)
                {
                    _ = CarregarUsuarisDelDepartamentAsync((int)_departamentSeleccionat.Id);
                }
            }
        }

        private async Task CarregarUsuarisDelDepartamentAsync(int departamentId)
        {
            try
            {
                var llista = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(departamentId);

                Usuaris.Clear();

                foreach (var d in llista)
                {
                    Usuaris.Add(d);
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error carregant usuaris: " + ex.Message);
            }
        }

        // TIPUS DE CERCA
        private DepartamentsTipusCerca _tipusCercaActualDepartaments;
        public DepartamentsTipusCerca TipusCercaActualDepartaments
        {
            get => _tipusCercaActualDepartaments;
            set
            {
                _tipusCercaActualDepartaments = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private int _parametreCercaDepartaments;
        public int ParametreCercaDepartaments
        {
            get => _parametreCercaDepartaments;
            set
            {
                _parametreCercaDepartaments = value;
                OnPropertyChanged();
            }
        }

        // COMANDAMENTS
        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            if (param == null) return;
            string mode = param.ToString();

            Departaments.Clear();
            CercaVisibility = Visibility.Visible;

            switch (mode)
            {
                case "TOTS":
                    TitolPantalla = "DEPARTAMENTS: TOTS";
                    ParametreCercaDepartaments = 0;
                    ClearUsuaris();
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarDepartamentsAsync();
                    break;

                case "PER_ID":
                    TitolPantalla = "DEPARTAMENTS: PER ID";
                    ParametreCercaDepartaments = 0;
                    ClearUsuaris();
                    TipusCercaActualDepartaments = DepartamentsTipusCerca.PerId;
                    break;
            }
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaDepartamentsAsync();
        });

        // MÈTODE DE CERCA AMB PROTECCIÓ DE NULLS
        public async Task CercaDepartamentsAsync()
        {
            try
            {
                Departaments.Clear();
                switch (TipusCercaActualDepartaments)
                {
                    case DepartamentsTipusCerca.PerId:
                        var departament = await _departamentsApiClient.GetDepartamentPerIdAsync(ParametreCercaDepartaments);

                        if (departament != null)
                        {
                            Departaments.Add(departament);
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap departament amb aquest ID.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error en la cerca: " + ex.Message);
            }
        }

        // MOSTRAR CURSOS
        public async Task MostrarDepartamentsAsync()
        {
            try
            {
                var llista = await _departamentsApiClient.GetAllDepartamentsAsync();
                Departaments.Clear();

                foreach (var d in llista)
                {
                    Departaments.Add(d);
                }
            }

            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error: " + ex.Message);
            }
        }

        // AFEGIR DEPARTAMENT
        public ICommand AfegirDepartamentMenu_Click => new RelayCommand(async _ =>
        {
            AfegirDepartament.Mostrar();
        });

        // EDITAR CURS
        public ICommand EditarDepartamentMenu_Click => new RelayCommand(async _ =>
        {
            if (_departamentSeleccionat != null)
            {
                await EditarDepartament.Mostrar(DepartamentSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un departament.");
            }
        });

        // ESBORRAR DEPARTAMENT

        public ICommand EsborrarDepartamentMenu_Click => new RelayCommand(async _ =>
        {
            if (_departamentSeleccionat != null)
            {
                int departamentId = (int)_departamentSeleccionat.Id;
                var comptarUsuaris = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(departamentId);

                if (comptarUsuaris.Count > 0)
                {
                    MissatgeError.Mostrar("Aquest departament conté un o varis professors, per seguretat, només es poden esborrar els departaments buits. " +
                        "Si vols esborrar aquest departament, primer has d'eliminar els seus professors.");
                }

                else
                {
                    ConfirmarEsborrar.Mostrar(_departamentSeleccionat, this);
                }
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un departament.");
            }
        });

        // BUIDAR LIST VIEW CURSOS
        public void ClearDepartaments()
        {
            Departaments.Clear();
        }

        // BUIDAR LIST VIEW USUARIS
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
    
}
