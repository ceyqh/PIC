using Microsoft.Win32;
using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Globalization;

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
        private readonly DispositiusApiClient _dispositiusApiClient;
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
            _dispositiusApiClient = new DispositiusApiClient();
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

        // RUTA DE FITXER

        private string _rutaFitxer = "";
        public string RutaFitxer
        {
            get => _rutaFitxer;
            set { _rutaFitxer = value; OnPropertyChanged(); }
        }

        private string _botoRutaFitxer = "[...]";
        public string BotoRutaFitxer
        {
            get => _botoRutaFitxer;
            set { _botoRutaFitxer = value; OnPropertyChanged(); }
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

        // SELECCIONAR FITXER
        public ICommand SeleccionarFitxer_Click => new RelayCommand(o =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fitxers Excel (*.xlsx)|*.xlsx|Fitxers CSV (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                RutaFitxer = openFileDialog.FileName;

                // Nom de l'arxiu per al botó
                string[] rutaSeparada = RutaFitxer.Split('\\');
                BotoRutaFitxer = rutaSeparada[rutaSeparada.Count() - 1];
            }
        });

        // FINALITZAR CURS
        public ICommand FinalitzarCurs_Click => new RelayCommand(async _ =>
        {
            // Si es pot finalitzar
            //if (!esPotFinalitzar) 
            //{ 
            //    return;
            //}

            //esPotFinalitzar = false;

            //using (var reader = new StreamReader(RutaFitxer))
            //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //{
            //    // Si tens una classe que coincideix amb les columnes
            //    var records = csv.GetRecords<Usuari>().ToList();

            //    foreach (var usuari in records)
            //    {
            //        Console.WriteLine($"{usuari.Nom} - {usuari.Email}");
            //    }
            //}

            return;
            // Si el codi de seguretat NO coincideix
            if (CodiSeguretat.ToString() != CodiSeguretatTextBox)
            {
                MissatgeError.Mostrar("El codi de seguretat no coincideix.");
                esPotFinalitzar = true;
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
                esPotFinalitzar = true;
                return;
            }

            var usuaris = await _usuarisApiClient.GetAllUsuarisAsync();

            // Si la consulta falla
            if (usuaris == null)
            {
                MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els usuaris. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                esPotFinalitzar = true;
                return;
            }

            foreach (var prestec in prestecs)
            {
                foreach (var usuari  in usuaris)
                {
                    // Si el préstec és d'un alumne
                    if (usuari.Tipus.ToLower() == "Alumne")
                    {
                        TextProces = $"// Esborrant préstec [{prestec.Id}]...";
                        var prestecEsborrat = await _prestecsApiClient.DeletePrestecAsync(prestec.Id);

                        Dispositiu dispositiuConsultaFinalitzar = await _dispositiusApiClient.GetDispositiuPerIdAsync(prestec.IdDispositiu);

                        // Si la consulta falla
                        if (dispositiuConsultaFinalitzar == null)
                        {
                            MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els dispositius. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                            esPotFinalitzar = true;
                            return;
                        }

                        Dispositiu dispositiuPrestatFinalitzar = new Dispositiu();
                        dispositiuPrestatFinalitzar.Id = prestec.IdDispositiu;
                        dispositiuPrestatFinalitzar.Nom = dispositiuConsultaFinalitzar.Nom;
                        dispositiuPrestatFinalitzar.IdCategoria = dispositiuConsultaFinalitzar.IdCategoria;
                        dispositiuPrestatFinalitzar.Estat = "Disponible";

                        int confirmarDispositiuPrestatFinalitzar = await _dispositiusApiClient.UpdateDispositiuAsync(dispositiuPrestatFinalitzar);

                        // Si la consulta falla
                        if (confirmarDispositiuPrestatFinalitzar == -1)
                        {
                            MissatgeError.Mostrar($"Hi ha hagut un problema al actualitzar el dispositius. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                            esPotFinalitzar = true;
                            return;
                        }

                        Usuari usuariConsultaFinalitzar = await _usuarisApiClient.GetUsuariPerIdAsync(prestec.IdUsuari);

                        // Si la consulta falla
                        if (usuariConsultaFinalitzar == null)
                        {
                            MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els usuaris. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                            esPotFinalitzar = true;
                            return;
                        }

                        // Crear registre
                        Registre registreFinalitzat = new Registre();
                        registreFinalitzat.IdPrestec = prestec.Id;
                        registreFinalitzat.Accio = "No finalitzat";
                        registreFinalitzat.NomUsuari = $"{usuariConsultaFinalitzar.Nom} {usuariConsultaFinalitzar.Cognom}";
                        registreFinalitzat.IdUsuari = (int)usuariConsultaFinalitzar.Id;
                        registreFinalitzat.NomDispositiu = dispositiuConsultaFinalitzar.Nom;
                        registreFinalitzat.IdDispositiu = (int)dispositiuConsultaFinalitzar.Id;
                        registreFinalitzat.NomGrup = usuariConsultaFinalitzar.Grup;
                        registreFinalitzat.IdGrup = (int)usuariConsultaFinalitzar.IdGrup;
                        registreFinalitzat.DataAccio = DateTime.Now;
                        registreFinalitzat.DataRetorn = prestec.DataRetorn;

                        Registre registreCreatFinalitzar = await _registresApiClient.PostRegistreAsync(registreFinalitzat);

                        if (registreCreatFinalitzar == null)
                        {
                            MissatgeError.Mostrar($"Hi ha hagut un problema al crear el registre. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                            esPotFinalitzar = true;
                            return;
                        }

                        if (prestecEsborrat == -1)
                        {
                            MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar el préstec amb ID[{prestec.Id}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                            esPotFinalitzar = true;
                            return;
                        }
                    }
                }                
            }

            // Esborrar alumnes
            TextProces = "// Consultant alumnes...";
            var alumnes = await _alumnesApiClient.GetAllAlumnesAsync();

            // Si la consulta falla
            if (alumnes == null)
            {
                MissatgeError.Mostrar($"Hi ha hagut un problema al consultar els alumnes. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                esPotFinalitzar = true;
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
                    esPotFinalitzar = true;
                    return;
                }
                if (usuariEsborrat == -1)
                {
                    MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar l'usuari amb ID[{alumne.IdUsuari}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                    esPotFinalitzar = true;
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
                esPotFinalitzar = true;
                return;
            }

            foreach (var curs in cursos)
            {
                TextProces = $"// Esborrant el curs {curs.Nom} [{curs.Id}]...";
                var alumneEsborrat = await _cursosApiClient.DeleteCursAsync((int)curs.Id);

                if (alumneEsborrat == -1)
                {
                    MissatgeError.Mostrar($"Hi ha hagut un problema al esborrar el curs amb ID[{curs.Id}]. Revisa la comunicació entre la API i l'aplicació i torna a executar aquesta funció.");
                    esPotFinalitzar = true;
                    return;
                }
            }

            EsVisible = Visibility.Collapsed;
            _picVM.ObrirNotificacio("S'ha finalitzat el curs amb éxit.");
        });
    }
}
