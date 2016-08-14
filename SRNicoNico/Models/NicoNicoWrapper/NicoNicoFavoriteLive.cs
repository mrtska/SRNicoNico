using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFavoriteLive {


        private const string LiveUrl = "http://www.nicovideo.jp/my/live";

        private const string NotifyUrl = "http://live.nicovideo.jp/notifybox";

        public IList<NicoNicoFavoriteLiveContent> GetLiveInformation() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(LiveUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                if(content == null) {

                    return null;
                }

                var outers = content.SelectNodes("child::div[@id='ch']/div/div[@class='outer']");

                //終了
                if(outers == null) {

                    return null;
                }

                var list = new List<NicoNicoFavoriteLiveContent>();

                foreach(var outer in outers) {

                    var entry = new NicoNicoFavoriteLiveContent();

                    entry.CommunityUrl = outer.SelectSingleNode("child::a[1]").Attributes["href"].Value;

                    var img = outer.SelectSingleNode("child::a/img");

                    entry.ThumbNailUrl = img.Attributes["src"].Value;
                    entry.CommunityName = "<a href=\"" + entry.CommunityUrl + "\">" + img.Attributes["alt"].Value + "</a>";

                    var section = outer.SelectSingleNode("child::div");

                    entry.Title = section.SelectSingleNode("child::h5/a").InnerText.Trim();
                    entry.LiveUrl = section.SelectSingleNode("child::h5/a").Attributes["href"].Value;
                    entry.StartTime = section.SelectSingleNode("child::p[@class='time']/small").InnerText;

                    list.Add(entry);
                }

                return list;
            } catch(RequestTimeout) {

                return null;
            }
        }
    }

    public class NicoNicoFavoriteLiveContent : NotificationObject {

        //生放送URL
        public string LiveUrl { get; set; }

        //コミュニティサムネイルURL
        public string ThumbNailUrl { get; set; }

        //コミュニティURL
        public string CommunityUrl { get; set; }

        //コミュニティの名前
        public string CommunityName { get; set; }

        //生放送タイトル
        public string Title { get; set; }

        //生放送開始時間
        public string StartTime { get; set; }


        public void OpenWebView() {

            App.ViewModelRoot.AddWebViewTab(LiveUrl, true);
        }

        public void OpenCommunity() {

            NicoNicoOpener.Open(CommunityUrl);
        }

        public void OpenCommunityWebView() {

            App.ViewModelRoot.AddWebViewTab(CommunityUrl, true);
        }

    }

}
