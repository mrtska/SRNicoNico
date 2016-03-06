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

        private const string SetEncodeHtml = @"<head>
                                                                        <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
                                                                        <style tyle=""text/css"">
                                                                            html { background: #222222; color: white; font-family: Yu Gothic UI, メイリオ, Arial; scrollbar-face-color: #666666; scrollbar-track-color: #3F3F46; scrollbar-arrow-color: #C8C8C8; scrollbar-highlight-color: #3F3F46; scrollbar-3dlight-color: #3F3F46; scrollbar-shadow-color: #3F3F46; }  
                                                                        </style>
                                                                   </head>
                                                                   <body onContextMenu=""return false;"">";
        private string LiveUrl;

        public NicoNicoLive(string url) {

            LiveUrl = url;
        }

        public NicoNicoLiveContent GetPage() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(LiveUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var content = new NicoNicoLiveContent();

                var hide = doc.DocumentNode.SelectSingleNode("//div[@class='hide']");

                if(hide != null) {


                    return null;
                }


                var ichiba = doc.DocumentNode.SelectSingleNode("//div[@class='ichiba']").InnerHtml;
                

                //タイムシフト試聴ゲート
                if(ichiba != null) {

                    var lefcom = doc.DocumentNode.SelectSingleNode("//div[@class='lefcom']");

                    if(lefcom != null) {

                        content.Type = LivePageType.TimeShiftGate;
                        content.EndTime = lefcom.InnerHtml.Replace("<br>", "").Trim();
                    } else {

                        content.Type = LivePageType.Gate;
                    }

                    content.Id = GetValue("id", ichiba);
                    content.Title = GetValue("title", ichiba);
                    content.Description = SetEncodeHtml + doc.DocumentNode.SelectSingleNode("//div[@class='text_area']").InnerHtml.Trim() + "</body>";
                    content.Description = content.Description.Replace("target=\"_blank\"", "");

                    content.ThumbNailUrl = doc.DocumentNode.SelectSingleNode("//meta[@itemprop='thumbnail']").Attributes["content"].Value;

                    var hmf = doc.DocumentNode.SelectSingleNode("//div[@class='hmf']");
                    content.StartTime = hmf.SelectSingleNode("child::div[@class='kaijo']").InnerHtml.Trim();

                    content.VisitorsAndComments = hmf.SelectSingleNode("child::div[@id='comment_area" + content.Id + "']").InnerHtml.Trim();
                    content.VisitorsAndComments = content.VisitorsAndComments.Split(new string[] { "<br>" }, StringSplitOptions.None)[1].Trim();
                    return content;
                }
                
                
                ;

                return content;
            } catch(RequestTimeout) {

                return null;
            }



        }

        //正規表現でhtmlからいろいろと抜き出す
        private string GetValue(string field, string input) {

            return Regex.Match(input, field + ":[^']+'([^']+)'").Groups[1].Value;

        }

    }


    public class NicoNicoLiveContent : NotificationObject {

        //そのページのタイプ
        public LivePageType Type { get; set; }

        //生放送ID
        public string Id { get; set; }

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

    public enum LivePageType {

        Live,
        Gate,
        TimeShift,
        TimeShiftGate

    }

}
