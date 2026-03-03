using PIC.APIClient;
using PIC.Model;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIC.ViewModel
{
    internal class CategoriesVM : Utilities.ViewModelBase
    {
        private readonly CategoriesApiClient _apiClient;

        public ObservableCollection<Categoria> Categories { get; set; }

        public CategoriesVM()
        {
            _apiClient = new CategoriesApiClient();
            Categories = new ObservableCollection<Categoria>();
        }

        public async Task LoadCategoriesAsync()
        {
            var llista = await _apiClient.GetAllCategoriesAsync();

            Categories.Clear();

            foreach (var p in llista)
            {
                Categories.Add(p);
            }
        }
    }
}
