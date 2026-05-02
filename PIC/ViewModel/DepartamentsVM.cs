using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

        // TEXT CERCA
        private string _textCerca = "// DEPARTAMENTS / TOTS";
        public string TextCerca
        {
            get => _textCerca;
            set { _textCerca = value; OnPropertyChanged(); }
        }

        // HABILITAR CERCA
        private bool _habilitarCerca = false;
        public bool HabilitarCerca
        {
            get => _habilitarCerca;
            set
            {
                _habilitarCerca = value;
                OnPropertyChanged();
            }
        }

        // MOSTRAR DEPARTAMENTS CB
        private string _mostrarDepartaments= "TOTS";
        public string MostrarDepartaments
        {
            get => _mostrarDepartaments;
            set
            {
                _mostrarDepartaments = value;
                OnPropertyChanged();

                ActualitzarModeCerca(value);
            }
        }

        // DEPARTAMENT SELECCIONAT
        private Departament _departamentSeleccionat;
        public Departament DepartamentSeleccionat
        {
            get => _departamentSeleccionat;
            set
            {
                _departamentSeleccionat = value;
                OnPropertyChanged();

                // Carregar els usuaris del departament
                if (_departamentSeleccionat != null)
                {
                    _ = CarregarUsuarisDelDepartamentAsync((int)_departamentSeleccionat.Id);
                }
            }
        }

        // CARREGAR USUARIS DEL DEPARTAMENT
        private async Task CarregarUsuarisDelDepartamentAsync(int departamentId)
        {
            Usuaris.Clear();
            var llista = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(departamentId);

            // Si la consulta funciona
            if (llista != null)
            {
                foreach (var d in llista)
                {
                    Usuaris.Add(d);
                }

            }

            TextCerca = $"// DEPARTAMENTS / ID: {DepartamentSeleccionat.Id} / USUARIS / RESULTATS: {Usuaris.Count}";
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
        private string _parametreCercaDepartaments;
        public string ParametreCercaDepartaments
        {
            get => _parametreCercaDepartaments;
            set
            {
                _parametreCercaDepartaments = value;
                OnPropertyChanged();
            }
        }
        private void ActualitzarModeCerca(string mode)
        {
            Departaments.Clear();
            ParametreCercaDepartaments= "";

            switch (mode)
            {
                case "TOTS":
                    TextCerca = "// DEPARTAMENTS / TOTS";
                    ParametreCercaDepartaments = "";
                    ClearUsuaris();
                    HabilitarCerca = false;
                    _ = MostrarDepartamentsAsync();
                    break;

                case "PER ID DEPARTAMENT":
                    TextCerca = "// DEPARTAMENTS / ID";
                    ParametreCercaDepartaments = "";
                    ClearUsuaris();
                    HabilitarCerca = true;
                    TipusCercaActualDepartaments = DepartamentsTipusCerca.PerId;
                    break;
            }
            OnPropertyChanged(nameof(MostrarDepartaments));
        }

        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            if (param != null)
            {
                ActualitzarModeCerca(param.ToString());
            }
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaDepartamentsAsync();
        });

        // MÈTODE DE CERCA
        public async Task CercaDepartamentsAsync()
        {
            // Si el textbox és buit
            if (string.IsNullOrEmpty(ParametreCercaDepartaments))
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                Departaments.Clear();
                switch (TipusCercaActualDepartaments)
                {
                    case DepartamentsTipusCerca.PerId:
                        var departament = await _departamentsApiClient.GetDepartamentPerIdAsync(int.Parse(ParametreCercaDepartaments));

                        if (departament == null)
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap departament amb aquest ID.");
                        }
                        else
                        {
                            Departaments.Add(departament);
                        }

                        TextCerca = $"// DEPARTAMENTS / ID: {ParametreCercaDepartaments} / RESULTATS: {Departaments.Count}";

                        break;
                }
            }
        }

        // MOSTRAR DEPARTAMENTS
        public async Task MostrarDepartamentsAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else 
            {
                var llista = await _departamentsApiClient.GetAllDepartamentsAsync();


                // Si la consulta falla
                if (llista == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar els Departaments. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                }
                else
                {
                    Departaments.Clear();

                    foreach (var d in llista)
                    {
                        Departaments.Add(d);
                    }

                    TextCerca = $"// DEPARTAMENTS / TOTS / RESULTATS: {Departaments.Count}";
                }
            }            
        }

        // AFEGIR DEPARTAMENT
        public ICommand AfegirDepartamentMenu_Click => new RelayCommand(async _ =>
        {
            AfegirDepartament.Mostrar();
        });

        // EDITAR DEPARTAMENT
        public ICommand EditarDepartamentMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap departament seleccionat
            if (_departamentSeleccionat != null)
            {
                EditarDepartament.Mostrar(DepartamentSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un departament.");
            }
        });

        // ESBORRAR DEPARTAMENT
        public ICommand EsborrarDepartamentMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap departament seleccionat
            if (_departamentSeleccionat != null)
            {
                int departamentId = (int)_departamentSeleccionat.Id;
                var comptarUsuaris = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(departamentId);

                // Si la consulta falla
                if (comptarUsuaris == null)
                {
                    MissatgeError.Mostrar("Hi ha hagut un prolema al intentar esborrar el departament.");
                }
                // Si el departament conté professors
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

        // BUIDAR LIST VIEW USUARIS
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
    
}
