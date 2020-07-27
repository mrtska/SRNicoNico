using Codeplex.Data;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFollowChannel : NotificationObject {

        #region ChannelCount変更通知プロパティ
        private int _ChannelCount = -2;

        public int ChannelCount {
            get { return _ChannelCount; }
            set { 
                if (_ChannelCount == value)
                    return;
                _ChannelCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region ChannelList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoFollowChannelEntry> _ChannelList;

        public ObservableSynchronizedCollection<NicoNicoFollowChannelEntry> ChannelList {
            get { return _ChannelList; }
            set { 
                if (_ChannelList == value)
                    return;
                _ChannelList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoFollowChannel() {

            ChannelList = new ObservableSynchronizedCollection<NicoNicoFollowChannelEntry>();
        }

        //自分がフォローしているチャンネルの数を取得する
        public async Task<string> FetchFollowedChannelCountAsync() {

            try {

                //取得するURL
                var url = "https://www.nicovideo.jp/my/channel";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favChannel']/h3/span");

                if (count == null) {

                    ChannelCount = 0;
                } else {

                    ChannelCount = int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
                }

                return "";
            } catch (RequestFailed) {

                return "フォローしているチャンネルの数の取得に失敗しました";
            }
        }

        public async Task<string> GetFollowedChannelAsync(int page) {

            try {

                ChannelList.Clear();

                var url = "https://public.api.nicovideo.jp/v1/user/followees/channels.json?limit=25&offset=" + (25 * (page - 1));
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var json = DynamicJson.Parse(a);
                if (json.meta.status != 200) {

                    return "フォローしているチャンネルの取得に失敗しました";
                }
                ChannelCount = (int)json.meta.total;

                foreach (var item in json.data) {

                    var channel = new NicoNicoFollowChannelEntry();

                    channel.Title = item.name;
                    channel.ChannelUrl = item.url;
                    channel.ThumbNailUrl = item.thumbnailUrl;

                    ChannelList.Add(channel);
                }

                return "";
            } catch (RequestFailed f) {

                if (f.FailedType == FailedType.Failed) {

                    return "フォローしているチャンネルの取得に失敗しました";
                } else {

                    return "フォローしているチャンネルの取得がタイムアウトになりました";
                }
            }
        }
    }
    //フォローしているチャンネル
    public class NicoNicoFollowChannelEntry {

        //チャンネル名
        public string Title { get; set; }

        //チャンネルURL
        public string ChannelUrl { get; set; }

        //チャンネルサムネ
        public string ThumbNailUrl { get; set; }

        public void Open() {

            NicoNicoOpener.Open(ChannelUrl);
        }
    }
}
