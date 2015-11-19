using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFavorite : NotificationObject {

        private int Page = 1;

        private bool IsEnd = false;

        public List<NicoNicoFavoriteUser> GetFavoriteUser() {

            var url = "http://www.nicovideo.jp/my/fav/user?page=" + Page++;
            var a = NicoNicoWrapperMain.Session.GetAsync(url).Result;

            List<NicoNicoFavoriteUser> ret = new List<NicoNicoFavoriteUser>();

            var doc = new HtmlDocument();
            doc.LoadHtml2(a);

            var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

            foreach(var entry in content.SelectNodes("child::div[@class='articleBody']/div[@class='outer']")) {

                NicoNicoFavoriteUser user = new NicoNicoFavoriteUser();

                user.UserPage = "http://www.nicovideo.jp" + entry.SelectSingleNode("child::div[@class='section']/h5/a").Attributes["href"].Value;
                user.Name = entry.SelectSingleNode("child::div[@class='section']/h5/a").InnerText.Trim();
                user.ThumbnailUrl = entry.SelectSingleNode("child::div[@class='thumbContainer']/a/img").Attributes["src"].Value;

                var p = entry.SelectSingleNode("child::div[@class='section']/p[1]");
                user.Description = p == null ? "" : p.InnerText.Trim();

                //説明がなかったら
                if(user.Description == "ニコレポリストに追加/編集する") {

                    user.Description = "";
                }

                //改行を空白に置換
                user.Description = user.Description.Replace('\n', ' ');

                ret.Add(user);
            }

            return ret;
        }


    }

    public class NicoNicoFavoriteUser : object {

        //ユーザーページURL
        public string UserPage { get; set; }

        //お気に入りユーザーの名前
        public string Name { get; set; }

        //簡易説明文
        public string Description { get; set; }

        //サムネイルURL
        public string ThumbnailUrl { get; set; }
    }
}
