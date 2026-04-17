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
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly ProfessorsApiClient _professorsApiClient;
        private readonly CursosApiClient _cursosApiClient;
        private readonly DepartamentsApiClient _departamentsApiClient;
        
        private readonly UsuarisVM _usuarisVM;
        private readonly CursosVM _cursosVM;

        private Usuari usuariSeleccionat;
        private Curs cursSeleccionat;
        private Departament departamentSeleccionat;
        private Dispositiu dispositiuSeleccionat;
        private Categoria categoriaSeleccionat;
        private Prestec prestecSeleccionat;

        // CONSTRUCTOR PER USUARIS
        public ConfirmarEsborrarVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            _usuarisApiClient = new UsuarisApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();          
        }

        // CONSTRUCTOR PER CURSOS
        public ConfirmarEsborrarVM(CursosVM cursosVM)
        {
            _cursosVM = cursosVM;

            _cursosApiClient = new CursosApiClient();
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
        public void Mostrar(Usuari usuariAEsborrar)
        {
            usuariSeleccionat = usuariAEsborrar;
            Missatge = $"Estàs a punt d'esborrar l'usuari { usuariAEsborrar.Nom} { usuariAEsborrar.Cognom} amb l'ID {usuariAEsborrar.Id}. Vols confirmar aquesta acció?";

            AEsborrar = ElementAEsborrar.Usuari;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UN CURS
        public void Mostrar(Curs cursAEsborrar)
        {
            cursSeleccionat = cursAEsborrar;

            Missatge = $"Estàs a punt d'esborrar el curs {cursAEsborrar.Nom} amb l'ID {cursAEsborrar.Id}. Vols confirmar aquesta acció?"; ;

            AEsborrar = ElementAEsborrar.Curs;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UN DEPARTAMENT
        public void Mostrar(Departament departamentAEsborrar)
        {
            departamentSeleccionat = departamentAEsborrar;

            Missatge = "";

            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UN DISPOSITIU
        public void Mostrar(Dispositiu dispositiuAEsborrar)
        {
            dispositiuSeleccionat = dispositiuAEsborrar;

            Missatge = "";

            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UNA CATEGORIA
        public void Mostrar(Categoria categoriaAEsborrar)
        {
            categoriaSeleccionat = categoriaAEsborrar;

            Missatge = "";

            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: QUAN ES VOL ESBORRAR UN PRESTEC
        public void Mostrar(Prestec prestecAEsborrar)
        {
            prestecSeleccionat = prestecAEsborrar;

            Missatge = "";

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
