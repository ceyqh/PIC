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
    internal class AfegirPrestecVM: Utilities.ViewModelBase
    {
        private readonly PrestecsApiClient _prestecsApiClient;
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly DispositiusApiClient _dispositiusApiClient;
        private readonly RegistresApiClient _registresApiClient;

        private readonly PrestecsVM _prestecsVM;

        // Validador per assegurar que no es dupliquen accions
        private bool esPotAfegir = true;

        public MissatgeErrorVM MissatgeError { get; set; }

        // CONSTRUCTOR
        public AfegirPrestecVM(PrestecsVM prestecsVM)
        {
            _prestecsVM = prestecsVM;

            MissatgeError = new MissatgeErrorVM();
            _prestecsApiClient = new PrestecsApiClient();
            _usuarisApiClient = new UsuarisApiClient();
            _dispositiusApiClient = new DispositiusApiClient();
            _registresApiClient = new RegistresApiClient();
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

        // ID USUARI
        private string _usuariID;
        public string UsuariID
        { 
            get => _usuariID;
            set
            {
                _usuariID = value;
                OnPropertyChanged();
            }
        }

        // ID DISPOSITIU
        private string _dispositiuID;
        public string DispositiuID
        {
            get => _dispositiuID;
            set
            {
                _dispositiuID = value;
                OnPropertyChanged();
            }
        }

        // DATA ENTREGA
        private DateTime _dataEntrega;
        public DateTime DataEntrega
        {
            get => _dataEntrega;
            set
            {
                _dataEntrega = value;
                OnPropertyChanged();
            }
        }

        // DATA RETORN
        private DateTime _dataRetorn;
        public DateTime DataRetorn
        {
            get => _dataRetorn;
            set
            {
                _dataRetorn = value;
                OnPropertyChanged();
            }
        }

        // HORA
        private int _horaSeleccionada;
        public int HoraSeleccionada
        {
            get => _horaSeleccionada;
            set
            {
                _horaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        // FINAL DE CURS
        private bool _finalCurs;
        public bool FinalCurs
        {
            get => _finalCurs;
            set
            {
                _finalCurs = value;
                OnPropertyChanged();
            }
        }

        // OBRIR FINESTRA
        public void Mostrar()
        {
            UsuariID = "";
            DispositiuID = "";
            DataEntrega = DateTime.Now;
            HoraSeleccionada = 21;
            DataRetorn = new DateTime(DataEntrega.Year, DataEntrega.Month, DataEntrega.Day, HoraSeleccionada, 0, 0);
            FinalCurs = false;

            esPotAfegir = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR PRÉSTEC
        public ICommand AfegirPrestec_Click => new RelayCommand(async _ =>
        {
            if (!esPotAfegir)
            {
                return;
            }

            esPotAfegir = false;

            // Si els camps d'ID estan buits
            if (string.IsNullOrWhiteSpace(UsuariID) || string.IsNullOrWhiteSpace(DispositiuID))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
                esPotAfegir = true;
                return;
            }
            
            // Comprovar que l'ID d'Usuari existeix
            var usuaris = await _usuarisApiClient.GetAllUsuarisAsync();

            // Si falla la consulta
            if (usuaris == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al consultar l'usuari.");
                esPotAfegir = true;
                return;
            }
                
            // Comprovar que l'usuari existeix                        
            Usuari usuariPrestec = new Usuari();

            bool existeixUsuari = false;
            int i = 0;

            while (i < usuaris.Count && !existeixUsuari)
            {
                if (usuaris[i].Id == int.Parse(UsuariID))
                {
                    existeixUsuari = true;
                    usuariPrestec = usuaris[i];
                }
                else { i++; }
            }

            // Si no existeix l'ID d'Usuari
            if (!existeixUsuari)
            {
                MissatgeError.Mostrar($"No existeix cap usuari amb ID({UsuariID}).");
                esPotAfegir = true;
                return;
            }
                    
            // Comprovar que l'ID de Dispositiu existeix
            var dispositius = await _dispositiusApiClient.GetAllDispositiusAsync();

            bool existeixDispositiu = false;
            int j = 0;

            while (j < dispositius.Count && !existeixDispositiu)
            {
                if (dispositius[j].Id == int.Parse(DispositiuID))
                {
                    existeixDispositiu = true;
                }
                else { j++; }
            }

            // Si no existeix l'ID de Dispositiu
            if (!existeixDispositiu)
            {
                MissatgeError.Mostrar($"No existeix cap dispositiu amb ID({DispositiuID}).");
                esPotAfegir = true;
                return;
            }
                        
            // Assignar data de retorn
            if (!FinalCurs)
            {
                DataRetorn = new DateTime(DataRetorn.Year, DataRetorn.Month, DataRetorn.Day, HoraSeleccionada, 0, 0);
            }
            // Si es marca que es retorna a final de curs
            else
            {
                DataRetorn = new DateTime(DataEntrega.Year, 6, 30, 21, 0, 0);
            }

            // Si la data de retorn és anterior a la d'entrega
            if (DataEntrega > DataRetorn)
            {
                MissatgeError.Mostrar("La data de retorn no pot ser anterior a la d'entrega.");
                esPotAfegir = true;
                return;
            }
                            
            Usuari usuariConsulta = await _usuarisApiClient.GetUsuariPerIdAsync(int.Parse(UsuariID));
            Dispositiu dispositiuConsulta = await _dispositiusApiClient.GetDispositiuPerIdAsync(int.Parse(DispositiuID));

            // Si falla la consulta a Usuaris 
            if (usuariConsulta == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al consultar l'usuari.");
                esPotAfegir = true;
                return;
            }
            
            // Si falla la consulta a Dispositius
            if (dispositiuConsulta == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al consultar el dispositiu.");
                esPotAfegir = true;
                return;
            }
            
            // Si el dispositiu està deshabilitat
            if (dispositiuConsulta.Estat.ToLower() == "no disponible")
            {
                MissatgeError.Mostrar("Aquest dispositiu no es troba disponible.");
                esPotAfegir = true;
                return;
            }
            
            // Si el dispositiu està en préstec
            if (dispositiuConsulta.Estat.ToLower() == "en prestec")
            {
                MissatgeError.Mostrar("Aquest dispositiu es troba en mig d'un préstec.");
                esPotAfegir = true;
                return;
            }

            // Nou réstec
            Prestec nouPrestec = new Prestec();
            nouPrestec.NomUsuari = usuariConsulta.Nom;
            nouPrestec.IdUsuari = int.Parse(UsuariID);
            nouPrestec.NomDispositiu = dispositiuConsulta.Nom;
            nouPrestec.IdDispositiu = int.Parse(DispositiuID);
            nouPrestec.DataEntrega = DataEntrega;
            nouPrestec.DataRetorn = DataRetorn;

            Prestec prestecCreat = await _prestecsApiClient.PostPrestecAsync(nouPrestec);

            // Dispositiu amb l'estat actualitzat
            Dispositiu dispositiuPrestat = new Dispositiu();
            dispositiuPrestat.Id = int.Parse(DispositiuID);
            dispositiuPrestat.Nom = dispositiuConsulta.Nom;
            dispositiuPrestat.IdCategoria = dispositiuConsulta.IdCategoria;
            dispositiuPrestat.Estat = "En prestec";

            int confirmarDispositiuPrestat = await _dispositiusApiClient.UpdateDispositiuAsync(dispositiuPrestat);

            // Crear registre
            Registre nouRegistre = new Registre();
            nouRegistre.IdPrestec = prestecCreat.Id;
            nouRegistre.Accio = "Prestat";
            nouRegistre.NomUsuari = $"{usuariPrestec.Nom} {usuariPrestec.Cognom}";
            nouRegistre.IdUsuari = (int)usuariPrestec.Id;
            nouRegistre.NomDispositiu = dispositiuPrestat.Nom;
            nouRegistre.IdDispositiu = (int)dispositiuPrestat.Id;
            nouRegistre.NomGrup = usuariPrestec.Grup;
            nouRegistre.IdGrup = (int)usuariPrestec.IdGrup;
            nouRegistre.DataAccio = DataEntrega;
            nouRegistre.DataRetorn = DataRetorn;

            Registre registreCreat = await _registresApiClient.PostRegistreAsync(nouRegistre);

            // Si crear el préstec falla
            if (prestecCreat == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al crear el préstec.");
                esPotAfegir = true;
                return;
            }
            
            // Si actualitzar el dispositiu falla
            if (confirmarDispositiuPrestat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al actualitzar el dispositiu.");
                esPotAfegir = true;
                return;
            }
            
            // Si crear el registre falla
            if (registreCreat == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al crear el registre.");
                esPotAfegir = true;
                return;
            }
            
            await _prestecsVM.MostrarPrestecsAsync();
            EsVisible = Visibility.Collapsed;
        });
    }
}
