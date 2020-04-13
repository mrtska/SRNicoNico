using System;
using Livet;
using Microsoft.Toolkit.Wpf.UI.Controls;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// スタートページクラスのDataContext
    /// </summary>
    public class WebViewContentViewModel : TabItemViewModel {

        private string _CurrentUrl;
        /// <summary>
        /// 現在表示しているページのURL
        /// </summary>
        public string CurrentUrl {
            get { return _CurrentUrl; }
            set { 
                if (_CurrentUrl == value)
                    return;
                _CurrentUrl = value;
                RaisePropertyChanged();
            }
        }

        public WebView WebView { get; private set; }


        public WebViewContentViewModel(string initialUrl) : base("WebView") {

            CurrentUrl = initialUrl;
            WebView = new WebView { Source = new Uri(initialUrl) };
        }


        /// <summary>
        /// 前に戻る
        /// </summary>
        public void GoBack() {

            if (WebView.CanGoBack) {

                WebView.GoBack();
            }
        }

        /// <summary>
        /// 次に進む
        /// </summary>
        public void GoForward() {

            if (WebView.CanGoForward) {

                WebView.GoForward();
            }
        }


    }
}
