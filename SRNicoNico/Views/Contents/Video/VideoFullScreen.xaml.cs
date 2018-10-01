using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class VideoFullScreen : Window {
        public VideoFullScreen() {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e) {

            if(DataContext is VideoViewModel vm) {

                vm.Html5Handler?.ReturnToNormalScreen();
            }
            NicoNicoUtil.ReleasePreventLockworkstation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            WindowState = WindowState.Maximized;
            NicoNicoUtil.PreventLockWorkstation();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {

            if (DataContext is VideoViewModel vm) {
                if (e.Key == Key.Escape) {

                    vm.Html5Handler?.ReturnToNormalScreen();
                    return;
                }
                vm.KeyDown(e);
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e) {

            if (DataContext is VideoViewModel vm) {

                vm.KeyUp(e);
            }
        }

        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            var url = e.Uri.OriginalString;

            if (DataContext is VideoViewModel vm) {

                if (url.StartsWith("#")) {

                    var time = url.Substring(1);

                    vm?.Html5Handler?.Seek(NicoNicoUtil.ConvertTime(time));
                } else {

                    //URLを書き換えて再読込
                    vm.Refresh(e.Uri.OriginalString);
                }
            }
        }

        // 動画用のツールチップを設定
        private void Hyperlink_Loaded(object sender, RoutedEventArgs e) {

            var link = (Hyperlink)sender;
            if (DataContext is VideoViewModel vm) {

                var uri = link.NavigateUri.OriginalString;

                if (NicoNicoOpener.GetType(uri) == NicoNicoUrlType.Video) {

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

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        private static IntPtr GetHwnd(Popup popup) {
            var source = (HwndSource)PresentationSource.FromVisual(popup.Child);
            return source.Handle;
        }
        private void comment_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            SetFocus(GetHwnd(commentPopup));
        }
    }
}
