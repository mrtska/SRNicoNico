using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
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
                //取得するURL
                var url = "https://www.nicovideo.jp/my/fav/user?page=" + page;

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                foreach (var outer in doc.DocumentNode.SelectNodes("//div[@class='articleBody']/div[@class='outer']")) {

                    var user = new NicoNicoFollowUserEntry();

                    user.Name = outer.SelectSingleNode("div/h5/a").InnerText.Trim();
                    user.UserPageUrl = "https://www.nicovideo.jp" + outer.SelectSingleNode("div/h5/a").Attributes["href"].Value;

                    user.ThumbNailUrl = outer.SelectSingleNode("div/a/img").Attributes["src"].Value;

                    //説明文がないユーザーはなしにする
                    var p = outer.SelectSingleNode("div/p");
                    user.Description = p == null ? "" : p.InnerText.Trim();

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
