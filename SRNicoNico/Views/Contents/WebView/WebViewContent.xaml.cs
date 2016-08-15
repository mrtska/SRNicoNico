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
using System.Threading;

using SRNicoNico.Models.NicoNicoWrapper;

using Livet;

namespace SRNicoNico.Views.Contents.WebView {
    /// <summary>
    /// WebViewContent.xaml の相互作用ロジック
    /// </summary>
    public partial class WebViewContent : UserControl {




        public WebViewContent() {
            InitializeComponent();
            
        }


        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is WebViewContentViewModel) {

                var vm = (WebViewContentViewModel) DataContext;
                vm.WebBrowser = browser;
                browser.Navigate(vm.Url);
            }

        }


        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if((bool)(e.NewValue)) {

                var vm = (WebViewContentViewModel)DataContext;
                browser.Navigate(vm.Url);
            }

        }
    }
}
