using Codeplex.Data;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLiveNotify : NotificationObject {

        private const string FollowUrl = "https://live.nicovideo.jp/follow";

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

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(FollowUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var data = doc.DocumentNode.SelectSingleNode("//script[@id='embedded-data']");

                if (data == null) {

                    return "生放送の取得に失敗しました";
                }

                var json = DynamicJson.Parse(HttpUtility.HtmlDecode(data.Attributes["data-props"].Value));

                foreach (var item in json.view.followedProgramListSection.programList) {

                    var entry = new NicoNicoLiveNotifyEntry();

                    switch (item.providerType) {
                        case "official":

                            entry.CommunityUrl = $"https://ch.nicovideo.jp/{item.socialGroup.id}";
                            break;
                        case "channel":

                            entry.CommunityUrl = $"https://ch.nicovideo.jp/{item.socialGroup.id}";
                            break;
                        case "community":
                            entry.CommunityUrl = $"https://com.nicovideo.jp/community/{item.socialGroup.id}";
                            break;
                    }
                    entry.CommunityName = "<a href=\"" + entry.CommunityUrl + "\">" + item.socialGroup.name + "</a>";

                    entry.ThumbNailUrl = item.thumbnailUrl;
                    if (string.IsNullOrEmpty(entry.ThumbNailUrl)) {

                        entry.ThumbNailUrl = item.socialGroup.thumbnailUrl;
                    }

                    entry.Title = item.title;
                    entry.LiveUrl = item.watchPageUrl;
                    entry.StartTime = UnixTime.FromUnixTime((long)item.beginAt / 1000).ToString();

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
