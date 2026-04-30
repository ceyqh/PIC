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
    internal class FinalitzarCursVM : Utilities.ViewModelBase
    {
        private readonly PicVM _picVM;
        private readonly PrestecsApiClient _prestecsApiClient;
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly CursosApiClient _cursosApiClient;
        private readonly RegistresApiClient _registresApiClient;
        public MissatgeErrorVM MissatgeError { get; set; }
        public NotificacioVM Notificacio { get; set; }

        // Validador
        private bool esPotFinalitzar = true;

        // CONSTRUCTOR
        public FinalitzarCursVM(PicVM picVM)
        {
            _picVM = picVM;
            MissatgeError = new MissatgeErrorVM();
            Notificacio = new NotificacioVM();
            _prestecsApiClient = new PrestecsApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _usuarisApiClient = new UsuarisApiClient();
            _cursosApiClient = new CursosApiClient();
            _registresApiClient = new RegistresApiClient();
        }

        // CODI DE SEGURETAT
        private int _codiSeguretat;
        public int CodiSeguretat
        {
            get => _codiSeguretat;
            set
            {
                _codiSeguretat = value;
                OnPropertyChanged(); // Crucial perquè la UI s'actualitzi
            }
        }

        // CODI DE SEGURETAT TEXTBOX
        private string _codiSeguretatTextBox;
        public string CodiSeguretatTextBox
        {
            get => _codiSeguretatTextBox;
            set
            {
                _codiSeguretatTextBox = value;
                OnPropertyChanged(); // Crucial perquè la UI s'actualitzi
            }
        }

        // CODI DE SEGURETAT TEXTBOX
        private string _textProces= "// Camp de processos.";
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
            CodiSeguretat = rnd.Next(1000001, 10000001);
            CodiSeguretatTextBox = "";
            esPotFinalitzar = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // FINALITZAR CURS
        public ICommand FinalitzarCurs_Click => new RelayCommand(async _ =>
        {
            // Si es pot finalitzar
            if (!esPotFinalitzar) 
            { 
                return;
            }
            
            // Si el codi de seguretat NO coincideix
            if (CodiSeguretat.ToString() != CodiSeguretatTextBox)
            {
                MissatgeError.Mostrar("El codi de seguretat no coincideix.");
                return;
            }
                           
            esPotFinalitzar = false;

            // Esborrar prèstecs
            TextProces = "// Consultant préstecs...";
            var prestecs = await _prestecsApiClient.GetAllPrestecsAsync();

            // Si la consulta falla
            if (prestecs == null)
            {
                MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els préstecs. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                return;
            }

            foreach (var prestec in prestecs)
            {
                TextProces = $"// Esborrant préstec [{prestec.Id}]...";
                var prestecEsborrat = await _prestecsApiClient.DeletePrestecAsync(prestec.Id);

                if (prestecEsborrat == -1)
                {
                    MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar el préstec amb ID[{prestec.Id}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                    return;
                }
            }

            // Esborrar alumnes
            TextProces = "// Consultant alumnes...";
            var alumnes = await _alumnesApiClient.GetAllAlumnesAsync();

            // Si la consulta falla
            if (alumnes == null)
            {
                MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els alumnes. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                return;
            }

            foreach (var alumne in alumnes)
            {
                TextProces = $"// Esborrant l'alumne [{alumne.IdUsuari}]...";
                var alumneEsborrat = await _alumnesApiClient.DeleteAlumneAsync((int)alumne.IdUsuari);
                                
                TextProces = $"// Esborrant l'usuari [{alumne.IdUsuari}]...";
                var usuariEsborrat = await _usuarisApiClient.DeleteUsuariAsync((int)alumne.IdUsuari);

                if (alumneEsborrat == -1)
                {
                    MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar l'alumne amb ID[{alumne.IdUsuari}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                    return;
                }
                if (usuariEsborrat == -1)
                {
                    MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar l'usuari amb ID[{alumne.IdUsuari}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                    return;
                }
            }

            // Esborrar cursos
            TextProces = "// Consultant cursos...";
            var cursos = await _cursosApiClient.GetAllCursosAsync();

            // Si la consulta falla
            if (cursos == null)
            {
                MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els cursos. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
            }

            foreach (var curs in cursos)
            {
                TextProces = $"// Esborrant el curs {curs.Nom} [{curs.Id}]...";
                var alumneEsborrat = await _cursosApiClient.DeleteCursAsync((int)curs.Id);

                if (alumneEsborrat == -1)
                {
                    MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar el curs amb ID[{curs.Id}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                    return;
                }
            }

            // Esborrar registres
            TextProces = "// Consultant registres...";
            var registres = await _registresApiClient.GetAllRegistresAsync();

            // Si la consulta falla
            if (registres == null)
            {
                MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els registres. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                return;
            }

            foreach (var registre in registres)
            {
                TextProces = $"// Esborrant el registre amb ID[{registre.Id}]...";
                var alumneEsborrat = await _registresApiClient.DeleteRegistreAsync((int)registre.Id);

                if (alumneEsborrat == -1)
                {
                    MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar el registre [{registre.Id}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                    return;
                }
            }

            EsVisible = Visibility.Collapsed;
            _picVM.ObrirNotificacio("S'ha finalitzat el curs amb éxit.");
        });
    }
}
