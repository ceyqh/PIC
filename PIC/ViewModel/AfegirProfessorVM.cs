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

        // CONSTRUCTOR
        public AfegirProfessorVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            _usuarisApiClient = new UsuarisApiClient();
            _professorsApiClient = new ProfessorsApiClient();
            _departamentsApiClient = new DepartamentsApiClient();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.FrameworkElement()))
            {
                CarregarDepartaments();
            }
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

            // OMPLIR EL COMBOBOX AMB ELS CURSOS
            if (departaments != null)
            {
                Departaments = departaments.ToList();
            }
            else
            {
                Departaments = new List<Departament>();
            }

            if (Departaments != null && Departaments.Count > 0)
            {
                DepartamentId = Departaments[0].Id;
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

        // AFEGIR PROFESSOR CLICK
        public ICommand AfegirProfessor_Click => new RelayCommand(async _ =>
        {
            await GuardarNouProfessor();
        });

        // GUARDAR NOU PROFESSOR (USUARI + PROFESSOR)
        private async Task GuardarNouProfessor()
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

            // Crear professor amb idUsuari + idDepartament
            var professor = new Professor
            {
                IdUsuari = usuariCreat.Id,
                IdDepartament = DepartamentId,
            };

            await CrearProfessorAsync(professor);

            // Tornar a mostrar la llista d'usuaris actualitzada amb el nou usuari
            await _usuarisVM.MostrarUsuarisAsync();

            // Tancar popup
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

        // CREAR PROFESSOR
        public async Task CrearProfessorAsync(Professor professorAAfegir)
        {
            Professor result = await _professorsApiClient.PostProfessorAsync(professorAAfegir);
        }
    }
}
