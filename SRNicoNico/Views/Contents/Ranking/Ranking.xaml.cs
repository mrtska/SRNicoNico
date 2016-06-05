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
using MetroRadiance.UI.Controls;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using mshtml;

namespace SRNicoNico.Views.Contents.Ranking {
    /// <summary>
    /// PlayList.xaml の相互作用ロジック
    /// </summary>
    public partial class Ranking : UserControl {
        public Ranking() {
            InitializeComponent();
        }

        private void browser_Navigating(object sender, NavigatingCancelEventArgs e) {

            if(e.Uri != null) {
                var url = e.Uri.OriginalString;

                if(NicoNicoOpener.GetType(url) != NicoNicoUrlType.Other) {

                    NicoNicoOpener.Open(url);
                    e.Cancel = true;
                }
            }
        }

        private void browser_LoadCompleted(object sender, NavigationEventArgs e) {

            var html = browser.Document as HTMLDocument;
            var a = html.Script as HTMLWindow2;
            var code = @"
document.getElementById('siteHeader').setAttribute('style','position: fixed; display: none;');

document.getElementsByTagName('header')[0].getElementsByClassName('siteHeader')[0].innerHTML = """";
document.getElementsByTagName('footer')[1].innerHTML = """";
document.getElementsByTagName('body')[0].setAttribute('style', 'background: #555555; font-family: Yu Gothic UI, メイリオ, Arial; scrollbar-face-color: #666666; scrollbar-track-color: #3F3F46; scrollbar-arrow-color: #C8C8C8; scrollbar-highlight-color: #3F3F46; scrollbar-3dlight-color: #3F3F46; scrollbar-shadow-color: #3F3F46;');
document.getElementsByClassName('rankingHeader matrixHeader')[0].setAttribute('style', 'background: #333333; padding-top: 14px;');
document.getElementsByTagName('h1')[0].setAttribute('style', 'color: #BBBBBB;');
document.getElementsByTagName('body')[0].setAttribute('oncontextmenu', 'return false;');
";
            a.execScript(code, "JavaScript");
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is RankingViewModel) {

                var vm = (RankingViewModel) DataContext;
                vm.WebBrowser = browser;

            }

        }
    }
}
