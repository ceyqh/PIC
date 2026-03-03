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
    internal class CursosVM : Utilities.ViewModelBase
    {
        private readonly CursosApiClient _apiClient;

        public ObservableCollection<Curs> Cursos { get; set; }

        public CursosVM()
        {
            _apiClient = new CursosApiClient();
            Cursos = new ObservableCollection<Curs>();
        }

        public async Task LoadCursosAsync()
        {
            var llista = await _apiClient.GetAllCursosAsync();

            Cursos.Clear();

            foreach (var p in llista)
            {
                Cursos.Add(p);
            }
        }
    }
}
