using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    internal class EditarAdministradorVM : Utilities.ViewModelBase
    {
        private readonly AdministradorsApiClient _administradorsApiClient;

        private readonly PicVM _picVM;
        private Administrador _administradorEnEdicio;
        private string anticNom;

        // Validador per assegurar que no es dupliquen accions
        private bool esPotEditar = true;

        public MissatgeErrorVM MissatgeError { get; set; }

        public EditarAdministradorVM(PicVM picVM)
        {
            _picVM = picVM;

            MissatgeError = new MissatgeErrorVM();
            _administradorsApiClient = new AdministradorsApiClient();
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

        // PERMISOS
        private string _permisSeleccionat;
        public string PermisSeleccionat
        {
            get => _permisSeleccionat;
            set
            {
                _permisSeleccionat = value;
                OnPropertyChanged();
            }
        }

        // OBRIR FINESTRA
        public void Mostrar(Administrador administrador)
        {
            _administradorEnEdicio = administrador;
            anticNom = administrador.Nom;

            Nom = administrador.Nom;
            PermisSeleccionat = administrador.Permisos;
            esPotEditar = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR ADMINISTRADOR
        public ICommand GuardarAdministrador_Click => new RelayCommand(async _ =>
        {
            if (!esPotEditar)
            {
                return;
            }

            esPotEditar = false;

            var administradors = await _administradorsApiClient.GetAllAdministradorsAsync();

            // Si la consulta falla
            if (administradors == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els administradors.");
                esPotEditar = true;
                return;
            }
            
            // Comprovar si existeix un usuari amb aquest nom    
            bool existeixUsuari = false;
            int i = 0;

            while (i < administradors.Count && !existeixUsuari)
            {
                if (administradors[i].Nom == Nom)
                {
                    existeixUsuari = true;
                }
                else { i++; }
            }

            // Si el nom trobat és el mateix que ja té
            if (anticNom == Nom)
            {
                existeixUsuari = false;
            }
            // Si existeix un administrador amb aquest nom
            if (existeixUsuari)
            {
                MissatgeError.Mostrar("Ja existeix un administrador amb aquest nom, prova'n un de diferent.");
                esPotEditar = true;
                return;
            }

            // Crear nou administrador
            Administrador nouAdministrador = new Administrador();
            nouAdministrador.Id = _administradorEnEdicio.Id;
            nouAdministrador.Nom = Nom;
            nouAdministrador.Permisos = PermisSeleccionat;

            int administradorActualitzat = await _administradorsApiClient.UpdateAdministradorAsync(nouAdministrador);

            // Si no s'actualitza l'administrador
            if (administradorActualitzat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al crear l'administradors.");
                esPotEditar = true;
                return;
            }

            await _picVM.MostrarAdministradorsAsync();
            EsVisible = Visibility.Collapsed;
        });
    }
}
