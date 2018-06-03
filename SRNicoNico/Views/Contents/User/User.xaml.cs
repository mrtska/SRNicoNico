using SRNicoNico.Models.NicoNicoViewer;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class User : UserControl {
        public User() {
            InitializeComponent();
        }

        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            NicoNicoOpener.Open(e.Uri.OriginalString);
        }

        public void InitializeToolTip(object sender, RoutedEventArgs e) {

            var link = sender as Hyperlink;
            if (link.Inlines.First() is Run inline) {

                var text = link.NavigateUri.OriginalString;
                link.ToolTip = text;
            }
        }
    }
}
