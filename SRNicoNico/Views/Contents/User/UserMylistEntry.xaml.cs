using SRNicoNico.ViewModels;
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

using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Views.Contents.User {
    /// <summary>
    /// NicoRepoResultEntry.xaml の相互作用ロジック
    /// </summary>
    public partial class UserMylistEntry : UserControl {
        public UserMylistEntry() {
            InitializeComponent();
        }


        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            var text = e.Uri.OriginalString;
            if(text.StartsWith("mylist/")) {

                text = "http://www.nicovideo.jp/" + text;
            }
            NicoNicoOpener.Open(text);
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
