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
using System.Text.RegularExpressions;

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
                value.DocumentCompleted += Value_DocumentCompleted;
                value.CanGoForwardChanged += Value_CanGoForwardChanged;
                value.CanGoBackChanged += Value_CanGoBackChanged;
                value.NewWindow += Value_NewWindow;

                value.ScriptErrorsSuppressed = true;
                value.IsWebBrowserContextMenuEnabled = false;

                RaisePropertyChanged();
            }
        }

        private void Value_NewWindow(object sender, CancelEventArgs e) {
            
            e.Cancel = true;
            if(WebBrowser.StatusText == "") {

                return;
            }
            Owner.AddTab(WebBrowser.StatusText);
        }

        private void Value_CanGoBackChanged(object sender, EventArgs e) {

            CanGoBack = WebBrowser.CanGoBack;
        }

        private void Value_CanGoForwardChanged(object sender, EventArgs e) {

            CanGoForward = WebBrowser.CanGoForward;
        }



        #region CanGoBack変更通知プロパティ
        private bool _CanGoBack;

        public bool CanGoBack {
            get { return _CanGoBack; }
            set { 
                if(_CanGoBack == value)
                    return;
                _CanGoBack = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region CanGoForward変更通知プロパティ
        private bool _CanGoForward;

        public bool CanGoForward {
            get { return _CanGoForward; }
            set { 
                if(_CanGoForward == value)
                    return;
                _CanGoForward = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        private void Value_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e) {
        }


        private void Value_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e) {

            Url = WebBrowser.Url.OriginalString;
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


        public void RemoveTab(WebViewContentViewModel vm) {

            Owner.RemoveTab(vm);
        }

        public void LoadCompleted(string doc) {

            Name = doc;
        }

        public void GoBack() {

            WebBrowser.GoBack();
        }

        public void GoForward() {

            WebBrowser.GoForward();
        }

        public void Load(string url) {

            var regex = new Regex(@"(.?.?)(https?://[\w/:%#\$&\?\(\)~\.=\+\-]+)");

            if(regex.Match(url).Success) {

                WebBrowser.Navigate(url);
            } else {

                WebBrowser.Navigate("https://www.google.co.jp/search?q=" + url);
            }
        }


    }
}
