using PIC.APIClient;
using PIC.Model;
using PIC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIC.ViewModel
{
    public enum CursosTipusCerca
    {
        PerId,
        UsuarisPerCurs
    }

    internal class CursosVM : Utilities.ViewModelBase
    {
        public ObservableCollection<Curs> Cursos { get; set; }
        public ObservableCollection<Usuari> Usuaris { get; set; }

        // CURS SELECCIONAT
        private Curs _cursSeleccionat;
        public Curs CursSeleccionat
        {
            get => _cursSeleccionat;
            set
            {
                if (_cursSeleccionat == value) return;
                if (value == null) return;

                _cursSeleccionat = value;
                OnPropertyChanged();

                int idCurs = (int)_cursSeleccionat.Id;
                CarregarUsuarisPerCurs(idCurs);
            }
        }

        // TIPUS DE CERCA
        private CursosTipusCerca _tipusCercaActual;
        public CursosTipusCerca TipusCercaActual
        {
            get => _tipusCercaActual;
            set
            {
                _tipusCercaActual = value;
                OnPropertyChanged();
            }
        }

        // PARAMETRE DE CERCA DEL CAMP DE TEXT
        private int _parametreCerca;
        public int ParametreCerca
        {
            get => _parametreCerca;
            set
            {
                _parametreCerca = value;
                OnPropertyChanged();
            }
        }

        // EXECUTAR CERCA
        public async Task CercaCursosAsync()
        {
            Cursos.Clear();

            switch (TipusCercaActual)
            {
                case CursosTipusCerca.PerId:
                    var curs = await _cursosApiClient.GetCursPerIdAsync(ParametreCerca);
                    Cursos.Add(curs);
                    break;
            }
        }

        // CONSTRUCTOR
        private readonly CursosApiClient _cursosApiClient;
        private readonly UsuarisApiClient _usuarisApiClient;

        public CursosVM()
        {
            _cursosApiClient = new CursosApiClient();
            _usuarisApiClient = new UsuarisApiClient();

            Cursos = new ObservableCollection<Curs>();
            Usuaris = new ObservableCollection<Usuari>();
        }

        // MOSTRAR TOTS ELS CURSOS
        public async Task MostrarCursosAsync()
        {
            var llista = await _cursosApiClient.GetAllCursosAsync();
            Cursos.Clear();

            foreach (var u in llista)
            {
                Cursos.Add(u);
            }
        }

        // CARREGAR ELS USUARIS D'UN CURS
        public async void CarregarUsuarisPerCurs(int idCurs)
        {
            await MostrarUsuarisPerCurs(idCurs);
        }

        // MOSTRAR ELS USUARIS PER CURS SEL·LECCIONAT
        public async Task MostrarUsuarisPerCurs(int idCurs)
        {
            Usuaris.Clear();

            var usuaris = await _usuarisApiClient.GetUsuarisPerIdCursAsync(idCurs);

            foreach (var u in usuaris)
            {
                Usuaris.Add(u);
            }
        }

        // AFEGIR CURS
        public async Task<Curs> AfegirCursAsync(Curs cursAAfegir)
        {
            try
            {
                Curs result = await _cursosApiClient.PostCursAsync(cursAAfegir);

                if (result != null)
                {
                    var curs = new Curs
                    {
                        Id = result.Id,
                        Nom = result.Nom
                    };

                    await MostrarCursosAsync();

                    return curs;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }

        // ESBORRAR CURS
        public async Task EsborrarCursAsync(int id)
        {
            try
            {
                int result = await _cursosApiClient.DeleteCursAsync(id);
                if (result <= 0)
                {
                    await MostrarCursosAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // BUIDAR LIST VIEW
        public void ClearCursos()
        {
            Cursos.Clear();
        }

        public void ClearUsuaris()
        {
            Usuaris.Clear();
        }
    }
}
