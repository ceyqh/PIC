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
        PerIdCurs,
        PerIdDepartament
    }

    internal class RegistresVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Registre> Registres { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly RegistresApiClient _registresApiClient;
        private readonly CursosApiClient _cursosApiClient;
        private readonly DepartamentsApiClient _departamentsApiClient;

        // CONSTRUCTOR
        public RegistresVM()
        {
            Registres = new ObservableCollection<Registre>();

            _registresApiClient = new RegistresApiClient();
            _cursosApiClient = new CursosApiClient();
            _departamentsApiClient = new DepartamentsApiClient();

            MissatgeError = new MissatgeErrorVM();
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarRegistresAsync();
        }

        // TEXT DE CERCA
        private string _textCerca = "// REGISTRES / TOTS";
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

        // ORDENAR PER
        private string _ordenarRegistres = "DATA ACCIÓ";
        public string OrdenarRegistres
        {
            get => _ordenarRegistres;
            set
            {
                if (value == "ID PRÉSTEC")
                {
                    var llistaOrdenada = Registres.OrderBy(d => d.IdPrestec).ToList();
                    Registres.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Registres.Add(d);
                    }
                }

                if (value == "USUARI")
                {
                    var llistaOrdenada = Registres.OrderBy(d => d.IdUsuari).ToList();
                    Registres.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Registres.Add(d);
                    }
                }

                if (value == "DATA ACCIÓ")
                {
                    var llistaOrdenada = Registres.OrderBy(d => d.DataAccio).ToList();
                    Registres.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Registres.Add(d);
                    }
                }

                if (value == "DATA RETORN")
                {
                    var llistaOrdenada = Registres.OrderBy(d => d.DataRetorn).ToList();
                    Registres.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Registres.Add(d);
                    }
                }
                _ordenarRegistres = value;
                OnPropertyChanged();
            }
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
                return;
            }
            
            string mode = param.ToString();

            Registres.Clear();

            switch (mode)
            {
                case "TOTS":
                    TextCerca = "// REGISTRES / TOTS";
                    ParametreCercaRegistres = "";
                    HabilitarCerca = false;
                    _ = MostrarRegistresAsync();
                    break;

                case "PER_ID_PRESTEC":
                    TextCerca = "// REGISTRES / ID_PRESTEC";
                    ParametreCercaRegistres = "";
                    TipusCercaActualRegistres = RegistresTipusCerca.PerIdPrestec;
                    HabilitarCerca = true;
                    break;

                case "PER_ID_DISPOSITIU":
                    TextCerca = "// REGISTRES / ID_DISPOSITIU";
                    ParametreCercaRegistres = "";
                    TipusCercaActualRegistres = RegistresTipusCerca.PerIdDispositiu;
                    HabilitarCerca = true;
                    break;

                case "PER_ID_CURS":
                    TextCerca = "// REGISTRES / ID_CURS";
                    ParametreCercaRegistres = "";
                    TipusCercaActualRegistres = RegistresTipusCerca.PerIdCurs;
                    HabilitarCerca = true;
                    break;

                case "PER_ID_DEPARTAMENT":
                    TextCerca = "// REGISTRES / ID_DEPARTAMENT";
                    ParametreCercaRegistres = "";
                    TipusCercaActualRegistres = RegistresTipusCerca.PerIdDepartament;
                    HabilitarCerca = true;
                    break;
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
                return;
            }

            var llista = await _registresApiClient.GetAllRegistresAsync();

            // Si la consulta falla
            if (llista == null)
            {
                MissatgeError.Mostrar("No s'han pogut mostrar els Registres. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                return;
            }

            Registres.Clear();

            foreach (var u in llista)
            {
                Registres.Add(u);
            }

            TextCerca = $"// REGISTRES / TOTS / RESULTATS: {Registres.Count}";
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
                        TextCerca = $"// REGISTRES / ID_PRESTEC: {ParametreCercaRegistres} / RESULTATS: {Registres.Count}";
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
                        TextCerca = $"// REGISTRES / ID_DISPOSITIU: {ParametreCercaRegistres} / RESULTATS: {Registres.Count}";
                        break;

                    case RegistresTipusCerca.PerIdCurs:
                        var cursos = await _cursosApiClient.GetAllCursosAsync();

                        // Si falla la consulta
                        if (cursos == null)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al consultar els cursos.");
                            return;
                        }

                        bool cursTrobat = false;
                        int i = 0;

                        while (i < cursos.Count && !cursTrobat)
                        {
                            if (cursos[i].Id == int.Parse(ParametreCercaRegistres))
                            {
                                cursTrobat = true;
                            }
                            else
                            {
                                i++;
                            }
                        }

                        // Si no es troba el curs
                        if (!cursTrobat)
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest ID de Curs.");
                            return;
                        }
                        var registresCurs = await _registresApiClient.GetAllRegistresAsync();

                        // Si falla la consulta
                        if (registresCurs == null)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al consultar els registres.");
                            return;
                        }

                        foreach (var r in registresCurs)
                        {
                            if (r.IdGrup == int.Parse(ParametreCercaRegistres))
                            {
                                Registres.Add(r);
                            }
                        }

                        TextCerca = $"// REGISTRES / ID_CURS: {ParametreCercaRegistres} / RESULTATS: {Registres.Count}";
                        break;

                    case RegistresTipusCerca.PerIdDepartament:
                        var departaments = await _departamentsApiClient.GetAllDepartamentsAsync();

                        // Si falla la consulta
                        if (departaments == null)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al consultar els departaments.");
                            return;
                        }

                        bool departamenTrobat = false;
                        int j = 0;

                        while (j < departaments.Count && !departamenTrobat)
                        {
                            if (departaments[j].Id == int.Parse(ParametreCercaRegistres))
                            {
                                departamenTrobat = true;
                            }
                            else
                            {
                                j++;
                            }
                        }

                        // Si no es troba el curs
                        if (!departamenTrobat)
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap registre amb aquest ID de Departament.");
                            return;
                        }
                        var registresDepartament = await _registresApiClient.GetAllRegistresAsync();

                        // Si falla la consulta
                        if (registresDepartament == null)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al consultar els registres.");
                            return;
                        }

                        foreach (var r in registresDepartament)
                        {
                            if (r.IdGrup== int.Parse(ParametreCercaRegistres))
                            {
                                Registres.Add(r);
                            }
                        }

                        TextCerca = $"// REGISTRES / ID_DEPARTAMENT: {ParametreCercaRegistres} / RESULTATS: {Registres.Count}";
                        break;
                }                
            }
        }
    }
}
