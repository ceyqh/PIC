using PIC.APIClient;
using PIC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.ViewModel
{
    internal class DepartamentsVM : Utilities.ViewModelBase
    {
        private readonly DepartamentsApiClient _apiClient;

        public ObservableCollection<Departament> Departaments { get; set; }

        public DepartamentsVM()
        {
            _apiClient = new DepartamentsApiClient();
            Departaments = new ObservableCollection<Departament>();
        }

        public async Task LoadDepartamentsAsync()
        {
            var llista = await _apiClient.GetAllDepartamentsAsync();

            Departaments.Clear();

            foreach (var p in llista)
            {
                Departaments.Add(p);
            }
        }
    }
}
