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

        private object _subCurrentView;
        public object SubCurrentView
        {
            get => _subCurrentView;
            set { _subCurrentView = value; OnPropertyChanged(); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand PrestecsCommand { get; set; }
        public ICommand DispositiusCommand { get; set; }
        public ICommand CategoriesCommand { get; set; }
        public ICommand UsuarisCommand { get; set; }
        public ICommand CursosCommand { get; set; }
        public ICommand DepartamentsCommand { get; set; }
        public ICommand PicCommand { get; set; }
        public ICommand RegistresCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand MainAppViewCommand { get; set; }

        private void Home(object obj) => SubCurrentView = new HomeVM();
        private void Prestecs(object obj) => SubCurrentView = new PrestecsVM();
        private void Dispositius(object obj) => SubCurrentView = new DispositiusVM();
        private void Categories(object obj) => SubCurrentView = new CategoriesVM();
        private void Usuaris(object obj) => SubCurrentView = new UsuarisVM();
        private void Cursos(object obj) => SubCurrentView = new CursosVM();
        private void Departaments(object obj) => SubCurrentView = new DepartamentsVM();
        private void Pic(object obj) => SubCurrentView = new PicVM();
        private void Registres(object obj) => SubCurrentView = new RegistresVM();
        private void Login(object obj) => CurrentView = new LoginVM(this);

        private void MainAppView(object obj)
        {
            SubCurrentView = new HomeVM();
            CurrentView = this;
        }

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
            RegistresCommand = new RelayCommand(Registres);
            LoginCommand = new RelayCommand(Login);
            MainAppViewCommand = new RelayCommand(MainAppView);

            // Startup Page
            CurrentView = new LoginVM(this);
        }
    }
}
