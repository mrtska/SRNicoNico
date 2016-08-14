using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models;
using System.Windows.Controls;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class WebViewViewModel : TabItemViewModel {



        #region WebViewTabs変更通知プロパティ
        private DispatcherCollection<WebViewContentViewModel> _WebViewTabs = new DispatcherCollection<WebViewContentViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<WebViewContentViewModel> WebViewTabs {
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

        }

        public void Initialize() {

            AddTab(Settings.Instance.WebViewDefaultPage);
        }

        public void AddTab() {

            AddTab(Settings.Instance.WebViewDefaultPage);
        }

        public void AddTab(string url) {

            var tab = new WebViewContentViewModel(url, this);
            WebViewTabs.Add(tab);
            SelectedTab = tab;
        }


        public void RemoveTab(WebViewContentViewModel vm) {

            
            if(WebViewTabs.Count == 1) {

                Home();
                return;
            }

            WebViewTabs.Remove(vm);
            SelectedTab = WebViewTabs.First();
        }

        public async void Home() {

            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                SelectedTab.WebBrowser.Navigate(Settings.Instance.WebViewDefaultPage);

            }));
        }
        public void Refresh() {

            SelectedTab?.WebBrowser.Refresh(System.Windows.Forms.WebBrowserRefreshOption.Completely);
        }



        public void PrevTab() {

            var index = WebViewTabs.IndexOf(SelectedTab);

            if(index == 0) {

                SelectedTab = WebViewTabs[WebViewTabs.Count - 1];
            } else {

                SelectedTab = WebViewTabs[index - 1];
            }
        }

        public void NextTab() {

            var index = WebViewTabs.IndexOf(SelectedTab);

            if(WebViewTabs.Count - 1 == index) {

                SelectedTab = WebViewTabs[0];
            } else {

                SelectedTab = WebViewTabs[index + 1];
            }
        }

        public override void KeyDown(KeyEventArgs e) {


            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                switch(e.Key) {
                    case Key.T:

                        AddTab();
                        break;
                    case Key.W:

                        RemoveTab(SelectedTab);
                        break;
                    case Key.R:

                        Refresh();
                        break;
                    case Key.Tab:

                        NextTab();
                        break;
                }
            } else {

                switch(e.Key) {

                    case Key.F5:
                        Refresh();
                        break;
                    case Key.Home:
                        Home();
                        break;
                }
            }


           

        }


    }
}
