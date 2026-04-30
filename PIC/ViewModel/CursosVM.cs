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
    public enum CursosTipusCerca
    {
        PerId,
        UsuarisPerCurs
    }

    internal class CursosVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Curs> Cursos { get; set; }
        public ObservableCollection<Usuari> Usuaris { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirCursVM AfegirCurs { get; set; }
        public EditarCursVM EditarCurs { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly CursosApiClient _cursosApiClient;
        private readonly UsuarisApiClient _usuarisApiClient;

        // CONSTRUCTOR
        public CursosVM()
        {
            Cursos = new ObservableCollection<Curs>();
            Usuaris = new ObservableCollection<Usuari>();

            _cursosApiClient = new CursosApiClient();
            _usuarisApiClient = new UsuarisApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirCurs = new AfegirCursVM(this);
            EditarCurs = new EditarCursVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarCursosAsync();
        }

        // PROPIETATS DE LA UI
        private Visibility _cercaVisibility = Visibility.Collapsed;
        public Visibility CercaVisibility
        {
            get => _cercaVisibility;
            set { _cercaVisibility = value; OnPropertyChanged(); }
        }

        private string _titolPantalla = "CURSOS: TOTS";
        public string TitolPantalla
        {
            get => _titolPantalla;
            set { _titolPantalla = value; OnPropertyChanged(); }
        }

        // CURS SELECCIONAT
        private Curs _cursSeleccionat;
        public Curs CursSeleccionat
        {
            get => _cursSeleccionat;
            set
            {
                _cursSeleccionat = value;
                OnPropertyChanged();

                // Carregar els alumnes del curs al seleccionar-ne un
                if (_cursSeleccionat != null)
                {
                    _ = CarregarUsuarisDelCursAsync((int)_cursSeleccionat.Id);
                }
            }
        }

        // CARREGAR USUARIS DEL CURS
        private async Task CarregarUsuarisDelCursAsync(int cursId)
        {
            Usuaris.Clear();
            var llista = await _usuarisApiClient.GetUsuarisPerIdCursAsync(cursId);

            // Si la consulta funciona
            if (llista != null)
            {
                foreach (var u in llista)
                {
                    Usuaris.Add(u);
                }
            }
        }

        // TIPUS DE CERCA
        private CursosTipusCerca _tipusCercaActualCursos;
        public CursosTipusCerca TipusCercaActualCursos
        {
            get => _tipusCercaActualCursos;
            set
            {
                _tipusCercaActualCursos = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private string _parametreCercaCursos;
        public string ParametreCercaCursos
        {
            get => _parametreCercaCursos;
            set
            {
                _parametreCercaCursos = value;
                OnPropertyChanged();
            }
        }

        // COMANDAMENTS
        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            // Si el paràmetre és buit
            if (param == null)
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                string mode = param.ToString();

                Cursos.Clear();
                CercaVisibility = Visibility.Visible;

                switch (mode)
                {
                    case "TOTS":
                        TitolPantalla = "CURSOS: TOTS";
                        ParametreCercaCursos = "";
                        ClearUsuaris();
                        CercaVisibility = Visibility.Collapsed;
                        _ = MostrarCursosAsync();
                        break;

                    case "PER_ID":
                        TitolPantalla = "CURSOS: PER ID";
                        ParametreCercaCursos = "";
                        ClearUsuaris();
                        TipusCercaActualCursos = CursosTipusCerca.PerId;
                        break;
                }
            }                
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaCursosAsync();
        });

        // MÈTODE DE CERCA
        public async Task CercaCursosAsync()
        {
            // Si el textbox és buit
            if (string.IsNullOrEmpty(ParametreCercaCursos))
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                Cursos.Clear();
                switch (TipusCercaActualCursos)
                {
                    case CursosTipusCerca.PerId:
                        var curs = await _cursosApiClient.GetCursPerIdAsync(int.Parse(ParametreCercaCursos));

                        if (curs != null)
                        {
                            Cursos.Add(curs);
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap curs amb aquest ID.");
                        }
                        break;
                }
            }
        }

        // MOSTRAR CURSOS
        public async Task MostrarCursosAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {                
                var llista = await _cursosApiClient.GetAllCursosAsync();

                // Si la consulta falla
                if (llista == null)
                {
                    MissatgeError.Mostrar("No s'han pogut retornar els Cursos. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                }
                else
                {
                    Cursos.Clear();

                    foreach (var u in llista)
                    {
                        Cursos.Add(u);
                    }
                }                
            }
        }

        // AFEGIR CURS
        public ICommand AfegirCursMenu_Click => new RelayCommand(async _ =>
        {
            AfegirCurs.Mostrar();
        });

        // EDITAR CURS
        public ICommand EditarCursMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap curs seleccionat
            if (_cursSeleccionat != null)
            {
                EditarCurs.Mostrar(CursSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un curs.");
            }
        });

        // ESBORRAR CURS
        public ICommand EsborrarCursMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap curs seleccionat
            if (_cursSeleccionat != null)
            {
                int cursId = (int)_cursSeleccionat.Id;
                var comptarUsuaris = await _usuarisApiClient.GetUsuarisPerIdCursAsync(cursId);

                // Si la consulta falla
                if (comptarUsuaris == null)
                {
                    MissatgeError.Mostrar("Hi ha hagut un prolema al intentar esborrar el curs.");
                }
                // Si el curs conté alumnes
                else if (comptarUsuaris.Count > 0)
                {
                    MissatgeError.Mostrar("Aquest curs conté un o varis alumnes, per seguretat, només es poden esborrar els cursos buits. " +
                        "Si vols esborrar aquest curs, primer hasd'eliminar els seus alumnes.");
                }
                else
                {
                    ConfirmarEsborrar.Mostrar(_cursSeleccionat, this);
                }
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un curs.");
            }
        });

        // BUIDAR LIST VIEW USUARIS
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
