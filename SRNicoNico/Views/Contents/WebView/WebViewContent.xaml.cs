using SRNicoNico.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class WebViewContent : UserControl {
        public WebViewContent() {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if (DataContext is WebViewContentViewModel vm) {

                vm.WebBrowser = browser;
                browser.Navigate(vm.Url);
            }
        }
    }
}
