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
    public partial class Usuaris : UserControl
    {
        public Usuaris()
        {
            InitializeComponent();
            // No cal fer res més aquí. 
            // El VM ja s'encarrega de carregar les dades inicials si el crides en el constructor del VM.
        }

        // OPCIONAL: Si vols mantenir la càrrega inicial quan el control es carrega
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is UsuarisVM vm)
            {
                await vm.MostrarUsuarisAsync();
            }
        }
    }
}
