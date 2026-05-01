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
    public enum DispositiusTipusCerca
    {
        PerId,
        PerCategoria,
    }

    internal class DispositiusVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Dispositiu> Dispositius { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirDispositiuVM AfegirDispositiu { get; set; }
        public EditarDispositiuVM EditarDispositiu { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly DispositiusApiClient _dispositiusApiClient;

        // CONSTRUCTOR
        public DispositiusVM()
        {
            Dispositius = new ObservableCollection<Dispositiu>();

            _dispositiusApiClient = new DispositiusApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirDispositiu = new AfegirDispositiuVM(this);
            EditarDispositiu = new EditarDispositiuVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarDispositiusAsync();
        }

        // PROPIETATS DE LA UI
        private string _titolPantalla = "DISPOSITIUS: TOTS";
        public string TitolPantalla
        {
            get => _titolPantalla;
            set { _titolPantalla = value; OnPropertyChanged(); }
        }

        // ORDENAR PER
        private string _ordenarDispositius = "ID";
        public string OrdenarDispositius
        {
            get => _ordenarDispositius;
            set
            {
                if (value == "ID")
                {
                    var llistaOrdenada = Dispositius.OrderBy(d => d.Id).ToList();
                    Dispositius.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Dispositius.Add(d);
                    }
                }

                if (value == "CATEGORIA")
                {
                    var llistaOrdenada = Dispositius.OrderBy(d => d.IdCategoria).ToList();
                    Dispositius.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Dispositius.Add(d);
                    }
                }
                _ordenarDispositius = value;
                OnPropertyChanged();
            }
        }

        // DISPOSITIU SELECCIONAT
        private Dispositiu _dispositiuSeleccionat;
        public Dispositiu DispositiuSeleccionat
        {
            get => _dispositiuSeleccionat;
            set
            {
                _dispositiuSeleccionat = value;
                OnPropertyChanged();
            }
        }

        // TIPUS DE CERCA
        private DispositiusTipusCerca _tipusCercaActualDispositius;
        public DispositiusTipusCerca TipusCercaActualDispositius
        {
            get => _tipusCercaActualDispositius;
            set
            {
                _tipusCercaActualDispositius = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private string _parametreCercaDispositius;
        public string ParametreCercaDispositius
        {
            get => _parametreCercaDispositius;
            set
            {
                _parametreCercaDispositius = value;
                OnPropertyChanged();
            }
        }

        // AFGIR DISPOSITIU
        public ICommand AfegirDispositiuMenu_Click => new RelayCommand(_ =>
        {
            AfegirDispositiu.Mostrar();
        });

        // EDITAR DISPOSITIU
        public ICommand EditarDispositiuMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap dispositiu seleccionat
            if (_dispositiuSeleccionat != null)
            {
                await EditarDispositiu.Mostrar(DispositiuSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un dispositiu.");
            }
        });

        // HABILITAR DISPOSITIU
        public ICommand HabilitarDispositiuMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap dispositiu seleccionat
            if (_dispositiuSeleccionat != null)
            {
                // Si el dispositiu està disponible
                if (_dispositiuSeleccionat.Estat.ToLower() == "disponible")
                {
                    MissatgeError.Mostrar("Aquest dispositiu ja està disponible.");
                }
                // Si el dispositiu està en préstec
                else if (_dispositiuSeleccionat.Estat.ToLower() == "en prestec")
                {
                    MissatgeError.Mostrar("Aquest dispositiu ja està disponible i es troba en mig d'un préstec.");
                }
                // Habilitar
                else
                {
                    ConfirmarEsborrar.Mostrar(_dispositiuSeleccionat, this, "habilitar");
                }
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un dispositiu.");
            }
        });

        // DESHABILITAR DISPOSITIU
        public ICommand DeshabilitarDispositiuMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap dispositiu seleccionat
            if (_dispositiuSeleccionat != null)
            {
                // Si el dispositiu no està disponible
                if (_dispositiuSeleccionat.Estat.ToLower() == "no disponible")
                {
                    MissatgeError.Mostrar("Aquest dispositiu ja està deshabilitat.");
                }
                // Si el dispositiu està en préstec
                else if (_dispositiuSeleccionat.Estat.ToLower() == "en prestec")
                {
                    MissatgeError.Mostrar("Aquest dispositiu es troba en mig d'un préstec.");
                }
                // Deshabilitar
                else
                {
                    ConfirmarEsborrar.Mostrar(_dispositiuSeleccionat, this, "deshabilitar");
                }
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un dispositiu.");
            }
        });

        // ESBORRAR DISPOSITIU
        public ICommand EsborrarMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap dispositiu seleccionat
            if (_dispositiuSeleccionat != null)
            {
                // Si el dispositiu està en préstec
                if (_dispositiuSeleccionat.Estat.ToLower() == "en prestec")
                {
                    MissatgeError.Mostrar("Aquest dispositiu es troba en mig d'un préstec.");
                }
                // Esborrar
                else
                {
                    ConfirmarEsborrar.Mostrar(_dispositiuSeleccionat, this, "esborrar");
                }
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un dispositiu.");
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

                Dispositius.Clear();

                switch (mode)
                {
                    case "TOTS":
                        TitolPantalla = "DISPOSITIUS: TOTS";
                        ParametreCercaDispositius = "";
                        OrdenarDispositius = "ID";
                        _ = MostrarDispositiusAsync();
                        break;

                    case "PER_ID":
                        TitolPantalla = "DISPOSITIUS: PER ID";
                        ParametreCercaDispositius = "";
                        TipusCercaActualDispositius = DispositiusTipusCerca.PerId;
                        OrdenarDispositius = "ID";
                        break;

                    case "PER_CATEGORIA":
                        TitolPantalla = "DISPOSITIUS: PER CATEGORIA";
                        ParametreCercaDispositius = "";
                        TipusCercaActualDispositius = DispositiusTipusCerca.PerCategoria;
                        OrdenarDispositius = "ID";
                        break;

                    case "DISPONIBLES":
                        TitolPantalla = "DISPOSITIUS: DISPONIBLES";
                        ParametreCercaDispositius = "";
                        _ = MostrarDispositiusDisponiblesAsync();
                        OrdenarDispositius = "ID";
                        break;

                    case "NO_DISPONIBLES":
                        TitolPantalla = "DISPOSITIUS: NO DISPONIBLES";
                        ParametreCercaDispositius = "";
                        _ = MostrarDispositiusNoDisponiblesAsync();
                        OrdenarDispositius = "ID";
                        break;

                    case "EN_PRESTEC":
                        TitolPantalla = "DISPOSITIUS: EN PRÉSTEC";
                        ParametreCercaDispositius = "";
                        _ = MostrarDispositiusEnPrestecAsync();
                        OrdenarDispositius = "ID";
                        break;
                }
            }                
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaDispositiusAsync();
        });

        // MOSTRAR TOTS ELS DISPOSITIUS
        public async Task MostrarDispositiusAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {
                var llista = await _dispositiusApiClient.GetAllDispositiusAsync();

                // Si la consulta falla
                if (llista == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar els Dispositius. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                }
                // Si la consulta funciona
                else
                {
                    Dispositius.Clear();

                    foreach (var u in llista)
                    {
                        Dispositius.Add(u);
                    }
                }
            }
        }

        // MOSTRAR DISPOSITIUS DISPONIBLES
        public async Task MostrarDispositiusDisponiblesAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            else
            {
                var dispositiusDisponibles = await _dispositiusApiClient.GetDispositiusDisponiblesAsync();
                Dispositius.Clear();

                // Si la consulta falla o és buida
                if (dispositiusDisponibles == null || !dispositiusDisponibles.Any())
                {
                    MissatgeError.Mostrar("No hi ha cap dispositiu disponible.");
                }
                // Si la consulta funciona
                else
                {
                    foreach (var u in dispositiusDisponibles)
                    {
                        Dispositius.Add(u);
                    }                    
                }
            }
        }

        // MOSTRAR DISPOSITIUS NO DISPONIBLES
        public async Task MostrarDispositiusNoDisponiblesAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {
                var dispositiusNoDisponibles = await _dispositiusApiClient.GetDispositiusNoDisponiblesAsync();
                Dispositius.Clear();

                // Si la consulta falla o és buida
                if (dispositiusNoDisponibles == null || !dispositiusNoDisponibles.Any())
                {
                    MissatgeError.Mostrar("No hi ha cap dispositiu disponible.");
                }
                // Si la consulta funciona
                else
                {
                    foreach (var u in dispositiusNoDisponibles)
                    {
                        Dispositius.Add(u);
                    }
                }
            }
        }

        // MOSTRAR DISPOSITIUS EN PRÉSTEC
        public async Task MostrarDispositiusEnPrestecAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            else
            {
                var dispositiusEnPrestec = await _dispositiusApiClient.GetDispositiusEnPrestecAsync();
                Dispositius.Clear();

                // Si la consulta falla o és buida
                if (dispositiusEnPrestec == null || !dispositiusEnPrestec.Any())
                {
                    MissatgeError.Mostrar("No hi ha cap dispositiu en préstec.");
                }
                // Si la consulta falla o és buida
                else
                {
                    foreach (var u in dispositiusEnPrestec)
                    {
                        Dispositius.Add(u);
                    }                    
                }
            }
        }

        // MÈTODE DE CERCA
        public async Task CercaDispositiusAsync()
        {
            // Si el textbox és buit
            if (string.IsNullOrEmpty(ParametreCercaDispositius))
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                Dispositius.Clear();
                switch (TipusCercaActualDispositius)
                {
                    case DispositiusTipusCerca.PerId:
                        var dispositiuId = await _dispositiusApiClient.GetDispositiuPerIdAsync(int.Parse(ParametreCercaDispositius));

                        // Si la consulta falla o és buida
                        if (dispositiuId == null)
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap dispositiu amb aquest ID.");
                        }
                        // Si la consulta funciona
                        else
                        {
                            Dispositius.Add(dispositiuId);
                        }
                        break;

                    case DispositiusTipusCerca.PerCategoria:
                        var dispositiuIdCategoria = await _dispositiusApiClient.GetDispositiusPerIdCategoriaAsync(int.Parse(ParametreCercaDispositius));

                        // Si la consulta falla o és buida
                        if (dispositiuIdCategoria == null || !dispositiuIdCategoria.Any())
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap dispositiu amb aquest ID de categoria.");
                        }
                        // Si la consulta falla o és buida
                        else
                        {
                            foreach (var u in dispositiuIdCategoria)
                            {
                                Dispositius.Add(u);
                            }
                        }
                        break;
                }
            }
        }

        // BUIDAR LIST VIEW
        public void ClearDispositius()
        {
            Dispositius.Clear();
        }
    }
}
