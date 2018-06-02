using Livet;
using Livet.Messaging;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Linq;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class WebViewViewModel : TabItemViewModel {

        #region WebViewTabs変更通知プロパティ
        private ObservableSynchronizedCollection<WebViewContentViewModel> _WebViewTabs;

        public ObservableSynchronizedCollection<WebViewContentViewModel> WebViewTabs {
            get { return _WebViewTabs; }
            set {
                if(_WebViewTabs == value)
                    return;
                _WebViewTabs = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedTab変更通知プロパティ
        private WebViewContentViewModel _SelectedTab;

        public WebViewContentViewModel SelectedTab {
            get { return _SelectedTab; }
            set {
                if(_SelectedTab == value)
                    return;
                _SelectedTab = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public WebViewViewModel() : base("WebView") {

            WebViewTabs = new ObservableSynchronizedCollection<WebViewContentViewModel>();
        }

        public void Initialize() {

            if(WebViewTabs.Count == 0) {

                AddTab();
            }
        }

        public void AddTab() {

            AddTab(Settings.Instance.WebViewDefaultPage);
        }

        //指定したURLでWebViewを開く
        public void AddTab(string url, bool forceUseWebView = false) {

            var tab = new WebViewContentViewModel(this, url, forceUseWebView);
            WebViewTabs.Add(tab);
            SelectedTab = tab;
        }

        public void RemoveTab(WebViewContentViewModel vm) {

            if(WebViewTabs.Count == 1) {

                Home();
                return;
            }
            vm.Dispose();

            WebViewTabs.Remove(vm);
            SelectedTab = WebViewTabs.First();
        }

        //左のタブに移動
        public void PrevTab() {

            var index = WebViewTabs.IndexOf(SelectedTab);

            if(index == 0) {

                SelectedTab = WebViewTabs[WebViewTabs.Count - 1];
            } else {

                SelectedTab = WebViewTabs[index - 1];
            }
        }

        //右のタブに移動 一番右の場合は一番左のタブに移動
        public void NextTab() {

            var index = WebViewTabs.IndexOf(SelectedTab);

            if(WebViewTabs.Count - 1 == index) {

                SelectedTab = WebViewTabs[0];
            } else {

                SelectedTab = WebViewTabs[index + 1];
            }
        }

        //現在のタブを設定しているデフォルトページに移動する
        public async void Home() {

            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                SelectedTab.WebBrowser.Navigate(Settings.Instance.WebViewDefaultPage);
            }));
        }
        public void Refresh() {

            SelectedTab?.WebBrowser.Refresh(true);
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {

                if(e.Key == Key.Tab) {

                    PrevTab();
                    //これをしないとFatalExecutionEngineErrorになる
                    e.Handled = true;
                }
            } else if(e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {

                switch(e.Key) {

                    case Key.T: //新しいタブを追加

                        AddTab(Settings.Instance.WebViewDefaultPage);
                        e.Handled = true;
                        break;
                    case Key.W: //現在のタブを消す

                        RemoveTab(SelectedTab);
                        e.Handled = true;
                        break;
                    case Key.R: //現在のタブをリロード

                        Refresh();
                        e.Handled = true;
                        break;
                    case Key.Tab:   //次のタブに移動

                        NextTab();
                        e.Handled = true;
                        break;
                }

            } else {

                switch(e.Key) {

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
        }

        public override void MouseDown(MouseButtonEventArgs e) {

            switch(e.ChangedButton) {
                case MouseButton.XButton1:  //ホイールの左ボタン
                    SelectedTab?.GoBack();
                    break;
                case MouseButton.XButton2:  //ホイールの右ボタン
                    SelectedTab?.GoForward();
                    break;
            }
        }

        public override bool CanShowHelp() {
            return true;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.WebViewHelpView), this, TransitionMode.NewOrActive));
        }
    }
}
