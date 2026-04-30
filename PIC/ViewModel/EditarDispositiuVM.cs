using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    internal class EditarDispositiuVM: Utilities.ViewModelBase
    {
        private readonly DispositiusVM _dispositiusVM;
        private readonly DispositiusApiClient _dispositiusApiClient;
        private readonly CategoriesApiClient _categoriesApiClient;

        private Dispositiu _dispositiuEnEdicio;
        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotEditar = true;

        public EditarDispositiuVM(DispositiusVM dispositiusVM)
        {
            _dispositiusVM = dispositiusVM;

            MissatgeError = new MissatgeErrorVM();
            _dispositiusApiClient = new DispositiusApiClient();
            _categoriesApiClient = new CategoriesApiClient();
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

        // ITEMS SELECCIONABLES
        private List<Categoria> _categories;
        public List<Categoria> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        // ID SELECCIONAT
        private long _categoriaId;
        public long CategoriaId
        {
            get => _categoriaId;
            set { _categoriaId = value; OnPropertyChanged(); }
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
        public async Task Mostrar(Dispositiu dispositiu)
        {
            _dispositiuEnEdicio = dispositiu;
            Nom = dispositiu.Nom;

            var categories = await _categoriesApiClient.GetAllCategoriesAsync();
            Categories = categories.ToList();
            CategoriaId = dispositiu.IdCategoria;

            esPotEditar = true;
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // GUARDAR DISPOSITIU
        public ICommand GuardarDispositiu_Click => new RelayCommand(async _ =>
        {
            if (!esPotEditar)
            {
                return;
            }

            esPotEditar = false;

            // Si hi ha camps buits
            if (string.IsNullOrWhiteSpace(Nom))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
                esPotEditar = true;
                return;
            }

            // Actuaitzar dispositiu
            _dispositiuEnEdicio.Nom = Nom;
            _dispositiuEnEdicio.IdCategoria = CategoriaId;
            int dispositiuActualitzat = await _dispositiusApiClient.UpdateDispositiuAsync(_dispositiuEnEdicio);

            // Si actualitzar el dispositiu falla
            if (dispositiuActualitzat == -1)
            {
                MissatgeError.Mostrar("Hi ha hagut un problema al actualitzar el dispositiu.");
                esPotEditar = true;
                return;
            }

            await _dispositiusVM.MostrarDispositiusAsync();
            EsVisible = Visibility.Collapsed;
        });
    }
}
