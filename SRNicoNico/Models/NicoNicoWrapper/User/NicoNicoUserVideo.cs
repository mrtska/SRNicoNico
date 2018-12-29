using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
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

        private readonly string UserPageUrl;

        public NicoNicoUserVideo(string url) {

            UserPageUrl = url;
            UserVideo = new ObservableSynchronizedCollection<NicoNicoSearchResultEntry>();
        }

        public async Task<int> GetUserVideoCountAsync() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(UserPageUrl + "/video");
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                var count = content.SelectSingleNode("h3").InnerText;

                var match = Regex.Match(count, @"(\d+)");

                if(match.Success) {

                    return int.Parse(match.Groups[1].Value);
                } else {

                    return 0;
                }
            } catch(RequestFailed) {

                return 0;
            }
        }

        public async Task<string> GetUserVideoAsync(int page) {

            UserVideo.Clear();
            var url = UserPageUrl + "/video?page=" + page;

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");
                var outers = content.SelectNodes("div[@class='articleBody']/div[@class='outer']");

                //終了
                if(outers == null) {

                    return "";
                }

                foreach(var outer in outers) {

                    //フォームだったら飛ばす
                    if(outer.SelectSingleNode("form") != null) {

                        continue;
                    }

                    var entry = new NicoNicoSearchResultEntry();

                    var thumb = outer.SelectSingleNode("div[@class='thumbContainer']");

                    entry.Cmsid = thumb.SelectSingleNode("a").Attributes["href"].Value.Substring(6);
                    entry.Cmsid = entry.Cmsid.Split('?')[0];

                    entry.ThumbnailUrl = thumb.SelectSingleNode("a/img").Attributes["src"].Value;
                    entry.Length = thumb.SelectSingleNode("span[@class='videoTime']").InnerText.Trim();

                    var section = outer.SelectSingleNode("div[@class='section']");

                    entry.Title = HttpUtility.HtmlDecode(section.SelectSingleNode("h5/a").InnerText.Trim());
                    entry.FirstRetrieve = section.SelectSingleNode("p").InnerText.Trim();

                    var metadata = section.SelectSingleNode("div[@class='dataOuter']/ul");

                    entry.ViewCounter = int.Parse(metadata.SelectSingleNode("li[@class='play']").InnerText.Trim().Split(':')[1].Replace(",", ""));
                    entry.CommentCounter = int.Parse(metadata.SelectSingleNode("li[@class='comment']").InnerText.Trim().Split(':')[1].Replace(",", ""));
                    entry.MylistCounter = int.Parse(metadata.SelectSingleNode("li[@class='mylist']/a").InnerText.Trim().Replace(",", ""));
                    entry.ContentUrl = "https://www.nicovideo.jp/watch/" + entry.Cmsid;

                    NicoNicoUtil.ApplyLocalHistory(entry);
                    UserVideo.Add(entry);
                }

                return "";
            } catch(RequestFailed) {

                return "投稿動画の取得に失敗しました。";
            }
        }
    }
}
