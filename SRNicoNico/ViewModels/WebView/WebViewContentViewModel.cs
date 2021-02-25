using System;
using System.Text.RegularExpressions;
using Livet;
using Microsoft.Web.WebView2.Wpf;

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

        private bool _OpenWithViewer;
        /// <summary>
        /// 動画リンクをNicoNicoViewerで開くかどうか
        /// </summary>
        public bool OpenWithViewer {
            get { return _OpenWithViewer; }
            set { 
                if (_OpenWithViewer == value)
                    return;
                _OpenWithViewer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// WebViewの実装
        /// </summary>
        public WebView2 WebView { get; private set; }

        private readonly WebViewViewModel Owner;

        public WebViewContentViewModel(WebViewViewModel vm, string initialUrl, bool useViewer) : base(initialUrl) {
            
            Owner = vm;
            CurrentUrl = initialUrl;
            OpenWithViewer = useViewer;

            WebView = new WebView2 { Source = new Uri(initialUrl) };
            Initialize();
        }

        /// <summary>
        /// WebViewを初期化する
        /// </summary>
        private async void Initialize() {
            
            CompositeDisposable.Add(WebView);

            // ページ遷移が始まった時に呼ばれる
            WebView.NavigationStarting += (o, e) => {

                if (OpenWithViewer) {

                    // TODO: ここにNicoNicoViewerで開く処理
                    //e.Cancel = true;
                    //return;
                }

                CurrentUrl = e.Uri;
                Name = CurrentUrl;
            };

            // ページ遷移が完了した時に呼ばれる
            WebView.NavigationCompleted += (o, e) => {

                CanGoBack = WebView.CanGoBack;
                CanGoForward = WebView.CanGoForward;
                Name = WebView.CoreWebView2.DocumentTitle;
            };

            await WebView.EnsureCoreWebView2Async();
            // target=_blankなどのリンクやShift+Clickした時に呼ばれる
            WebView.CoreWebView2.NewWindowRequested += (o, e) => {

                Owner.AddTab(e.Uri, OpenWithViewer);
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

        private readonly Regex UrlRegex = new Regex(@"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+");

        /// <summary>
        /// 指定されたURLをロードする
        /// アドレスバーにURLを入力した時にも呼ばれる
        /// </summary>
        /// <param name="url">遷移したいURL</param>
        public void Load(string url) {

            if (UrlRegex.Match(url).Success) {

                WebView.CoreWebView2.Navigate(url);
            } else {

                WebView.CoreWebView2.Navigate("https://www.google.co.jp/search?q=" + url);
            }
        }

        /// <summary>
        /// WebViewを更新する
        /// </summary>
        public void Refresh() {

            WebView.Reload();
        }
    }
}
