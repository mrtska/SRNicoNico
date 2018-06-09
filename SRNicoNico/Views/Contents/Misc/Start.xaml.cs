using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class Start : UserControl {

        public Start() {
            InitializeComponent();

        }

        private void browser_Navigating(object sender, NavigatingCancelEventArgs e) {

            if(e.Uri.OriginalString != "https://mrtska.net/niconicoviewer/releasenote") {

                Process.Start(e.Uri.OriginalString);
                e.Cancel = true;
            }
        }
    }
}
