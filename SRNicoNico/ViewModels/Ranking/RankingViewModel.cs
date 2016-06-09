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
using mshtml;

namespace SRNicoNico.ViewModels {
    public class RankingViewModel : TabItemViewModel {


        #region RankingPageUrl変更通知プロパティ
        public Uri RankingPageUrl {
            get { return Properties.Settings.Default.RankingPageUrl; }
            set { 
                if(Properties.Settings.Default.RankingPageUrl == value)
                    return;
                Properties.Settings.Default.RankingPageUrl = value;
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
                RaisePropertyChanged();
            }
        }
        #endregion


        public RankingViewModel() : base("ランキング") {

        }

        public void Home() {

            Task.Run(() => {

                try {

                    var a = NicoNicoWrapperMain.Session.GetAsync(RankingPageUrl.OriginalString).Result;

                    var doc = new HtmlDocument();
                    doc.LoadHtml2(a);

                    //doc.DocumentNode.SelectSingleNode("//div[@id='siteHeaderInner']/ul[@class='siteHeaderGlovalNavigation']").InnerHtml = "";
                    //doc.DocumentNode.SelectSingleNode("//div[@class='banner leadBanner ads']").InnerHtml = "";
                    //doc.DocumentNode.SelectSingleNode("//header[@class='header']").InnerHtml = "";
                    //doc.DocumentNode.SelectSingleNode("//footer[@class='footer']").InnerHtml = "";

                    //doc.DocumentNode.SelectSingleNode("//body").Attributes.Add("oncontextmenu", "return false;");
                    //doc.DocumentNode.SelectSingleNode("//body").Attributes.Add("style", "bgcolor: #222222 !important;");

                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                        WebBrowser.Navigate(RankingPageUrl);
                      
                    }));
                   
                } catch(RequestFailed) {


                }
            });

        }

        public void LoadCompleted() {

            var html = WebBrowser.Document as HTMLDocument;
            var a = html.Script as HTMLWindow2;
            var code = @"
//document.getElementById('siteHeader').setAttribute('style','position: fixed; display: none;');

//document.getElementsByTagName('header')[0].getElementsByClassName('siteHeader')[0].innerHTML = """";
//document.getElementsByTagName('footer')[1].innerHTML = """";
document.getElementsByTagName('body')[0].setAttribute('style', 'font-family: Yu Gothic UI, メイリオ, Arial; scrollbar-face-color: #666666; scrollbar-track-color: #3F3F46; scrollbar-arrow-color: #C8C8C8; scrollbar-highlight-color: #3F3F46; scrollbar-3dlight-color: #3F3F46; scrollbar-shadow-color: #3F3F46;');
//document.getElementsByClassName('rankingHeader matrixHeader')[0].setAttribute('style', 'background: #333333; padding-top: 14px;');
//document.getElementsByTagName('h1')[0].setAttribute('style', 'color: #BBBBBB;');
document.getElementsByTagName('body')[0].setAttribute('oncontextmenu', 'return false;');
";
            a.execScript(code, "JavaScript");

        }

        public void Initialize() {

            Home();
        }

        public void Refresh() {

            WebBrowser.Refresh(true);


        }




    }
}
