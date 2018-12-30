using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class VideoDescription : UserControl {
        public VideoDescription() {
            InitializeComponent();
        }
        private void Hyperlink_MouseDown(object sender, MouseButtonEventArgs e) {

            var link = (Hyperlink)sender;
            if (e.MiddleButton == MouseButtonState.Pressed && DataContext is VideoViewModel vm && link.NavigateUri != null) {

                vm?.Html5Handler?.Pause();
                NicoNicoOpener.Open(link.NavigateUri.OriginalString);
            }
        }
        public void OpenHyperLink(object sender, RoutedEventArgs e) {

            var link = (Hyperlink)sender;
            var url = "";

            if (link.NavigateUri != null) {

                url = link.NavigateUri.OriginalString;
            } else {

                url = ((Run)link.Inlines.FirstInline).Text;
            }
            if (DataContext is VideoViewModel vm) {

                if (url.StartsWith("#")) {

                    var time = url.Substring(1);

                    vm?.Html5Handler?.Seek(NicoNicoUtil.ConvertTime(time));
                } else {

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || NicoNicoOpener.GetType(url) != NicoNicoUrlType.Video) {

                        vm?.Html5Handler?.Pause();
                        NicoNicoOpener.Open(url);
                    } else {

                        //URLを書き換えて再読込
                        vm.Refresh(url);
                    }
                }
            }
        }

        // 動画用のツールチップを設定
        private void Hyperlink_Loaded(object sender, RoutedEventArgs e) {

            var link = (Hyperlink)sender;
            if(DataContext is VideoViewModel vm && link.NavigateUri != null) {

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
