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
using MetroRadiance.UI.Controls;
using SRNicoNico.ViewModels;
using System.Windows.Controls.Primitives;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Views {
    /// <summary>
    /// WindowFullScreenView.xaml の相互作用ロジック
    /// </summary>
    public partial class WindowFullScreenView : MetroWindow {
        public WindowFullScreenView() {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {

            if(DataContext is VideoViewModel) {

                var vm = (VideoViewModel)DataContext;
                if(vm.Comment.Post.IsCommentPopupOpen) {

                    App.ViewModelRoot.KeyDown(e);
                } else {

                    if (e.Key == Key.Escape) {

                        Close();
                        return;
                    }
                    App.ViewModelRoot.KeyDown(e);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e) {

            if(DataContext is VideoViewModel) {
                var vm = (VideoViewModel)DataContext;
                var temp = vm.FullScreenWebBrowser;
                vm.FullScreenWebBrowser = null;
                vm.WebBrowser = temp;

                var temp2 = vm.FullScreenController;
                vm.FullScreenController = null;
                vm.Controller = temp2;

                vm.IsFullScreen = false;

                if(vm.IsPlayList()) {

                    GetWindow(temp).Visibility = Visibility.Visible;
                } else {

                    //VideoViewから削除
                    App.ViewModelRoot.MainContent.AddVideoView(vm);
                }

            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e) {

            if (DataContext is VideoViewModel) {
                var vm = (VideoViewModel)DataContext;
                vm.KeyUp(e);
            }
        }

        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            var url = e.Uri.OriginalString;

            if (DataContext is VideoViewModel) {
                var vm = (VideoViewModel)DataContext;
                if (url.StartsWith("#")) {

                    vm.Handler.Seek(NicoNicoUtil.ConvertTime((url.Substring(1))));
                } else {

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || NicoNicoOpener.GetType(e.Uri.OriginalString) != NicoNicoUrlType.Video) {

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

            var inline = link.Inlines.First() as Run;
            if(inline != null) {

                var uri = link.NavigateUri;
                //#○○:×× リンクだとnullになるので
                if(uri == null) {

                    var time = inline.Text;

                    if(time.StartsWith("#")) {

                        link.NavigateUri = new Uri(time, UriKind.Relative);
                    }

                    return;
                }
                var text = uri.OriginalString;
                var type = NicoNicoOpener.GetType(text);

                if(type == NicoNicoUrlType.Video) {


                    link.ToolTip = text;
                } else {

                    link.ToolTip = text;
                }
            }
        }
    }
}
