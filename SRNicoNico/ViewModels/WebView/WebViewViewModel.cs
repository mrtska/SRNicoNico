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


        #region WebBrowser変更通知プロパティ
        private WebBrowser _WebBrowser;

        public WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                if(_WebBrowser == value)
                    return;
                _WebBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public WebViewViewModel() : base("WebView") {

        }

        public async void Home() {

            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                WebBrowser.Navigate(Settings.Instance.WebViewDefaultPage);
                      
            }));
        }

        public void LoadCompleted() {


        }

        public void Initialize() {

            Home();
        }

        public void Refresh() {

            WebBrowser.Refresh(true);


        }




    }
}
