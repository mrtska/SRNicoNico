using MetroRadiance.UI.Controls;
using SRNicoNico.ViewModels;
using System.Windows.Input;

namespace SRNicoNico.Views {
    public partial class MainWindow : MetroWindow {
        public MainWindow() {
            InitializeComponent();
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e) {

            if (DataContext is MainWindowViewModel vm) {
                vm.KeyDown(e);
            }
        }

        private void MetroWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) {

            if (DataContext is MainWindowViewModel vm) {
                vm.MouseDown(e);
            }
        }

        private void MetroWindow_PreviewKeyUp(object sender, KeyEventArgs e) {

            var vm = (MainWindowViewModel)DataContext;
            if (DataContext is MainWindowViewModel) {

                vm.KeyUp(e);
            }
        }
    }
}
