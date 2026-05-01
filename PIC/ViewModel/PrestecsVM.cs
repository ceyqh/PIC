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
        PerUsuari,
        PerDispositiu
    }
    internal class PrestecsVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Prestec> Prestecs { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirPrestecVM AfegirPrestec { get; set; }
        public EditarPrestecVM EditarPrestec{ get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly PrestecsApiClient _prestecsApiClient;

        // CONSTRUCTOR
        public PrestecsVM()
        {
            Prestecs = new ObservableCollection<Prestec>();

            _prestecsApiClient = new PrestecsApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirPrestec = new AfegirPrestecVM(this);
            EditarPrestec = new EditarPrestecVM(this);
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

        // ORDENAR PER
        private string _ordenarPrestecs= "ID";
        public string OrdenarPrestecs
        {
            get => _ordenarPrestecs;
            set
            {
                if (value == "ID")
                {
                    var llistaOrdenada = Prestecs.OrderBy(d => d.Id).ToList();
                    Prestecs.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Prestecs.Add(d);
                    }
                }

                if (value == "USUARI")
                {
                    var llistaOrdenada = Prestecs.OrderBy(d => d.IdUsuari).ToList();
                    Prestecs.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Prestecs.Add(d);
                    }
                }

                if (value == "DATA ENTREGA")
                {
                    var llistaOrdenada = Prestecs.OrderBy(d => d.DataEntrega).ToList();
                    Prestecs.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Prestecs.Add(d);
                    }
                }

                if (value == "DATA RETORN")
                {
                    var llistaOrdenada = Prestecs.OrderBy(d => d.DataRetorn).ToList();
                    Prestecs.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Prestecs.Add(d);
                    }
                }
                _ordenarPrestecs = value;
                OnPropertyChanged();
            }
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
        private string _parametreCercaPrestecs;
        public string ParametreCercaPrestecs
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
        public ICommand EditarPrestecMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap préstec seleccionat
            if (_prestecSeleccionat != null)
            {
                EditarPrestec.Mostrar(PrestecSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un préstec.");
            }
        });

        // FINALITZAR PRÉSTEC
        public ICommand FinalitzarPrestec_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap préstec seleccionat
            if (_prestecSeleccionat != null)
            {
                ConfirmarEsborrar.Mostrar(_prestecSeleccionat, this, "finalitzar");
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un préstec.");
            }
        });

        // ESBORRAR PRÉSTEC
        public ICommand EsborrarMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap préstec seleccionat
            if (_prestecSeleccionat != null)
            {
                ConfirmarEsborrar.Mostrar(_prestecSeleccionat, this, "esborrar");
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un préstec.");
            }
        });

        // COMANDAMENTS
        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            // Si no hi ha paràmetre
            if (param == null) 
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                string mode = param.ToString();

                Prestecs.Clear();
                CercaVisibility = Visibility.Visible;

                switch (mode)
                {
                    case "TOTS":
                        TitolPantalla = "PRÉSTECS: TOTS";
                        ParametreCercaPrestecs = "";
                        CercaVisibility = Visibility.Collapsed;
                        OrdenarPrestecs = "ID";
                        _ = MostrarPrestecsAsync();
                        break;

                    case "PER_ID":
                        TitolPantalla = "PRÉSTECS: PER ID";
                        ParametreCercaPrestecs = "";
                        TipusCercaActualPrestecs = PrestecsTipusCerca.PerId;
                        OrdenarPrestecs = "ID";
                        break;

                    case "PER_USUARI":
                        TitolPantalla = "PRÉSTECS: PER USUARI";
                        ParametreCercaPrestecs = "";
                        TipusCercaActualPrestecs = PrestecsTipusCerca.PerUsuari;
                        OrdenarPrestecs = "ID";
                        break;

                    case "PER_DISPOSITIU":
                        TitolPantalla = "PRÉSTECS: PER DISPOSITIU";
                        ParametreCercaPrestecs = "";
                        TipusCercaActualPrestecs = PrestecsTipusCerca.PerDispositiu;
                        OrdenarPrestecs = "ID";
                        break;

                    case "EN_CURS":
                        TitolPantalla = "PRÉSTECS: EN CURS";
                        ParametreCercaPrestecs = "";
                        CercaVisibility = Visibility.Collapsed;
                        OrdenarPrestecs = "ID";
                        _ = MostrarPrestecsEnCursAsync();
                        break;

                    case "CADUCATS":
                        TitolPantalla = "USUARIS: CADUCATS";
                        ParametreCercaPrestecs = "";
                        CercaVisibility = Visibility.Collapsed;
                        OrdenarPrestecs = "ID";
                        _ = MostrarPrestecsCaducatsAsync();
                        break;
                }
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
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {
                var llista = await _prestecsApiClient.GetAllPrestecsAsync();

                // Si la consulta falla
                if (llista == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar els Préstecs. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                }
                // Si la consulta funciona
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

        // MOSTRAR PRÉSTECS EN CURS
        public async Task MostrarPrestecsEnCursAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {
                Prestecs.Clear();
                var enCurs = await _prestecsApiClient.GetAllPrestecsAsync();

                // Si la consulta falla
                if (enCurs == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar els Préstecs. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");

                }
                // Si la consulta funciona
                else
                {
                    foreach (var u in enCurs)
                    {
                        if (u.Estat.ToLower() == "en curs")
                        {
                            Prestecs.Add(u);
                        }
                    }
                }
            }            
        }

        public async Task MostrarPrestecsCaducatsAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {
                Prestecs.Clear();
                var caducats = await _prestecsApiClient.GetAllPrestecsAsync();

                // Si la consulta falla
                if (caducats == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar els Préstecs. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");

                }
                // Si la consulta funciona
                else
                {
                    foreach (var u in caducats)
                    {
                        if (u.Estat.ToLower() == "caducat")
                        {
                            Prestecs.Add(u);
                        }
                    }
                }
            }
        }

        // MÈTODE DE CERCA
        public async Task CercaPrestecsAsync()
        {
            if (string.IsNullOrEmpty(ParametreCercaPrestecs))
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                Prestecs.Clear();
                switch (TipusCercaActualPrestecs)
                {
                    case PrestecsTipusCerca.PerId:
                        var perId = await _prestecsApiClient.GetPrestecPerIdAsync(int.Parse(ParametreCercaPrestecs));

                        // Si la consulta falla o no es troba
                        if (perId == null)
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap préstec amb aquest ID.");

                        }
                        else
                        {
                            Prestecs.Add(perId);
                        }
                        break;

                    case PrestecsTipusCerca.PerUsuari:
                        var perIdUsuari = await _prestecsApiClient.GetPrestecsPerIdUsuariAsync(int.Parse(ParametreCercaPrestecs));

                        // Si la consulta falla o no es troba
                        if (perIdUsuari == null || !perIdUsuari.Any())
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap préstec amb aquest ID d'Usuari.");
                        }
                        else
                        {
                            foreach (var u in perIdUsuari)
                            {
                                Prestecs.Add(u);

                            }
                        }
                        break;

                    case PrestecsTipusCerca.PerDispositiu:
                        var perIdDispositiu = await _prestecsApiClient.GetPrestecsPerIdDispositiuAsync(int.Parse(ParametreCercaPrestecs));

                        // Si la consulta falla o no es troba
                        if (perIdDispositiu == null || !perIdDispositiu.Any())
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap préstec amb aquest ID de Dispositiu.");
                        }
                        else
                        {
                            foreach (var u in perIdDispositiu)
                            {
                                Prestecs.Add(u);

                            }
                        }
                        break;
                }
            }
        }
    }
}
