using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Contents.PublicMylist {
    /// <summary>
    /// PublicMylist.xaml の相互作用ロジック
    /// </summary>
    public partial class PublicMylist : UserControl {
        public PublicMylist() {
            InitializeComponent();
        }


        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            NicoNicoOpener.Open(e.Uri.OriginalString);
        }

        public void InitializeToolTip(object sender, RoutedEventArgs e) {

            var link = sender as Hyperlink;
            var inline = link.Inlines.First() as Run;
            if(inline != null) {

                var text = link.NavigateUri.OriginalString;
                link.ToolTip = text;
            }
        }
    }
}
