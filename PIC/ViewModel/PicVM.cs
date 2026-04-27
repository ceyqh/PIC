using PIC.APIClient;
using PIC.Model;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIC.ViewModel
{
    internal class PicVM: Utilities.ViewModelBase
    {
        public ObservableCollection<Administrador> Administradors { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly AdministradorsApiClient _administradorsApiClient;

        // CONSTRUCTOR
        public PicVM()
        {
            Administradors = new ObservableCollection<Administrador>();

            _administradorsApiClient = new AdministradorsApiClient();

            MissatgeError = new MissatgeErrorVM();
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarAdministradorsAsync();
        }

        public async Task MostrarAdministradorsAsync()
        {
            try
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

            catch (Exception ex)
            {
                MissatgeError.Mostrar("No es pot connectar amb el servidor: " + ex.Message);
            }
        }
    }
}
