using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    internal class AfegirAlumneVM : Utilities.ViewModelBase
    {
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly CursosApiClient _cursosApiClient;

        private readonly UsuarisVM _usuarisVM;
        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotAfegir = true;

        // CONSTRUCTOR
        public AfegirAlumneVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            MissatgeError = new MissatgeErrorVM();
            _usuarisApiClient = new UsuarisApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _cursosApiClient = new CursosApiClient();

            CarregarCursos();
        }

        // NOM
        private string _nom;
        public string Nom
        {
            get => _nom;
            set
            {
                _nom = value;
                OnPropertyChanged();
            }
        }

        // COGNOM
        private string _cognom;
        public string Cognom
        {
            get => _cognom;
            set
            {
                _cognom = value;
                OnPropertyChanged();
            }
        }

        // VISIBILITAT MENU
        private Visibility _esVisble = Visibility.Collapsed;
        public Visibility EsVisible
        {
            get => _esVisble;
            set
            {
                _esVisble = value;
                OnPropertyChanged();
            }
        }

        // LLISTA CURSOS
        private List<Curs> _cursos;
        public List<Curs> Cursos
        {
            get => _cursos;
            set
            {
                _cursos = value;
                OnPropertyChanged();
            }
        }

        // CURS SELECCIONAT
        private long _cursId;
        public long CursId
        {
            get => _cursId;
            set
            {
                _cursId = value;
                OnPropertyChanged();
            }
        }

        // CARREGAR CURSOS
        private async void CarregarCursos()
        {
            var cursos = await _cursosApiClient.GetAllCursosAsync();

            // Si falla la càrrega de cursos
            if (cursos == null)
            {
                Cursos = new List<Curs>();
            }
            // Si es retornen els cursos correctament
            else
            {
                Cursos = cursos.ToList();
            }

            // Si es retornen buits
            if (Cursos == null || Cursos.Count < 1)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al carregar els cursos.");
            }
            else
            {
                CursId = Cursos[0].Id;
            }
        }

        // OBRIR FINESTRA
        public void Mostrar()
        {
            Nom = "";
            Cognom = "";

            esPotAfegir = true;
            EsVisible = Visibility.Visible;
        }
        
        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR NOU ALUMNE
        public ICommand AfegirAlumne_Click => new RelayCommand(async _ =>
        {
            if (esPotAfegir)
            {
                // Si hi ha camps buits
                if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Nom))
                {
                    MissatgeError.Mostrar("No hi poden haver camps buits.");
                }
                else
                {
                    // Crear usuari
                    var nouUsuari = new NouUsuari
                    {
                        Nom = Nom,
                        Cognom = Cognom
                    };

                    NouUsuari usuariCreat = await _usuarisApiClient.PostUsuariAsync(nouUsuari);

                    // Si crear l'usuari falla
                    if (usuariCreat == null)
                    {
                        MissatgeError.Mostrar("Hi ha hagut un problema al crear l'usuari.");
                    }
                    // Si crear l'usuari funciona
                    else
                    {
                        var nouAlumne = new Alumne
                        {
                            IdUsuari = usuariCreat.Id,
                            IdCurs = CursId
                        };

                        Alumne alumneCreat = await _alumnesApiClient.PostAlumneAsync(nouAlumne);

                        // Si crear l'alumne falla
                        if (alumneCreat == null)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al crear l'alumne.");
                        }
                        // Si crear l'alumne funciona
                        else
                        {
                            esPotAfegir = false;
                            await _usuarisVM.MostrarUsuarisAsync();
                            EsVisible = Visibility.Collapsed;
                        }
                    }
                }
            }            
        });
    }
}
