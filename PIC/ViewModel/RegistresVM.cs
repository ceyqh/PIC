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
        private int _parametreCercRegistres;
        public int ParametreCercaRegistres
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
            if (param == null) return;
            string mode = param.ToString();

            // Reset de la llista i visibilitat per defecte
            Registres.Clear();
            CercaVisibility = Visibility.Visible;

            switch (mode)
            {
                case "TOTS":
                    TitolPantalla = "REGISTRES: TOTS";
                    ParametreCercaRegistres = 0;
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarRegistresAsync();
                    break;

                case "PER_ID_PRESTEC":
                    TitolPantalla = "REGISTRES: PER ID PRESTEC";
                    ParametreCercaRegistres = 0;
                    TipusCercaActualRegistres = RegistresTipusCerca.PerIdPrestec;
                    break;

                case "PER_ID_DISPOSITIU":
                    TitolPantalla = "REGISTRES: PER ID DISPOSITIU";
                    ParametreCercaRegistres = 0;
                    TipusCercaActualRegistres = RegistresTipusCerca.PerIdDispositiu;
                    break;

                case "PER_NOM_GRUP":
                    TitolPantalla = "REGISTRES: PER NOM GRUP";
                    ParametreCercaRegistres = 0;
                    TipusCercaActualRegistres = RegistresTipusCerca.PerNomGrup;
                    break;
            }
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaRegistresAsync();
        });

        // MOSTRAR TOTS ELS USUARIS
        public async Task MostrarRegistresAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
                {
                    MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
                }
                else
                {
                    var llista = await _registresApiClient.GetAllRegistresAsync();
                    if (llista == null)
                    {
                        MissatgeError.Mostrar("No s'han pogut mostrar els Registres. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                    }
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

            catch (Exception ex)
            {
                MissatgeError.Mostrar("No es pot connectar amb el servidor: " + ex.Message);
            }
        }

        // MÈTODE DE CERCA
        public async Task CercaRegistresAsync()
        {
            try
            {
                Registres.Clear();
                switch (TipusCercaActualRegistres)
                {
                    case RegistresTipusCerca.PerIdPrestec:
                        var prestec = await _registresApiClient.GetRegistresPerIdPrestecAsync(ParametreCercaRegistres);

                        if (prestec != null && prestec.Any())
                        {
                            foreach (var u in prestec)
                            {
                                Registres.Add(u);

                            }
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest ID de Préstec.");
                        }
                        break;

                    case RegistresTipusCerca.PerIdDispositiu:
                        var curs = await _registresApiClient.GetRegistresPerIdDispositiuAsync(ParametreCercaRegistres);

                        if (curs != null && curs.Any())
                        {
                            foreach (var u in curs)
                            {
                                Registres.Add(u);

                            }
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest ID de Dispositiu.");
                        }
                        break;

                    case RegistresTipusCerca.PerNomGrup:
                        var dep = await _registresApiClient.GetRegistresPerNomGrupAsync(ParametreCercaRegistres.ToString());

                        if (dep != null && dep.Any())
                        {
                            foreach (var u in dep)
                            {
                                Registres.Add(u);
                            }
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest nom de Grup.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error en la cerca: " + ex.Message);
            }
        }
    }
}
