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

            labelIdCurs.Background = transp;
            labelIdDepartament.Background = transp;
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

            labelIdCurs.Background = transp;
            labelIdDepartament.Background = transp;
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

            labelIdCurs.Background = colorit;
            labelIdDepartament.Background = transp;
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

            labelIdCurs.Background = transp;
            labelIdDepartament.Background = colorit;
            labelId.Background = transp;
        }

        // FER LA CERCA
        private async void Buscar_Click(object sender, RoutedEventArgs e)
        {
            await ((UsuarisVM)DataContext).BuscarAsync();
        }

        // EDITAR USUARI
        private async void EditarUsuariMenu_Click(object sender, RoutedEventArgs e)
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

                    cbGrups.SelectedValue = usuariSeleccionat.IdGrup;
                }

                else if (usuariSeleccionat.Tipus == "Professor")
                {
                    cbGrups.ItemsSource = null;
                    await departamentsVM.LoadDepartamentsAsync();
                    cbGrups.ItemsSource = departamentsVM.Departaments;

                    cbGrups.SelectedValue = usuariSeleccionat.IdGrup;

                }

                textNom.Text = usuariSeleccionat.Nom;
                textCognom.Text = usuariSeleccionat.Cognom;

                // ACTUALITZAR USUARI AQUI
            }
        }

        // ESBORRAR USUARI
        private void EsborrarUsuariMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TancarOverlay_Click(object sender, RoutedEventArgs e)
        {
            OverlayEditar.Visibility = Visibility.Collapsed;
            OverlayAfegirAlumne.Visibility = Visibility.Collapsed;
            OverlayAfegirProfessor.Visibility = Visibility.Collapsed;
        }

        // GUARDAR CANVIS USUARI
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            usuariSeleccionat.Nom = textNom.Text.Trim();
            usuariSeleccionat.Cognom = textCognom.Text.Trim();
            usuariSeleccionat.IdGrup = (long)cbGrups.SelectedValue;

            try
            {
                var vm = (UsuarisVM)DataContext;

                if (usuariSeleccionat.Tipus == "Alumne")
                {
                    Alumne alumneDades = new Alumne
                    {
                        IdUsuari = (int)usuariSeleccionat.Id,
                        IdCurs = (int)usuariSeleccionat.IdGrup
                    };

                    await vm.ActualitzarAlumneAsync(alumneDades);
                }

                if (usuariSeleccionat.Tipus == "Professor")
                {
                    Professor professorDades = new Professor
                    {
                        IdUsuari = (int)usuariSeleccionat.Id,
                        IdDepartament = (int)usuariSeleccionat.IdGrup
                    };

                    await vm.ActualitzarProfessorAsync(professorDades);
                }

                await vm.ActualitzarUsuariAsync(usuariSeleccionat);

                OverlayEditar.Visibility = Visibility.Collapsed;
                MessageBox.Show("Usuari i curs actualitzats correctament.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el procés de guardat: {ex.Message}");
            }
        }

        private async void AfegirAlumneMenu_Click(object sender, RoutedEventArgs e)
        {
            aa_textNom.Text = "";
            aa_textCognom.Text = "";

            OverlayAfegirAlumne.Visibility = Visibility.Visible;

            cbCursos.ItemsSource = null;
            await cursosVM.LoadCursosAsync();
            cbCursos.ItemsSource = cursosVM.Cursos;

            if (cbCursos.ItemsSource != null)
            {
                cbCursos.SelectedIndex = 0;
            }
        }

        private async void AfegirProfessorMenu_Click(object sender, RoutedEventArgs e)
        {
            ap_textNom.Text = "";
            ap_textCognom.Text = "";

            OverlayAfegirProfessor.Visibility = Visibility.Visible;

            cbDepartaments.ItemsSource = null;
            await departamentsVM.LoadDepartamentsAsync();
            cbDepartaments.ItemsSource = departamentsVM.Departaments;

            if (cbDepartaments.ItemsSource != null)
            {
                cbDepartaments.SelectedIndex = 0;
            }
        }

        private async void AfegirAlumne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NouUsuari nouUsuari = new NouUsuari
                {
                    Nom = aa_textNom.Text,
                    Cognom = aa_textCognom.Text

                };

                var vm = (UsuarisVM)DataContext;
                Usuari usuariCreat = await vm.AfegirUsuariAsync(nouUsuari);

                //MessageBox.Show(usuariCreat.Id + " " + cbCursos.SelectedValue);

                if (usuariCreat != null)
                {
                    Alumne nouAlumne = new Alumne
                    {
                        IdUsuari = usuariCreat.Id,
                        IdCurs = (long)cbCursos.SelectedValue
                    };

                    await vm.AfegirAlumneAsync(nouAlumne);
                }

                OverlayAfegirAlumne.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void AfegirProfessor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NouUsuari nouUsuari = new NouUsuari
                {
                    Nom = ap_textNom.Text,
                    Cognom = ap_textCognom.Text

                };

                var vm = (UsuarisVM)DataContext;
                Usuari usuariCreat = await vm.AfegirUsuariAsync(nouUsuari);

                //MessageBox.Show(usuariCreat.Id + " " + cbCursos.SelectedValue);

                if (usuariCreat != null)
                {
                    Professor nouProfessor = new Professor
                    {
                        IdUsuari = usuariCreat.Id,
                        IdDepartament = (long)cbDepartaments.SelectedValue
                    };

                    await vm.AfegirProfessorAsync(nouProfessor);
                }

                OverlayAfegirProfessor.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
