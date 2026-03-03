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
    internal class UsuarisVM : Utilities.ViewModelBase
    {
        private readonly UsuarisApiClient _apiClient;

        public ObservableCollection<Usuari> Usuaris { get; set; }

        public UsuarisVM()
        {
            _apiClient = new UsuarisApiClient();
            Usuaris = new ObservableCollection<Usuari>();
        }

        public async Task LoadUsuarisAsync()
        {
            var llista = await _apiClient.GetAllUsuarisAsync();

            Usuaris.Clear();

            foreach (var p in llista)
            {
                Usuaris.Add(p);
            }
        }
    }
}
