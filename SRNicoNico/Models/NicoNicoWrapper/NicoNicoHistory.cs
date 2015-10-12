using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using Livet;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

using SRNicoNico.ViewModels;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoHistory : NotificationObject {
        
        //視聴履歴URL やはりこれが一番コストが安い
        private const string HistoryUrl = "http://www.nicovideo.jp/my/history";
        
        //視聴履歴を返すAPI ちょっと情報が足りない
        private const string HistroyApiUrl = "http://www.nicovideo.jp/api/videoviewhistory/list";

        //動画情報を取得するVitaAPI コミュ限動画なども取得できる コストが高い
        private const string VideoDataApiUrl = "http://api.ce.nicovideo.jp/nicoapi/v1/video.info?__format=json&v=";

        //動画情報を一括で取得するVitaAPI コミュ限動画などは取得出来ない
        private const string VideoDataArrayApiUrl = "http://api.ce.nicovideo.jp/nicoapi/v1/video.array?__format=json&v=";

        //動画情報を取得するXPath
        private const string GetVideoInfoXPath = "//div[parent::div[@id='historyList']]";



        private HistoryViewModel History;

        public NicoNicoHistory(HistoryViewModel vm) {

            History = vm;
        }




        //たまに失敗するから注意
        public List<NicoNicoHistoryData> GetHistroyData() {


            History.Status = "視聴履歴取得中";
            int retry = 1;
            start:
            //たまに失敗する
            
            HttpResponseMessage result = NicoNicoWrapperMain.GetSession().GetResponseAsync(HistoryUrl).Result;

            //失敗
            if(result.StatusCode == HttpStatusCode.ServiceUnavailable) {

                History.Status = "視聴履歴取得失敗(" + retry++ + "回)";
                goto start;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml2(result.Content.ReadAsStringAsync().Result);

            var info = doc.DocumentNode.SelectNodes(GetVideoInfoXPath);

            List<NicoNicoHistoryData> ret = new List<NicoNicoHistoryData>();

            if(info == null) {

                History.Status = "視聴履歴はありません。";

                return ret;
            }

            foreach(HtmlNode node in info) {

                NicoNicoHistoryData data = new NicoNicoHistoryData();

                //---各種情報取得---
                data.ThumbnailUrl = node.SelectSingleNode("child::div[@class='thumbContainer']/a/img").Attributes["src"].Value;

                //削除されていない動画だったら
                if(!data.ThumbnailUrl.Contains("deleted")) {

                    data.Length = node.SelectSingleNode("child::div[@class='thumbContainer']/span").InnerText;
                } else {

                    data.ThumbnailUrl = "http://www.nicovideo.jp/" + data.ThumbnailUrl;
                }
                data.WatchDate = node.SelectSingleNode("child::div[@class='section']/p").ChildNodes["#text"].InnerText;
                data.WatchCount = node.SelectSingleNode("child::div[@class='section']/p/span").InnerText;

                data.Title = node.SelectSingleNode("child::div[@class='section']/h5/a").InnerText;
                data.Id = node.SelectSingleNode("child::div[@class='section']/h5/a").Attributes["href"].Value.Substring(6);

                data.ViewCounter = node.SelectSingleNode("child::div[@class='section']/ul[@class='metadata']/li[@class='play']").InnerText;
                data.CommentCounter = node.SelectSingleNode("child::div[@class='section']/ul[@class='metadata']/li[@class='comment']").InnerText;
                data.MylistCounter = node.SelectSingleNode("child::div[@class='section']/ul[@class='metadata']/li[@class='mylist']/a").InnerText;
                data.PostDate = node.SelectSingleNode("child::div[@class='section']/ul[@class='metadata']/li[@class='posttime']").InnerText;
                

                ret.Add(data);
            }

            History.Status = "視聴履歴取得完了";


            return ret;
        }
    }

    public class NicoNicoHistoryData {

        //動画ID
        public string Id { get; set; }

        //動画サムネイル
        public string ThumbnailUrl { get; set; }

        //動画タイトル
        public string Title { get; set; }

        //動画の長さ
        public string Length { get; set; }

        //視聴日時
        public string WatchDate { get; set; }

        //視聴回数
        public string WatchCount { get; set; }

        //再生数
        public string ViewCounter { get; set; }

        //コメント数
        public string CommentCounter { get; set; }

        //マイリスト数
        public string MylistCounter { get; set; }

        //投稿日時
        public string PostDate { get; set; }
    }

}
