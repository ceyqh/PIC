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
    internal class MissatgeErrorVM : Utilities.ViewModelBase
    {
        private string _missatge;
        public string Missatge
        {
            get => _missatge;
            set
            {
                _missatge = value;
                OnPropertyChanged();
            }
        }

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

        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        public void Mostrar(string message)
        {
            Missatge = message;
            EsVisible = Visibility.Visible;
        }
    }
}
