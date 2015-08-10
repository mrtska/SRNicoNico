using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Livet;
using HtmlAgilityPack;

namespace SRNicoNico.Models.NicoNicoWrapper {


    public class NicoNicoNicoRepo : NotificationObject {




        static NicoNicoNicoRepo() {

            

        }


       
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





        //Otherタイプ ユーザー定義二コレポリストID
        private string Id;

        //過去ページへのURL
        private string NextUrl;





        public NicoNicoNicoRepo(string id) {


            Id = id;
        }


        public NicoNicoNicoRepoData GetNicoRepo() {


            var api = @"http://www.nicovideo.jp/my/top/" + Id + @"?innerPage=1&mode=next_page";
           


            var all = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(api).Result;

            NicoNicoNicoRepoData data = new NicoNicoNicoRepoData();


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(all);

            if(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div").Attributes["class"].Value.Equals("empty")) {

                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                entry.Description = "二コレポが存在しません。";
                data.DataCollection.Add(entry);

                return data;
            }
            ;




            //アイコンURL取得
            var iconUrls = doc.DocumentNode.SelectNodes("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-author ']/a/img");

            //二コレポタイトル取得
            var titles = doc.DocumentNode.SelectNodes("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[@class='log-body']");

            //二コレポサムネイル取得
            var thumbNails = doc.DocumentNode.SelectNodes("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-target-thumbnail']/a/img");

            //二コレポ取得
            var descriptions = doc.DocumentNode.SelectNodes("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-target-info']/a");

            //時間取得
            var nicorepoTimes = doc.DocumentNode.SelectNodes(@"/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-footer']/div/a[@class='log-footer-date ']/time|/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='timeline']/div/div[@class='log-content']/div[2]/div[@class='log-footer']/div/a[@class='log-footer-date hot']/time");



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

                entry.Description = "これより過去の二コレポは存在しません。";
                data.DataCollection.Add(entry);


            } else {

                //過去二コレポのURL
                string attr = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='next-page']/a").Attributes["href"].Value);
                NextUrl = @"http://www.nicovideo.jp" + attr;
            }



            return data;

        }

        public void NextNicoRepo() {


        }



    }

    public class NicoNicoNicoRepoData : NotificationObject {




        public ObservableSynchronizedCollection<NicoNicoNicoRepoDataEntry> DataCollection = new ObservableSynchronizedCollection<NicoNicoNicoRepoDataEntry>();

    }


    public class NicoNicoNicoRepoDataEntry : NotificationObject {


        //二コレポタイトル
        public string Title { get; set; }

        //内容
        public string Description { get; set; }

        //ビデオとは限らないがURL
        public string VideoUrl { get; set; }

        //ユーザーとかコミュニティのアイコンのURL
        public string IconUrl { get; set; }

        //動画とかその辺のURL
        public string ImageUrl { get; set; }

        //二コレポ日時
        public string Time { get; set; }






    }
}
