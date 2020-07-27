using Codeplex.Data;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFollowUser : NotificationObject {

        #region UserCount変更通知プロパティ
        private int _UserCount = -2;

        public int UserCount {
            get { return _UserCount; }
            set { 
                if (_UserCount == value)
                    return;
                _UserCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region UserList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoFollowUserEntry> _UserList;

        public ObservableSynchronizedCollection<NicoNicoFollowUserEntry> UserList {
            get { return _UserList; }
            set { 
                if (_UserList == value)
                    return;
                _UserList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoFollowUser() {

            UserList = new ObservableSynchronizedCollection<NicoNicoFollowUserEntry>();
        }

        //自分がフォローしているユーザーの数を取得する
        public async Task<string> FetchFollowedUserCountAsync() {

            try {

                //取得するURL
                var url = "https://www.nicovideo.jp/my/fav/user";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favUser']/h3/span");

                if (count == null) {

                    UserCount = 0;
                } else {

                    UserCount = int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
                }
                return "";
            } catch (RequestFailed) {

                return "フォローしているユーザーの数の取得に失敗しました";
            }
        }

        public async Task<string> GetFollowedUserAsync(int page) {

            try {

                UserList.Clear();

                var query = new GetRequestQuery("https://nvapi.nicovideo.jp/v1/users/me/following/users");
                query.AddQuery("pageSize", 600);
                query.AddQuery("page", 1);

                var request = new HttpRequestMessage(HttpMethod.Get, query.TargetUrl);
                request.Headers.Add("X-Frontend-Id", "6");
                request.Headers.Add("X-Frontend-Version", "0");
                request.Headers.Add("X-Niconico-Language", "ja-jp");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var json = DynamicJson.Parse(a);
                if (json.meta.status != 200) {

                    return "フォローしているユーザーの取得に失敗しました";
                }

                foreach (var entry in json.data.items) {

                    var user = new NicoNicoFollowUserEntry {
                        Name = entry.nickname,
                        UserPageUrl = $"https://www.nicovideo.jp/user/{entry.id}",
                        ThumbNailUrl = entry.icons.small,
                        Description = entry.strippedDescription
                    };

                    UserList.Add(user);
                }

                return "";
            } catch (RequestFailed f) {

                if (f.FailedType == FailedType.Failed) {

                    return "フォローしているユーザーの取得に失敗しました";
                } else {

                    return "フォローしているユーザーの取得がタイムアウトになりました";
                }
            }
        }
    }
    //フォローしているユーザー
    public class NicoNicoFollowUserEntry {

        //ユーザーページURL
        public string UserPageUrl { get; set; }

        //お気に入りユーザーの名前
        public string Name { get; set; }

        //簡易説明文
        public string Description { get; set; }

        //サムネイルURL
        public string ThumbNailUrl { get; set; }

        public void Open() {

            NicoNicoOpener.Open(UserPageUrl);
        }
    }
}
