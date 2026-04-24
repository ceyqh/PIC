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
    internal class AfegirCategoriaVM : Utilities.ViewModelBase
    {
        private readonly CategoriesApiClient _categoriesApiClient;
        private readonly CategoriesVM _categoriesVM;

        public MissatgeErrorVM MissatgeError { get; set; }

        // Validador per assegurar que no es dupliquen accions
        private bool esPotAfegir = true;

        // CONSTRUCTOR
        public AfegirCategoriaVM(CategoriesVM categoriesVM)
        {
            _categoriesVM = categoriesVM;

            MissatgeError = new MissatgeErrorVM();
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
            Nom = "";
            EsVisible = Visibility.Visible;
        }

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // AFEGIR NOVA CATEGORIA
        public ICommand AfegirCategoria_Click => new RelayCommand(async _ =>
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
                    Categoria novaCategoria = new Categoria();
                    novaCategoria.Nom = Nom;

                    Categoria categoriaCreada = await _categoriesApiClient.PostCategoriaAsync(novaCategoria);

                    // Si crear la categoria falla
                    if (categoriaCreada == null)
                    {
                        MissatgeError.Mostrar("Hi ha hagut un problema al crear la categoria.");
                    }
                    // Si crear la categoria funciona
                    else
                    {
                        esPotAfegir = false;
                        await _categoriesVM.MostrarCategoriesAsync();
                        EsVisible = Visibility.Collapsed;                         
                    }
                }
            }            
        });
    }
}
