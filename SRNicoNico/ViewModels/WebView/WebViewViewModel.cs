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

        public void Home() {

            Task.Run(() => {

                try {

                    //var a = NicoNicoWrapperMain.Session.GetAsync(Settings.Instance.RankingPageUrl.OriginalString).Result;

                    //var doc = new HtmlDocument();
                    //doc.LoadHtml2(a);

                    //doc.DocumentNode.SelectSingleNode("//div[@id='siteHeaderInner']/ul[@class='siteHeaderGlovalNavigation']").InnerHtml = "";
                    //doc.DocumentNode.SelectSingleNode("//div[@class='banner leadBanner ads']").InnerHtml = "";
                    //doc.DocumentNode.SelectSingleNode("//header[@class='header']").InnerHtml = "";
                    //doc.DocumentNode.SelectSingleNode("//footer[@class='footer']").InnerHtml = "";

                    //doc.DocumentNode.SelectSingleNode("//body").Attributes.Add("oncontextmenu", "return false;");
                    //doc.DocumentNode.SelectSingleNode("//body").Attributes.Add("style", "bgcolor: #222222 !important;");

                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                        WebBrowser.Navigate(Settings.Instance.RankingPageUrl);
                      
                    }));
                   
                } catch(RequestFailed) {


                }
            });

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
