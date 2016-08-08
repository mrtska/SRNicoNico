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

using CefSharp;
using CefSharp.Wpf;
using System.Windows;

namespace SRNicoNico.ViewModels {
    public class WebViewContentViewModel : TabItemViewModel {


        #region WebBrowser変更通知プロパティ
        private ChromiumWebBrowser _WebBrowser;

        public ChromiumWebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                if(_WebBrowser == value)
                    return;
                _WebBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Url変更通知プロパティ
        private string _Url;

        public string Url {
            get { return _Url; }
            set { 
                if(_Url == value)
                    return;
                _Url = value;
                RaisePropertyChanged();
                WebBrowser?.WebBrowser.Load(value);
            }
        }
        #endregion


        public WebViewContentViewModel(string url) {

            Url = url;
        }

        public void LoadCompleted(dynamic doc) {

            Name = doc.Title;
        }








    }
}
