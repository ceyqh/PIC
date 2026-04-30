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
    internal class AfegirProfessorVM: Utilities.ViewModelBase
    {
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly ProfessorsApiClient _professorsApiClient;
        private readonly DepartamentsApiClient _departamentsApiClient;
        private readonly UsuarisVM _usuarisVM;
        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotAfegir = true;

        // CONSTRUCTOR
        public AfegirProfessorVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            MissatgeError = new MissatgeErrorVM();
            _usuarisApiClient = new UsuarisApiClient();
            _professorsApiClient = new ProfessorsApiClient();
            _departamentsApiClient = new DepartamentsApiClient();

            CarregarDepartaments();
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

        // LLISTA DEPARTAMENTS
        private List<Departament> _departaments;
        public List<Departament> Departaments
        {
            get => _departaments;
            set
            {
                _departaments = value;
                OnPropertyChanged();
            }
        }

        // DEPARTAMENT SELECCIONAT
        private long _departamentId;
        public long DepartamentId
        {
            get => _departamentId;
            set
            {
                _departamentId = value;
                OnPropertyChanged();
            }
        }

        // CARREGAR DEPARTAMENTS
        private async void CarregarDepartaments()
        {
            var departaments = await _departamentsApiClient.GetAllDepartamentsAsync();

            // Si falla la càrrega de cursos
            if (departaments == null)
            {
                Departaments = new List<Departament>();
            }
            // Si es retornen els departaments correctament
            else
            {
                Departaments = departaments.ToList();                
            }

            // Si es retornen buits
            if (Departaments == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al carregar els departaments.");
            }
            else if (Departaments.Count < 1)
            {
                MissatgeError.Mostrar("No hi ha departaments disponibles. No podràs crear l'usuari.");
            }
            else
            {
                DepartamentId = Departaments[0].Id;
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

        // AFEGIR PROFESSOR CLICK
        public ICommand AfegirProfessor_Click => new RelayCommand(async _ =>
        {
            if (!esPotAfegir)
            {
                return;
            }

            esPotAfegir = false;

            // Si els camps no estan buits
            if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Nom))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
                esPotAfegir = true;
                return;
            }

            // Si els camp departaments és buit
            if (Departaments == null || !Departaments.Any())
            {
                MissatgeError.Mostrar("No hi ha departaments disponibles.");
                esPotAfegir = true;
                return;
            }

            // Crear nou usuari
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
                esPotAfegir = true;
                return;
            }

            // Crear nou professor
            var nouProfessor = new Professor
            {
                IdUsuari = usuariCreat.Id,
                IdDepartament = DepartamentId
            };

            Professor professorCreat = await _professorsApiClient.PostProfessorAsync(nouProfessor);

            // Si crear el professor falla
            if (professorCreat == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al crear el professor.");
                esPotAfegir = true;
                return;
            }

            await _usuarisVM.MostrarUsuarisAsync();
            EsVisible = Visibility.Collapsed;
        });
    }
}
