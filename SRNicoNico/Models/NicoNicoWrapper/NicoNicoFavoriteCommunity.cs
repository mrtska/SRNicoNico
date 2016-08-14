using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Livet;
using System.Web;
using System.Windows;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFavoriteCommunity {

        //ロードするページ
        private int Page = 1;
        private bool IsEnd = false;

        public List<NicoNicoFavoriteCommunityContent> GetFavoriteCommunity() {

            //無駄にアクセスしないように
            if(IsEnd && Page != 1) {

                return null;
            }

            var url = "http://www.nicovideo.jp/my/community?page=" + Page++;
            var a = NicoNicoWrapperMain.Session.GetAsync(url).Result;

            var ret = new List<NicoNicoFavoriteCommunityContent>();

            var doc = new HtmlDocument();
            doc.LoadHtml2(a);
            
            var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

            var outers = content.SelectNodes("child::div[@class='articleBody']/div[@class='outer']");

            //終了
            if(outers == null) {

                IsEnd = true;
                return null;
            }
            

            foreach(var entry in outers) {

                var user = new NicoNicoFavoriteCommunityContent();

                var section = entry.SelectSingleNode("child::div[@class='section']");

                user.CommunityPage =  section.SelectSingleNode("child::h5/a").Attributes["href"].Value;
                user.Name = HttpUtility.HtmlDecode(section.SelectSingleNode("child::h5/a").InnerText.Trim());
                user.ThumbnailUrl = entry.SelectSingleNode("child::div[@class='thumbContainer']/a/img").Attributes["src"].Value;


                var p = section.SelectSingleNode("child::p[1]");
                user.VideoAndMember = section.SelectSingleNode("child::ul/li[1]").InnerText.Trim() + " " + section.SelectSingleNode("child::ul/li[2   ]").InnerText.Trim();
                user.Description = p == null ? "" : p.InnerText.Trim();

                //説明がなかったら
                if(user.Description == "ニコレポリストに追加/編集する") {

                    user.Description = "";
                }
                //改行を空白に置換
                user.Description = user.Description.Replace('\n', ' ').Replace('\r', ' ');

                user.Description = HttpUtility.HtmlDecode(user.Description);
                ret.Add(user);
            }
            return ret;
        }

        public void ResetFavoriteCommunity() {

            Page = 1;
        }




    }

    public class NicoNicoFavoriteCommunityContent {

        //コミュニティページURL
        public string CommunityPage { get; set; }

        //お気に入りユーザーの名前
        public string Name { get; set; }

        //動画登録数とメンバー数
        public string VideoAndMember { get; set; }

        //簡易説明文
        public string Description { get; set; }

        //サムネイルURL
        public string ThumbnailUrl { get; set; }


        public void OpenWebView() {

            App.ViewModelRoot.AddWebViewTab(CommunityPage, true);
        }

        public void CopyUrl() {

            Clipboard.SetText(CommunityPage);
        }
    }
}
