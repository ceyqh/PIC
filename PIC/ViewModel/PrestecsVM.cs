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
    public enum PrestecsTipusCerca
    {
        PerId,
        PerDispositiu,
        PerCategoria,
        PerCurs,
        PerDepartament
    }
    internal class PrestecsVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Prestec> Prestecs { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirPrestecVM AfegirPrestec { get; set; }
        //public EditarUsuariVM EditarUsuari { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly PrestecsApiClient _prestecsApiClient;

        // CONSTRUCTOR
        public PrestecsVM()
        {
            Prestecs = new ObservableCollection<Prestec>();

            _prestecsApiClient = new PrestecsApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirPrestec = new AfegirPrestecVM(this);
            //EditarUsuari = new EditarUsuariVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarPrestecsAsync();
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

        // PRÉSTEC SELECCIONAT
        private Prestec _prestecSeleccionat;
        public Prestec PrestecSeleccionat
        {
            get => _prestecSeleccionat;
            set
            {
                _prestecSeleccionat = value;
                OnPropertyChanged();
            }
        }

        // TIPUS DE CERCA
        private PrestecsTipusCerca _tipusCercaActualPrestecs;
        public PrestecsTipusCerca TipusCercaActualPrestecs
        {
            get => _tipusCercaActualPrestecs;
            set
            {
                _tipusCercaActualPrestecs = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private int _parametreCercaPrestecs;
        public int ParametreCercaPrestecs
        {
            get => _parametreCercaPrestecs;
            set
            {
                _parametreCercaPrestecs = value;
                OnPropertyChanged();
            }
        }

        // AFGIR PRÉSTEC
        public ICommand AfegirPrestecMenu_Click => new RelayCommand(_ =>
        {
            AfegirPrestec.Mostrar();
        });

        // EDITAR PRÉSTEC
        public ICommand EditarPrestecMenu_Click => new RelayCommand(async _ =>
        {
            if (_prestecSeleccionat != null)
            {
                //await EditarUsuari.Mostrar(UsuariSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un préstec.");
            }
        });

        // ESBORRAR PRÉSTEC
        public ICommand EsborrarMenu_Click => new RelayCommand(_ =>
        {
            if (_prestecSeleccionat != null)
            {
                //ConfirmarEsborrar.Mostrar(_usuariSeleccionat, this);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un préstec.");
            }
        });

        // COMANDAMENTS
        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            if (param == null) return;
            string mode = param.ToString();

            // Reset de la llista i visibilitat per defecte
            Prestecs.Clear();
            CercaVisibility = Visibility.Visible;

            switch (mode)
            {
                case "TOTS":
                    TitolPantalla = "PRÉSTECS: TOTS";
                    ParametreCercaPrestecs = 0;
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarPrestecsAsync();
                    break;

                case "PER_ID":
                    TitolPantalla = "PRÉSTECS: PER ID";
                    ParametreCercaPrestecs = 0;
                    TipusCercaActualPrestecs = PrestecsTipusCerca.PerId;
                    break;

                case "PER_DISPOSITIU":
                    TitolPantalla = "PRÉSTECS: PER DISPOSITIU";
                    ParametreCercaPrestecs = 0;
                    TipusCercaActualPrestecs = PrestecsTipusCerca.PerDispositiu;
                    break;

                case "PER_CATEGRIA":
                    TitolPantalla = "PRÉSTECS: PER CATEGORIA";
                    ParametreCercaPrestecs = 0;
                    TipusCercaActualPrestecs = PrestecsTipusCerca.PerCategoria;
                    break;

                case "PER_DEPARTAMENT":
                    TitolPantalla = "PRÉSTECS: PER DEPARTAMENT";
                    ParametreCercaPrestecs = 0;
                    TipusCercaActualPrestecs = PrestecsTipusCerca.PerDepartament;
                    break;

                case "PER_CURS":
                    TitolPantalla = "USUARIS: PER CURS";
                    ParametreCercaPrestecs = 0;
                    TipusCercaActualPrestecs = PrestecsTipusCerca.PerCurs;
                    break;
            }
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ => 
        {
            await CercaPrestecsAsync();
        });

        // MOSTRAR TOTS ELS PRÉSTECS
        public async Task MostrarPrestecsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
                {
                    MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
                }
                else
                {
                    var llista = await _prestecsApiClient.GetAllPrestecsAsync();
                    if (llista == null)
                    {
                        MissatgeError.Mostrar("No s'han pogut mostrar els Préstecs. Comprova que la connexió entre l'API i l'aplicació o la seva configuració.");
                    }
                    else
                    {
                        Prestecs.Clear();

                        foreach (var u in llista)
                        {
                            Prestecs.Add(u);
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                MissatgeError.Mostrar("No es pot connectar amb el servidor: " + ex.Message);
            }
        }

        // MÈTODE DE CERCA
        public async Task CercaPrestecsAsync()
        {
            try
            {
                Prestecs.Clear();
                switch (TipusCercaActualPrestecs)
                {
                    case PrestecsTipusCerca.PerId:
                        var usuari = await _prestecsApiClient.GetPrestecPerIdAsync(ParametreCercaPrestecs);

                        if (usuari != null)
                        {
                            Prestecs.Add(usuari);
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap préstec amb aquest ID.");
                        }
                        break;

                    //case PrestecsTipusCerca.PerCurs:
                    //    var curs = await _usuarisApiClient.GetUsuarisPerIdCursAsync(ParametreCercaUsuaris);

                    //    if (curs != null && curs.Any())
                    //    {
                    //        foreach (var u in curs)
                    //        {
                    //            Usuaris.Add(u);

                    //        }
                    //    }
                    //    else
                    //    {
                    //        MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID de Curs.");
                    //    }
                    //    break;

                    //case PrestecsTipusCerca.PerDepartament:
                    //    var dep = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(ParametreCercaUsuaris);

                    //    if (dep != null && dep.Any())
                    //    {
                    //        foreach (var u in dep)
                    //        {
                    //            Usuaris.Add(u);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID de Departament.");
                    //    }
                    //    break;
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error en la cerca: " + ex.Message);
            }
        }
    }
}
