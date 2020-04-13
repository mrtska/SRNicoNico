using System;
using System.Collections.Generic;
using System.Text;
using Livet;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// WebViewクラスのDataContext
    /// </summary>
    public class WebViewViewModel : TabItemViewModel {

        /// <summary>
        /// WebViewのリスト
        /// </summary>
        public ObservableSynchronizedCollection<WebViewContentViewModel> WebViewItems { get; private set; }

        private WebViewContentViewModel _SelectedItem;
        /// <summary>
        /// 現在選択されているWebViewのページ
        /// </summary>
        public WebViewContentViewModel SelectedItem {
            get { return _SelectedItem; }
            set { 
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public WebViewViewModel(MainWindowViewModel vm) : base("WebView") {

            WebViewItems = new ObservableSynchronizedCollection<WebViewContentViewModel>();
        }
        /// <summary>
        /// 指定したURLでWebViewのタブを開く
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="forceUseWebView"></param>
        public void AddTab(string url, bool forceUseWebView = false) {

            var tab = new WebViewContentViewModel(url);

            // リストに追加し、選択状態にする
            WebViewItems.Add(tab);
            SelectedItem = tab;
        }
        public void AddNewTab() {

            AddTab("https://www.google.co.jp");
        }
    }
}
