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

        private readonly PrestecsVM _prestecsVM;

        public MissatgeErrorVM MissatgeError { get; set; }

        // CONSTRUCTOR
        public AfegirPrestecVM(PrestecsVM prestecsVM)
        {
            _prestecsVM = prestecsVM;

            MissatgeError = new MissatgeErrorVM();
            _usuarisApiClient = new UsuarisApiClient();
            _dispositiusApiClient = new DispositiusApiClient();
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
        private int _usuariID;
        public int UsuariID
        { 
            get => _usuariID;
            set
            {
                _usuariID = value;
                OnPropertyChanged();
            }
        }

        // ID DISPOSITIU
        private int _dispositiuID;
        public int DispositiuID
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
            UsuariID = 0;
            DispositiuID = 0;
            DataEntrega = DateTime.Now;
            HoraSeleccionada = 21;
            DataRetorn = new DateTime(DataEntrega.Year, DataEntrega.Month, DataEntrega.Day, HoraSeleccionada, 0, 0);
            FinalCurs = false;

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
            // Si els camps d'ID estan buits
            if (string.IsNullOrWhiteSpace(UsuariID.ToString()) || string.IsNullOrWhiteSpace(DispositiuID.ToString()))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
            }
            else
            {
                // Comprovar que l'ID d'Usuari existeix
                var usuaris = await _usuarisApiClient.GetAllUsuarisAsync();

                bool existeixUsuari = false;
                int i = 0;

                while (i < usuaris.Count && !existeixUsuari)
                {
                    if (usuaris[i].Id == UsuariID)
                    {
                        existeixUsuari = true;
                    }
                    else { i++; }
                }

                // Si no existeix l'ID d'Usuari
                if (!existeixUsuari)
                {
                    MissatgeError.Mostrar($"No existeix cap usuari amb ID({UsuariID}).");
                }
                else
                {
                    // Comprovar que l'ID d'Usuari existeix
                    var dispositius = await _dispositiusApiClient.GetAllDispositiusAsync();

                    bool existeixDispositiu = false;
                    int j = 0;

                    while (j < dispositius.Count && !existeixDispositiu)
                    {
                        if (dispositius[j].Id == DispositiuID)
                        {
                            existeixDispositiu = true;
                        }
                        else { j++; }
                    }

                    // Si no existeix l'ID de Dispositiu
                    if (!existeixDispositiu)
                    {
                        MissatgeError.Mostrar($"No existeix cap dispositiu amb ID({UsuariID}).");
                    }
                    else
                    {
                        // Assignar data de retorn
                        if (FinalCurs)
                        {
                            DataRetorn = new DateTime(DataEntrega.Year, 6, 30, 21, 0, 0);
                        }
                        else
                        {
                            DataRetorn = new DateTime(DataRetorn.Year, DataRetorn.Month, DataRetorn.Day, HoraSeleccionada, 0, 0);
                        }

                        // Si la data de retorn és anterior a la d'entrega
                        if (DataEntrega > DataRetorn)
                        {
                            MissatgeError.Mostrar("La data de retorn no pot ser anterior a la d'entrega");
                        }
                    }
                }  
            }


            
        });
    }
}
