using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PIC.ViewModel
{
    internal class HomeVM : Utilities.ViewModelBase
    {
        private readonly Administrador _administrador;

        private string _textBenvinguda = $"Hola, usuari. Benvingut/da a PIC, el gestor de préstecs de material de l'Institut Rafael Campalans.";
        public string TextBenvinguda
        {
            get => _textBenvinguda;
            set { _textBenvinguda = value; OnPropertyChanged(); }
        }
        public HomeVM(Administrador administrador)
        {
            _administrador = administrador;

            TextBenvinguda = $"Hola, {_administrador.Nom}. Benvingut/da a PIC, el gestor de préstecs de material de l'Institut Rafael Campalans.";
        }
    }
}
