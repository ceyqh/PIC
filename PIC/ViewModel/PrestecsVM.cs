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
    internal class PrestecsVM : Utilities.ViewModelBase
    {
        private readonly PrestecsApiClient _apiClient;

        public ObservableCollection<Prestec> Prestecs { get; set; }

        public PrestecsVM()
        {
            _apiClient = new PrestecsApiClient();
            Prestecs = new ObservableCollection<Prestec>();
        }

        public async Task LoadPrestecsAsync()
        {
            var llista = await _apiClient.GetAllPrestecsAsync();

            Prestecs.Clear();

            foreach (var p in llista)
            {
                Prestecs.Add(p);
            }
        }
    }
}
