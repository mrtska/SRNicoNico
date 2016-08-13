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


using System.Windows;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class WebViewContentViewModel : TabItemViewModel {



        #region Status変更通知プロパティ

        public new string Status {
            get { return Owner.Status; }
            set { 
                if(Owner.Status == value)
                    return;
                Owner.Status = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region WebBrowser変更通知プロパティ
        private System.Windows.Forms.WebBrowser _WebBrowser;

        public System.Windows.Forms.WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                if(_WebBrowser == value)
                    return;
                _WebBrowser = value;
                value.DocumentTitleChanged += Value_DocumentTitleChanged;
                value.Navigating += Value_Navigating;
                value.Navigated += Value_Navigated;
                value.ProgressChanged += Value_ProgressChanged;
                value.IsWebBrowserContextMenuEnabled = false;
                RaisePropertyChanged();
            }
        }
            
        private void Value_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e) {

            
            Url = WebBrowser.Url.OriginalString;
        }

        private void Value_ProgressChanged(object sender, System.Windows.Forms.WebBrowserProgressChangedEventArgs e) {

            
        }

        private void Value_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e) {

            if(NicoNicoOpener.GetType(e.Url) != NicoNicoUrlType.Other && !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {

                e.Cancel = true;
                NicoNicoOpener.Open(e.Url);
                return;
            }


        }

        private void Value_DocumentTitleChanged(object sender, EventArgs e) {

            Name = WebBrowser.DocumentTitle;
        }

        private void Value_TitleChanged(object sender, DependencyPropertyChangedEventArgs e) {

            Name = e.NewValue as string;
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
            }
        }
        #endregion

        private WebViewViewModel Owner;

        public WebViewContentViewModel(string url, WebViewViewModel vm) {

            Url = url;
            Owner = vm;
        }

        public void LoadCompleted(string doc) {

            Name = doc;
        }


        public void Load(string url) {

            WebBrowser.Navigate(url);
        }







    }
}
