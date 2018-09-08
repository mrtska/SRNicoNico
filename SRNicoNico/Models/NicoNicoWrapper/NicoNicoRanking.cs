using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoRanking : NotificationObject {

        private static string TransPeriod(RankingPeriod period) {

            switch(period) {
                case RankingPeriod.Hourly:
                    return "hourly";
                case RankingPeriod.Daily:
                    return "daily";
                case RankingPeriod.Weekly:
                    return "weekly";
                case RankingPeriod.Monthly:
                    return "monthly";
                case RankingPeriod.Total:
                    return "total";
                default:
                    throw new ArgumentException("そんなバカな");
            }
        }

        private static RankingPeriod TransPeriod(string period) {

            switch(period) {
                case "毎時":
                    return RankingPeriod.Hourly;
                case "24時間":
                    return RankingPeriod.Daily;
                case "週間":
                    return RankingPeriod.Weekly;
                case "月間":
                    return RankingPeriod.Monthly;
                case "合計":
                    return RankingPeriod.Total;
                default:
                    throw new ArgumentException("そんなバカな");
            }
        }

        private static string TransTarget(RankingTarget target) {

            switch(target) {
                case RankingTarget.View:
                    return "view";
                case RankingTarget.Comment:
                    return "res";
                case RankingTarget.Mylist:
                    return "mylist";
                case RankingTarget.All:
                    return "fav";
                default:
                    throw new ArgumentException("そんなバカな");
            }
        }
        private static RankingTarget TransTarget(string target) {

            switch(target) {
                case "再生":
                    return RankingTarget.View;
                case "コメント":
                    return RankingTarget.Comment;
                case "マイリスト":
                    return RankingTarget.Mylist;
                case "総合":
                    return RankingTarget.All;
                default:
                    throw new ArgumentException("そんなバカな");
            }
        }

        private RankingPeriod Period;

        private RankingTarget Target;

        private const string ApiBaseUrl = "http://www.nicovideo.jp/ranking/{0}/{1}/";
        private string ApiUrl = string.Empty;

        #region RankingList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoRankingEntry> _RankingList;

        public ObservableSynchronizedCollection<NicoNicoRankingEntry> RankingList {
            get { return _RankingList; }
            set { 
                if (_RankingList == value)
                    return;
                _RankingList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsPreparing変更通知プロパティ
        private bool _IsPreparing = false;

        public bool IsPreparing {
            get { return _IsPreparing; }
            set {
                if (_IsPreparing == value)
                    return;
                _IsPreparing = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoRanking(string period, string target) : this(TransPeriod(period), TransTarget(target)) {
        }

        public NicoNicoRanking(RankingPeriod period, RankingTarget target) {

            RankingList = new ObservableSynchronizedCollection<NicoNicoRankingEntry>();
            SetRankingSpan(period, target);
        }

        private void SetRankingSpan(RankingPeriod period, RankingTarget target) {

            Period = period;
            Target = target;

            ApiUrl = string.Format(ApiBaseUrl, TransTarget(Target), TransPeriod(Period)) + "{0}?page={1}";
        }
 
        public async Task<string> GetRankingAsync(string category, int page) {

            try {

                RankingList.Clear();

                IsPreparing = false;
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(string.Format(ApiUrl, category, page));

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var nodes = doc.DocumentNode.SelectNodes("//div[@class='contentBody video videoList01']/ul/li");
                if(nodes == null) {

                    IsPreparing = true;
                    return null;
                }
                
                //ランキングページから各種データをXPathで取得
                foreach(var ranking in nodes) {

                    var item = new NicoNicoRankingEntry();

                    item.Rank = ranking.SelectSingleNode("div[@class='rankingNumWrap']/p[@class='rankingNum']").InnerText;
                    item.RankingPoint = ranking.SelectSingleNode("div[@class='rankingNumWrap']/p[@class='rankingPt']").InnerText;

                    var wrap = ranking.SelectSingleNode("div[@class='videoList01Wrap']");

                    item.PostAt = wrap.SelectSingleNode("p[contains(@class, 'itemTime')]").InnerText;

                    item.Length = wrap.SelectSingleNode("div[@class='itemThumbBox']/span").InnerText;
                    item.ThumbNail = wrap.SelectSingleNode("div[@class='itemThumbBox']/div/a/img").Attributes["data-original"].Value;

                    var content = ranking.SelectSingleNode("div[@class='itemContent']");

                    item.ContentUrl = "http://www.nicovideo.jp/" + content.SelectSingleNode("p/a").Attributes["href"].Value;
                    item.Title = content.SelectSingleNode("p/a").InnerText;

                    item.Description = content.SelectSingleNode("div[@class='wrap']/p[@class='itemDescription ranking']").InnerText;

                    var itemdata = content.SelectSingleNode("div[@class='itemData']/ul");

                    item.ViewCount = itemdata.SelectSingleNode("li[@class='count view']/span").InnerText;
                    item.CommentCount = itemdata.SelectSingleNode("li[@class='count comment']/span").InnerText;
                    item.MylistCount = itemdata.SelectSingleNode("li[@class='count mylist']/span").InnerText;
                    

                    NicoNicoUtil.ApplyLocalHistory(item);

                    //そのページのランキングは存在しないか準備中
                    if (item.Rank == "1" && page != 1) {

                        IsPreparing = true;
                        break;
                    }

                    RankingList.Add(item);
                }
                return "";
            } catch(RequestFailed) {

                return "ランキングの取得に失敗しました";
            }
        }
    }
    public class NicoNicoRankingEntry : IWatchable {

        //動画URL
        public bool IsWatched { get; set; }

        public string ContentUrl { get; set; }

        //動画サムネイル
        public string ThumbNail { get; set; }

        //動画タイトル
        public string Title { get; set; }

        //動画説明文
        public string Description { get; set; }

        //動画投稿日
        public string PostAt { get; set; }

        //順位
        public string Rank { get; set; }

        //ランキングポイント
        public string RankingPoint { get; set; }

        //動画の長さ
        public string Length { get; set; }

        //再生数
        public string ViewCount { get; set; }

        //コメント数
        public string CommentCount { get; set; }

        //マイリスト数
        public string MylistCount { get; set; }

        // 広告ポイント数
        public string AdsCount { get; set; }
    }

    public enum RankingPeriod {

        Hourly,
        Daily,
        Weekly,
        Monthly,
        Total
    }
    public enum RankingTarget {

        All,
        View,
        Comment,
        Mylist
    }
}
