using PIC.Model;
using PIC.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace PIC.View
{
    /// <summary>
    /// Lógica de interacción para Usuaris.xaml
    /// </summary>
    public partial class Usuaris : UserControl
    {
        Brush colorit = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1fb9f9"));
        Brush transp = new SolidColorBrush(Colors.Transparent);

        private CursosVM cursosVM = new CursosVM();
        private DepartamentsVM departamentsVM = new DepartamentsVM();

        private Usuari usuariSeleccionat = new Usuari();

        public Usuaris()
        {
            InitializeComponent();
            inputArea.Visibility = Visibility.Hidden;
            inputText.Visibility = Visibility.Hidden;

            Loaded += async (s, e) =>
            {
                await ((UsuarisVM)DataContext).LoadUsuarisAsync();
            };
        }

        // MOSTRAR TOTS
        private async void Tots_Click(object sender, RoutedEventArgs e)
        {
            usuarisTitol.Text = "USUARIS: TOTS";
            inputArea.Visibility = Visibility.Hidden;
            inputText.Visibility = Visibility.Hidden;

            var vm = (UsuarisVM)DataContext;
            vm.TipusCercaActual = TipusCerca.Tots;
            await vm.LoadUsuarisAsync();

            labelIdGrup.Background = transp;
            labelId.Background = transp;
        }

        // CERCAR PER ID
        private void PerId_Click(object sender, RoutedEventArgs e)
        {
            usuarisTitol.Text = "USUARIS: PER ID";
            inputArea.Visibility = Visibility.Visible;
            inputText.Visibility = Visibility.Visible;

            var vm = (UsuarisVM)DataContext;
            vm.ClearUsuaris();
            ((UsuarisVM)DataContext).TipusCercaActual = TipusCerca.PerId;

            labelIdGrup.Background = transp;
            labelId.Background = colorit;
        }

        // CERCAR PER CURS
        private void PerCurs_Click(object sender, RoutedEventArgs e)
        {
            usuarisTitol.Text = "USUARIS: PER CURS (ID)";
            inputArea.Visibility = Visibility.Visible;
            inputText.Visibility = Visibility.Visible;

            var vm = (UsuarisVM)DataContext;
            vm.ClearUsuaris();
            ((UsuarisVM)DataContext).TipusCercaActual = TipusCerca.PerCurs;

            labelIdGrup.Background = colorit;
            labelId.Background = transp;
        }

        // CERCAR PER DEPARTAMENT
        private void PerDepartament_Click(object sender, RoutedEventArgs e)
        {
            usuarisTitol.Text = "USUARIS: PER DEPARTAMENT (ID)";
            inputArea.Visibility = Visibility.Visible;
            inputText.Visibility = Visibility.Visible;

            var vm = (UsuarisVM)DataContext;
            vm.ClearUsuaris();
            ((UsuarisVM)DataContext).TipusCercaActual = TipusCerca.PerDepartament;

            labelIdGrup.Background = colorit;
            labelId.Background = transp;
        }

        // FER LA CERCA
        private async void Buscar_Click(object sender, RoutedEventArgs e)
        {
            await ((UsuarisVM)DataContext).BuscarAsync();
        }

        // EDITAR USUARI
        private async void EditarUsuari_Click(object sender, RoutedEventArgs e)
        {
            textNom.Text = "";
            textCognom.Text = "";

            usuariSeleccionat = llistaUsuaris.SelectedItem as Usuari;

            //SI NO HI HA CAP USUARI SEL·LECCIONAT
            if (usuariSeleccionat == null)
            {
                MessageBox.Show("Escull un usuari");
            }

            //SI HI HA UN USUARI SEL·LECCIONAT
            else
            {
                OverlayEditar.Visibility = Visibility.Visible;

                if (usuariSeleccionat.Tipus == "Alumne")
                {
                    cbGrups.ItemsSource = null;
                    await cursosVM.LoadCursosAsync();
                    cbGrups.ItemsSource = cursosVM.Cursos;

                    cbGrups.SelectedItem = cursosVM.Cursos.FirstOrDefault(c => c.Id == usuariSeleccionat.IdGrup);
                }

                else if (usuariSeleccionat.Tipus == "Professor")
                {
                    cbGrups.ItemsSource = null;
                    await departamentsVM.LoadDepartamentsAsync();
                    cbGrups.ItemsSource = departamentsVM.Departaments;

                    cbGrups.SelectedItem = departamentsVM.Departaments.FirstOrDefault(c => c.Id == usuariSeleccionat.IdGrup);

                }

                textNom.Text = usuariSeleccionat.Nom;
                textCognom.Text = usuariSeleccionat.Cognom;

                // ACTUALITZAR USUARI AQUI
            }
        }

        // ESBORRAR USUARI
        private void EsborrarUsuari_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TancarOverlay_Click(object sender, RoutedEventArgs e)
        {
            OverlayEditar.Visibility = Visibility.Collapsed;
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            usuariSeleccionat.Nom = textNom.Text.Trim();
            usuariSeleccionat.Cognom = textCognom.Text.Trim();

            // Actualitzar l'usuari via la ViewModel
            try
            {
                await((UsuarisVM)DataContext).ActualitzarUsuariAsync(usuariSeleccionat);
                OverlayEditar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error actualitzant l'usuari: {ex.Message}");
            }

            OverlayEditar.Visibility = Visibility.Collapsed;
        }
    }
}
