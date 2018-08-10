using SRNicoNico.Models.NicoNicoViewer;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace SRNicoNico.Themes {
    public partial class Hyperlink : ResourceDictionary {
        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            NicoNicoOpener.Open(e.Uri.OriginalString);
        }

        public void InitializeToolTip(object sender, RoutedEventArgs e) {

            var link = sender as System.Windows.Documents.Hyperlink;
            if (link.Inlines.First() is Run inline) {

                var text = link.NavigateUri.OriginalString;
                link.ToolTip = text;
            }
        }
    }
}
