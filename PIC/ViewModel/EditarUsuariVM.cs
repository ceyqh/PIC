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
    internal class EditarUsuariVM : Utilities.ViewModelBase
    {
        private readonly UsuarisVM _usuarisVM;
        private readonly CursosApiClient _cursosApiClient;
        private readonly DepartamentsApiClient _departamentsApiClient;
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly ProfessorsApiClient _professorsApiClient;
        private readonly UsuarisApiClient _usuarisApiClient;

        private Usuari _usuariEnEdicio;
        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotEditar = true;

        public EditarUsuariVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            MissatgeError = new MissatgeErrorVM();
            _cursosApiClient = new CursosApiClient();
            _departamentsApiClient = new DepartamentsApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();
            _usuarisApiClient = new UsuarisApiClient();
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

        // ITEMS SELECCIONABLES
        private List<object> _itemsSeleccionables;
        public List<object> ItemsSeleccionables
        {
            get => _itemsSeleccionables;
            set { _itemsSeleccionables = value; OnPropertyChanged(); }
        }

        // ID SELECCIONAT (CursId / DepartamentId)
        private long _idSeleccionat;
        public long IdSeleccionat
        {
            get => _idSeleccionat;
            set { _idSeleccionat = value; OnPropertyChanged(); }
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

        // OBRIR FINESTRA
        public async Task Mostrar(Usuari usuari)
        {
            _usuariEnEdicio = usuari;

            // Omplim els camps del formulari amb les dades de l'usuari seleccionat
            Nom = usuari.Nom;
            Cognom = usuari.Cognom;

            if (usuari.Tipus == "Alumne")
            {
                var cursos = await _cursosApiClient.GetAllCursosAsync();
                ItemsSeleccionables = cursos.Cast<object>().ToList();
            }
            else if (usuari.Tipus == "Professor")
            {
                var depts = await _departamentsApiClient.GetAllDepartamentsAsync();
                ItemsSeleccionables = depts.Cast<object>().ToList();
            }

            IdSeleccionat = usuari.IdGrup;

            esPotEditar = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // GUARDAR CANVIS USUARI
        public ICommand GuardarUsuari_Click => new RelayCommand(async _ =>
        {
            if (esPotEditar)
            {
                // Si hi ha camps buits
                if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Cognom))
                {
                    MissatgeError.Mostrar("No hi poden haver camps buits.");
                }
                else
                {
                    _usuariEnEdicio.Nom = Nom;
                    _usuariEnEdicio.Cognom = Cognom;
                    _usuariEnEdicio.IdGrup = IdSeleccionat;

                    // Si és un alumne
                    if (_usuariEnEdicio.Tipus.ToLower() == "alumne")
                    {
                        Alumne alumne = new Alumne();
                        alumne.IdUsuari = _usuariEnEdicio.Id;
                        alumne.IdCurs = _usuariEnEdicio.IdGrup;

                        int alumneActualitzat = await _alumnesApiClient.UpdateAlumneAsync(alumne);

                        // Si actualitzar l'alumne falla
                        if (alumneActualitzat == -1)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar l'alumne.");
                        }
                    }
                    // Si és un professor
                    else if (_usuariEnEdicio.Tipus.ToLower() == "professor")
                    {
                        Professor professor = new Professor();
                        professor.IdUsuari = _usuariEnEdicio.Id;
                        professor.IdDepartament = _usuariEnEdicio.IdGrup;

                        int professorActualitzat = await _professorsApiClient.UpdateProfessorAsync(professor);

                        // Si actualitzar el professor falla
                        if (professorActualitzat == -1)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar el professor.");
                        }
                    }

                    int usuariActualitzat = await _usuarisApiClient.UpdateUsuariAsync(_usuariEnEdicio);

                    // Si actualitzar l'usuari falla
                    if (usuariActualitzat == -1)
                    {
                        MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar l'usuari.");
                    }
                    // Si actualitzar l'usuari funciona
                    else
                    {
                        esPotEditar = false;
                        await _usuarisVM.MostrarUsuarisAsync();
                        EsVisible = Visibility.Collapsed;
                    }                        
                }
            }
        });
    }
}
