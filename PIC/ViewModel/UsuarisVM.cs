using PIC.APIClient;
using PIC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIC.ViewModel
{
    public enum TipusCerca
    {
        Tots,
        PerId,
        PerCurs,
        PerDepartament
    }

    internal class UsuarisVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Usuari> Usuaris { get; set; }

        // TIPUS DE CERCA
        private TipusCerca _tipusCercaActual;
        public TipusCerca TipusCercaActual
        {
            get => _tipusCercaActual;
            set
            {
                _tipusCercaActual = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private string _parametreCerca;
        public string ParametreCerca
        {
            get => _parametreCerca;
            set
            {
                _parametreCerca = value;
                OnPropertyChanged();
            }
        }

        // CONSTRUCTOR
        private readonly UsuarisApiClient _usuarisApiClient;
        private readonly AlumnesApiClient _alumnesApiClient;
        private readonly ProfessorsApiClient _professorsApiClient;

        public UsuarisVM()
        {
            _usuarisApiClient = new UsuarisApiClient();
            _alumnesApiClient = new AlumnesApiClient();
            _professorsApiClient = new ProfessorsApiClient();

            Usuaris = new ObservableCollection<Usuari>();
            TipusCercaActual = TipusCerca.Tots;
        }

        // CARREGAR TOTS ELS USUARIS
        public async Task LoadUsuarisAsync()
        {
            try
            {
                var llista = await _usuarisApiClient.GetAllUsuarisAsync();
                Usuaris.Clear();

                foreach (var u in llista)
                {
                    Usuaris.Add(u);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear l'usuari: {ex.Message}");
            }
        }

        // AFEGIR USUARI
        public async Task<Usuari> AfegirUsuariAsync(NouUsuari usuariAAfegir)
        {
            try
            {
                NouUsuari result = await _usuarisApiClient.PostUsuariAsync(usuariAAfegir);

                if (result != null)
                {
                    var usuari = new Usuari
                    {
                        Id = result.Id,
                        Nom = result.Nom,
                        Cognom = result.Cognom
                    };

                    await LoadUsuarisAsync();

                    return usuari;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error usuari: {ex.Message}");
            }

            return null;
        }

        // ACTUALITZAR USUARI
        public async Task ActualitzarUsuariAsync(Usuari usuariAModificar)
        {
            try
            {
                int result = await _usuarisApiClient.UpdateUsuariAsync(usuariAModificar);
                if (result > 0)
                {
                    //MessageBox.Show("Usuari actualitzat correctament!");
                    await LoadUsuarisAsync();
                }
                else
                {
                    MessageBox.Show("No s'ha pogut actualitzar l'usuari.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // EXECUTAR CERCA
        public async Task BuscarAsync()
        {
            Usuaris.Clear();

            switch (TipusCercaActual)
            {
                case TipusCerca.PerId:
                    var usuari = await _usuarisApiClient.GetUsuariPerIdAsync(int.Parse(ParametreCerca));
                    Usuaris.Add(usuari);
                    break;

                case TipusCerca.PerCurs:
                    var curs = await _usuarisApiClient.GetUsuarisPerIdCursAsync(int.Parse(ParametreCerca));
                    foreach (var u in curs)
                        Usuaris.Add(u);
                    break;

                case TipusCerca.PerDepartament:
                    var dep = await _usuarisApiClient.GetUsuarisPerIdDepartamentAsync(int.Parse(ParametreCerca));
                    foreach (var u in dep)
                        Usuaris.Add(u);
                    break;
            }
        }

        // AFEGIR ALUMNE
        public async Task AfegirAlumneAsync(Alumne alumneAAfegir)
        {
            try
            {
                Alumne result = await _alumnesApiClient.PostAlumneAsync(alumneAAfegir);
                if (result != null)
                {
                    await LoadUsuarisAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Alumne: {ex.Message}");
            }
        }

        // ACTUALITZAR ALUMNE
        public async Task ActualitzarAlumneAsync(Alumne alumneAModificar)
        {
            try
            {
                int result = await _alumnesApiClient.UpdateAlumneAsync(alumneAModificar);
                if (result <= 0)
                {
                    //MessageBox.Show("No s'ha pogut actualitzar l'alumne.");
                    await LoadUsuarisAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Alumne: {ex.Message}");
            }
        }

        // ACTUALITZAR PROFESSOR
        public async Task ActualitzarProfessorAsync(Professor professorAModificar)
        {
            try
            {
                int result = await _professorsApiClient.UpdateProfessorAsync(professorAModificar);
                if (result <= 0)
                {
                    //MessageBox.Show("No s'ha pogut actualitzar el professor.");
                    await LoadUsuarisAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Alumne: {ex.Message}");
            }
        }

        // BUIDAR LIST VIEW
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
