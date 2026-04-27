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
    public enum RegistresTipusCerca
    {
        PerIdPrestec,
        PerIdDispositiu,
        PerNomGrup
    }

    internal class RegistresVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Registre> Registres { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly RegistresApiClient _registresApiClient;

        // CONSTRUCTOR
        public RegistresVM()
        {
            Registres = new ObservableCollection<Registre>();

            _registresApiClient = new RegistresApiClient();

            MissatgeError = new MissatgeErrorVM();
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarRegistresAsync();
        }

        // PROPIETATS DE LA UI
        private Visibility _cercaVisibility = Visibility.Collapsed;
        public Visibility CercaVisibility
        {
            get => _cercaVisibility;
            set { _cercaVisibility = value; OnPropertyChanged(); }
        }

        private string _titolPantalla = "REGISTRES";
        public string TitolPantalla
        {
            get => _titolPantalla;
            set { _titolPantalla = value; OnPropertyChanged(); }
        }

        // TIPUS DE CERCA
        private RegistresTipusCerca _tipusCercaActualRegistres;
        public RegistresTipusCerca TipusCercaActualRegistres
        {
            get => _tipusCercaActualRegistres;
            set
            {
                _tipusCercaActualRegistres = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private string _parametreCercRegistres;
        public string ParametreCercaRegistres
        {
            get => _parametreCercRegistres;
            set
            {
                _parametreCercRegistres = value;
                OnPropertyChanged();
            }
        }

        // COMANDAMENTS
        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            // Si el textbox és buit
            if (param == null)
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                string mode = param.ToString();

                Registres.Clear();
                CercaVisibility = Visibility.Visible;

                switch (mode)
                {
                    case "TOTS":
                        TitolPantalla = "REGISTRES: TOTS";
                        ParametreCercaRegistres = "";
                        CercaVisibility = Visibility.Collapsed;
                        _ = MostrarRegistresAsync();
                        break;

                    case "PER_ID_PRESTEC":
                        TitolPantalla = "REGISTRES: PER ID PRESTEC";
                        ParametreCercaRegistres = "";
                        TipusCercaActualRegistres = RegistresTipusCerca.PerIdPrestec;
                        break;

                    case "PER_ID_DISPOSITIU":
                        TitolPantalla = "REGISTRES: PER ID DISPOSITIU";
                        ParametreCercaRegistres = "";
                        TipusCercaActualRegistres = RegistresTipusCerca.PerIdDispositiu;
                        break;

                    case "PER_NOM_GRUP":
                        TitolPantalla = "REGISTRES: PER NOM GRUP";
                        ParametreCercaRegistres = "";
                        TipusCercaActualRegistres = RegistresTipusCerca.PerNomGrup;
                        break;
                }
            }               
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaRegistresAsync();
        });

        // MOSTRAR TOTS ELS REGISTRES
        public async Task MostrarRegistresAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {
                var llista = await _registresApiClient.GetAllRegistresAsync();

                // Si la consulta falla
                if (llista == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar els Registres. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                }
                // Si la consulta funciona
                else
                {
                    Registres.Clear();

                    foreach (var u in llista)
                    {
                        Registres.Add(u);
                    }
                }

            }
        }

        // MÈTODE DE CERCA
        public async Task CercaRegistresAsync()
        {
            // Si el textbox és buit
            if (string.IsNullOrEmpty(ParametreCercaRegistres))
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                Registres.Clear();
                switch (TipusCercaActualRegistres)
                {
                    case RegistresTipusCerca.PerIdPrestec:
                        var perIdPrestec = await _registresApiClient.GetRegistresPerIdPrestecAsync(int.Parse(ParametreCercaRegistres));

                        // Si la consulta falla o és buida
                        if (perIdPrestec == null || !perIdPrestec.Any())
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest ID de Préstec.");
                        }
                        // Si la consulta funciona
                        else
                        {
                            foreach (var u in perIdPrestec)
                            {
                                Registres.Add(u);

                            }
                        }
                        break;

                    case RegistresTipusCerca.PerIdDispositiu:
                        var perIdDispositiu = await _registresApiClient.GetRegistresPerIdDispositiuAsync(int.Parse(ParametreCercaRegistres));

                        // Si la consulta falla o és buida
                        if (perIdDispositiu == null || !perIdDispositiu.Any())
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest ID de Dispositiu.");
                        }
                        // Si la consulta funciona
                        else
                        {
                            foreach (var u in perIdDispositiu)
                            {
                                Registres.Add(u);

                            }                            
                        }
                        break;

                    case RegistresTipusCerca.PerNomGrup:
                        var perNomGrup = await _registresApiClient.GetRegistresPerNomGrupAsync(ParametreCercaRegistres.ToString());

                        // Si la consulta falla o és buida
                        if (perNomGrup == null || !perNomGrup.Any())
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest nom de Grup.");
                        }
                        // Si la consulta funciona
                        else
                        {
                            foreach (var u in perNomGrup)
                            {
                                Registres.Add(u);
                            }                            
                        }
                        break;
                }                
            }
        }
    }
}
