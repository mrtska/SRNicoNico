using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Livet;
using HtmlAgilityPack;

namespace SRNicoNico.Models.NicoNicoWrapper {


    public class NicoNicoNicoRepo : NotificationObject {

       
        //すべて
        private const string NicoRepoAllAPI = @"http://www.nicovideo.jp/my/top?innerPage=1&mode=next_page";

        //自分
        private const string NicoRepoMyselfAPI = @"http://www.nicovideo.jp/my/top/myself?innerPage=1&mode=next_page";

        //お気に入りユーザー
        private const string NicoRepoUserAPI = @"http://www.nicovideo.jp/my/top/user?innerPage=1&mode=next_page";

        //チャンネル＆コミュニティ
        private const string NicoRepoChComAPI = @"http://www.nicovideo.jp/my/top/chcom?innerPage=1&mode=next_page";

        //マイリスト
        private const string NicoRepoMylistAPI = @"http://www.nicovideo.jp/my/top?innerPage=1&mode=next_page";

        //htmlからいろいろ持ってくるXPath
        private const string IconUrlXPath = "/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-author ']/a/img";
        private const string TitleXPath = "/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[@class='log-body']";
        private const string ThumbnailXPath = "/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-target-thumbnail']/a/img";
        private const string DescriptionXPath = "/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-target-info']/a";
        private const string TimeXPath = "/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-footer']/div/a[@class='log-footer-date ']/time|/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-footer']/div/a[@class='log-footer-date hot']/time";
        //ID ユーザー定義ニコレポリストID
        private string Id;

        //過去ページへのURL
        private string NextUrl;

        //取得したい二コレポのID allとかmyselfとかuserとかいろいろ
        public NicoNicoNicoRepo(string id) {


            Id = id;
        }


        public NicoNicoNicoRepoData GetNicoRepo() {

            //APIと言うのか謎
            var api = @"http://www.nicovideo.jp/my/top/" + Id + @"?innerPage=1&mode=next_page";
           
            //html
            var html = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(api).Result;

            NicoNicoNicoRepoData data = new NicoNicoNicoRepoData();

            //XPathでhtmlから抜き出す
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            //ニコレポが存在しなかったら存在しないというエントリを返す
            if(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div").Attributes["class"].Value.Equals("empty")) {

                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                entry.ImageUrl = entry.IconUrl = null;
                entry.Description = "ニコレポが存在しません。";
                data.DataCollection.Add(entry);

                return data;
            }
            ;

            //アイコンURL取得
            var iconUrls = doc.DocumentNode.SelectNodes(IconUrlXPath);

            //ニコレポタイトル取得
            var titles = doc.DocumentNode.SelectNodes(TitleXPath);

            //ニコレポサムネイル取得
            var thumbNails = doc.DocumentNode.SelectNodes(ThumbnailXPath);

            //ニコレポ取得
            var descriptions = doc.DocumentNode.SelectNodes(DescriptionXPath);

            //時間取得
            var nicorepoTimes = doc.DocumentNode.SelectNodes(TimeXPath);


            //XPathで取得したデータをエントリに詰める
            for(int i = 0; i < iconUrls.Count; i++) {

                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                entry.IconUrl = iconUrls[i].Attributes["data-original"].Value;

                entry.Title = titles[i].InnerText.Trim();

                entry.ImageUrl = thumbNails[i].Attributes["data-original"].Value;

                entry.VideoUrl = descriptions[i].Attributes["href"].Value;

                entry.Description = descriptions[i].InnerText.Trim();

                entry.Time = nicorepoTimes[i].InnerText.Trim();

                data.DataCollection.Add(entry);
            }
                
            //ニコレポが中途半端に終わっていたら
            if(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='no-next-page']") != null) {

                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                NextUrl = "end";
                entry.ImageUrl = entry.IconUrl = null;
                entry.Description = "これより過去のニコレポは存在しません。";
                data.DataCollection.Add(entry);
            } else {

                //過去ニコレポのURL
                string attr = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='next-page']/a").Attributes["href"].Value);
                NextUrl = @"http://www.nicovideo.jp" + attr;
            }

            return data;
        }

        //ニコレポの
        public NicoNicoNicoRepoData NextNicoRepo() {

            //もう過去の二コレポは存在しない
            if(NextUrl.Equals("end")) {

                return null;
            }

            //APIと言うのか謎
            var api = NextUrl;

            //html
            var html = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(api).Result;

            NicoNicoNicoRepoData data = new NicoNicoNicoRepoData();

            //XPathでhtmlから抜き出す
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            //アイコンURL取得
            var iconUrls = doc.DocumentNode.SelectNodes(IconUrlXPath);

            //ニコレポタイトル取得
            var titles = doc.DocumentNode.SelectNodes(TitleXPath);

            //ニコレポサムネイル取得
            var thumbNails = doc.DocumentNode.SelectNodes(ThumbnailXPath);

            //ニコレポ取得
            var descriptions = doc.DocumentNode.SelectNodes(DescriptionXPath);

            //時間取得
            var nicorepoTimes = doc.DocumentNode.SelectNodes(TimeXPath);


            //エントリに詰める
            for(int i = 0; i < iconUrls.Count; i++) {

                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                entry.IconUrl = iconUrls[i].Attributes["data-original"].Value;

                entry.Title = titles[i].InnerText.Trim();

                entry.ImageUrl = thumbNails[i].Attributes["data-original"].Value;

                entry.VideoUrl = descriptions[i].Attributes["href"].Value;

                entry.Description = descriptions[i].InnerText.Trim();

                entry.Time = nicorepoTimes[i].InnerText.Trim();

                data.DataCollection.Add(entry);
            }

            if(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='no-next-page']") != null) {


                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                NextUrl = "end";
                entry.ImageUrl = entry.IconUrl = null;
                entry.Description = "これより過去のニコレポは存在しません。";
                data.DataCollection.Add(entry);


            } else {

                //過去ニコレポのURL
                string attr = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='next-page']/a").Attributes["href"].Value);
                NextUrl = @"http://www.nicovideo.jp" + attr;
            }



            return data;
        }



    }

    public class NicoNicoNicoRepoData : NotificationObject {




        public ObservableSynchronizedCollection<NicoNicoNicoRepoDataEntry> DataCollection = new ObservableSynchronizedCollection<NicoNicoNicoRepoDataEntry>();

    }


    public class NicoNicoNicoRepoDataEntry : NotificationObject {


        //ニコレポタイトル
        public string Title { get; set; }

        //内容
        public string Description { get; set; }

        //ビデオとは限らないがURL
        public string VideoUrl { get; set; }

        //ユーザーとかコミュニティのアイコンのURL
        public string IconUrl { get; set; }

        //動画とかその辺のURL
        public string ImageUrl { get; set; }

        //ニコレポ日時
        public string Time { get; set; }






    }
}
