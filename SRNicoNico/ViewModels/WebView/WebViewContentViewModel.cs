using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using System.Windows.Navigation;
using SRNicoNico.Models.NicoNicoViewer;
using System.Runtime.InteropServices;

namespace SRNicoNico.ViewModels {
    public class WebViewContentViewModel : TabItemViewModel {


        #region WebBrowser変更通知プロパティ
        private WebBrowser _WebBrowser;
        private WebBrowserHostUIHandler UIHandler;

        public WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                if(_WebBrowser == value)
                    return;
                _WebBrowser = value;
                value.Navigating += WebViewNavigating;
                value.Navigated += WebViewNavigated;
                value.LoadCompleted += WebViewLoadCompleted;

                UIHandler = new WebBrowserHostUIHandler(value);
                CompositeDisposable.Add(value);
                RaisePropertyChanged();
            }
        }

        private void Value_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            throw new NotImplementedException();
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



        #region OpenWithViewer変更通知プロパティ
        private bool _OpenWithViewer;

        public bool OpenWithViewer {
            get { return _OpenWithViewer; }
            set { 
                if(_OpenWithViewer == value)
                    return;
                _OpenWithViewer = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        private WebViewViewModel Owner { get; set; }


        public WebViewContentViewModel(WebViewViewModel vm, string url, bool forceUseWebView = false) {
            
            OpenWithViewer = !forceUseWebView;
            Url = url;
            Owner = vm;
        }


        public WebViewContentViewModel() : base("WebView") {
        }

        //前に戻る
        public void GoBack() {

            if(WebBrowser.CanGoBack) {

                WebBrowser.GoBack();
            }
        }

        //次に進む
        public void GoForward() {

            if(WebBrowser.CanGoForward) {

                WebBrowser.GoForward();
            }
        }

        public void Load(string url) {

            if(url.StartsWith("javascript:")) {

                WebBrowser.InvokeScript("eval", url.Split(new char[] { ':' }, 2)[1]);
                return;
            }
            var regex = new Regex(@"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+");

            if(regex.Match(url).Success) {

                WebBrowser.Navigate(url);
            } else {

                WebBrowser.Navigate("https://www.google.co.jp/search?q=" + url);
            }
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
        private interface IServiceProvider {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object QueryService(ref Guid guidService, ref Guid riid);
        }
        private bool isInitialized = false;
        private void WebViewLoadCompleted(object sender, NavigationEventArgs e) {

        }


        private void OnNewWindow(string url, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed) {
            Processed = true;
            Owner.AddTab(url, true);
        }



        private string PrevUrl = "";

        //画面遷移する直前
        private void WebViewNavigating(object sender, NavigatingCancelEventArgs e) {


            if(e.Uri.OriginalString.StartsWith("javascript:")) {

                WebBrowser.InvokeScript("eval", e.Uri.OriginalString.Split(new char[] { ':' }, 2)[1]);
                e.Cancel = true;
                return;
            }

            //Viewerで開けるURLはViewerで開く
            if(NicoNicoOpener.GetType(e.Uri.OriginalString) != NicoNicoUrlType.Other && OpenWithViewer) {

                //なぜかNavgatingイベントが２回呼ばれる箇所があるので対策
                if(PrevUrl == e.Uri.OriginalString) {

                    e.Cancel = true;
                    return;
                }

                PrevUrl = e.Uri.OriginalString;
                e.Cancel = true;
                NicoNicoOpener.Open(e.Uri.OriginalString);
            } else {

                IsActive = true;
                Owner.Status = "読み込み中: " + e.Uri.OriginalString;
            }
        }

        //画面遷移時に
        private void WebViewNavigated(object sender, NavigationEventArgs e) {

            IsActive = false;
            dynamic doc = WebBrowser.Document;

            Name = doc.title;

            if(Name.Length == 0) {

                Name = WebBrowser.Source.OriginalString;
            }

            CanGoBack = WebBrowser.CanGoBack;
            CanGoForward = WebBrowser.CanGoForward;
            Url = WebBrowser.Source.OriginalString;
            Owner.Status = "";

            if(!isInitialized) {
                isInitialized = true;
                var serviceProvider = (IServiceProvider)WebBrowser.Document;
                var serviceGuid = new Guid("0002DF05-0000-0000-C000-000000000046");
                var iid = typeof(SHDocVw.IWebBrowser2).GUID;
                var wbEvents = (SHDocVw.DWebBrowserEvents_Event)serviceProvider.QueryService(ref serviceGuid, ref iid);
                wbEvents.NewWindow += OnNewWindow;
            }
        }
    }
}
