using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLiveNotify : NotificationObject {

        private const string LiveUrl = "https://www.nicovideo.jp/my/live";

        #region NowLiveList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoLiveNotifyEntry> _NowLiveList;

        public ObservableSynchronizedCollection<NicoNicoLiveNotifyEntry> NowLiveList {
            get { return _NowLiveList; }
            set {
                if (_NowLiveList == value)
                    return;
                _NowLiveList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoLiveNotify() {

            NowLiveList = new ObservableSynchronizedCollection<NicoNicoLiveNotifyEntry>();
        }

        public async Task<string> GetLiveInformationAsync() {

            try {

                NowLiveList.Clear();

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

                    NowLiveList.Add(entry);
                }
                return "";
            } catch(RequestFailed) {

                return "生放送の取得に失敗しました";
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
