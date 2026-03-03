using PIC.Model;
using PIC.APIClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.ViewModel
{
    internal class DispositiusVM : Utilities.ViewModelBase
    {
        private readonly DispositiusApiClient _apiClient;

        public ObservableCollection<Dispositiu> Dispositius { get; set; }

        public DispositiusVM()
        {
            _apiClient = new DispositiusApiClient();
            Dispositius = new ObservableCollection<Dispositiu>();
        }

        public async Task LoadDispositiusAsync()
        {
            var llista = await _apiClient.GetAllDispositiusAsync();

            Dispositius.Clear();

            foreach (var p in llista)
            {
                Dispositius.Add(p);
            }
        }
    }
}
