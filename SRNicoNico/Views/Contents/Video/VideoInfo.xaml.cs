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

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoWrapper;
using Livet;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoInfo : UserControl {
        public VideoInfo() {
            InitializeComponent();
        }

        //投稿者を開く
        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            var vm = DataContext as VideoViewModel;

            if(vm != null) {

                if(vm.VideoData.ApiData.IsChannelVideo) {

                    NicoNicoOpener.Open("http://ch.nicovideo.jp/channel/ch" + e.Uri.OriginalString);
                } else {

                    NicoNicoOpener.Open("http://www.nicovideo.jp/user/" + e.Uri.OriginalString);
                }
            }


        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {

            var vm = ((MenuItem)sender).DataContext as CommentEntryViewModel;
            var time = NicoNicoUtil.ConvertTime(vm.Entry.RenderTime);

            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => vm.Owner.Seek(time)));
            
        }

        private void NGComment_Click(object sender, RoutedEventArgs e) {

            var vm = ((MenuItem)sender).DataContext as CommentEntryViewModel;
            vm.Owner.Comment.RegisterNGComment(vm.Entry);
        }

        private void NGUser_Click(object sender, RoutedEventArgs e) {


            var vm = ((MenuItem)sender).DataContext as CommentEntryViewModel;
            vm.Owner.Comment.RegisterNGUser(vm.Entry);
        }
    }
}
