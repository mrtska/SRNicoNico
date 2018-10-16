using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFollowCommunity : NotificationObject {

        #region CommunityCount変更通知プロパティ
        private int _CommunityCount = -2;

        public int CommunityCount {
            get { return _CommunityCount; }
            set { 
                if (_CommunityCount == value)
                    return;
                _CommunityCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommunityList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoFollowCommunityEntry> _CommunityList;

        public ObservableSynchronizedCollection<NicoNicoFollowCommunityEntry> CommunityList {
            get { return _CommunityList; }
            set { 
                if (_CommunityList == value)
                    return;
                _CommunityList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoFollowCommunity() {

            CommunityList = new ObservableSynchronizedCollection<NicoNicoFollowCommunityEntry>();
        }

        //自分がフォローしているコミュニティの数を取得する
        public async Task<string> FetchFollowedCommunityCountAsync() {

            try {

                //取得するURL
                var url = "https://www.nicovideo.jp/my/community";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favCommunity']/h3/span");

                if (count == null) {

                    CommunityCount = 0;
                } else {

                    CommunityCount = int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
                }
                return "";
            } catch (RequestFailed) {

                return "フォローしているコミュニティの数の取得に失敗しました";
            }
        }

        public async Task<string> GetFollowedCommunityAsync(int page) {

            try {

                CommunityList.Clear();

                //取得するURL
                var url = "https://www.nicovideo.jp/my/community?page=" + page;

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                foreach (var outer in doc.DocumentNode.SelectNodes("//div[@class='articleBody']/div[@class='outer']")) {

                    var community = new NicoNicoFollowCommunityEntry();

                    community.Title = HttpUtility.HtmlDecode(outer.SelectSingleNode("div/h5/a").InnerText.Trim());
                    community.CommunityUrl = outer.SelectSingleNode("div/h5/a").Attributes["href"].Value;

                    community.ThumbNailUrl = outer.SelectSingleNode("div/a/img").Attributes["src"].Value;

                    var data = outer.SelectSingleNode("div/ul");

                    community.Data = data.SelectSingleNode("li[1]").InnerText.Trim();

                    var p = outer.SelectSingleNode("div/p");
                    community.Description = p.Attributes["class"] == null ? HttpUtility.HtmlDecode(p.InnerText.Trim()) : "";

                    CommunityList.Add(community);
                }

                return "";
            } catch (RequestFailed f) {

                if (f.FailedType == FailedType.Failed) {

                    return "フォローしているコミュニティの取得に失敗しました";
                } else {

                    return "フォローしているコミュニティの取得がタイムアウトになりました";
                }
            }
        }
    }

    //フォローしているコミュニティ
    public class NicoNicoFollowCommunityEntry {

        //コミュニティ名
        public string Title { get; set; }

        //コミュニティURL
        public string CommunityUrl { get; set; }

        //コミュニティサムネ
        public string ThumbNailUrl { get; set; }

        //コミュニティデータ
        public string Data { get; set; }

        public string Description { get; set; }

        public void Open() {

            NicoNicoOpener.Open(CommunityUrl);
        }
    }
}
