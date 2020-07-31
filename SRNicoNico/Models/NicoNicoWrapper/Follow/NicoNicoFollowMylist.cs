using Codeplex.Data;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFollowMylist : NotificationObject {


        #region MylistCount変更通知プロパティ
        private int _MylistCount = -2;

        public int MylistCount {
            get { return _MylistCount; }
            set {
                if (_MylistCount == value)
                    return;
                _MylistCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region MylistList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoFollowMylistEntry> _MylistList;

        public ObservableSynchronizedCollection<NicoNicoFollowMylistEntry> MylistList {
            get { return _MylistList; }
            set {
                if (_MylistList == value)
                    return;
                _MylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoFollowMylist() {

            MylistList = new ObservableSynchronizedCollection<NicoNicoFollowMylistEntry>();
        }

        //自分がフォローしているマイリストの数を取得する
        public async Task<string> FetchFollowedMylistCountAsync() {

            try {

                //取得するURL
                var url = "https://www.nicovideo.jp/my/fav/mylist";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favMylist']/h3/span");

                if (count == null) {

                    MylistCount = 0;
                } else {

                    MylistCount = int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
                }
                return "";
            } catch (RequestFailed) {

                MylistCount = -1;
                return "フォローしているマイリストの数の取得に失敗しました";
            }
        }

        public async Task<string> GetFollowedMylistAsync(int page) {

            try {
                MylistList.Clear();
                var query = new GetRequestQuery("https://nvapi.nicovideo.jp/v1/users/me/following/mylists");
                query.AddQuery("sampleItemCount", 1);

                var request = new HttpRequestMessage(HttpMethod.Get, query.TargetUrl);
                request.Headers.Add("X-Frontend-Id", "6");
                request.Headers.Add("X-Frontend-Version", "0");
                request.Headers.Add("X-Niconico-Language", "ja-jp");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var json = DynamicJson.Parse(a);
                if (json.meta.status != 200) {

                    return "フォローしているユーザーの取得に失敗しました";
                }

                foreach (var item in json.data.mylists) {

                    if (item.status == "deleted") {

                        continue;
                    }

                    var detail = item.detail;

                    var mylist = new NicoNicoFollowMylistEntry {
                        Title = detail.name,
                        MylistPageUrl = $"https://www.nicovideo.jp/mylist/{detail.id}",
                        Description = detail.description
                    };

                    foreach (var sample in detail.sampleItems) {

                        mylist.HasVideoLink = true;
                        mylist.ThumbNailUrl = sample.video.thumbnail.url;
                        mylist.VideoTitle = sample.video.title;
                        mylist.VideoUrl = $"https://www.nicovideo.jp/watch/{sample.watchId}";
                        mylist.PostedAt = DateTime.Parse(sample.video.registeredAt).ToString();
                    }

                    MylistList.Add(mylist);
                }

                return "";
            } catch (RequestFailed f) {

                if (f.FailedType == FailedType.Failed) {

                    return "フォローしているマイリストの取得に失敗しました";
                } else {

                    return "フォローしているマイリストの取得がタイムアウトになりました";
                }
            }
        }
    }

    //フォローしているマイリスト
    public class NicoNicoFollowMylistEntry {

        //マイリストタイトル
        public string Title { get; set; }

        //マイリストページURL
        public string MylistPageUrl { get; set; }

        //簡易説明文
        public string Description { get; set; }

        //代表動画があるかどうか
        public bool HasVideoLink { get; set; }

        //代表動画のサムネ
        public string ThumbNailUrl { get; set; }

        //代表動画URL
        public string VideoUrl { get; set; }

        //代表動画タイトル
        public string VideoTitle { get; set; }

        //代表動画投稿日時
        public string PostedAt { get; set; }

        //マイリストが削除されているか
        public bool Deleted { get; set; }

        public void Open() {

            if (!Deleted) {

                NicoNicoOpener.Open(MylistPageUrl);
            }
        }
    }
}
