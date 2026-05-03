using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PIC.ViewModel
{
    internal class LoginVM : Utilities.ViewModelBase
    {
        private readonly NavigationVM _navigation;
        private readonly AdministradorsApiClient _administradorsApiClient;

        public LoginVM(NavigationVM nav)
        {
            _navigation = nav; 
            _administradorsApiClient = new AdministradorsApiClient();
        }        

        // USUARI
        private string _usuari;
        public string Usuari
        {
            get => _usuari;
            set
            {
                _usuari = value;
                OnPropertyChanged();
            }
        }

        // CONTRASENYA
        private string _contrasenya;
        public string Contrasenya
        {
            get => _contrasenya;
            set
            {
                _contrasenya = value;
                OnPropertyChanged();
            }
        }

        // TEXT ERROR
        private string _textError;
        public string TextError
        {
            get => _textError;
            set
            {
                _textError = value;
                OnPropertyChanged();
            }
        }

        // FER LOGIN
        public ICommand Login_Click => new RelayCommand(async _=> {
            TextError = "";
            if (string.IsNullOrEmpty(Usuari) || string.IsNullOrEmpty(Contrasenya))
            {
                TextError = "Els camps no poden quedar buits.";
                return;
            }

            var administradors = await _administradorsApiClient.GetAllAdministradorsAsync();

            if (administradors == null)
            {
                TextError = "Hi ha hagut un problema amb la API.";
                return;
            }

            Administrador admin = new Administrador();
            bool adminTrobat = false;
            int i = 0;

            while (i < administradors.Count && !adminTrobat)
            {
                if (administradors[i].Nom == Usuari)
                {
                    admin = administradors[i];
                    adminTrobat = true;
                }
                else
                {
                    i++;
                }
            }

            if (!adminTrobat)
            {
                TextError = "No existeix cap usuari amb aquest nom.";
                return;
            }

            if (admin.Contrasenya != Contrasenya)
            {
                TextError = "Contrasenya incorrecte.";
                return;
            }

            _navigation.MainAppViewCommand.Execute(null);
        });
    }
}
