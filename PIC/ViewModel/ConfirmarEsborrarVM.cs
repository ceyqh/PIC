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
    public enum ElementAEsborrar
    {
        Usuari,
        Curs,
        Departament,
        Dispositiu,
        Categoria,
        Prestec
    }

    internal class ConfirmarEsborrarVM: Utilities.ViewModelBase
    {
        private readonly UsuarisApiClient _usuarisApiClient = new UsuarisApiClient();
        private readonly AlumnesApiClient _alumnesApiClient = new AlumnesApiClient();
        private readonly ProfessorsApiClient _professorsApiClient = new ProfessorsApiClient();
        private readonly CursosApiClient _cursosApiClient = new CursosApiClient();
        private readonly DepartamentsApiClient _departamentsApiClient = new DepartamentsApiClient();

        private UsuarisVM _usuarisVM;
        private CursosVM _cursosVM;
        private DepartamentsVM _departamentsVM;

        private Usuari usuariSeleccionat;
        private Curs cursSeleccionat;
        private Departament departamentSeleccionat;
        private Dispositiu dispositiuSeleccionat;
        private Categoria categoriaSeleccionat;
        private Prestec prestecSeleccionat;

        public ConfirmarEsborrarVM() 
        {

        }

        public void SetVM(UsuarisVM vm) => _usuarisVM = vm;
        public void SetVM(CursosVM vm) => _cursosVM = vm;
        public void SetVM(DepartamentsVM vm) => _departamentsVM = vm;

        // CONSTRUCTOR PER USUARIS
        public ConfirmarEsborrarVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            _usuarisApiClient = new UsuarisApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();          
        }

        private ElementAEsborrar _aEsborrar;
        public ElementAEsborrar AEsborrar
        {
            get => _aEsborrar;
            set
            {
                _aEsborrar = value;
                OnPropertyChanged();
            }
        }

        // MISSATGE QUE ES MOSTRA
        private string _missatge;
        public string Missatge
        {
            get => _missatge;
            set
            {
                _missatge = value;
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

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UN USUARI
        public void Mostrar(Usuari usuariAEsborrar, UsuarisVM parentVM)
        {
            this._usuarisVM = parentVM;
            this.usuariSeleccionat = usuariAEsborrar;

            Missatge = $"Estàs a punt d'esborrar l'usuari {usuariAEsborrar.Nom} {usuariAEsborrar.Cognom} amb ID({usuariAEsborrar.Id}). Vols confirmar aquesta acció?";

            AEsborrar = ElementAEsborrar.Usuari;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UN CURS
        public void Mostrar(Curs cursAEsborrar, CursosVM parentVM)
        {
            this._cursosVM = parentVM;
            this.cursSeleccionat = cursAEsborrar;

            Missatge = $"Estàs a punt d'esborrar el curs {cursAEsborrar.Nom} amb ID({cursAEsborrar.Id}). Vols confirmar aquesta acció?";

            AEsborrar = ElementAEsborrar.Curs;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UN DEPARTAMENT
        public void Mostrar(Departament departamentAEsborrar, DepartamentsVM parentVM)
        {
            this._departamentsVM = parentVM; // Ens assegurem que tenim la referència actual
            this.departamentSeleccionat = departamentAEsborrar;

            Missatge = $"Estàs a punt d'esborrar el departament {departamentAEsborrar.Nom} amb ID({departamentAEsborrar.Id}). Vols confirmar aquesta acció?";

            AEsborrar = ElementAEsborrar.Departament;
            EsVisible = Visibility.Visible;
        }

        // ESBORRAR L'ELEMENT DESITJAT
        public ICommand Esborrar_Click => new RelayCommand(async _ =>
        {
            switch (AEsborrar)
            {
                // Si és un USUARI
                case ElementAEsborrar.Usuari:
                    if (usuariSeleccionat.Tipus.ToLower() == "alumne")
                    {
                        await _alumnesApiClient.DeleteAlumneAsync((int)usuariSeleccionat.Id);
                    }

                    if (usuariSeleccionat.Tipus.ToLower() == "professor")
                    {
                        await _professorsApiClient.DeleteProfessorAsync((int)usuariSeleccionat.Id);
                    }

                    await _usuarisApiClient.DeleteUsuariAsync((int)usuariSeleccionat.Id);
                    await _usuarisVM.MostrarUsuarisAsync();
                    break;

                // Si és un CURS
                case ElementAEsborrar.Curs:
                    await _cursosApiClient.DeleteCursAsync((int)cursSeleccionat.Id);
                    await _cursosVM.MostrarCursosAsync();
                    break;

                // Si és un DEPARTAMENT
                case ElementAEsborrar.Departament:
                    await _departamentsApiClient.DeleteDepartamentAsync((int)departamentSeleccionat.Id);
                    await _departamentsVM.MostrarDepartamentsAsync();
                    break;

                // Si és un DISPOSITIU
                case ElementAEsborrar.Dispositiu:
                    break;

                // Si és una CATEGORIA
                case ElementAEsborrar.Categoria:
                    break;

                // Si és un PRÉSTEC
                case ElementAEsborrar.Prestec:
                    break;
            }

            EsVisible = Visibility.Collapsed;
        });
    }
}
