using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class PrivacyPolicy : UserControl {
        public PrivacyPolicy() {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {

            Process.Start(e.Uri.OriginalString);
            e.Handled = true;
        }
    }
}
