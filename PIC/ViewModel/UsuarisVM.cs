using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using PIC.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly ProfessorsApiClient _professorsApiClient;

        // CONSTRUCTOR
        public UsuarisVM()
        {
            Usuaris = new ObservableCollection<Usuari>();
            
            _usuarisApiClient = new UsuarisApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirAlumne = new AfegirAlumneVM(this);
            AfegirProfessor = new AfegirProfessorVM(this);
            EditarUsuari= new EditarUsuariVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM(this);
        }

        // USUARIO SEL·LECCIONAT
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
            if (_usuariSeleccionat != null)
            {
                await EditarUsuari.Mostrar(UsuariSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un usuari.");
            }

        });

        public ICommand EsborrarMenu_Click => new RelayCommand(_ =>
        {
            if (_usuariSeleccionat != null)
            {
                ConfirmarEsborrar.Mostrar(UsuariSeleccionat);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar un usuari.");
            }
            
        });


        // TIPUS DE CERCA
        private UsuarisTipusCerca _tipusCercaActual;
        public UsuarisTipusCerca TipusCercaActual
        {
            get => _tipusCercaActual;
            set
            {
                _tipusCercaActual = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private int _parametreCerca;
        public int ParametreCerca
        {
            get => _parametreCerca;
            set
            {
                _parametreCerca = value;
                OnPropertyChanged();
            }
        }

        // MOSTRAR TOTS ELS USUARIS
        public async Task MostrarUsuarisAsync()
        {
            try
            {
                var llista = await _usuarisApiClient.GetAllUsuarisAsync();
                Usuaris.Clear();

                foreach (var u in llista)
                {
                    Usuaris.Add(u);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // EXECUTAR CERCA
        public async Task CercaUsuarisAsync()
        {
            Usuaris.Clear();

            switch (TipusCercaActual)
            {
                case UsuarisTipusCerca.PerId:
                    var usuari = await _usuarisApiClient.GetUsuariPerIdAsync(ParametreCerca);
                    Usuaris.Add(usuari);
                    break;

                case UsuarisTipusCerca.PerCurs:                    
                    var curs = await _usuarisApiClient.GetUsuarisPerIdCursAsync(ParametreCerca);
                    foreach (var u in curs)
                        Usuaris.Add(u);
                    break;

                case UsuarisTipusCerca.PerDepartament:
                    var dep = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(ParametreCerca);
                    foreach (var u in dep)
                        Usuaris.Add(u);
                    break;
            }
        }

        // ESBORRAR USUARI
        public async Task EsborrarUsuariAsync(int id)
        {
            try
            {
                int result = await _usuarisApiClient.DeleteUsuariAsync(id);
                if (result <= 0)
                {
                    //MessageBox.Show("No s'ha pogut actualitzar el professor.");
                    await MostrarUsuarisAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // ESBORRAR ALUMNE
        public async Task EsborrarAlumneAsync(int id)
        {
            try
            {
                int result = await _alumnesApiClient.DeleteAlumneAsync(id);
                if (result <= 0)
                {
                    //MessageBox.Show("No s'ha pogut actualitzar el professor.");
                    await MostrarUsuarisAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // ESBORRAR PROFESSOR
        public async Task EsborrarProfessorAsync(int id)
        {
            try
            {
                int result = await _professorsApiClient.DeleteProfessorAsync(id);
                if (result <= 0)
                {
                    //MessageBox.Show("No s'ha pogut actualitzar el professor.");
                    await MostrarUsuarisAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // BUIDAR LIST VIEW
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
