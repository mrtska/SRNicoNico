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
using System.Windows.Navigation;
using System.Runtime.InteropServices;

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
        private WebBrowser _WebBrowser;

        public WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                if(_WebBrowser == value)
                    return;
                _WebBrowser = value;
                value.LoadCompleted += WebBrowser_LoadCompleted;
                value.Navigating += Value_Navigating1;
                value.Navigated += Value_Navigated1;
                /*value.DocumentTitleChanged += Value_DocumentTitleChanged;
                value.Navigating += Value_Navigating;
                value.Navigated += Value_Navigated;
                value.DocumentCompleted += Value_DocumentCompleted;
                value.CanGoForwardChanged += Value_CanGoForwardChanged;
                value.CanGoBackChanged += Value_CanGoBackChanged;
                value.NewWindow += Value_NewWindow;

                value.ScriptErrorsSuppressed = true;
                value.IsWebBrowserContextMenuEnabled = false;*/

                RaisePropertyChanged();
            }
        }
        private bool isInitialized = false;
        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e) {


            if(!isInitialized) {    
                IServiceProvider serviceProvider = (IServiceProvider)WebBrowser.Document;
                Guid serviceGuid = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid iid = typeof(SHDocVw.IWebBrowser2).GUID;
                SHDocVw.DWebBrowserEvents_Event wbEvents = (SHDocVw.DWebBrowserEvents_Event)serviceProvider.QueryService(ref serviceGuid, ref iid);
                wbEvents.NewWindow += OnNewWindow;
                isInitialized = true;
            }
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
        private interface IServiceProvider {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object QueryService(ref Guid guidService, ref Guid riid);
        }

        private void Value_Navigated1(object sender, NavigationEventArgs e) {

            dynamic doc = WebBrowser.Document;
            Name = doc.title;

            CanGoBack = WebBrowser.CanGoBack;
            CanGoForward = WebBrowser.CanGoForward;
        }

        private void Value_Navigating1(object sender, NavigatingCancelEventArgs e) {

            Url = e.Uri.OriginalString;

            if(!ForceWebView && NicoNicoOpener.GetType(e.Uri) != NicoNicoUrlType.Other && !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {

                e.Cancel = true;
                NicoNicoOpener.Open(e.Uri);
                return;
            }
        }

        private void OnNewWindow(string url, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed) {
            Processed = true;
            Owner.AddTab(url);
        }
        private void Value_NewWindow(object sender, CancelEventArgs e) {
            
           /* e.Cancel = true;
            if(WebBrowser.StatusText == "") {

                
                return;
            }
            Owner.AddTab(WebBrowser.StatusText);*/
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

            //Url = WebBrowser.Url.OriginalString;
        }

        private void Value_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e) {


        }

        private void Value_DocumentTitleChanged(object sender, EventArgs e) {

           // Name = WebBrowser.DocumentTitle;
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

        private bool ForceWebView;

        public WebViewContentViewModel(string url, WebViewViewModel vm, bool forceWebView = false) {

            ForceWebView = forceWebView;
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
