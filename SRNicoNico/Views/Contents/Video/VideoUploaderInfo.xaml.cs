using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class VideoUploaderInfo : UserControl {
        public VideoUploaderInfo() {
            InitializeComponent();
        }

        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            NicoNicoOpener.Open(e.Uri.OriginalString);
        }
    }
}
