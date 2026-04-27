using PIC.APIClient;
using PIC.Model;
using PIC.Utilities;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PIC.ViewModel
{
    public enum AccioAConfirmar
    {
        EsborrarUsuari,
        EsborrarCurs,
        EsborrarDepartament,
        EsborrarDispositiu,
        HabilitarDispositiu,
        DeshabilitarDispositiu,
        EsborrarCategoria,
        EsborrarPrestec,
        FinalitzarPrestec
    }

    internal class ConfirmarEsborrarVM: Utilities.ViewModelBase
    {
        private readonly UsuarisApiClient _usuarisApiClient = new UsuarisApiClient();
        private readonly AlumnesApiClient _alumnesApiClient = new AlumnesApiClient();
        private readonly ProfessorsApiClient _professorsApiClient = new ProfessorsApiClient();
        private readonly CursosApiClient _cursosApiClient = new CursosApiClient();
        private readonly DepartamentsApiClient _departamentsApiClient = new DepartamentsApiClient();
        private readonly CategoriesApiClient _categoriesApiClient = new CategoriesApiClient();
        private readonly DispositiusApiClient _dispositiusApiClient = new DispositiusApiClient();
        private readonly PrestecsApiClient _prestecsApiClient = new PrestecsApiClient();
        private readonly RegistresApiClient _registresApiClient = new RegistresApiClient();

        private UsuarisVM _usuarisVM;
        private CursosVM _cursosVM;
        private DepartamentsVM _departamentsVM;
        private CategoriesVM _categoriesVM;
        private DispositiusVM _dispositiusVM;
        private PrestecsVM _prestecsVM;

        private Usuari usuariSeleccionat;
        private Curs cursSeleccionat;
        private Departament departamentSeleccionat;
        private Dispositiu dispositiuSeleccionat;
        private Categoria categoriaSeleccionat;
        private Prestec prestecSeleccionat;

        public ConfirmarEsborrarVM() 
        {

        }

        //public void SetVM(UsuarisVM vm) => _usuarisVM = vm;
        //public void SetVM(CursosVM vm) => _cursosVM = vm;
        //public void SetVM(DepartamentsVM vm) => _departamentsVM = vm;

        // CONSTRUCTOR PER USUARIS
        public ConfirmarEsborrarVM(UsuarisVM usuarisVM)
        {
            _usuarisVM = usuarisVM;

            _usuarisApiClient = new UsuarisApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();          
            _categoriesApiClient = new CategoriesApiClient();          
            _dispositiusApiClient = new DispositiusApiClient();          
            _prestecsApiClient = new PrestecsApiClient();          
            _registresApiClient = new RegistresApiClient();          
        }

        // TEXT BOTO
        private string _textBoto = "ESBORRAR";
        public string TextBoto
        {
            get => _textBoto;
            set { _textBoto = value; OnPropertyChanged(); }
        }

        // COLOR FINESTRA
        private string _colorFons = "#C44545";
        public string ColorFons
        {
            get => _colorFons;
            set { _colorFons = value; OnPropertyChanged(); }
        }

        private AccioAConfirmar _aEsborrar;
        public AccioAConfirmar AEsborrar
        {
            get => _aEsborrar;
            set
            {
                _aEsborrar = value;
                OnPropertyChanged();
            }
        }

        // MISSATGE QUE ES MOSTRA
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

        // VISIBILITAT MENU
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

        // TANCAR FINESTRA
        public ICommand TancarFinestra => new RelayCommand(_ =>
        {
            EsVisible = Visibility.Collapsed;
        });

        // OBRIR FINESTRA: USUARI
        public void Mostrar(Usuari usuariAEsborrar, UsuarisVM parentVM)
        {
            this._usuarisVM = parentVM;
            this.usuariSeleccionat = usuariAEsborrar;

            Missatge = $"Estàs a punt d'esborrar l'usuari {usuariAEsborrar.Nom} {usuariAEsborrar.Cognom} amb ID({usuariAEsborrar.Id}). Vols confirmar aquesta acció?";

            AEsborrar = AccioAConfirmar.EsborrarUsuari;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: CURS
        public void Mostrar(Curs cursAEsborrar, CursosVM parentVM)
        {
            this._cursosVM = parentVM;
            this.cursSeleccionat = cursAEsborrar;

            Missatge = $"Estàs a punt d'esborrar el curs {cursAEsborrar.Nom} amb ID({cursAEsborrar.Id}). Vols confirmar aquesta acció?";

            AEsborrar = AccioAConfirmar.EsborrarCurs;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: DEPARTAMENT
        public void Mostrar(Departament departamentAEsborrar, DepartamentsVM parentVM)
        {
            this._departamentsVM = parentVM; // Ens assegurem que tenim la referència actual
            this.departamentSeleccionat = departamentAEsborrar;

            Missatge = $"Estàs a punt d'esborrar el departament {departamentAEsborrar.Nom} amb ID({departamentAEsborrar.Id}). Vols confirmar aquesta acció?";

            AEsborrar = AccioAConfirmar.EsborrarDepartament;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: CATEGORIA
        public void Mostrar(Categoria categoriaAEsborrar, CategoriesVM parentVM)
        {
            this._categoriesVM = parentVM; // Ens assegurem que tenim la referència actual
            this.categoriaSeleccionat = categoriaAEsborrar;

            Missatge = $"Estàs a punt d'esborrar la categoria {categoriaAEsborrar.Nom} amb ID({categoriaAEsborrar.Id}). Vols confirmar aquesta acció?";

            AEsborrar = AccioAConfirmar.EsborrarCategoria;
            EsVisible = Visibility.Visible;
        }

        // OBRIR FINESTRA: DISPOSITIU
        public void Mostrar(Dispositiu disposittiuAModificar, DispositiusVM parentVM, string accio)
        {
            if (accio.ToLower() == "esborrar")
            {
                this._dispositiusVM = parentVM; // Ens assegurem que tenim la referència actual
                this.dispositiuSeleccionat = disposittiuAModificar;

                Missatge = $"Estàs a punt d'esborrar el dispositiu {disposittiuAModificar.Nom} amb ID({disposittiuAModificar.Id}). Vols confirmar aquesta acció?";

                AEsborrar = AccioAConfirmar.EsborrarDispositiu;
                EsVisible = Visibility.Visible;
            }

            if (accio.ToLower() == "habilitar")
            {
                this._dispositiusVM = parentVM; // Ens assegurem que tenim la referència actual
                this.dispositiuSeleccionat = disposittiuAModificar;

                Missatge = $"Estàs a punt d'habilitar la categoria {disposittiuAModificar.Nom} amb ID({disposittiuAModificar.Id}). Vols confirmar aquesta acció?";

                TextBoto = "HABILITAR";
                AEsborrar = AccioAConfirmar.HabilitarDispositiu;
                EsVisible = Visibility.Visible;
            }

            if (accio.ToLower() == "deshabilitar")
            {
                this._dispositiusVM = parentVM; // Ens assegurem que tenim la referència actual
                this.dispositiuSeleccionat = disposittiuAModificar;

                Missatge = $"Estàs a punt de deshabilitar la categoria {disposittiuAModificar.Nom} amb ID({disposittiuAModificar.Id}). Vols confirmar aquesta acció?";

                TextBoto = "DESHABILITAR";
                AEsborrar = AccioAConfirmar.DeshabilitarDispositiu;
                EsVisible = Visibility.Visible;
            }

        }

        // OBRIR FINESTRA: PRÉSTECS
        public void Mostrar(Prestec prestecAEsborrar, PrestecsVM parentVM, string accio)
        {
            if (accio.ToLower() == "esborrar")
            {
                this._prestecsVM = parentVM;
                this.prestecSeleccionat = prestecAEsborrar;

                Missatge = $"Estàs a punt d'esborrar el préstec de {prestecAEsborrar.NomDispositiu} amb ID({prestecAEsborrar.IdDispositiu}) prestat a en/na  {prestecAEsborrar.NomUsuari} amb ID({prestecAEsborrar.IdUsuari}). Vols confirmar aquesta acció?";

                AEsborrar = AccioAConfirmar.EsborrarPrestec;
                EsVisible = Visibility.Visible;
            }

            if (accio.ToLower() == "finalitzar")
            {
                TextBoto = "FINALITZAR";
                this._prestecsVM = parentVM;
                this.prestecSeleccionat = prestecAEsborrar;

                Missatge = $"Estàs a punt de finalitzar el préstec de {prestecAEsborrar.NomDispositiu} amb ID({prestecAEsborrar.IdDispositiu}) prestat a en/na  {prestecAEsborrar.NomUsuari} amb ID({prestecAEsborrar.IdUsuari}). Vols confirmar aquesta acció?";

                AEsborrar = AccioAConfirmar.FinalitzarPrestec;
                EsVisible = Visibility.Visible;
            }            
        }

        // ACCIONS
        public ICommand Esborrar_Click => new RelayCommand(async _ =>
        {
            switch (AEsborrar)
            {
                // Si és un USUARI
                case AccioAConfirmar.EsborrarUsuari:
                    if (usuariSeleccionat.Tipus.ToLower() == "alumne")
                    {
                        await _alumnesApiClient.DeleteAlumneAsync((int)usuariSeleccionat.Id);
                    }

                    if (usuariSeleccionat.Tipus.ToLower() == "professor")
                    {
                        await _professorsApiClient.DeleteProfessorAsync((int)usuariSeleccionat.Id);
                    }

                    await _usuarisApiClient.DeleteUsuariAsync((int)usuariSeleccionat.Id);
                    await _usuarisVM.MostrarUsuarisAsync();
                    break;

                // Si és un CURS
                case AccioAConfirmar.EsborrarCurs:
                    await _cursosApiClient.DeleteCursAsync((int)cursSeleccionat.Id);
                    await _cursosVM.MostrarCursosAsync();
                    break;

                // Si és un DEPARTAMENT
                case AccioAConfirmar.EsborrarDepartament:
                    await _departamentsApiClient.DeleteDepartamentAsync((int)departamentSeleccionat.Id);
                    await _departamentsVM.MostrarDepartamentsAsync();
                    break;

                // Si és un DISPOSITIU (esborrar)
                case AccioAConfirmar.EsborrarDispositiu:
                    await _dispositiusApiClient.DeleteDispositiuAsync((int)dispositiuSeleccionat.Id);
                    await _dispositiusVM.MostrarDispositiusAsync();
                    break;

                // Si és un DISPOSITIU (habilitar)
                case AccioAConfirmar.HabilitarDispositiu:
                    dispositiuSeleccionat.Estat = "Disponible";
                    await _dispositiusApiClient.UpdateDispositiuAsync(dispositiuSeleccionat);
                    await _dispositiusVM.MostrarDispositiusAsync();
                    break;

                // Si és un DISPOSITIU (deshabilitar)
                case AccioAConfirmar.DeshabilitarDispositiu:
                    dispositiuSeleccionat.Estat = "No disponible";
                    await _dispositiusApiClient.UpdateDispositiuAsync(dispositiuSeleccionat);
                    await _dispositiusVM.MostrarDispositiusAsync();
                    break;

                // Si és una CATEGORIA
                case AccioAConfirmar.EsborrarCategoria:
                    await _categoriesApiClient.DeleteCategoriaAsync((int)categoriaSeleccionat.Id);
                    await _categoriesVM.MostrarCategoriesAsync();
                    break;

                // Si és un PRÉSTEC
                case AccioAConfirmar.EsborrarPrestec:
                    Dispositiu dispositiuConsultaEsborrar = await _dispositiusApiClient.GetDispositiuPerIdAsync(prestecSeleccionat.IdDispositiu);

                    Dispositiu dispositiuPrestatEsborrar = new Dispositiu();
                    dispositiuPrestatEsborrar.Id = prestecSeleccionat.IdDispositiu;
                    dispositiuPrestatEsborrar.Nom = dispositiuConsultaEsborrar.Nom;
                    dispositiuPrestatEsborrar.IdCategoria = dispositiuConsultaEsborrar.IdCategoria;
                    dispositiuPrestatEsborrar.Estat = "Disponible";

                    int confirmarDispositiuPrestatEsborrar = await _dispositiusApiClient.UpdateDispositiuAsync(dispositiuPrestatEsborrar);

                    Usuari usuariConsultaEsborrar = await _usuarisApiClient.GetUsuariPerIdAsync(prestecSeleccionat.IdUsuari);

                    // Crear registre
                    Registre registreModificat = new Registre();
                    registreModificat.IdPrestec = prestecSeleccionat.Id;
                    registreModificat.Accio = "Cancel·lat";
                    registreModificat.NomUsuari = $"{usuariConsultaEsborrar.Nom} {usuariConsultaEsborrar.Cognom}";
                    registreModificat.IdUsuari = (int)usuariConsultaEsborrar.Id;
                    registreModificat.NomDispositiu = dispositiuPrestatEsborrar.Nom;
                    registreModificat.IdDispositiu = (int)dispositiuPrestatEsborrar.Id;
                    registreModificat.NomGrup = usuariConsultaEsborrar.Grup;
                    registreModificat.IdGrup = (int)usuariConsultaEsborrar.IdGrup;
                    registreModificat.DataAccio = DateTime.Now;
                    registreModificat.DataRetorn = prestecSeleccionat.DataRetorn;

                    Registre registreCreatEsborrar = await _registresApiClient.PostRegistreAsync(registreModificat);

                    await _prestecsApiClient.DeletePrestecAsync((int)prestecSeleccionat.Id);
                    await _prestecsVM.MostrarPrestecsAsync();
                    break;

                case AccioAConfirmar.FinalitzarPrestec:
                    Dispositiu dispositiuConsultaFinalitzar = await _dispositiusApiClient.GetDispositiuPerIdAsync(prestecSeleccionat.IdDispositiu);

                    Dispositiu dispositiuPrestatFinalitzar = new Dispositiu();
                    dispositiuPrestatFinalitzar.Id = prestecSeleccionat.IdDispositiu;
                    dispositiuPrestatFinalitzar.Nom = dispositiuConsultaFinalitzar.Nom;
                    dispositiuPrestatFinalitzar.IdCategoria = dispositiuConsultaFinalitzar.IdCategoria;
                    dispositiuPrestatFinalitzar.Estat = "Disponible";

                    int confirmarDispositiuPrestatFinalitzar = await _dispositiusApiClient.UpdateDispositiuAsync(dispositiuPrestatFinalitzar);

                    Usuari usuariConsultaFinalitzar = await _usuarisApiClient.GetUsuariPerIdAsync(prestecSeleccionat.IdUsuari);

                    // Crear registre
                    Registre registreFinalitzat = new Registre();
                    registreFinalitzat.IdPrestec = prestecSeleccionat.Id;
                    registreFinalitzat.Accio = "Finalitzat";
                    registreFinalitzat.NomUsuari = $"{usuariConsultaFinalitzar.Nom} {usuariConsultaFinalitzar.Cognom}";
                    registreFinalitzat.IdUsuari = (int)usuariConsultaFinalitzar.Id;
                    registreFinalitzat.NomDispositiu = dispositiuConsultaFinalitzar.Nom;
                    registreFinalitzat.IdDispositiu = (int)dispositiuConsultaFinalitzar.Id;
                    registreFinalitzat.NomGrup = usuariConsultaFinalitzar.Grup;
                    registreFinalitzat.IdGrup = (int)usuariConsultaFinalitzar.IdGrup;
                    registreFinalitzat.DataAccio = DateTime.Now;
                    registreFinalitzat.DataRetorn = prestecSeleccionat.DataRetorn;

                    Registre registreCreatFinalitzar = await _registresApiClient.PostRegistreAsync(registreFinalitzat);

                    await _prestecsApiClient.DeletePrestecAsync((int)prestecSeleccionat.Id);
                    await _prestecsVM.MostrarPrestecsAsync();
                    break;
            }

            EsVisible = Visibility.Collapsed;
        });
    }
}
