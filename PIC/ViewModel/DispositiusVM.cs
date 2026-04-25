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
        private Visibility _cercaVisibility = Visibility.Collapsed;
        public Visibility CercaVisibility
        {
            get => _cercaVisibility;
            set { _cercaVisibility = value; OnPropertyChanged(); }
        }

        private string _titolPantalla = "DISPOSITIUS: TOTS";
        public string TitolPantalla
        {
            get => _titolPantalla;
            set { _titolPantalla = value; OnPropertyChanged(); }
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
        private int _parametreCercaDispositius;
        public int ParametreCercaDispositius
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

        // EDITAR USUARI
        public ICommand EditarDispositiuMenu_Click => new RelayCommand(async _ =>
        {
            if (_dispositiuSeleccionat != null)
            {
                await EditarDispositiu.Mostrar(DispositiuSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un dispositiu.");
            }
        });

        public ICommand HabilitarDispositiuMenu_Click => new RelayCommand(async _ =>
        {
            if (_dispositiuSeleccionat != null)
            {
                if (_dispositiuSeleccionat.Estat.ToLower() == "disponible")
                {
                    MissatgeError.Mostrar("Aquest dispositiu ja està disponible.");
                }
                else if (_dispositiuSeleccionat.Estat.ToLower() == "en prestec")
                {
                    MissatgeError.Mostrar("Aquest dispositiu ja està disponible i es troba en mig d'un préstec.");
                }
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

        public ICommand DeshabilitarDispositiuMenu_Click => new RelayCommand(async _ =>
        {
            if (_dispositiuSeleccionat != null)
            {
                if (_dispositiuSeleccionat.Estat.ToLower() == "no disponible")
                {
                    MissatgeError.Mostrar("Aquest dispositiu ja està deshabilitat.");
                }
                else if (_dispositiuSeleccionat.Estat.ToLower() == "en prestec")
                {
                    MissatgeError.Mostrar("Aquest dispositiu es troba en mig d'un préstec.");
                }
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
            if (_dispositiuSeleccionat != null)
            {
                if (_dispositiuSeleccionat.Estat.ToLower() == "en prestec")
                {
                    MissatgeError.Mostrar("Aquest dispositiu es troba en mig d'un préstec.");
                }
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
            if (param == null) return;
            string mode = param.ToString();

            // Reset de la llista i visibilitat per defecte
            Dispositius.Clear();
            CercaVisibility = Visibility.Visible;

            switch (mode)
            {
                case "TOTS":
                    TitolPantalla = "DISPOSITIUS: TOTS";
                    ParametreCercaDispositius= 0;
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarDispositiusAsync();
                    break;

                case "PER_ID":
                    TitolPantalla = "DISPOSITIUS: PER ID";
                    ParametreCercaDispositius = 0;
                    TipusCercaActualDispositius = DispositiusTipusCerca.PerId;
                    break;

                case "PER_CATEGORIA":
                    TitolPantalla = "DISPOSITIUS: PER CATEGORIA";
                    ParametreCercaDispositius = 0;
                    TipusCercaActualDispositius = DispositiusTipusCerca.PerCategoria;
                    break;

                case "DISPONIBLES":
                    TitolPantalla = "DISPOSITIUS: DISPONIBLES";
                    ParametreCercaDispositius = 0;
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarDispositiusDisponiblesAsync();
                    break;

                case "NO_DISPONIBLES":
                    TitolPantalla = "DISPOSITIUS: NO DISPONIBLES";
                    ParametreCercaDispositius = 0;
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarDispositiusNoDisponiblesAsync();
                    break;

                case "EN_PRESTEC":
                    TitolPantalla = "DISPOSITIUS: EN PRÉSTEC";
                    ParametreCercaDispositius = 0;
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarDispositiusEnPrestecAsync();
                    break;
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
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
                {
                    MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
                }
                else
                {
                    var llista = await _dispositiusApiClient.GetAllDispositiusAsync();
                    if (llista == null)
                    {
                        MissatgeError.Mostrar("No s'han pogut mostrar els Dispositius. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                    }
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

            catch (Exception ex)
            {
                MissatgeError.Mostrar("No es pot connectar amb el servidor: " + ex.Message);
            }
        }

        // MOSTRAR DISPOSITIUS DISPONIBLES
        public async Task MostrarDispositiusDisponiblesAsync()
        {
            try
            {
                var llista = await _dispositiusApiClient.GetDispositiusDisponiblesAsync();
                Dispositius.Clear();

                if (llista != null && llista.Any())
                {
                    foreach (var u in llista)
                    {
                        Dispositius.Add(u);
                    }
                }
                else
                {
                    MissatgeError.Mostrar("No hi ha cap dispositiu disponible.");
                }
            }

            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error: " + ex.Message);
            }
        }

        // MOSTRAR DISPOSITIUS NO DISPONIBLES
        public async Task MostrarDispositiusNoDisponiblesAsync()
        {
            try
            {
                var llista = await _dispositiusApiClient.GetDispositiusNoDisponiblesAsync();
                Dispositius.Clear();

                if (llista != null && llista.Any())
                {
                    foreach (var u in llista)
                    {
                        Dispositius.Add(u);
                    }
                }
            }

            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error: " + ex.Message);
            }
        }

        // MOSTRAR DISPOSITIUS EN PRÉSTEC
        public async Task MostrarDispositiusEnPrestecAsync()
        {
            try
            {
                var llista = await _dispositiusApiClient.GetDispositiusEnPrestecAsync();
                Dispositius.Clear();

                if (llista != null && llista.Any())
                {
                    foreach (var u in llista)
                    {
                        Dispositius.Add(u);
                    }
                }
                else
                {
                    MissatgeError.Mostrar("No hi ha cap dispositiu en préstec.");
                }
            }

            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error: " + ex.Message);
            }
        }

        // MÈTODE DE CERCA
        public async Task CercaDispositiusAsync()
        {
            try
            {
                Dispositius.Clear();
                switch (TipusCercaActualDispositius)
                {
                    case DispositiusTipusCerca.PerId:
                        var dispositiu = await _dispositiusApiClient.GetDispositiuPerIdAsync(ParametreCercaDispositius);

                        if (dispositiu != null)
                        {
                            Dispositius.Add(dispositiu);
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap dispositiu amb aquest ID.");
                        }
                        break;

                    case DispositiusTipusCerca.PerCategoria:
                        var categoria = await _dispositiusApiClient.GetDispositiusPerIdCategoriaAsync(ParametreCercaDispositius);

                        if (categoria != null && categoria.Any())
                        {
                            foreach (var u in categoria)
                            {
                                Dispositius.Add(u);
                            }
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap dispositiu amb aquest ID de categoria.");
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
        public void ClearDispositius()
        {
            Dispositius.Clear();
        }
    }
}
