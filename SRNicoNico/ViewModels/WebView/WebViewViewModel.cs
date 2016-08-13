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



        public void AddTab(string url) {

            WebViewTabs.Add(new WebViewContentViewModel(url, this));
        }

        public async void Home() {

            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                SelectedTab.WebBrowser.Navigate(Settings.Instance.WebViewDefaultPage);

            }));
        }
        public void Refresh() {

            SelectedTab?.WebBrowser.Refresh(System.Windows.Forms.WebBrowserRefreshOption.Completely);
        }



    }
}
