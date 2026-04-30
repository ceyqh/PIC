using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    internal class EditarPrestecVM: Utilities.ViewModelBase
    {
        private readonly PrestecsApiClient _prestecsApiClient;
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly DispositiusApiClient _dispositiusApiClient;
        private readonly RegistresApiClient _registresApiClient;

        private readonly PrestecsVM _prestecsVM;

        private Prestec _prestecEnEdicio;

        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotEditar = true;

        // ID Dispositiu antic
        int DispositiuAnticID;

        // ID Usuari antic
        int UsuariAnticID;

        // CONSTRUCTOR
        public EditarPrestecVM(PrestecsVM prestecsVM)
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
        public void Mostrar(Prestec prestec)
        {
            _prestecEnEdicio = prestec;
            UsuariID = _prestecEnEdicio.IdUsuari.ToString();
            DispositiuID = _prestecEnEdicio.IdDispositiu.ToString();
            DataEntrega = _prestecEnEdicio.DataEntrega;
            HoraSeleccionada = _prestecEnEdicio.DataRetorn.Hour;
            DataRetorn = _prestecEnEdicio.DataRetorn;
            
            UsuariAnticID = _prestecEnEdicio.IdUsuari;
            DispositiuAnticID = _prestecEnEdicio.IdDispositiu;

            if (DataRetorn == new DateTime(
                DataEntrega.Year,
                6,
                30,
                21,
                DataRetorn.Minute,
                DataRetorn.Second
             ))
            {
                FinalCurs = true;
            }
            else
            {
                FinalCurs = false;
            }

            esPotEditar = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR PRÉSTEC
        public ICommand GuardarPrestec_Click => new RelayCommand(async _ =>
        {
            if (!esPotEditar)
            {
                return;
            }

            esPotEditar = false;

            // Si els camps d'ID estan buits
            if (string.IsNullOrWhiteSpace(UsuariID.ToString()) || string.IsNullOrWhiteSpace(DispositiuID.ToString()))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
                esPotEditar = true;
                return;
            }
                
            // Comprovar que l'ID d'Usuari existeix
            var usuaris = await _usuarisApiClient.GetAllUsuarisAsync();

            // Si falla la consulta
            if (usuaris == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema consultant l'usuari");
                esPotEditar = true;
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
                esPotEditar = true;
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
                esPotEditar = true;
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
                MissatgeError.Mostrar("La data de retorn no pot ser anterior a la d'entrega");
                esPotEditar = true;
                return;
            }
             
            Usuari usuariConsulta = await _usuarisApiClient.GetUsuariPerIdAsync(int.Parse(UsuariID));
            Dispositiu dispositiuConsulta = await _dispositiusApiClient.GetDispositiuPerIdAsync(int.Parse(DispositiuID));

            // Si falla la consulta a Usuaris 
            if (usuariConsulta == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema consultant l'usuari");
                esPotEditar = true;
                return;
            }

            // Si falla la consulta a Dispositius
            if (dispositiuConsulta == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema consultant el dispositiu");
                esPotEditar = true;
                return;
            }
                    
            // Npu préstec
            Prestec nouPrestec = new Prestec();
            nouPrestec.Id = _prestecEnEdicio.Id;
            nouPrestec.NomUsuari = usuariConsulta.Nom;
            nouPrestec.IdUsuari = int.Parse(UsuariID);
            nouPrestec.NomDispositiu = dispositiuConsulta.Nom;
            nouPrestec.IdDispositiu = int.Parse(DispositiuID);
            nouPrestec.DataEntrega = DataEntrega;
            nouPrestec.DataRetorn = DataRetorn;

            int prestecActualitzat = await _prestecsApiClient.UpdatePrestecAsync(nouPrestec);

            // Nou dispositiu amb l'estat actualitzat
            Dispositiu nouDispositiuPrestat = new Dispositiu();
            nouDispositiuPrestat.Id = int.Parse(DispositiuID);
            nouDispositiuPrestat.Nom = dispositiuConsulta.Nom;
            nouDispositiuPrestat.IdCategoria = dispositiuConsulta.IdCategoria;
            nouDispositiuPrestat.Estat = "En prestec";

            int confirmarDispositiuPrestat = await _dispositiusApiClient.UpdateDispositiuAsync(nouDispositiuPrestat);
                                        
            // Antic dispositiu amb l'estat actualitzat
            Dispositiu anticDispositiuPrestat = new Dispositiu();
            anticDispositiuPrestat.Id = DispositiuAnticID;
            anticDispositiuPrestat.Nom = dispositiuConsulta.Nom;
            anticDispositiuPrestat.IdCategoria = dispositiuConsulta.IdCategoria;
            anticDispositiuPrestat.Estat = "Disponible";

            int confirmarDispositiuActualitzat= await _dispositiusApiClient.UpdateDispositiuAsync(anticDispositiuPrestat);

            // Crear registre
            Registre nouRegistre = new Registre();
            nouRegistre.IdPrestec = _prestecEnEdicio.Id;
            nouRegistre.NomUsuari = $"{usuariPrestec.Nom} {usuariPrestec.Cognom}";
            nouRegistre.Accio = "Modificat";
            nouRegistre.IdUsuari = (int)usuariPrestec.Id;
            nouRegistre.NomDispositiu = dispositiuConsulta.Nom;
            nouRegistre.IdDispositiu = (int)dispositiuConsulta.Id;
            nouRegistre.NomGrup = usuariPrestec.Grup;
            nouRegistre.IdGrup = (int)usuariPrestec.IdGrup;
            nouRegistre.DataAccio = DateTime.Now;
            nouRegistre.DataRetorn = DataRetorn;

            Registre registreActualitzat = await _registresApiClient.PostRegistreAsync(nouRegistre);

            // Si actualitzar el préstec falla
            if (prestecActualitzat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al actualitzar el préstec");
                esPotEditar = true;
                return;
            }
            
            // Si actualitzar el nou dispositiu falla            
            if (confirmarDispositiuPrestat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al actualitzar el nou dispositius.");
                esPotEditar = true;
                return;
            }

            // Si actualitzar l'antic dispositiu falla
            if (confirmarDispositiuActualitzat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al actualitzar l'antic dispositius.");
                esPotEditar = true;
                return;
            }
            
            // Si crear el registre falla
            if (registreActualitzat == null)
            {
                MissatgeError.Mostrar("Hi ha hagun un problema al crear el registre.");
                esPotEditar = true;
                return;
            }

            await _prestecsVM.MostrarPrestecsAsync();
            EsVisible = Visibility.Collapsed;
        });
    }
}
