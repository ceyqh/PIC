using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using PIC.ViewModel;
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
        private readonly PrestecsApiClient _prestecsApiClient;

        // CONSTRUCTOR
        public UsuarisVM()
        {
            Usuaris = new ObservableCollection<Usuari>();

            _usuarisApiClient = new UsuarisApiClient();
            _prestecsApiClient = new PrestecsApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirAlumne = new AfegirAlumneVM(this);
            AfegirProfessor = new AfegirProfessorVM(this);
            EditarUsuari = new EditarUsuariVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarUsuarisAsync();
        }

        // TEXT DE CERCA
        private string _textCerca = "// USUARIS / TOTS";
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

        // MOSTRAR USUARIS CB
        private string _mostrarUsuaris = "TOTS";
        public string MostrarUsuaris
        {
            get => _mostrarUsuaris;
            set
            {
                _mostrarUsuaris = value;
                OnPropertyChanged();

                ActualitzarModeCerca(value);
            }
        }

        // ORDENAR PER
        private string _ordenarUsuaris= "ID";
        public string OrdenarUsuaris
        {
            get => _ordenarUsuaris;
            set
            {
                if (value == "ID")
                {
                    var llistaOrdenada = Usuaris.OrderBy(d => d.Id).ToList();
                    Usuaris.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Usuaris.Add(d);
                    }
                }

                if (value == "GRUP")
                {
                    var llistaOrdenada = Usuaris.OrderBy(d => d.IdGrup).ToList();
                    Usuaris.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Usuaris.Add(d);
                    }
                }

                if (value == "TIPUS")
                {
                    var llistaOrdenada = Usuaris.OrderBy(d => d.Tipus).ToList();
                    Usuaris.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Usuaris.Add(d);
                    }
                }

                if (value == "COGNOM")
                {
                    var llistaOrdenada = Usuaris.OrderBy(d => d.Cognom).ToList();
                    Usuaris.Clear();

                    foreach (var d in llistaOrdenada)
                    {
                        Usuaris.Add(d);
                    }
                }
                _ordenarUsuaris = value;
                OnPropertyChanged();
            }
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
        private string _parametreCercaUsuaris;
        public string ParametreCercaUsuaris
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
            // Si no hi ha cap usuari seleccionat
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
        public ICommand EsborrarMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap usuari seleccionat
            if (_usuariSeleccionat != null)
            {                
                List<Prestec> llistaPrestecs = await _prestecsApiClient.GetAllPrestecsAsync();

                bool usuariTrobat = false;
                int i = 0;

                while (i < llistaPrestecs.Count && !usuariTrobat)
                {
                    if (llistaPrestecs[i].IdUsuari == UsuariSeleccionat.Id)
                    {
                        usuariTrobat |= true;
                    }
                    else
                    {
                        i++;
                    }
                }

                // Si l'usuari té un prestec en curs
                if (usuariTrobat)
                {
                    MissatgeError.Mostrar("Aquest usuari es troba en mig d'un préstec.");
                }
                else
                {
                    ConfirmarEsborrar.Mostrar(_usuariSeleccionat, this);
                }
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un usuari.");
            }            
        });

        private void ActualitzarModeCerca(string mode)
        {
            Usuaris.Clear();
            ParametreCercaUsuaris = "";

            switch (mode)
            {
                case "TOTS":
                    TextCerca = "// USUARIS / TOTS";
                    HabilitarCerca = false;
                    _ = MostrarUsuarisAsync();
                    break;

                case "PER ID USUARI":
                    TextCerca = "// USUARIS / ID";
                    TipusCercaActualUsuaris = UsuarisTipusCerca.PerId;
                    HabilitarCerca = true;
                    _mostrarUsuaris = "PER ID USUARI";
                    break;

                case "PER ID CURS":
                    TextCerca = "// USUARIS / ID_CURS";
                    TipusCercaActualUsuaris = UsuarisTipusCerca.PerCurs;
                    HabilitarCerca = true;
                    _mostrarUsuaris = "PER ID CURS";
                    break;

                case "PER ID DEPARTAMENT":
                    TextCerca = "// USUARIS / ID_DEPARTAMENT";
                    TipusCercaActualUsuaris = UsuarisTipusCerca.PerDepartament;
                    HabilitarCerca = true;
                    _mostrarUsuaris = "PER ID DEPARTAMENT";
                    break;
            }
            OnPropertyChanged(nameof(MostrarUsuaris));
        }

        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            if (param != null)
            {
                ActualitzarModeCerca(param.ToString());
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
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
                return;
            }

            var llista = await _usuarisApiClient.GetAllUsuarisAsync();

            // Si la consulta falla
            if (llista == null)
            {
                MissatgeError.Mostrar("No s'han pogut mostrar els Usuaris. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                return;
            }

            Usuaris.Clear();

            foreach (var u in llista)
            {
                Usuaris.Add(u);
            }

            TextCerca = $"// USUARIS / TOTS / RESULTATS: {Usuaris.Count}";
        }

        // MÈTODE DE CERCA
        public async Task CercaUsuarisAsync()
        {
            // Si el textbox és buit
            if (string.IsNullOrEmpty(ParametreCercaUsuaris))
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
                return;
            }

            Usuaris.Clear();
            switch (TipusCercaActualUsuaris)
            {
                case UsuarisTipusCerca.PerId:
                    var perId = await _usuarisApiClient.GetUsuariPerIdAsync(int.Parse(ParametreCercaUsuaris));

                    // Si la consulta falla o és buida
                    if (perId == null)
                    {
                        MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID.");
                        return;
                    }

                    Usuaris.Add(perId);

                    TextCerca = $"// USUARIS / ID: {ParametreCercaUsuaris} / RESULTATS: {Usuaris.Count}";
                    break;

                case UsuarisTipusCerca.PerCurs:
                    var perIdCurs = await _usuarisApiClient.GetUsuarisPerIdCursAsync(int.Parse(ParametreCercaUsuaris));

                    // Si la consulta falla o és buida
                    if (perIdCurs == null || !perIdCurs.Any())
                    {
                        MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID de Curs.");
                        return;
                    }

                    foreach (var u in perIdCurs)
                    {
                        Usuaris.Add(u);

                    }

                    TextCerca = $"// USUARIS / ID_CURS: {ParametreCercaUsuaris} / RESULTATS: {Usuaris.Count}";
                    break;

                case UsuarisTipusCerca.PerDepartament:
                    var perIdDepartament = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(int.Parse(ParametreCercaUsuaris));

                    // Si la consulta falla o és buida
                    if (perIdDepartament == null || !perIdDepartament.Any())
                    {
                        MissatgeError.Mostrar("No s'ha trobat cap usuari amb aquest ID de Departament.");
                        return;
                    }

                    foreach (var u in perIdDepartament)
                    {
                        Usuaris.Add(u);
                    }

                    TextCerca = $"// USUARIS / ID_DEPARTAMENT: {ParametreCercaUsuaris} / RESULTATS: {Usuaris.Count}";
                    break;
            }             
        }

        // BUIDAR LIST VIEW
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
