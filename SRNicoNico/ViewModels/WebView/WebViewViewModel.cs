using System;
using System.Linq;
using System.Windows.Input;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// WebViewクラスのDataContext
    /// </summary>
    public class WebViewViewModel : TabItemViewModel {

        /// <summary>
        /// WebViewのリスト
        /// </summary>
        public ObservableSynchronizedCollection<WebViewContentViewModel> WebViewItems { get; private set; }

        private WebViewContentViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているWebViewのページ
        /// </summary>
        public WebViewContentViewModel? SelectedItem {
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
        /// 初めてWebViewが表示された時に実行される
        /// </summary>
        public void Loaded() {

            // 1つもWebViewを開いていない時は自動的に開く
            if (WebViewItems.Count == 0) {
                
                AddNewTab();
            }
        }

        /// <summary>
        /// 指定したURLでWebViewのタブを開く
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="useViewer">リンクをNicoNicoViewerで開くか</param>
        public void AddTab(string url, bool useViewer = false) {

            var tab = new WebViewContentViewModel(this, url, useViewer);

            // リストに追加し、選択状態にする
            WebViewItems.Add(tab);
            CompositeDisposable.Add(tab);

            SelectedItem = tab;
        }

        /// <summary>
        /// 新しいタブを開く
        /// </summary>
        public void AddNewTab() {

            AddTab(Settings.Instance.DefaultWebViewPageUrl, true);
        }

        /// <summary>
        /// タブを閉じる
        /// </summary>
        /// <param name="vm">対象のタブのViewModel</param>
        public void RemoveTab(WebViewContentViewModel vm) {

            // タブが一つしか無い時はそのタブをホームに戻す
            if (WebViewItems.Count == 1) {

                Home();
                return;
            }
            // WebViewを開放する
            CompositeDisposable.Remove(vm);
            vm.Dispose();

            WebViewItems.Remove(vm);
            
            // 選択されているタブを消した時はリストの最後のタブを選択する
            if (SelectedItem == null) {

                SelectedItem = WebViewItems.Last();
            }
        }

        /// <summary>
        /// 設定されているホームに遷移する
        /// </summary>
        public void Home() {

            SelectedItem?.WebView.CoreWebView2.Navigate(Settings.Instance.DefaultWebViewPageUrl);
        }

        /// <summary>
        /// 現在選択されているWebViewを更新する
        /// </summary>
        public void Refresh() {

            SelectedItem?.Refresh();
        }

        public override void KeyDown(KeyEventArgs e) {

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {

                // Ctrl+Shift+Tabのショートカット処理 左のタブに移動する
                if (e.Key == Key.Tab) {

                    if (SelectedItem == null || WebViewItems.Count == 1) {
                        return;
                    }
                    var index = WebViewItems.IndexOf(SelectedItem);

                    SelectedItem = index == 0 ? WebViewItems[^1] : WebViewItems[index - 1];
                    e.Handled = true;
                }
                return;
            }
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {

                switch (e.Key) {
                    case Key.T:     // Ctrl+T 新しいタブを追加

                        AddTab(Settings.Instance.DefaultWebViewPageUrl);
                        e.Handled = true;
                        break;
                    case Key.W:   // Ctrl+W 現在のタブを消す

                        if (SelectedItem != null) {

                            RemoveTab(SelectedItem);
                        }
                        e.Handled = true;
                        break;
                    case Key.R:    // Ctrl+R 現在のタブをリロード

                        Refresh();
                        e.Handled = true;
                        break;
                    case Key.Tab:   // Ctrl+Tab 次のタブに移動

                        if (SelectedItem == null || WebViewItems.Count == 1) {
                            return;
                        }
                        var index = WebViewItems.IndexOf(SelectedItem);

                        SelectedItem = WebViewItems.Count - 1 == index ? WebViewItems[0] : WebViewItems[index + 1];
                        e.Handled = true;
                        break;
                }
                return;
            }
            switch (e.Key) {

                case Key.F5:
                    Refresh();
                    e.Handled = true;
                    break;
                case Key.Home:
                    Home();
                    e.Handled = true;
                    break;
            }
        }

        public override void MouseDown(MouseButtonEventArgs e) {

            switch (e.ChangedButton) {
                case MouseButton.XButton1:  // ホイールの左ボタン
                    SelectedItem?.GoBack();
                    break;
                case MouseButton.XButton2:  // ホイールの右ボタン
                    SelectedItem?.GoForward();
                    break;
            }
        }
    }
}
