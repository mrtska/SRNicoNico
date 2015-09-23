using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Livet;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

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

        //ID ユーザー定義ニコレポリストID
        private string Id;


        //過去ページへのURL
        private string PrevUrl;
        private string NextUrl;

        //取得したい二コレポのID allとかmyselfとかuserとかいろいろ
        public NicoNicoNicoRepo(string id) {

            Id = id;
        }


        public IList<NicoNicoNicoRepoDataEntry> GetNicoRepo() {

            //APIと言うのか謎
            var api = PrevUrl = @"http://www.nicovideo.jp/my/top/" + Id + @"?innerPage=1&mode=next_page";
           
            //html
            var html = NicoNicoWrapperMain.GetSession().GetAsync(api).Result;

            IList<NicoNicoNicoRepoDataEntry> data = new List<NicoNicoNicoRepoDataEntry>();

            //XPathでhtmlから抜き出す
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml2(html);

            //ニコレポが存在しなかったら存在しないというエントリを返す
            if(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div").Attributes["class"].Value.Equals("empty")) {

                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                entry.ImageUrl = entry.IconUrl = null;
                entry.Description = "ニコレポが存在しません。";
                data.Add(entry);

                return data;
            }

            StoreData(doc, data);
            

            return data;
        }

        //ニコレポの
        public IList<NicoNicoNicoRepoDataEntry> NextNicoRepo() {

            //もう過去の二コレポは存在しない
            if(NextUrl == null || NextUrl.Equals("end")) {

                return null;
            }

            //ビヘイビア暴発
            if(PrevUrl == NextUrl) {

                return null;
            }

            //APIと言うのか謎
            var api = PrevUrl = NextUrl;

            //html
            var html = NicoNicoWrapperMain.GetSession().GetAsync(api).Result;

            IList<NicoNicoNicoRepoDataEntry> data = new List<NicoNicoNicoRepoDataEntry>();

            //XPathでhtmlから抜き出す
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml2(html);

            StoreData(doc, data);

            return data;
        }

        private void StoreData(HtmlDocument doc, IList<NicoNicoNicoRepoDataEntry> list) {


            var timeline = doc.DocumentNode.SelectNodes("//div[@class='timeline']/div");

            //ニコレポタイムライン走査
            foreach(HtmlNode node in timeline) {

                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                entry.IconUrl = node.SelectSingleNode("child::div[@class='log-author ']/a/img").Attributes["data-original"].Value;
                entry.Title = node.SelectSingleNode("child::div[@class='log-content']/div[@class='log-body']").InnerText.Trim();

                HtmlNode thumbnail = node.SelectSingleNode("child::div[@class='log-content']/div/div[@class='log-target-thumbnail']/a/img");

                entry.ImageUrl = thumbnail != null ? thumbnail.Attributes["data-original"].Value : entry.IconUrl;

                HtmlNode desc = node.SelectSingleNode("child::div[@class='log-content']/div/div[@class='log-target-info']/a");

                entry.Description = desc != null ? desc.InnerText.Trim() : "";

                HtmlNode time = node.SelectSingleNode("child::div[@class='log-content']/div/div[@class='log-footer']/div/a[contains(@class, 'log-footer-date')]/time");

                entry.Time = time.InnerText.Trim();

                list.Add(entry);

            }


            if(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='no-next-page']") != null) {


                NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                NextUrl = "end";
                entry.ImageUrl = entry.IconUrl = null;
                entry.Description = "これより過去のニコレポは存在しません。";
                list.Add(entry);


            } else {

                //過去ニコレポのURL
                string attr = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("/div[@class='nicorepo']/div[@class='nicorepo-page']/div[@class='next-page']/a").Attributes["href"].Value);
                NextUrl = @"http://www.nicovideo.jp" + attr;
            }
        }
    }
    

    public class NicoNicoNicoRepoDataEntry {


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

        //自分のニコレポかどうか
        public bool IsMyNicoRepo { get; set; }





    }
}
