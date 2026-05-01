using ExcelDataReader;
using Microsoft.Win32;
using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace PIC.ViewModel
{
    internal class ExportarRegistresVM : Utilities.ViewModelBase
    {
        private readonly RegistresApiClient _registresApiClient;
        public MissatgeErrorVM MissatgeError { get; set; }
        public NotificacioVM Notificacio { get; set; }

        // Validador
        private bool esPotExportar = true;

        // CONSTRUCTOR
        public ExportarRegistresVM()
        {
            MissatgeError = new MissatgeErrorVM();
            Notificacio = new NotificacioVM();
            _registresApiClient = new RegistresApiClient();;
        }

        // RUTA DE CARPETA
        private string _rutaCarpeta = "";
        public string RutaCarpeta
        {
            get => _rutaCarpeta;
            set { _rutaCarpeta = value; OnPropertyChanged(); }
        }

        // RUTA DE CARPETA BOTO
        private string _botoRutaCarpeta = "[...]";
        public string BotoRutaCarpeta
        {
            get => _botoRutaCarpeta;
            set { _botoRutaCarpeta = value; OnPropertyChanged(); }
        }

        // RUTA DE CARPETA BOTO
        private string _nomArxiu;
        public string NomArxiu
        {
            get => _nomArxiu;
            set { _nomArxiu = value; OnPropertyChanged(); }
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
        public void Mostrar()
        {
            Random rnd = new Random();
            esPotExportar = true;
            EsVisible = Visibility.Visible;
            BotoRutaCarpeta = "[...]";
            RutaCarpeta = "";
            NomArxiu = "";
            esPotExportar = true;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // SELECCIONAR FITXER
        public ICommand SeleccionarCarpeta_Click => new RelayCommand(o =>
        {
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.Description = "Selecciona una carpeta per a l'exportació.";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RutaCarpeta = folderDialog.SelectedPath;
                    BotoRutaCarpeta = RutaCarpeta;
                }
            }
        });
        public ICommand ExportarRegistres_Click => new RelayCommand(async o =>
        {
            if (!esPotExportar)
            {
                return;
            }

            esPotExportar = false;

            if (string.IsNullOrEmpty(NomArxiu) || string.IsNullOrEmpty(RutaCarpeta))
            {
                MissatgeError.Mostrar("Els camps no poden quedar buits.");
                esPotExportar = true;
                return;
            }

            DirectoryInfo carpetaActual = new DirectoryInfo(RutaCarpeta);
            FileInfo[] fitxersCarpeta = carpetaActual.GetFiles();

            foreach (FileInfo fitxer in fitxersCarpeta)
            {
                if (fitxer.Name == (NomArxiu + ".csv"))
                {
                    MissatgeError.Mostrar("Ja existeix un arxiu amb aquest nom, per seguretat, escull un nom diferent.");
                    esPotExportar = true;
                    return;
                }
            }

            string rutaFitxer = $"{RutaCarpeta}\\{NomArxiu}.csv";

            // Dades de mostra
            var linies = new List<string>
            {
                "ID_REGISTRE,ID_PRESTEC,ACCIO,NOM_USUARI,ID_USUARI,NOM_DISPOSITIU,ID_DISPOSITIU,NOM_GRUOP,ID_GRUP,DATA_ACCIO,DATA_RETORN",
            };

            var registres = await _registresApiClient.GetAllRegistresAsync();

            // Si falla la consulta
            if (registres == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els registres.");
                esPotExportar = true;
                return;
            }

            foreach (var registre in registres)
            {
                List<string> linia = new List<string>();
                linia.Add(registre.Id.ToString());
                linia.Add(registre.IdPrestec.ToString());
                linia.Add(registre.Accio);
                linia.Add(registre.NomUsuari);
                linia.Add(registre.IdUsuari.ToString());
                linia.Add(registre.NomDispositiu);
                linia.Add(registre.IdDispositiu.ToString());
                linia.Add(registre.NomGrup);
                linia.Add(registre.IdGrup.ToString());
                linia.Add(registre.DataAccio.ToString());
                linia.Add(registre.DataRetorn.ToString());

                string liniaComplerta = string.Join(", ", linia);
                linies.Add(liniaComplerta);
            }

            // Crear fitxer
            File.WriteAllLines(rutaFitxer, linies, Encoding.UTF8);

            EsVisible = Visibility.Collapsed;
        });
    }
}
