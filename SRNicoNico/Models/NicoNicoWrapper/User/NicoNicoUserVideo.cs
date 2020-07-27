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


    public class NicoNicoUserVideo : NotificationObject {

        #region Closed変更通知プロパティ
        private bool _Closed;

        public bool Closed {
            get { return _Closed; }
            set {
                if (_Closed == value)
                    return;
                _Closed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region UserVideo変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoSearchResultEntry> _UserVideo;

        public ObservableSynchronizedCollection<NicoNicoSearchResultEntry> UserVideo {
            get { return _UserVideo; }
            set {
                if (_UserVideo == value)
                    return;
                _UserVideo = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public int VideoCount { get; set; }

        private readonly string UserId;

        public NicoNicoUserVideo(string id) {

            UserId = id;
            UserVideo = new ObservableSynchronizedCollection<NicoNicoSearchResultEntry>();
        }

        public async Task<int> GetUserVideoCountAsync() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(UserId + "/video");
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@id='video']");

                var count = content.SelectSingleNode("h3").InnerText;

                var match = Regex.Match(count, @"(\d+)");

                if (match.Success) {

                    return int.Parse(match.Groups[1].Value);
                } else {

                    return 0;
                }
            } catch (RequestFailed) {

                return 0;
            }
        }

        public async Task<string> GetUserVideoAsync(int page) {

            UserVideo.Clear();

            try {

                var query = new GetRequestQuery($"https://nvapi.nicovideo.jp/v1/users/{UserId}/videos");
                query.AddQuery("pageSize", 25);
                query.AddQuery("page", page);
                query.AddQuery("sortKey", "registeredAt");
                query.AddQuery("sortOrder", "desc");

                var request = new HttpRequestMessage(HttpMethod.Get, query.TargetUrl);
                request.Headers.Add("X-Frontend-Id", "6");
                request.Headers.Add("X-Frontend-Version", "0");
                request.Headers.Add("X-Niconico-Language", "ja-jp");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var json = DynamicJson.Parse(a);
                if (json.meta.status != 200) {

                    return "投稿動画の取得に失敗しました";
                }
                VideoCount = (int) json.data.totalCount;

                foreach (var item in json.data.items) {

                    var entry = new NicoNicoSearchResultEntry {
                        Cmsid = item.id,
                        ThumbnailUrl = item.thumbnail.url,
                        Length = NicoNicoUtil.ConvertTime((int)item.duration),
                        Title = item.title,
                        FirstRetrieve = DateTime.Parse(item.registeredAt).ToString(),
                        ViewCounter = (int)item.count.view,
                        CommentCounter = (int)item.count.comment,
                        MylistCounter = (int)item.count.mylist,
                        ContentUrl = $"https://www.nicovideo.jp/watch/{item.id}"
                    };

                    NicoNicoUtil.ApplyLocalHistory(entry);
                    UserVideo.Add(entry);
                }

                return "";
            } catch (RequestFailed) {

                return "投稿動画の取得に失敗しました。";
            }
        }
    }
}
