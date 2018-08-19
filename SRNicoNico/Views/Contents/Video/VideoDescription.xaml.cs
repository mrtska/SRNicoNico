using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;
using System;
using System.Linq;
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

        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            var url = e.Uri.OriginalString;

            if (DataContext is VideoViewModel vm) {

                if (url.StartsWith("#")) {

                    var time = url.Substring(1);

                    vm?.Handler?.Seek(NicoNicoUtil.ConvertTime((time)));
                } else {


                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || NicoNicoOpener.GetType(e.Uri.OriginalString) != NicoNicoUrlType.Video || vm.IsPlayList()) {

                        vm?.Handler?.Pause();
                        NicoNicoOpener.Open(e.Uri.OriginalString);
                    } else {

                        //URLを書き換えて再読込
                        vm.VideoUrl = e.Uri.OriginalString;
                        vm.Initialize();
                    }
                }
            }
        }

        public void InitializeToolTip(object sender, RoutedEventArgs e) {

            var link = sender as Hyperlink;

            //すでにツールチップがあったらスキップ
            if(link.ToolTip != null) {

                return;
            }

            if (link.Inlines.First() is Run inline) {

                var uri = link.NavigateUri;
                //#○○:×× リンクだとnullになるので
                if (uri == null) {

                    var time = inline.Text;

                    if (time.StartsWith("#")) {

                        link.NavigateUri = new Uri(time, UriKind.Relative);
                    }

                    return;
                }
                var text = uri.OriginalString;
                var type = NicoNicoOpener.GetType(text);

                if (type == NicoNicoUrlType.Video) {

                    link.ToolTip = text;
                } else {

                    link.ToolTip = text;
                }
            }
        }
    }
}
