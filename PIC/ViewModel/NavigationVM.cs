using PIC.Model;
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
    class NavigationVM : ViewModelBase
    {
        private Administrador _usuariLoguejat;
        public Administrador UsuariLoguejat
        {
            get => _usuariLoguejat;
            set { _usuariLoguejat = value; OnPropertyChanged(); }
        }
        public MissatgeErrorVM MissatgeError { get; set; }

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

        private void Home(object obj) => SubCurrentView = new HomeVM(UsuariLoguejat);
        private void Prestecs(object obj) => SubCurrentView = new PrestecsVM(UsuariLoguejat);
        private void Dispositius(object obj) => SubCurrentView = new DispositiusVM(UsuariLoguejat);
        private void Categories(object obj) => SubCurrentView = new CategoriesVM(UsuariLoguejat);
        private void Usuaris(object obj) => SubCurrentView = new UsuarisVM(UsuariLoguejat);
        private void Cursos(object obj) => SubCurrentView = new CursosVM(UsuariLoguejat);
        private void Departaments(object obj) => SubCurrentView = new DepartamentsVM(UsuariLoguejat);
        private void Registres(object obj) => SubCurrentView = new RegistresVM();
        private void Login(object obj) => CurrentView = new LoginVM(this);

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

            CurrentView = new LoginVM(this);

            MissatgeError = new MissatgeErrorVM();
        }

        private void MainAppView(object obj)
        {
            if (obj is Administrador admin)
            {
                UsuariLoguejat = admin;
            }

            SubCurrentView = new HomeVM(UsuariLoguejat);
            CurrentView = this;
        }

        private void Pic(object obj)
        {
            if (UsuariLoguejat.Permisos == "Préstecs")
            {
                MissatgeError.Mostrar("No tens permisos per accedir a aquest apartat.");
                return;
            }

            SubCurrentView = new PicVM(UsuariLoguejat);
        }

        // TANCAR SESSIO
        public ICommand TancarSessio_Click => new RelayCommand(_ =>
        {
            CurrentView = new LoginVM(this);
        });
    }
}
