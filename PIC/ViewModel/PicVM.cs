using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    internal class PicVM: Utilities.ViewModelBase
    {
        public ObservableCollection<Administrador> Administradors { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirAdministradorVM AfegirAdministrador { get; set; }
        public EditarAdministradorVM EditarAdministrador { get; set; }
        public FinalitzarCursVM FinalitzarCurs { get; set; }
        public ExportarRegistresVM ExportarRegistres { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }
        public NotificacioVM Notificacio { get; set; }

        private readonly AdministradorsApiClient _administradorsApiClient;

        // ADMINISTRADOR SELECCIONAT
        private Administrador _administradorSeleccionat;
        public Administrador AdministradorSeleccionat
        {
            get => _administradorSeleccionat;
            set
            {
                _administradorSeleccionat = value;
                OnPropertyChanged();
            }
        }

        // CONSTRUCTOR
        public PicVM()
        {
            Administradors = new ObservableCollection<Administrador>();

            _administradorsApiClient = new AdministradorsApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirAdministrador = new AfegirAdministradorVM(this);
            FinalitzarCurs = new FinalitzarCursVM(this);
            ExportarRegistres = new ExportarRegistresVM();
            EditarAdministrador = new EditarAdministradorVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();
            Notificacio = new NotificacioVM();

            _ = MostrarAdministradorsAsync();
        }

        public async Task MostrarAdministradorsAsync()
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            else
            {
                List<Administrador> llista = await _administradorsApiClient.GetAllAdministradorsAsync();
                if (llista == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar els Administradors. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                }
                else
                {
                        
                    Administradors.Clear();

                    foreach (var u in llista)
                    {
                        Administradors.Add(u);
                    }
                }

            }
        }

        // TANCAR FINESTRA
        public ICommand AfegirAdministradorMenu_Click => new RelayCommand(_ =>
        {
            AfegirAdministrador.Mostrar();
        });

        public ICommand EditarAdministradorMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap administrador seleccionat
            if (_administradorSeleccionat == null)
            {
                MissatgeError.Mostrar("Cal seleccionar un administrador.");
            }
            else
            {
                EditarAdministrador.Mostrar(_administradorSeleccionat);
            }
        });

        public ICommand EsborrarAdministradorMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap administrador seleccionat
            if (_administradorSeleccionat == null)
            {
                MissatgeError.Mostrar("Cal seleccionar un administrador.");
            }
            else
            {
                ConfirmarEsborrar.Mostrar(_administradorSeleccionat, this);
            }
        });

        public ICommand FinalitzarCursMenu_Click => new RelayCommand(_ =>
        {
            FinalitzarCurs.Mostrar();
        });

        public ICommand ExportarRegistresMenu_Click => new RelayCommand(_ =>
        {
            ExportarRegistres.Mostrar();
        });

        public void ObrirNotificacio(string missatge)
        {
            Notificacio.Mostrar(missatge);
        }
    }
}
