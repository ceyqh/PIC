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
        private readonly Administrador _administrador;
        public ObservableCollection<Administrador> Administradors { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirAdministradorVM AfegirAdministrador { get; set; }
        public EditarAdministradorVM EditarAdministrador { get; set; }
        public FinalitzarCursVM FinalitzarCurs { get; set; }
        public ExportarRegistresVM ExportarRegistres { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }
        public RepararSistemaVM RepararSistema { get; set; }
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

        // TEXT CERCA
        private string _textCerca;
        public string TextCerca
        {
            get => _textCerca;
            set
            {
                _textCerca = value;
                OnPropertyChanged();
            }
        }

        // CONSTRUCTOR
        public PicVM(Administrador administrador)
        {
            _administrador = administrador;

            Administradors = new ObservableCollection<Administrador>();

            _administradorsApiClient = new AdministradorsApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirAdministrador = new AfegirAdministradorVM(this);
            FinalitzarCurs = new FinalitzarCursVM(this);
            ExportarRegistres = new ExportarRegistresVM();
            EditarAdministrador = new EditarAdministradorVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();
            RepararSistema = new RepararSistemaVM(this);
            Notificacio = new NotificacioVM();

            _ = MostrarAdministradorsAsync();

            TextCerca = "// PIC / ADMINISTRADORS";
        }

        public async Task MostrarAdministradorsAsync()
        {
            // Si falla l'API
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            else
            {
                List<Administrador> llista = await _administradorsApiClient.GetAllAdministradorsAsync();

                // Si falla la consulta
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

                    TextCerca = $"// PIC / ADMINISTRADORS / RESULTATS: {Administradors.Count}";
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
                return;
            }

            // Si l'administrador és el mateix que está en ús ara mateix
            if (_administrador.Nom == AdministradorSeleccionat.Nom)
            {
                MissatgeError.Mostrar("No pots editar el teu propi usuari.");
                return;
            }

            EditarAdministrador.Mostrar(_administradorSeleccionat);
        });

        public ICommand EsborrarAdministradorMenu_Click => new RelayCommand(_ =>
        {
            // Si no hi ha cap administrador seleccionat
            if (_administradorSeleccionat == null)
            {
                MissatgeError.Mostrar("Cal seleccionar un administrador.");
                return;
            }

            // Si l'administrador és el mateix que está en ús ara mateix
            if (_administrador.Nom == AdministradorSeleccionat.Nom)
            {
                MissatgeError.Mostrar("No pots esborrar el teu propi usuari.");
                return;
            }

            // Si no hi ha cap administrador seleccionat
            if (_administradorSeleccionat == null)
            {
                MissatgeError.Mostrar("Cal seleccionar un administrador.");
                return;
            }

            ConfirmarEsborrar.Mostrar(_administradorSeleccionat, this);
        });

        public ICommand FinalitzarCursMenu_Click => new RelayCommand(_ =>
        {
            FinalitzarCurs.Mostrar();
        });

        public ICommand ExportarRegistresMenu_Click => new RelayCommand(_ =>
        {
            ExportarRegistres.Mostrar();
        });

        public ICommand RepararSistema_Click => new RelayCommand(_ =>
        {
            RepararSistema.Mostrar();
        });

        public void ObrirNotificacio(string missatge)
        {
            Notificacio.Mostrar(missatge);
        }
    }
}
