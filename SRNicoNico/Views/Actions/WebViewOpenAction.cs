using System.Windows;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Actions {
    /// <summary>
    /// 指定したURLをWebViewで開くトリガーアクション
    /// </summary>
    public class WebViewOpenAction : TriggerAction<DependencyObject> {
        /// <summary>
        /// URL本体
        /// </summary>
        public string Url {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register(nameof(Url), typeof(string), typeof(WebViewOpenAction), new PropertyMetadata(""));

        protected override void Invoke(object parameter) {

            // MainWindowからMainWindowViewModelを経由してWebViewを開くようにする
            if (Application.Current.MainWindow.DataContext is MainWindowViewModel vm) {

                vm.MainContent.WebView?.AddTab(Url, false);
                // WebViewをActiveにする
                vm.MainContent.SelectedItem = vm.MainContent.WebView;
            }
        }
    }
}
