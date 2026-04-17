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
        public Cursos()
        {
            InitializeComponent();
            // No cal fer res més aquí. 
            // El VM ja s'encarrega de carregar les dades inicials si el crides en el constructor del VM.
        }

        // OPCIONAL: Si vols mantenir la càrrega inicial quan el control es carrega
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CursosVM vm)
            {
                //await vm.MostrarUsuarisAsync();
            }
        }
    }
}
