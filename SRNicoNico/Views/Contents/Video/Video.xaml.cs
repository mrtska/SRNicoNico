using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using SRNicoNico.Models;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views {
    public partial class Video : UserControl {

        private VideoViewModel ViewModel => (VideoViewModel)DataContext;

        public Video() {
            InitializeComponent();
        }

        public void OpenHyperlink(object sender, RoutedEventArgs e) {

            var hyperlink = (Hyperlink)sender;
            var url = hyperlink.NavigateUri.OriginalString;

            if (ViewModel.NicoNicoViewer.DetectUrlType(url) == NicoNicoUrlType.Video) {

                ViewModel.NavigateTo(url[31..].Split('?')[0]);
            } else {

                // 別のタブで開くので動画を一時停止する濫觴
                ViewModel.Pause();
                ViewModel.NicoNicoViewer.OpenUrl(hyperlink.NavigateUri.OriginalString);
            }
        }
    }
}
