using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class VideoInformation : UserControl {
        public VideoInformation() {
            InitializeComponent();
        }

        private void ManagedPopup_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {

            e.Handled = true;
        }

        private void ManagedPopup_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            e.Handled = true;

        }
    }
}
