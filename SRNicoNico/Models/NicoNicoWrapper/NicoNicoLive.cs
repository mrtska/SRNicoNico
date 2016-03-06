using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Codeplex.Data;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLive : NotificationObject {

        private const string GetPlayerStatusApiUrl = "http://live.nicovideo.jp/api/getplayerstatus";

        private const string GetThreadsApiUrl = "http://live.nicovideo.jp/api/getthreads";

        private const string HeartBeatApiUrl = "http://live.nicovideo.jp/api/heartbeat";


        private string LiveUrl;

        public NicoNicoLive(string url) {

            LiveUrl = url;

        }

        public void GetPage() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(LiveUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var ichiba = doc.DocumentNode.SelectSingleNode("//div[@class='ichiba']").InnerHtml;

                //タイムシフト試聴ゲート
                if(ichiba != null) {


                }

                var desc = new Regex("description:[^']+'([^']+)'").Match(ichiba).Groups[1].Value;
                
                ;


            } catch(RequestTimeout) {


            }



        }
    }


    public class NicoNicoLiveContent : NotificationObject {

        //生放送タイトル
        public string Title { get; set; }

        //生放送説明文
        public string Description { get; set; }

        //サムネイル
        public string ThumbNailUrl { get; set; }

        //開演時間とか開場時間とか日時とかまとめたやつ
        public string StartTime { get; set; }

        //終わった時間 (タイムシフト試聴時のみ)
        public string EndTime { get; set; }

        //来場者数とコメント数
        public string VisitorsAndComments { get; set; }

        //登録タグ
        public IList<string> TagEntries { get; set; }

    }

}
