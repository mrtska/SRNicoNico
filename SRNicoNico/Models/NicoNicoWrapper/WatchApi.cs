using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Http;

using Livet;


using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoWrapper {


    //動画を見るうえで必要な情報をこのAPIだけで全て取得できるヤバいAPI
    public class WatchApi : NotificationObject {



        //動画ページを指定
        public WatchApiData GetWatchApiData(string videoPage) {

            //動画ページのhtml取得
            string html = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(videoPage).Result;


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml2(html);

            //htmlからAPIデータだけを綺麗に抜き出す すごい
            string data = doc.DocumentNode.QuerySelector("#watchAPIDataContainer").InnerHtml;

            //html特殊文字をデコードする
            data = System.Web.HttpUtility.HtmlDecode(data);

            //jsonとしてAPIデータを展開していく
            var json = DynamicJson.Parse(data);

            //GetFlvの結果
            string flv = json.flashvars.flvInfo;

            //2重にエンコードされてるので二回
            flv = System.Web.HttpUtility.UrlDecode(flv);
            flv = System.Web.HttpUtility.UrlDecode(flv);


            WatchApiData ret = new WatchApiData();

            //&で繋がれているので剥がす
            Dictionary<string, string> getFlv = flv.Split(new char[] { '&' }).ToDictionary(source => source.Substring(0, source.IndexOf('=')),
            source => Uri.UnescapeDataString(source.Substring(source.IndexOf('=') + 1)));

            ret.GetFlv = new NicoNicoGetFlvData(getFlv);

            //動画情報
            var videoDetail = json.videoDetail;

            //---情報を詰める---
            ret.Cmsid = videoDetail.id;
            ret.Title = videoDetail.title;
            ret.Thumbnail = videoDetail.thumbnail;
            ret.Description = videoDetail.description_original;
            ret.PostedAt = videoDetail.postedAt;
            ret.Length = (int) videoDetail.length;
            ret.ViewCounter = (int) videoDetail.viewCount;
            ret.CommentCounter = (int) videoDetail.commentCount;
            ret.MylistCounter = (int) videoDetail.mylistCount;
            ret.YesterdayRank = videoDetail.yesterday_rank;
            ret.HighestRank = videoDetail.highest_rank;

            //ret.Description = ret.Description.Replace("<", "[").Replace(">", "]");
            Console.WriteLine(ret.Description);

            foreach(var tag in videoDetail.tagList) {

                NicoNicoTag entry = new NicoNicoTag() {

                    Id = tag.id,
                    Tag = tag.tag,
                    Dic = tag.dic(),
                    Lck = tag.lck == "1" ? true : false,
                    Cat = tag.cat()
                };

                ret.TagList.Add(entry); 
            }
            //------


            return ret;
        }







    }


    public class WatchApiData : NotificationObject {

        //GetFlvAPIの結果
        public NicoNicoGetFlvData GetFlv { get; set; }

        public string Cmsid { get; set; }

        //動画タイトル
        public string Title { get; set; }

        //動画サムネイルURL
        public string Thumbnail { get; set; }

        //動画説明文
        public string Description { get; set; }

        //動画投稿日時
        public string PostedAt { get; set; }

        //動画時間
        public int Length { get; set; }

        //再生数
        public int ViewCounter { get; set; }

        //コメント数
        public int CommentCounter { get; set; }

        //マイリスト数
        public int MylistCounter { get; set; }

        //昨日のランキング
        public string YesterdayRank { get; set; }

        //最高ランキング
        public string HighestRank { get; set; }

        //タグリスト
        public List<NicoNicoTag> TagList = new List<NicoNicoTag>();
    }

    //タグ情報
    public class NicoNicoTag : NotificationObject {

        //タグID
        public string Id { get; set; }

        //タグ名
        public string Tag { get; set; }

        //大百科が存在するか
        public bool Dic { get; set; }

        //ロックされているか
        public bool Lck { get; set; }

        //カテゴリタグか否か
        public bool Cat { get; set; }
    }


    public class NicoNicoGetFlvData : NotificationObject {

        //getflvAPIで取得できるデータ

        //スレッドID
        public uint ThreadID { get; internal set; }

        //長さ
        public uint Length { get; internal set; }

        //動画URL
        public string VideoUrl { get; internal set; }

        //コメントサーバーURL
        public Uri CommentServerUrl { get; internal set; }

        //サブコメントサーバーURL
        public Uri SubCommentServerUrl { get; internal set; }

        //非公開理由
        public int ClosedReason { get; internal set; }

        public NicoNicoGetFlvData(Dictionary<string, string> wwwData) {

            this.ThreadID = uint.Parse(wwwData["thread_id"]);
            this.Length = uint.Parse(wwwData["l"]);
            this.VideoUrl = wwwData["url"];
            this.CommentServerUrl = new Uri(wwwData["ms"]);
            this.SubCommentServerUrl = new Uri(wwwData["ms_sub"]);
        }

    }
}
