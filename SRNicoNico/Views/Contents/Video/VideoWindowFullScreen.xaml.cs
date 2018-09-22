using MetroRadiance.UI.Controls;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace SRNicoNico.Views {
    public partial class VideoWindowFullScreen : MetroWindow {
        public VideoWindowFullScreen() {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e) {

            if(DataContext is VideoViewModel vm) {

                vm.Html5Handler?.ReturnToNormalScreen();
            }
            NicoNicoUtil.ReleasePreventLockworkstation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

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
    }
}
