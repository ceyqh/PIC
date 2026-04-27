using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    public enum CategoriesTipusCerca
    {
        PerId,
        DispositiusPerCategoria
    }

    internal class CategoriesVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Categoria> Categories { get; set; }
        public ObservableCollection<Dispositiu> Dispositius { get; set; }
        public MissatgeErrorVM MissatgeError { get; set; }
        public AfegirCategoriaVM AfegirCategoria { get; set; }
        public EditarCategoriaVM EditarCategoria { get; set; }
        public ConfirmarEsborrarVM ConfirmarEsborrar { get; set; }

        private readonly CategoriesApiClient _categoriesApiClient;
        private readonly DispositiusApiClient _dispositiusApiClient;

        public CategoriesVM()
        {
            Categories = new ObservableCollection<Categoria>();
            Dispositius = new ObservableCollection<Dispositiu>();

            _categoriesApiClient = new CategoriesApiClient();
            _dispositiusApiClient = new DispositiusApiClient();

            MissatgeError = new MissatgeErrorVM();
            AfegirCategoria = new AfegirCategoriaVM(this);
            EditarCategoria = new EditarCategoriaVM(this);
            ConfirmarEsborrar = new ConfirmarEsborrarVM();

            _ = MostrarCategoriesAsync();
        }

        // PROPIETATS DE LA UI
        private Visibility _cercaVisibility = Visibility.Collapsed;
        public Visibility CercaVisibility
        {
            get => _cercaVisibility;
            set { _cercaVisibility = value; OnPropertyChanged(); }
        }

        private string _titolPantalla = "CATEGORIES: TOTS";
        public string TitolPantalla
        {
            get => _titolPantalla;
            set { _titolPantalla = value; OnPropertyChanged(); }
        }

        // CATEGORIA SELECCIONADA
        private Categoria _categoriaSeleccionada;
        public Categoria CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                _categoriaSeleccionada = value;
                OnPropertyChanged();

                // Carregar els dispositius de la categoria al seleccionar-ne una
                if (_categoriaSeleccionada != null)
                {
                    _ = CarregarDispositiusDeCategoriaAsync((int)_categoriaSeleccionada.Id);
                }
            }
        }

        // CARREGAR DISPOSITIUS DE LA CATEGORIA
        private async Task CarregarDispositiusDeCategoriaAsync(int cursId)
        {
            Dispositius.Clear();
            var llista = await _dispositiusApiClient.GetDispositiusPerIdCategoriaAsync(cursId);

            // Si la consulta funciona
            if (llista != null)
            {
                foreach (var u in llista)
                {
                    Dispositius.Add(u);
                }
            }            
        }

        // TIPUS DE CERCA
        private CategoriesTipusCerca _tipusCercaActualCategories;
        public CategoriesTipusCerca TipusCercaActualCategories
        {
            get => _tipusCercaActualCategories;
            set
            {
                _tipusCercaActualCategories = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private string _parametreCercaCategories;
        public string ParametreCercaCategories
        {
            get => _parametreCercaCategories;
            set
            {
                _parametreCercaCategories = value;
                OnPropertyChanged();
            }
        }

        // COMANDAMENTS
        public ICommand CanviarModeCercaCommand => new RelayCommand(param =>
        {
            // Si el el paràmetre és buit
            if (param == null)
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                string mode = param.ToString();

                Categories.Clear();
                CercaVisibility = Visibility.Visible;

                switch (mode)
                {
                    case "TOTS":
                        TitolPantalla = "CATEGORIES: TOTS";
                        ParametreCercaCategories = "";
                        ClearDispositius();
                        CercaVisibility = Visibility.Collapsed;
                        _ = MostrarCategoriesAsync();
                        break;

                    case "PER_ID":
                        TitolPantalla = "CATEGORIES: PER ID";
                        ParametreCercaCategories = "";
                        ClearDispositius();
                        TipusCercaActualCategories = CategoriesTipusCerca.PerId;
                        break;
                }
            }            
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaCategoriesAsync();
        });

        // MÈTODE DE CERCA
        public async Task CercaCategoriesAsync()
        {
            // Si el textbox és buit
            if (string.IsNullOrEmpty(ParametreCercaCategories))
            {
                MissatgeError.Mostrar("El camp no pot quedar buit.");
            }
            else
            {
                Categories.Clear();
                switch (TipusCercaActualCategories)
                {
                    case CategoriesTipusCerca.PerId:
                        var curs = await _categoriesApiClient.GetCategoriaPerIdAsync(int.Parse(ParametreCercaCategories));

                        // Si la consulta falla o és buida
                        if (curs == null)
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap categoria amb aquest ID.");
                        }
                        // Si la consulta funciona
                        else
                        {
                            Categories.Add(curs);
                        }
                        break;
                }
            }           
        }

        // MOSTRAR CATEGORIES
        public async Task MostrarCategoriesAsync()
        {
            // Si la api falla
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
            {
                MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
            }
            // Si la api funciona
            else
            {
                var llista = await _categoriesApiClient.GetAllCategoriesAsync();

                // Si la consulta falla
                if (llista == null)
                {
                    MissatgeError.Mostrar("No s'han pogut mostrar les Categories. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                }
                // Si la consulta funciona
                else
                {
                    Categories.Clear();

                    foreach (var u in llista)
                    {
                        Categories.Add(u);
                    }
                }
            }
        }

        // AFEGIR CATEGORIA
        public ICommand AfegirCategoriaMenu_Click => new RelayCommand(async _ =>
        {
            AfegirCategoria.Mostrar();
        });

        // EDITAR CATEGORIA
        public ICommand EditarCategoriaMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap categoria seleccionada
            if (_categoriaSeleccionada != null)
            {
                await EditarCategoria.Mostrar(CategoriaSeleccionada);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar una categoria.");
            }
        });

        // ESBORRAR CATEGORIA
        public ICommand EsborrarCategoriaMenu_Click => new RelayCommand(async _ =>
        {
            // Si no hi ha cap categoria seleccionada
            if (_categoriaSeleccionada != null)
            {
                int categoriad = (int)_categoriaSeleccionada.Id;
                var comptarUsuaris = await _dispositiusApiClient.GetDispositiusPerIdCategoriaAsync(categoriad);

                // Si la consulta falla
                if (comptarUsuaris == null)
                {
                    MissatgeError.Mostrar("Hi ha hagut un prolema al intentar esborrar la categoria.");
                }
                // Si la categoria conté dispositius
                else if (comptarUsuaris.Count > 0)
                {
                    MissatgeError.Mostrar("Aquest curs conté un o varis alumnes, per seguretat, només es poden esborrar els cursos buits. " +
                        "Si vols esborrar aquest curs, primer hasd'eliminar els seus alumnes.");
                }
                else
                {
                    ConfirmarEsborrar.Mostrar(_categoriaSeleccionada, this);
                }
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar una categoria.");
            }
        });

        // BUIDAR LIST VIEW CATEGORIES
        public void ClearDispositius()
        {
            Dispositius.Clear();
        }
    }
}
