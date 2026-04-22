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
    internal class EditarCategoriaVM : Utilities.ViewModelBase
    {
        private readonly CategoriesApiClient _categoriesApiClient;
        private readonly CategoriesVM _categoriesVM;

        private Categoria _categoriaEnEdicio;
        public MissatgeErrorVM MissatgeError { get; set; }

        // CONSTRUCTOR
        public EditarCategoriaVM(CategoriesVM categoriesVM)
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
        public async Task Mostrar(Categoria categoria)
        {
            _categoriaEnEdicio = categoria;
            Nom = categoria.Nom;

            EsVisible = Visibility.Visible;
        }

        // GUARDAR CURS
        public ICommand GuardarCategoria_Click => new RelayCommand(async _ =>
        {
            if (string.IsNullOrWhiteSpace(Nom))
            {
                MissatgeError.Mostrar("No hi poden haver camps buits.");
            }
            else
            {
                _categoriaEnEdicio.Nom = Nom;

                //MessageBox.Show($"{_cursEnEdicio.Id} + {_cursEnEdicio.Nom}");
                await _categoriesApiClient.UpdateCategoriaAsync(_categoriaEnEdicio);
                await _categoriesVM.MostrarCategoriesAsync();

                EsVisible = Visibility.Collapsed;
            }            
        });

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
    }
}
