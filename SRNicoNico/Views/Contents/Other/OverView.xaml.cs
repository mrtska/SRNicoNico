using System.Windows.Controls;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class OverView : UserControl {
        public OverView() {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {

            System.Diagnostics.Process.Start(e.Uri.OriginalString);
        }
    }
}
