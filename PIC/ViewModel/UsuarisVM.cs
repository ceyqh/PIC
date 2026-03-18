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
        private readonly UsuarisApiClient _apiClient;
        public UsuarisVM()
        {
            _apiClient = new UsuarisApiClient();
            Usuaris = new ObservableCollection<Usuari>();
            TipusCercaActual = TipusCerca.Tots;
        }

        // CARREGAR TOTS ELS USUARIS
        public async Task LoadUsuarisAsync()
        {
            try
            {
                var llista = await _apiClient.GetAllUsuarisAsync();
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
        public async Task AfegirUsuariAsync(Usuari nouUsuari)
        {
            try
            {
                var usuariCreat = await _apiClient.PostUsuariAsync(nouUsuari);
                if (usuariCreat != null)
                {
                    Usuaris.Add(usuariCreat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear l'usuari: {ex.Message}");
            }
        }

        // ACTUALITZAR USUARI
        public async Task ActualitzarUsuariAsync(Usuari usuariAModificar)
        {
            try
            {
                int result = await _apiClient.UpdateUsuariAsync(usuariAModificar);
                if (result > 0)
                {
                    MessageBox.Show("Usuari actualitzat correctament!");
                    
                    // refrescar la llista
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
                    var usuari = await _apiClient.GetUsuariPerIdAsync(int.Parse(ParametreCerca));
                    Usuaris.Add(usuari);
                    break;

                case TipusCerca.PerCurs:
                    var curs = await _apiClient.GetUsuarisPerIdCursAsync(int.Parse(ParametreCerca));
                    foreach (var u in curs)
                        Usuaris.Add(u);
                    break;

                case TipusCerca.PerDepartament:
                    var dep = await _apiClient.GetUsuarisPerIdDepartamentAsync(int.Parse(ParametreCerca));
                    foreach (var u in dep)
                        Usuaris.Add(u);
                    break;
            }
        }

        // BUIDAR LIST VIEW
        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
