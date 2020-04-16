using System;
using Livet;
using Microsoft.Toolkit.Wpf.UI.Controls;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// WebViewContentクラスのDataContext
    /// </summary>
    public class WebViewContentViewModel : TabItemViewModel {

        private string? _CurrentUrl;
        /// <summary>
        /// 現在表示しているページのURL
        /// </summary>
        public string? CurrentUrl {
            get { return _CurrentUrl; }
            set { 
                if (_CurrentUrl == value)
                    return;
                _CurrentUrl = value;
                RaisePropertyChanged();
            }
        }

        private bool _CanGoBack;
        /// <summary>
        /// バックボタンが有効かどうか
        /// </summary>
        public bool CanGoBack {
            get { return _CanGoBack; }
            set { 
                if (_CanGoBack == value)
                    return;
                _CanGoBack = value;
                RaisePropertyChanged();
            }
        }

        private bool _CanGoForward;
        /// <summary>
        /// 進むボタンが有効かどうか
        /// </summary>
        public bool CanGoForward {
            get { return _CanGoForward; }
            set { 
                if (_CanGoForward == value)
                    return;
                _CanGoForward = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// WebViewの実装 現時点でDeprecated
        /// そのうちWebView2に実装を変更する必要有り
        /// </summary>
        public WebView WebView { get; private set; }


        public WebViewContentViewModel(string initialUrl) : base(initialUrl) {

            CurrentUrl = initialUrl;
            WebView = new WebView { Source = new Uri(initialUrl) };
            Initialize();
        }

        /// <summary>
        /// WebViewを初期化する
        /// </summary>
        private void Initialize() {
            
            CompositeDisposable.Add(WebView);

            // ページ遷移が始まった時に呼ばれる
            WebView.NavigationStarting += (o, e) => {

                CurrentUrl = e.Uri.OriginalString;
                Name = CurrentUrl;
            };

            // ページ遷移が完了した時に呼ばれる
            WebView.NavigationCompleted += (o, e) => {

                CanGoBack = WebView.CanGoBack;
                CanGoForward = WebView.CanGoForward;
                Name = WebView.DocumentTitle;
            };
            WebView.PreviewMouseDown += (o, e) => {

                ;
            };
            WebView.PreviewKeyDown += (o, e) => {

                ;
            };
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

        /// <summary>
        /// WebViewを更新する
        /// </summary>
        public void Refresh() {

            WebView.Refresh();
        }
    }
}
