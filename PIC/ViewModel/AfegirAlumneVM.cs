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

            // OMPLIR EL COMBOBOX AMB ELS CURSOS
            if (cursos != null)
            {
                Cursos = cursos.ToList();
            }
            else
            {
                Cursos = new List<Curs>();
            }

            if (Cursos != null && Cursos.Count > 0)
            {
                CursId = Cursos[0].Id;
            }
        }

        // OBRIR FINESTRA
        public void Mostrar()
        {
            Nom = "";
            Cognom = "";
            EsVisible = Visibility.Visible;
        }
        
        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR ALUMNE CLICK
        public ICommand AfegirAlumne_Click => new RelayCommand(async _ =>
        {
            if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Nom))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
            }
            else
            {
                await GuardarNouAlumne(); 
            }
        });

        // GUARDAR NOU ALUMNE (USUARI + ALUMNE)
        private async Task GuardarNouAlumne()
        {
            // Crear usuari
            var nouUsuari = new NouUsuari
            {
                Nom = Nom,
                Cognom = Cognom
            };

            var usuariCreat = await CrearUsuariAsync(nouUsuari);

            if (usuariCreat == null)
                return;

            // Crear alumne amb idUsuari + idCurs
            var alumne = new Alumne
            {
                IdUsuari = usuariCreat.Id,
                IdCurs = CursId
            };

            await CrearAlumneAsync(alumne);
            await _usuarisVM.MostrarUsuarisAsync();

            EsVisible = Visibility.Collapsed;
        }

        // CREAR USUARI
        public async Task<Usuari> CrearUsuariAsync(NouUsuari usuariAAfegir)
        {
            NouUsuari result = await _usuarisApiClient.PostUsuariAsync(usuariAAfegir);

            if (result != null)
            {
                var usuari = new Usuari
                {
                    Id = result.Id,
                    Nom = result.Nom,
                    Cognom = result.Cognom
                };

                return usuari;                
            }

            return null;
        }

        // CREAR ALUMNE
        public async Task CrearAlumneAsync(Alumne alumneAAfegir)
        {
            Alumne result = await _alumnesApiClient.PostAlumneAsync(alumneAAfegir);
        }
    }
}
