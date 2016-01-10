using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Livet;

using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoCommunity : NotificationObject {


        public string CommunityUrl;

        public NicoNicoCommunity(string url) {

            CommunityUrl = url;
        }

        public NicoNicoCommunityContent GetCommunity() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(CommunityUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var community_main = doc.DocumentNode.SelectSingleNode("//div[@id='community_main']");
                var profile = community_main.SelectSingleNode("child::div/div/div[@id='cbox_profile']");
                var news = community_main.SelectNodes("//div[parent::div[@id='community_news']]");

                var ret = new NicoNicoCommunityContent();

                ret.CommunityUrl = CommunityUrl;
                ret.ThumbnailUrl = profile.SelectSingleNode("child::table/tr/td/p/img").Attributes["src"].Value;
                ret.OwnerUrl = community_main.SelectSingleNode("child::div/div/div/div[@class='r']/p/a").Attributes["href"].Value;
                ret.OwnerName = community_main.SelectSingleNode("child::div/div/div/div[@class='r']/p/a/strong").InnerText;
                ret.CommunityTitle = community_main.SelectSingleNode("child::div/div/h1").InnerText;
                ret.OpeningDate = community_main.SelectSingleNode("child::div/div/div/div/p/strong").InnerText;

                //---お知らせ---
                ret.CommunityNews = new List<NicoNicoCommunityNews>();
                if(news != null) {

                    foreach(var notify in news) {

                        var b = new NicoNicoCommunityNews();

                        b.Title = notify.SelectSingleNode("child::h2").InnerText;
                        b.Description = HyperLinkParser.Parse(notify.SelectSingleNode("child::div[@class='desc']").InnerHtml);
                        b.Date = notify.SelectSingleNode("child::div[@class='date']").InnerText.Trim();

                        ret.CommunityNews.Add(b);
                    }
                }
                //------

                ret.CommunityLevel = profile.SelectSingleNode("child::table/tr/td/table/tr[1]/td[2]/strong").InnerText;
                ret.CommunityStars = profile.SelectSingleNode("child::table/tr/td/table/tr[1]/td[2]/span").InnerText;
                ret.CommunityMember = profile.SelectSingleNode("child::table/tr/td/table/tr[2]/td[2]").InnerHtml.Trim();

                //---登録タグ---
                ret.CommunityTags = new List<string>();
                var tags = profile.SelectNodes("child::table/tr/td/table/tr[4]/td[2]/a");

                if(tags != null) {

                    foreach(var tag in tags) {

                        ret.CommunityTags.Add(tag.SelectSingleNode("child::strong").InnerText);
                    }
                }
                //------

                ret.CommunityProfile = HyperLinkParser.Parse(profile.SelectSingleNode("child::div[@id='community_description']/div/div/div").InnerHtml.Trim());

                //---特権---
                ret.Privilege = new List<string>();
                var privileges = profile.SelectNodes("child::table/tr/td/table/tr[7]/td[2]/div[2]/p");

                ret.Privilege.Add(profile.SelectSingleNode("child::table/tr/td/table/tr[7]/td[2]/div[1]/p").InnerText);
                if(privileges != null) {
                    foreach(var privilege in privileges) {

                        ret.Privilege.Add(privilege.InnerText);
                    }
                }
                //------

                ret.TotalVisitors = profile.SelectSingleNode("child::table/tr/td/table/tr[6]/td[2]/strong").InnerText;
                ret.Videos = profile.SelectSingleNode("child::table/tr/td/table/tr[9]/td[2]").InnerHtml.Trim();


                return ret;

            } catch(RequestTimeout) {

                return null;
            }
        }




    }

    public class NicoNicoCommunityContent : NotificationObject {

        //コミュニティURL
        public string CommunityUrl { get; set; }

        //コミュニティサムネイルURL
        public string ThumbnailUrl { get; set; }

        //オーナーURL
        public string OwnerUrl { get; set; }
        
        //オーナー名
        public string OwnerName { get; set; }

        //コミュニティ名
        public string CommunityTitle { get; set; }

        //開設日
        public string OpeningDate { get; set; }

        //お知らせ
        public IList<NicoNicoCommunityNews> CommunityNews { get; set; }

        //コミュニティレベル
        public string CommunityLevel { get; set; }

        //コミュニティレベルの☆の数
        public string CommunityStars { get; set; }

        //コミュニティメンバー
        public string CommunityMember { get; set; }

        //コミュニティ登録タグ
        public IList<string> CommunityTags { get; set; }

        //プロフィール
        public string CommunityProfile { get; set; }

        //特権
        public IList<string> Privilege { get; set; }

        //累計来場者数
        public string TotalVisitors { get; set; }

        //投稿動画数
        public string Videos { get; set; }
    }

    public class NicoNicoCommunityNews {

        //お知らせタイトル
        public string Title { get; set; }

        //内容
        public string Description { get; set; }

        //日付
        public string Date { get; set; }
    }
}
