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
    internal class AfegirDispositiuVM: Utilities.ViewModelBase
    {
        private readonly DispositiusApiClient _dispositiusApiClient;
        private readonly CategoriesApiClient _categoriesApiClient;

        private readonly DispositiusVM _dispositiusVM;
        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotAfegir = true;

        // CONSTRUCTOR
        public AfegirDispositiuVM(DispositiusVM dispositiusVM)
        {
            _dispositiusVM = dispositiusVM;

            MissatgeError = new MissatgeErrorVM();
            _dispositiusApiClient = new DispositiusApiClient();
            _categoriesApiClient = new CategoriesApiClient();

            CarregarCategories();
        }

        // NOM
        private string _nom;
        public string Nom
        {
            get => _nom;
            set
            {
                _nom = value;
                OnPropertyChanged();
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

        // LLISTA CATEGORIES
        private List<Categoria> _categories;
        public List<Categoria> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        // CATEGORIA SELECCIONAT
        private long _categoriaId;
        public long CategoriaId
        {
            get => _categoriaId;
            set
            {
                _categoriaId = value;
                OnPropertyChanged();
            }
        }

        // CARREGAR CATEGORIES
        private async void CarregarCategories()
        {
            var categories = await _categoriesApiClient.GetAllCategoriesAsync();

            // Si falla la càrrega de categories
            if (categories == null)
            {
                Categories = new List<Categoria>();
            }
            // Si es retornen les categories correctament
            else
            {
                Categories = categories.ToList();
            }

            // Si es retornen buits
            if (Categories == null || Categories.Count < 1)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al carregar les categories.");
            }
            else
            {
                CategoriaId = Categories[0].Id;
            }
        }

        // OBRIR FINESTRA
        public void Mostrar()
        {
            Nom = "";
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR NOU DISPOSITIU
        public ICommand AfegirDispositiu_Click => new RelayCommand(async _ =>
        {
            if (esPotAfegir)
            {
                // Si hi ha camps buits
                if (string.IsNullOrWhiteSpace(Nom))
                {
                    MissatgeError.Mostrar("No hi poden haver camps buits.");
                }
                else
                {
                    esPotAfegir = false;

                    NouDispositiu dispositiuCurs = new NouDispositiu();
                    dispositiuCurs.IdCategoria = CategoriaId;
                    dispositiuCurs.Nom = Nom;
                    dispositiuCurs.Estat = "Disponible";

                    NouDispositiu dispositiuCreat = await _dispositiusApiClient.PostDispositiuAsync(dispositiuCurs);

                    // Si crear el dispositiu falla
                    if (dispositiuCreat == null)
                    {
                        MissatgeError.Mostrar("Hi ha hagut un problema al crear el dispositiu.");
                    }
                    // Si crear el dispositiu funciona
                    else
                    {
                        await _dispositiusVM.MostrarDispositiusAsync();
                        EsVisible = Visibility.Collapsed;
                    }
                }
            }                       
        });
    }
}
