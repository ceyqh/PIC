using PIC.Model;
using PIC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PIC.View
{
    public partial class Cursos : UserControl
    {
        private Curs cursSeleccionat = new Curs();

        public Cursos()
        {
            InitializeComponent();
            inputArea.Visibility = Visibility.Hidden;
            inputBoto.Visibility = Visibility.Hidden;

            CarregarCursos();
        }

        public async void CarregarCursos()
        {
            var vm = (CursosVM)DataContext;
            await vm.MostrarCursosAsync();
        }

        // MOSTRAR TOTS
        private async void Tots_Click(object sender, RoutedEventArgs e)
        {
            usuarisTitol.Text = "CURSOS: TOTS";
            inputArea.Visibility = Visibility.Hidden;
            inputBoto.Visibility = Visibility.Hidden;

            var vm = (CursosVM)DataContext;
            await vm.MostrarCursosAsync();

            //labelIdCurs.Background = colorTransparent;
            //labelIdDepartament.Background = colorTransparent;
            //labelId.Background = colorTransparent;
        }

        // CERCAR PER ID
        private void PerId_Click(object sender, RoutedEventArgs e)
        {
            usuarisTitol.Text = "CURSOS: PER ID";
            inputArea.Visibility = Visibility.Visible;
            inputBoto.Visibility = Visibility.Visible;

            var vm = (CursosVM)DataContext;
            vm.ClearCursos();
            vm.ClearUsuaris();
            ((CursosVM)DataContext).TipusCercaActual = CursosTipusCerca.PerId;

            //labelIdCurs.Background = colorTransparent;
            //labelIdDepartament.Background = colorTransparent;
            //labelId.Background = colorAplicat;
        }

        // FER LA CERCA
        private async void Buscar_Click(object sender, RoutedEventArgs e)
        {
            await ((CursosVM)DataContext).CercaCursosAsync();
        }

        // OBRIR MENU AFEGIR CURS
        private void AfegirCursMenu_Click(object sender, RoutedEventArgs e)
        {
            aa_textNom.Text = "";
            OverlayAfegirCurs.Visibility = Visibility.Visible;
        }

        // GUARDAR EL NOU CURS
        private async void AfegirCurs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Curs nouCurs = new Curs();
                nouCurs.Nom = aa_textNom.Text;

                var vm = (CursosVM)DataContext;
                Curs cursCreat = await vm.AfegirCursAsync(nouCurs);

                //MessageBox.Show(cursCreat.Nom);

                OverlayAfegirCurs.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // ESBORRAR CURS
        private async void EsborrarCursMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cursSeleccionat = llistaCursos.SelectedItem as Curs;

                if (cursSeleccionat != null)
                {
                    int usuariId = (int)cursSeleccionat.Id;
                    var vm = (CursosVM)DataContext;
                    await vm.EsborrarCursAsync(usuariId);
                }
                else
                {
                    MessageBox.Show("Escull un usuari.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // TANCAR MENÚS
        private void TancarOverlay_Click(object sender, RoutedEventArgs e)
        {
            OverlayAfegirCurs.Visibility = Visibility.Collapsed;
        }        
    }
}
