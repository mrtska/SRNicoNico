using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFavorite : NotificationObject {

        public List<NicoNicoFavoriteUser> GetFavoriteUser() {

            var url = "http://www.nicovideo.jp/my/fav/user";
            var a = NicoNicoWrapperMain.Session.GetAsync(url).Result;

            List<NicoNicoFavoriteUser> ret = new List<NicoNicoFavoriteUser>();

            var doc = new HtmlDocument();
            doc.LoadHtml2(a);

            var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

            foreach(var entry in content.SelectNodes("child::div[@class='articleBody']")) {


            }


            return ret;
        }


    }

    public class NicoNicoFavoriteUser : object {

        //お気に入りユーザーの名前
        public string Name { get; set; }

        //簡易説明文
        public string Description { get; set; }

        //サムネイルURL
        public string ThumbnailUrl { get; set; }
    }
}
