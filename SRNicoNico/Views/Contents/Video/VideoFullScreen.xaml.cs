using System.Windows;
using System.Windows.Input;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views {
    public partial class VideoFullScreen : Window {
        public VideoFullScreen() {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {

            if (DataContext is VideoViewModel vm) {
                vm.KeyDown(e);
            }
        }
    }
}
