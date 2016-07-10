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

namespace SRNicoNico.Views.Contents.WebView {
    /// <summary>
    /// WebView.xaml の相互作用ロジック
    /// </summary>
    public partial class WebView : UserControl {
        public WebView() {
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


            if(DataContext is WebViewViewModel) {

                var vm = (WebViewViewModel)DataContext;
                vm.LoadCompleted();

            }

        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is WebViewViewModel) {

                var vm = (WebViewViewModel) DataContext;
                vm.WebBrowser = browser;

            }

        }
    }
}
