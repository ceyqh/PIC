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

        // CURS SEL·LECCIONAT
        private Curs _cursSeleccionat;
        public Curs CursSeleccionat
        {
            get => _cursSeleccionat;
            set
            {
                _cursSeleccionat = value;
                OnPropertyChanged();

                if (_cursSeleccionat != null)
                {
                    _ = CarregarUsuarisDelCursAsync((int)_cursSeleccionat.Id);
                }
            }
        }

        private async Task CarregarUsuarisDelCursAsync(int cursId)
        {
            try
            {
                var llista = await _usuarisApiClient.GetUsuarisPerIdCursAsync(cursId);

                Usuaris.Clear();

                foreach (var u in llista)
                {
                    Usuaris.Add(u);
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error carregant usuaris: " + ex.Message);
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
        private int _parametreCercaCursos;
        public int ParametreCercaCursos
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
            if (param == null) return;
            string mode = param.ToString();

            // Reset de la llista i visibilitat per defecte
            Cursos.Clear();
            CercaVisibility = Visibility.Visible;

            switch (mode)
            {
                case "TOTS":
                    TitolPantalla = "CURSOS: TOTS";
                    ParametreCercaCursos = 0;
                    ClearUsuaris();
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarCursosAsync();
                    break;

                case "PER_ID":
                    TitolPantalla = "CURSOS: PER ID";
                    ParametreCercaCursos = 0;
                    ClearUsuaris();
                    TipusCercaActualCursos = CursosTipusCerca.PerId;
                    break;
            }
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaCursosAsync();
        });

        // MÈTODE DE CERCA AMB PROTECCIÓ DE NULLS
        public async Task CercaCursosAsync()
        {
            try
            {
                Cursos.Clear();
                switch (TipusCercaActualCursos)
                {
                    case CursosTipusCerca.PerId:
                        var curs = await _cursosApiClient.GetCursPerIdAsync(ParametreCercaCursos);

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
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error en la cerca: " + ex.Message);
            }
        }

        // MOSTRAR CURSOS
        public async Task MostrarCursosAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
                {
                    MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
                }
                else
                {
                    var llista = await _cursosApiClient.GetAllCursosAsync();
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

            catch (Exception ex)
            {
                MissatgeError.Mostrar("No es pot connectar amb el servidor: " + ex.Message);
            }
        }

        // AFEGIR CURS
        public ICommand AfegirCursMenu_Click => new RelayCommand(async _ =>
        {
            AfegirCurs.Mostrar();
        });

        // EDITAR CURS
        public ICommand EditarCursMenu_Click => new RelayCommand(async _ =>
        {
            if (_cursSeleccionat != null)
            {
                await EditarCurs.Mostrar(CursSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un curs.");
            }
        });

        // ESBORRAR CURS
        public ICommand EsborrarCursMenu_Click => new RelayCommand(async _ =>
        {
            if (_cursSeleccionat != null)
            {
                int cursId = (int)_cursSeleccionat.Id;
                var comptarUsuaris = await _usuarisApiClient.GetUsuarisPerIdCursAsync(cursId);

                if (comptarUsuaris.Count > 0)
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

        // BUIDAR LIST VIEW CURSOS
        public void ClearCursos()
        {
            Cursos.Clear();
        }

        // BUIDAR LIST VIEW USUARIS
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
