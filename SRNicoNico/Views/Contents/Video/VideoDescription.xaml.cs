using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SRNicoNico.Views {
    public partial class VideoDescription : UserControl {
        public VideoDescription() {
            InitializeComponent();
        }

        // 動画用のツールチップを設定
        private void Hyperlink_Loaded(object sender, RoutedEventArgs e) {

            var link = (Hyperlink)sender;
            if(DataContext is VideoViewModel vm) {

                var uri = link.NavigateUri.OriginalString;

                if(NicoNicoOpener.GetType(uri) == NicoNicoUrlType.Video) {

                    var id = Regex.Match(uri, "watch/(.+)").Groups[1].Value;
                    var newvm = new VideoPopupViewModel(id, vm.Model.ApiData.PlaylistToken);
                    var tip = new VideoPopup() {
                        DataContext = newvm
                    };

                    link.ToolTip = tip;
                } else {

                    link.ToolTip = uri;
                }
            }
        }
    }
}
