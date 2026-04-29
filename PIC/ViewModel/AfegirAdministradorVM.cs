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
    internal class AfegirAdministradorVM: Utilities.ViewModelBase
    {
        private readonly AdministradorsApiClient _administradorsApiClient;

        private readonly PicVM _picVM;

        // Validador per assegurar que no es dupliquen accions
        private bool esPotAfegir = true;

        public MissatgeErrorVM MissatgeError { get; set; }

        public AfegirAdministradorVM(PicVM picVM)
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

        // CONTRASENYA
        private string _contrasenya;
        public string Contrasenya
        {
            get => _contrasenya;
            set
            {
                _contrasenya = value;
                OnPropertyChanged();
            }
        }

        // REPETIR CONTRASENYA
        private string _repetirContrasenya;
        public string RepetirContrasenya
        {
            get => _repetirContrasenya;
            set
            {
                _repetirContrasenya = value;
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
        public void Mostrar()
        {
            Nom = "";
            Contrasenya = "";
            RepetirContrasenya = "";
            PermisSeleccionat = "Permis1";
            esPotAfegir = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR ADMINISTRADOR
        public ICommand AfegirAdministrador_Click => new RelayCommand(async _ =>
        {
            var administradors = await _administradorsApiClient.GetAllAdministradorsAsync();

            // Si la consulta falla
            if (administradors == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els administradors.");
            }
            // Si la consulta funciona
            else
            {
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

                // Si existeix un administrador amb aquest nom
                if (existeixUsuari)
                {
                    MissatgeError.Mostrar("Ja existeix un administrador amb aquest nom, prova'n un de diferent.");
                }
                else
                {
                    if (Contrasenya != RepetirContrasenya)
                    {
                        MissatgeError.Mostrar("Les contrasenyes no coincideixen.");
                    }
                    else if (Contrasenya.Length < 4)
                    {
                        MissatgeError.Mostrar("La contrasenya és massa curta, ha de tenir un mínim de 4 caràcters.");
                    }
                    else
                    {
                        Administrador nouAdministrador = new Administrador();
                        nouAdministrador.Nom = Nom;
                        nouAdministrador.Contrasenya = Contrasenya;
                        nouAdministrador.Permisos = PermisSeleccionat;

                        var administradorCreat = await _administradorsApiClient.PostAdministradorAsync(nouAdministrador);

                        if (administradorCreat == null)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al crear l'administradors.");
                        }
                        else
                        {
                            esPotAfegir = false;
                            await _picVM.MostrarAdministradorsAsync();
                            EsVisible = Visibility.Collapsed;
                        }
                    }
                }
            }                
        });
    }
}
