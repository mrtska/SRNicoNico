using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Livet;
using System.Threading.Tasks;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Windows;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoRanking : NotificationObject {


        private readonly RankingPeriod Period;

        private readonly RankingTarget Target;

        private const string ApiBaseUrl = "http://www.nicovideo.jp/ranking/{0}/{1}/";
        private readonly string ApiUrl;


        public NicoNicoRanking(string period, string target) : this(TransPeriod(period), TransTarget(target)) {
        }


        public NicoNicoRanking(RankingPeriod period, RankingTarget target) {

            Period = period;
            Target = target;

            ApiUrl = string.Format(ApiBaseUrl, TransTarget(Target), TransPeriod(Period)) + "{0}?page={1}";
        }
        

        public async Task<NicoNicoRankingEntry> GetRankingAsync(string category, int page) {

            try {

                var ret = new NicoNicoRankingEntry();

                ret.Period = Period;
                ret.Target = Target;

                var a = await NicoNicoWrapperMain.Session.GetAsync(string.Format(ApiUrl, category, page));

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                ret.ItemList = new List<RankingItem>();

                foreach(var ranking in doc.DocumentNode.SelectNodes("//div[@class='contentBody video videoList01']/ul/li")) {

                    var item = new RankingItem();

                    item.Rank = ranking.SelectSingleNode("div[@class='rankingNumWrap']/p[@class='rankingNum']").InnerText;
                    item.RankingPoint = ranking.SelectSingleNode("div[@class='rankingNumWrap']/p[@class='rankingPt']").InnerText;

                    var wrap = ranking.SelectSingleNode("div[@class='videoList01Wrap']");

                    item.PostAt = wrap.SelectSingleNode("p[contains(@class, 'itemTime')]").InnerText;

                    item.Length = wrap.SelectSingleNode("div[@class='itemThumbBox']/span").InnerText;
                    item.ThumbNail = wrap.SelectSingleNode("div[@class='itemThumbBox']/div/a/img[2]").Attributes["data-original"].Value;

                    var content = ranking.SelectSingleNode("div[@class='itemContent']");

                    item.VideoUrl = "http://www.nicovideo.jp/" + content.SelectSingleNode("p/a").Attributes["href"].Value;
                    item.Title = content.SelectSingleNode("p/a").InnerText;

                    item.Description = content.SelectSingleNode("div[@class='wrap']/p[@class='itemDescription ranking']").InnerText;

                    var itemdata = content.SelectSingleNode("div[@class='itemData']/ul");

                    item.ViewCount = itemdata.SelectSingleNode("li[@class='count view']/span").InnerText;
                    item.CommentCount = itemdata.SelectSingleNode("li[@class='count comment']/span").InnerText;
                    item.MylistCount = itemdata.SelectSingleNode("li[@class='count mylist']/span").InnerText;

                    ret.ItemList.Add(item);
                }

                return ret;
            }catch(RequestTimeout) {

                return null;
            }
        }


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
                    throw new InvalidOperationException("そんなバカな");
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
                    throw new InvalidOperationException("そんなバカな");
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

    }

    public class NicoNicoRankingEntry {

        //ランキング集計期間
        public RankingPeriod Period { get; set; }

        //ランキング集計対象
        public RankingTarget Target { get; set; }

        public List<RankingItem> ItemList { get; set; }

    }

    public class RankingItem {

        //動画URL
        public string VideoUrl { get; set; }

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

        public void OpenWebView() {

            App.ViewModelRoot.AddWebViewTab(VideoUrl, true);
        }

        public void CopyUrl() {

            Clipboard.SetText(VideoUrl);
        }

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
