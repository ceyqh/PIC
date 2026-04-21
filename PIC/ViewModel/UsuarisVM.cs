using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using PIC.ViewModel;
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
    public enum UsuarisTipusCerca
    {
        PerId,
        PerCurs,
        PerDepartament
    }

    internal class UsuarisVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Usuari> Usuaris { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirAlumneVM AfegirAlumne { get; set; }
        public AfegirProfessorVM AfegirProfessor{ get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }
        public EditarUsuariVM EditarUsuari{ get; set; }

        private readonly UsuarisApiClient _usuarisApiClient;

        // CONSTRUCTOR
        public UsuarisVM()
        {
            Usuaris = new ObservableCollection<Usuari>();

            _usuarisApiClient = new UsuarisApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirAlumne = new AfegirAlumneVM(this);
            AfegirProfessor = new AfegirProfessorVM(this);
            EditarUsuari = new EditarUsuariVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarUsuarisAsync();
        }

        // PROPIETATS DE LA UI
        private Visibility _cercaVisibility = Visibility.Collapsed;
        public Visibility CercaVisibility
        {
            get => _cercaVisibility;
            set { _cercaVisibility = value; OnPropertyChanged(); }
        }

        private string _titolPantalla = "USUARIS: TOTS";
        public string TitolPantalla
        {
            get => _titolPantalla;
            set { _titolPantalla = value; OnPropertyChanged(); }
        }

        // USUARI SELECCIONAT
        private Usuari _usuariSeleccionat;
        public Usuari UsuariSeleccionat
        {
            get => _usuariSeleccionat;
            set
            {
                _usuariSeleccionat = value;
                OnPropertyChanged();
            }
        }

        // TIPUS DE CERCA
        private UsuarisTipusCerca _tipusCercaActualUsuaris;
        public UsuarisTipusCerca TipusCercaActualUsuaris
        {
            get => _tipusCercaActualUsuaris;
            set
            {
                _tipusCercaActualUsuaris = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private int _parametreCercaUsuaris;
        public int ParametreCercaUsuaris
        {
            get => _parametreCercaUsuaris;
            set
            {
                _parametreCercaUsuaris = value;
                OnPropertyChanged();
            }
        }

        // AFGIR ALUMNE
        public ICommand AfegirAlumneMenu_Click => new RelayCommand(_ =>
        {
            AfegirAlumne.Mostrar();
        });

        // AFGIR PROFESSOR 
        public ICommand AfegirProfessorMenu_Click => new RelayCommand(_ =>
        {
            AfegirProfessor.Mostrar();
        });

        // EDITAR USUARI
        public ICommand EditarUsuariMenu_Click => new RelayCommand(async _ =>
        {
            if (_usuariSeleccionat != null)
            {
                await EditarUsuari.Mostrar(UsuariSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un usuari.");
            }
        });

        // ESBORRAR USUARI
        public ICommand EsborrarMenu_Click => new RelayCommand(_ =>
        {
            if (_usuariSeleccionat != null)
            {
                ConfirmarEsborrar.Mostrar(_usuariSeleccionat, this);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un usuari.");
            }            
        });

        // COMANDAMENTS
        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            if (param == null) return;
            string mode = param.ToString();

            // Reset de la llista i visibilitat per defecte
            Usuaris.Clear();
            CercaVisibility = Visibility.Visible;

            switch (mode)
            {
                case "TOTS":
                    TitolPantalla = "USUARIS: TOTS";
                    ParametreCercaUsuaris = 0;
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarUsuarisAsync();
                    break;

                case "PER_ID":
                    TitolPantalla = "USUARIS: PER ID";
                    ParametreCercaUsuaris = 0;
                    TipusCercaActualUsuaris = UsuarisTipusCerca.PerId;
                    break;

                case "PER_CURS":
                    TitolPantalla = "USUARIS: PER CURS";
                    ParametreCercaUsuaris = 0;
                    TipusCercaActualUsuaris = UsuarisTipusCerca.PerCurs;
                    break;

                case "PER_DEPARTAMENT":
                    TitolPantalla = "USUARIS: PER DEPARTAMENT";
                    ParametreCercaUsuaris = 0;
                    TipusCercaActualUsuaris = UsuarisTipusCerca.PerDepartament;
                    break;
            }
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ => 
        {
            await CercaUsuarisAsync();
        });

        // MOSTRAR TOTS ELS USUARIS
        public async Task MostrarUsuarisAsync()
        {
            try
            {
                var llista = await _usuarisApiClient.GetAllUsuarisAsync();
                Usuaris.Clear();

                foreach (var u in llista)
                {
                    Usuaris.Add(u);
                }
            }

            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error: " + ex.Message);
            }
        }

        // MÈTODE DE CERCA
        public async Task CercaUsuarisAsync()
        {
            try
            {
                Usuaris.Clear();
                switch (TipusCercaActualUsuaris)
                {
                    case UsuarisTipusCerca.PerId:
                        var usuari = await _usuarisApiClient.GetUsuariPerIdAsync(ParametreCercaUsuaris);
                        
                        if (usuari != null)
                        {
                            Usuaris.Add(usuari);
                        }                            
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID.");
                        }                            
                        break;

                    case UsuarisTipusCerca.PerCurs:
                        var curs = await _usuarisApiClient.GetUsuarisPerIdCursAsync(ParametreCercaUsuaris);
                        
                        if (curs != null && curs.Any())
                        {
                            foreach (var u in curs)
                            {
                                Usuaris.Add(u);

                            }
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID de Curs.");
                        }
                        break;

                    case UsuarisTipusCerca.PerDepartament:
                        var dep = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(ParametreCercaUsuaris);
                        
                        if (dep != null && dep.Any())
                        {
                            foreach (var u in dep)
                            {
                                Usuaris.Add(u);
                            }
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID de Departament.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error en la cerca: " + ex.Message);
            }
        }


        // BUIDAR LIST VIEW
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
