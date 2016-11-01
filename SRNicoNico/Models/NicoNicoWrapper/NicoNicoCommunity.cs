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

                var node = doc.DocumentNode;

                var community_main = node.SelectSingleNode("//main");
                var header = node.SelectSingleNode("//header[@class='area-communityHeader']");
                var news = node.SelectNodes("//div[parent::div[@id='community_news']]");

                var ret = new NicoNicoCommunityContent();

                ret.CommunityUrl = CommunityUrl;
                ret.ThumbnailUrl = header.SelectSingleNode("div/div/div[@class='communityThumbnail']/a/img").Attributes["src"].Value;
                ret.OwnerUrl = node.SelectSingleNode("//table[@class='communityDetail']/tr[1]/td/a").Attributes["href"].Value;
                ret.OwnerName = "<a href=\"" + ret.OwnerUrl + "\">" + header.SelectSingleNode("//table[@class='communityDetail']/tr[1]/td/a").InnerText.Trim() + "</a>";
                ret.CommunityTitle = header.SelectSingleNode("div/div/div[@class='communityData']/h2/a").InnerText;
                ret.OpeningDate = header.SelectSingleNode("//table[@class='communityDetail']/tr[2]/td").InnerText;

                //---お知らせ---
                ret.CommunityNews = new List<NicoNicoCommunityNews>();
                if(news != null) {

                    foreach(var notify in news) {

                        var b = new NicoNicoCommunityNews();

                        b.Title = notify.SelectSingleNode("child::h2").InnerText;
                        b.Description = HyperLinkReplacer.Replace(notify.SelectSingleNode("child::div[@class='desc']").InnerHtml.Trim());
                        b.Date = notify.SelectSingleNode("child::div[@class='date']").InnerText.Trim();

                        ret.CommunityNews.Add(b);
                    }
                }
                //------

                ret.CommunityLevel = node.SelectSingleNode("//dl[@class='communityScale']/dd[1]").InnerText;
                ret.CommunityMember = node.SelectSingleNode("//dl[@class='communityScale']/dd[1]").InnerHtml.Trim();

                //---登録タグ---
                ret.CommunityTags = new List<string>();
                var tags = node.SelectNodes("//ul[@class='tagList']/li");

                if(tags != null) {

                    foreach(var tag in tags) {

                        ret.CommunityTags.Add(tag.SelectSingleNode("a").InnerText);
                    }
                }
                //------

                ret.CommunityProfile = HyperLinkReplacer.Replace(node.SelectSingleNode("//span[@id='profile_text_content']").InnerHtml.Trim());

                //---特権---
                ret.Privilege = new List<string>();
                var privileges = node.SelectNodes("//div[@id='comLivePrivileged_data']/div/p");

                ret.Privilege.Add(node.SelectSingleNode("//div[@id='comLivePrivileged_data']/p").InnerText);
                if(privileges != null) {
                    foreach(var privilege in privileges) {

                        ret.Privilege.Add(privilege.InnerText);
                    }
                }
                //------



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

    public class NicoNicoCommunityLive {

        //生放送URL
        public string LiveUrl { get; set; }

        //説明
        public string ShortDescription { get; set; }


    }

}
