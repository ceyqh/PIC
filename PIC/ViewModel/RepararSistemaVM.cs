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
    internal class RepararSistemaVM : Utilities.ViewModelBase
    {
        private readonly PicVM _picVM;
        private readonly UsuarisBaseApiClient _usuarisBaseApiClient;
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly ProfessorsApiClient _professorsApiClient;
        private readonly PrestecsApiClient _prestecsApiClient;
        private readonly RegistresApiClient _registresApiClient;
        private readonly DispositiusApiClient _dispositiusApiClient;
        public MissatgeErrorVM MissatgeError { get; set; }
        public NotificacioVM Notificacio { get; set; }

        // Validador
        private bool esPotReparar = true;

        // CONSTRUCTOR
        public RepararSistemaVM(PicVM picVM)
        {
            _picVM = picVM;

            MissatgeError = new MissatgeErrorVM();
            Notificacio = new NotificacioVM();

            _usuarisBaseApiClient = new UsuarisBaseApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();
            _prestecsApiClient = new PrestecsApiClient();
            _registresApiClient = new RegistresApiClient();
            _dispositiusApiClient = new DispositiusApiClient();
        }

        // CODI DE SEGURETAT TEXTBOX
        private string _textProces = "// Camp de processos.";
        public string TextProces
        {
            get => _textProces;
            set
            {
                _textProces = value;
                OnPropertyChanged(); // Crucial perquè la UI s'actualitzi
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

        // OBRIR FINESTRA
        public void Mostrar()
        {
            Random rnd = new Random();
            esPotReparar= true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // REPARAR SISTEMA
        public ICommand RepararSistema_Click => new RelayCommand(async _ =>
        {
            if (!esPotReparar)
            {
                return;
            }

            esPotReparar = false;

            List<UsuariBase> usuarisAEsborrar = new List<UsuariBase>();
            List<Dispositiu> dispositiusAModificar = new List<Dispositiu>();

            TextProces = "// Consultant usuaris...";
            var usuaris = await _usuarisBaseApiClient.GetAllUsuarisAsync();

            if (usuaris == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els usuaris.");
                esPotReparar = true;
                return;
            }

            TextProces = "// Consultant alumnes...";
            var alumnes = await _alumnesApiClient.GetAllAlumnesAsync();

            if (alumnes == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els alumnes.");
                esPotReparar = true;
                return;
            }

            TextProces = "// Consultant professors...";
            var professors = await _professorsApiClient.GetAllProfessorsAsync();

            if (professors == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els professors.");
                esPotReparar = true;
                return;
            }

            foreach (var usuari in usuaris)
            {
                bool usuariTrobat = false;

                // Revisar els alumnes
                TextProces = $"// Buscant l'alumne {usuari.Nom}...";

                bool alumneTrobat = false;
                int i = 0;

                while (i < alumnes.Count && !alumneTrobat)
                {
                    if (usuari.Id == alumnes[i].IdUsuari)
                    {
                        alumneTrobat = true;
                        usuariTrobat = true;
                    }
                    else
                    {
                        i++;
                    }
                }

                // Revisar els professors
                TextProces = $"// Buscant el professor {usuari.Nom}...";

                bool professorTrobat = false;
                int j = 0;

                while (j < professors.Count && !professorTrobat)
                {
                    if (usuari.Id == professors[j].IdUsuari)
                    {
                        professorTrobat = true;
                        usuariTrobat = true;
                    }
                    else
                    {
                        j++;
                    }
                }

                if (!usuariTrobat)
                {
                    usuarisAEsborrar.Add(usuari);
                    var usuariEsborrat = _usuarisBaseApiClient.DeleteUsuariAsync((int)usuari.Id);

                    if (professors == null)
                    {
                        MissatgeError.Mostrar("Hi ha hagut un problema al esborrar l'usuari.");
                        esPotReparar = true;
                        return;
                    }
                }
            }

            var prestecs = await _prestecsApiClient.GetAllPrestecsAsync();

            // Si falla la consulta
            if (prestecs == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els préstecs.");
                esPotReparar = true;
                return;
            }

            var dispositius = await _dispositiusApiClient.GetAllDispositiusAsync();

            // Si falla la consulta
            if (dispositius == null)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al consultar els dispositius.");
                esPotReparar = true;
                return;
            }

            // Revisar els dispositius
            foreach (var dispositiu in dispositius)
            {
                TextProces = $"// Consultant  {dispositiu.Nom}...";

                bool dispositiuTrobat = false;
                int i = 0;

                while (i < prestecs.Count && !dispositiuTrobat)
                {
                    if (dispositiu.Id == prestecs[i].IdUsuari)
                    {
                        dispositiuTrobat = true;
                    }
                    else
                    {
                        i++;
                    }
                }

                // Si es troba el dispositiu
                if (!dispositiuTrobat)
                {
                    if (dispositiu.Estat.ToLower() == "en préstec")
                    {
                        Dispositiu nouDispositiu = new Dispositiu();
                        nouDispositiu.Id = dispositiu.Id;
                        nouDispositiu.Nom = dispositiu.Nom;
                        nouDispositiu.IdCategoria = dispositiu.IdCategoria;
                        nouDispositiu.Estat = "Disponible";

                        var dispositiuActualitzat = await _dispositiusApiClient.UpdateDispositiuAsync(nouDispositiu);

                        if (dispositiuActualitzat == -1)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar el dispositiu.");
                            esPotReparar = true;
                            return;
                        }

                        dispositiusAModificar.Add(dispositiu);
                    }
                }
                else
                {
                    if (dispositiu.Estat.ToLower() != "en préstec")
                    {
                        Dispositiu nouDispositiu = new Dispositiu();
                        nouDispositiu.Id = dispositiu.Id;
                        nouDispositiu.Nom = dispositiu.Nom;
                        nouDispositiu.IdCategoria = dispositiu.IdCategoria;
                        nouDispositiu.Estat = "En préstec";

                        var dispositiuActualitzat = await _dispositiusApiClient.UpdateDispositiuAsync(nouDispositiu);

                        if (dispositiuActualitzat == -1)
                        {
                            MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar el dispositiu.");
                            esPotReparar = true;
                            return;
                        }

                        dispositiusAModificar.Add(dispositiu);
                    }
                }
            }

            EsVisible = Visibility.Collapsed;

            _picVM.ObrirNotificacio($"Usuaris esborrats: {usuarisAEsborrar.Count}. Dispositius reparats: {dispositiusAModificar.Count}.");
        });
    }
}
