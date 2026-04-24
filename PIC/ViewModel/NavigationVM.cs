using PIC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PIC.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }
        public ICommand HomeCommand { get; set; }
        public ICommand PrestecsCommand { get; set; }
        public ICommand DispositiusCommand { get; set; }
        public ICommand CategoriesCommand { get; set; }
        public ICommand UsuarisCommand { get; set; }
        public ICommand CursosCommand { get; set; }
        public ICommand DepartamentsCommand { get; set; }
        public ICommand PicCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Prestecs(object obj) => CurrentView = new PrestecsVM();
        private void Dispositius(object obj) => CurrentView = new DispositiusVM();
        private void Categories(object obj) => CurrentView = new CategoriesVM();
        private void Usuaris(object obj) => CurrentView = new UsuarisVM();
        private void Cursos(object obj) => CurrentView = new CursosVM();
        private void Departaments(object obj) => CurrentView = new DepartamentsVM();
        private void Pic(object obj) => CurrentView = new PicVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            PrestecsCommand = new RelayCommand(Prestecs);
            DispositiusCommand = new RelayCommand(Dispositius);
            CategoriesCommand = new RelayCommand(Categories);
            UsuarisCommand = new RelayCommand(Usuaris);
            CursosCommand = new RelayCommand(Cursos);
            DepartamentsCommand = new RelayCommand(Departaments);
            PicCommand = new RelayCommand(Pic);

            // Startup Page
            CurrentView = new HomeVM();
        }
    }
}
