using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Livet;
using HtmlAgilityPack;

namespace SRNicoNico.Models.NicoNicoWrapper {

    //二コレポタイプ
    public enum NicoNicoNicoRepoType {

        All,
        Myself,
        User,
        ChCom,
        Mylist,
        Other
    }


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



        //二コレポタイプ
        private readonly NicoNicoNicoRepoType Type;

        //Otherタイプ ユーザー定義二コレポリストID
        private string OtherId;

        //過去ページへのURL
        private string NextUrl;



        public NicoNicoNicoRepo(NicoNicoNicoRepoType type) {

            Type = type;
        }

        public NicoNicoNicoRepo(string id) {

            Type = NicoNicoNicoRepoType.Other;
            OtherId = id;
        }


        public NicoNicoNicoRepoData GetNicoRepo() {


            var api = "about:blanks";
            switch(Type) {
                case NicoNicoNicoRepoType.All:
                    api = NicoRepoAllAPI;
                    break;
                case NicoNicoNicoRepoType.Myself:
                    api = NicoRepoMyselfAPI;
                    break;
                case NicoNicoNicoRepoType.User:
                    api = NicoRepoUserAPI;
                    break;
                case NicoNicoNicoRepoType.ChCom:
                    api = NicoRepoChComAPI;
                    break;
                case NicoNicoNicoRepoType.Mylist:
                    api = NicoRepoMylistAPI;
                    break;
                default:
                    //Other ユーザー定義
                    api = @"http://www.nicovideo.jp/my/top/" + OtherId + @"?innerPage=1&mode=next_page";
                    break;
            }

            var all = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(api).Result;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(all);


            //過去二コレポのURL
            string attr = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='next-page']/a").Attributes["href"].Value);
            NextUrl = @"http://www.nicovideo.jp" + attr;


            NicoNicoNicoRepoData data = new NicoNicoNicoRepoData();

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
