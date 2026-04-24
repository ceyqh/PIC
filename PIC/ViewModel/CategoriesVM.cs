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

        // CURS SEL·LECCIONAT
        private Categoria _categoriaSeleccionada;
        public Categoria CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                _categoriaSeleccionada = value;
                OnPropertyChanged();

                if (_categoriaSeleccionada != null)
                {
                    _ = CarregarDispositiusDeCategoriaAsync((int)_categoriaSeleccionada.Id);
                }
            }
        }

        private async Task CarregarDispositiusDeCategoriaAsync(int cursId)
        {
            try
            {
                var llista = await _dispositiusApiClient.GetDispositiusPerIdCategoriaAsync(cursId);

                Dispositius.Clear();

                foreach (var u in llista)
                {
                    Dispositius.Add(u);
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error carregant dispositius: " + ex.Message);
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
        private int _parametreCercaCategories;
        public int ParametreCercaCategories
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
            if (param == null) return;
            string mode = param.ToString();

            // Reset de la llista i visibilitat per defecte
            Categories.Clear();
            CercaVisibility = Visibility.Visible;

            switch (mode)
            {
                case "TOTS":
                    TitolPantalla = "CATEGORIES: TOTS";
                    ParametreCercaCategories = 0;
                    ClearUsuaris();
                    CercaVisibility = Visibility.Collapsed;
                    _ = MostrarCategoriesAsync();
                    break;

                case "PER_ID":
                    TitolPantalla = "CATEGORIES: PER ID";
                    ParametreCercaCategories = 0;
                    ClearUsuaris();
                    TipusCercaActualCategories = CategoriesTipusCerca.PerId;
                    break;
            }
        });

        // EXECUTAR CERCA
        public ICommand BuscarCommand => new RelayCommand(async _ =>
        {
            await CercaCategoriesAsync();
        });

        // MÈTODE DE CERCA AMB PROTECCIÓ DE NULLS
        public async Task CercaCategoriesAsync()
        {
            try
            {
                Categories.Clear();
                switch (TipusCercaActualCategories)
                {
                    case CategoriesTipusCerca.PerId:
                        var curs = await _categoriesApiClient.GetCategoriaPerIdAsync(ParametreCercaCategories);

                        if (curs != null)
                        {
                            Categories.Add(curs);
                        }
                        else
                        {
                            MissatgeError.Mostrar("No s'ha trobat cap categoria amb aquest ID.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MissatgeError.Mostrar("Error en la cerca: " + ex.Message);
            }
        }

        // MOSTRAR CURSOS
        public async Task MostrarCategoriesAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseUri"]))
                {
                    MissatgeError.Mostrar("Error: La configuració 'BaseUri' no s'ha trobat al fitxer App.config.");
                    
                }
                else
                {
                    var llista = await _categoriesApiClient.GetAllCategoriesAsync();
                    if (llista == null)
                    {
                        MissatgeError.Mostrar("No s'han pogut mostrar les Categories. Comprova la connexió entre l'API i l'aplicació o la seva configuració.");
                    }
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

            catch (Exception ex)
            {
                MissatgeError.Mostrar("No es pot connectar amb el servidor: " + ex.Message);
            }
        }

        // AFEGIR CURS
        public ICommand AfegirCategoriaMenu_Click => new RelayCommand(async _ =>
        {
            AfegirCategoria.Mostrar();
        });

        // EDITAR CURS
        public ICommand EditarCategoriaMenu_Click => new RelayCommand(async _ =>
        {
            if (_categoriaSeleccionada != null)
            {
                await EditarCategoria.Mostrar(CategoriaSeleccionada);
            }
            else
            {
                MissatgeError.Mostrar("Cal seleccionar una categoria.");
            }
        });

        // ESBORRAR CURS
        public ICommand EsborrarCategoriaMenu_Click => new RelayCommand(async _ =>
        {
            if (_categoriaSeleccionada != null)
            {
                int categoriad = (int)_categoriaSeleccionada.Id;
                var comptarUsuaris = await _dispositiusApiClient.GetDispositiusPerIdCategoriaAsync(categoriad);

                if (comptarUsuaris.Count > 0)
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

        // BUIDAR LIST VIEW CURSOS
        public void ClearCursos()
        {
            Categories.Clear();
        }

        // BUIDAR LIST VIEW USUARIS
        public void ClearUsuaris()
        {
            Dispositius.Clear();
        }
    }
}
