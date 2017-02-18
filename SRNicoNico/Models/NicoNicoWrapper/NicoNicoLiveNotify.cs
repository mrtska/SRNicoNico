using HtmlAgilityPack;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLiveNotify {

        private const string LiveUrl = "http://www.nicovideo.jp/my/live";

        private readonly LiveNotifyViewModel Owner;

        public NicoNicoLiveNotify(LiveNotifyViewModel vm) {

            Owner = vm;
        }

        public async Task<List<NicoNicoLiveNotifyEntry>> GetLiveInformationAsync() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(LiveUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                if(content == null) {

                    return null;
                }

                var outers = content.SelectNodes("div[@id='ch']/div/div[@class='outer']");

                //終了
                if(outers == null) {

                    return null;
                }

                var list = new List<NicoNicoLiveNotifyEntry>();

                foreach(var outer in outers) {

                    var entry = new NicoNicoLiveNotifyEntry();

                    entry.CommunityUrl = outer.SelectSingleNode("a[1]").Attributes["href"].Value;

                    var img = outer.SelectSingleNode("a/img");

                    entry.ThumbNailUrl = img.Attributes["src"].Value;
                    entry.CommunityName = "<a href=\"" + entry.CommunityUrl + "\">" + img.Attributes["alt"].Value + "</a>";

                    var section = outer.SelectSingleNode("div");

                    entry.Title = HttpUtility.HtmlDecode(section.SelectSingleNode("h5/a").InnerText.Trim());
                    entry.LiveUrl = section.SelectSingleNode("h5/a").Attributes["href"].Value;
                    entry.StartTime = section.SelectSingleNode("p[@class='time']/small").InnerText;

                    list.Add(entry);
                }

                return list;
            } catch(RequestFailed) {

                Owner.Status = "生放送の取得に失敗しました";
                return null;
            }
        }
    }

    public class NicoNicoLiveNotifyEntry {

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
    }
}
