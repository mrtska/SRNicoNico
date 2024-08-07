using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views {
    public partial class VideoDescription : UserControl {

        private VideoViewModel ViewModel => (VideoViewModel)DataContext;

        public VideoDescription() {
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

        private void InitializeHyperlink(object sender, RoutedEventArgs e) {

            var hyperlink = (Hyperlink)sender;

            hyperlink.ToolTip = new TextBlock { Text = hyperlink.NavigateUri?.OriginalString ?? "" };
            hyperlink.ToolTipOpening += OnToolTipOpening;
        }

        private async void OnToolTipOpening(object sender, ToolTipEventArgs e) {

            var hyperlink = (Hyperlink)sender;
            var url = hyperlink.NavigateUri?.OriginalString ?? "";

            if (ViewModel.NicoNicoViewer.DetectUrlType(url) == NicoNicoUrlType.Video) {

                // 動画情報をレイジーロードする
                if (hyperlink.Tag == null) {

                    hyperlink.Tag = await ViewModel.VideoService.WatchAsync(url[31..].Split('?')[0], true);
                    hyperlink.ToolTip = new VideoToolTip { DataContext = ((WatchApiData)hyperlink.Tag).Video };
                }
            }
        }
    }
}
