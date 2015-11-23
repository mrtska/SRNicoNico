using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Codeplex.Data;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Livet;

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoUser : NotificationObject {

        //ユーザー名取得API
        public const string UserLookUpURL = "http://seiga.nicovideo.jp/api/user/info?id=";

        //ユーザーIDからユーザーネームを取得する
        public static string LookupUserName(string userId) {

			string uri = UserLookUpURL + userId;

			string json = NicoNicoUtil.XmlToJson(NicoNicoWrapperMain.GetSession().GetAsync(uri).Result);
            
			return DynamicJson.Parse(json).response.user.nickname;
		}

        private string UserPage;

        private UserViewModel Owner;

        //ニコレポオフセット
        private int Offset = 0;


        public NicoNicoUser(UserViewModel vm, string pageUrl) {

            Owner = vm;
            UserPage = pageUrl;
        }

        public NicoNicoUserEntry GetUserInfo() {

            Owner.Status = "ユーザー情報取得中";
            var ret = new NicoNicoUserEntry();

            //ユーザーページのhtmlを取得
            var a = NicoNicoWrapperMain.Session.GetAsync(UserPage).Result;

            //htmlをロード
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml2(a);

            //ユーザープロファイル
            HtmlNode detail = doc.DocumentNode.SelectSingleNode("//div[@class='userDetail']");
            HtmlNode profile = detail.SelectSingleNode("child::div[@class='profile']");
            HtmlNode account = profile.SelectSingleNode("child::div[@class='account']");

            ret.UserIconUrl = detail.SelectSingleNode("child::div[@class='avatar']/img").Attributes["src"].Value;
            ret.UserName = profile.SelectSingleNode("child::h2").InnerText.Trim();
            ret.Id = account.SelectSingleNode("child::p[@class='accountNumber']").InnerText.Trim();
            ret.Gender = account.SelectSingleNode("child::p[2]").InnerText.Trim();
            ret.BirthDay = account.SelectSingleNode("child::p[3]").InnerText.Trim();
            ret.Region = account.SelectSingleNode("child::p[4]").InnerText.Trim();

            var temp = profile.SelectSingleNode("child::ul[@class='userDetailComment channel_open_mt0']/li/p/span");

            ret.Description = temp == null ? "" : temp.InnerHtml;

            ret.UserPage = UserPage;

            //URLをハイパーリンク化する
            ret.Description = HyperLinkParser.Parse(ret.Description);

            Owner.Status = "";
            return ret;
        }

        //一度nullを返してきたら二度と呼ばない
        public List<NicoNicoNicoRepoDataEntry> GetUserNicoRepo() {

            var url = UserPage + "/top?innerPage=1&offset=" + Offset;
            Owner.Status = "ユーザーニコレポ取得中";

            List<NicoNicoNicoRepoDataEntry> ret = new List<NicoNicoNicoRepoDataEntry>();
            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(url).Result;
                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var timeline = doc.DocumentNode.SelectNodes("//div[@class='timeline']/div");

                //タイムラインを取得できなかったら終了
                if(timeline == null) {

                    Owner.Status = "";
                    return null;
                }

                //ニコレポタイムライン走査
                foreach(HtmlNode node in timeline) {

                    NicoNicoNicoRepoDataEntry entry = new NicoNicoNicoRepoDataEntry();

                    entry.IconUrl = node.SelectSingleNode("child::div[contains(@class, 'log-author ')]/a/img").Attributes["data-original"].Value;
                    entry.Title = HttpUtility.HtmlDecode(node.SelectSingleNode("child::div[@class='log-content']/div[@class='log-body']").InnerHtml.Trim());

                    HtmlNode thumbnail = node.SelectSingleNode("child::div[@class='log-content']/div/div[@class='log-target-thumbnail']/a/img");
                    entry.ImageUrl = thumbnail != null ? thumbnail.Attributes["data-original"].Value : entry.IconUrl;

                    HtmlNode desc = node.SelectSingleNode("child::div[@class='log-content']/div/div[@class='log-target-info']/a");
                    entry.Description = desc != null ? HttpUtility.HtmlDecode(desc.InnerText.Trim()) : "";
                    entry.VideoUrl = desc != null ? desc.Attributes["href"].Value : "";

                    HtmlNode time = node.SelectSingleNode("child::div[@class='log-content']/div/div[@class='log-footer']/div/a[contains(@class, 'log-footer-date')]/time");
                    entry.Time = time.InnerText.Trim();

                    ret.Add(entry);
                }

                Owner.Status = "";
                Offset += 20;
                return ret;
            } catch(RequestTimeout) {

                Owner.Status = "ユーザーニコレポの取得に失敗しました";
                return null;
            }
        }
        public List<NicoNicoUserMylistEntry> GetUserMylist() {

            var url = UserPage + "/mylist";
            Owner.Status = "ユーザーマイリスト取得中";

            List<NicoNicoUserMylistEntry> ret = new List<NicoNicoUserMylistEntry>();
            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(url).Result;
                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                var outers = content.SelectNodes("child::div[@class='articleBody']/div[@class='outer']");

                //終了
                if(outers == null) {

                    Owner.Status = "";
                    return null;
                }
                //ニコレポタイムライン走査
                foreach(var node in outers) {

                    NicoNicoUserMylistEntry entry = new NicoNicoUserMylistEntry();

                    //h4タグ
                    var h4 = node.SelectSingleNode("child::div/h4");

                    entry.Url = "http://www.nicovideo.jp/" + h4.SelectSingleNode("child::a").Attributes["href"].Value;

                    //名前取得
                    entry.Name = h4.SelectSingleNode("child::a").InnerText.Trim();

                    //説明文取得
                    var desc = node.SelectSingleNode("child::div/p[@data-nico-mylist-desc-full='true']");
                    entry.Description = desc == null ? "" : desc.InnerText.Trim();

                    entry.Description = HyperLinkParser.Parse(entry.Description);

                    //サムネイル取得
                    var thumb1 = node.SelectSingleNode("child::div/ul/li[1]/img");
                    var thumb2 = node.SelectSingleNode("child::div/ul/li[2]/img");
                    var thumb3 = node.SelectSingleNode("child::div/ul/li[3]/img");

                    if(thumb1 != null) {

                        entry.ThumbNail1Available = true;
                        entry.ThumbNail1Url = thumb1.Attributes["src"].Value;
                        entry.ThumbNail1ToolTip = HttpUtility.HtmlDecode(thumb1.Attributes["alt"].Value);
                    } else {
                        goto next;
                    }

                    if(thumb2 != null) {

                        entry.ThumbNail2Available = true;
                        entry.ThumbNail2Url = thumb2.Attributes["src"].Value;
                        entry.ThumbNail2ToolTip = HttpUtility.HtmlDecode(thumb2.Attributes["alt"].Value);
                    } else {
                        goto next;
                    }

                    if(thumb3 != null) {

                        entry.ThumbNail3Available = true;
                        entry.ThumbNail3Url = thumb3.Attributes["src"].Value;
                        entry.ThumbNail3ToolTip = HttpUtility.HtmlDecode(thumb3.Attributes["alt"].Value);
                    }

                    next:
                    ret.Add(entry);
                }

                Owner.Status = "";
                return ret;
            } catch(RequestTimeout) {

                Owner.Status = "ユーザーマイリストの取得に失敗しました";
                return null;
            }
        }

        private int Page = 1;

        public List<NicoNicoVideoInfoEntry> GetUserVideo() {

            var url = UserPage + "/video?page=" + Page++;
            Owner.Status = "投稿動画を取得中";

            List<NicoNicoVideoInfoEntry> ret = new List<NicoNicoVideoInfoEntry>();

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(url).Result;
                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                var outers = content.SelectNodes("child::div[@class='articleBody']/div[@class='outer']");

                //終了
                if(outers == null) {

                    Owner.Status = "";
                    return null;
                }

                foreach(var outer in outers) {

                    //フォームだったら飛ばす
                    if(outer.SelectSingleNode("child::form") != null) {

                        continue;
                    }

                    var entry = new NicoNicoVideoInfoEntry();

                    var thumb = outer.SelectSingleNode("child::div[@class='thumbContainer']");

                    entry.Cmsid = thumb.SelectSingleNode("child::a").Attributes["href"].Value.Substring(6);
                    entry.ThumbnailUrl = thumb.SelectSingleNode("child::a/img").Attributes["src"].Value;
                    entry.Length = thumb.SelectSingleNode("child::span[@class='videoTime']").InnerText.Trim();

                    var section = outer.SelectSingleNode("child::div[@class='section']");

                    entry.Title = HttpUtility.HtmlDecode(section.SelectSingleNode("child::h5/a").InnerText.Trim());
                    entry.FirstRetrieve = section.SelectSingleNode("child::p").InnerText.Trim();

                    var metadata = section.SelectSingleNode("child::div[@class='dataOuter']/ul");

                    entry.ViewCounter = int.Parse(metadata.SelectSingleNode("child::li[@class='play']").InnerText.Trim().Split(':')[1].Replace(",", ""));
                    entry.CommentCounter = int.Parse(metadata.SelectSingleNode("child::li[@class='comment']").InnerText.Trim().Split(':')[1].Replace(",", ""));
                    entry.MylistCounter = int.Parse(metadata.SelectSingleNode("child::li[@class='mylist']/a").InnerText.Trim().Replace(",", ""));

                    ret.Add(entry);
                }

                Owner.Status = "";
                return ret;
            } catch(RequestTimeout) {

                Owner.Status = "投稿動画の取得に失敗しました。";
                return null;
            }
        }

    }
    public class NicoNicoUserEntry {

        //ユーザーID
        public string UserId { get; set; }

        //ユーザーページ
        public string UserPage { get; set; }

        //ユーザーネーム
        public string UserName { get; set; }

        //ユーザー説明文
        public string Description { get; set; }

        //ユーザーアイコン
        public string UserIconUrl { get; set; }

        //IDと会員クラス
        public string Id { get; set; }

        //性別
        public string Gender { get; set; }

        //生年月日
        public string BirthDay { get; set; }

        //お住まいの地域
        public string Region { get; set; }

        //お気に入り登録された数
        public int FavoriteCount { get; set; }

        //スタンプ経験値
        public int StampExp { get; set; }
    }

    public class NicoNicoUserMylistEntry {

        //マイリストURL
        public string Url { get; set; }

        //マイリストの名前
        public string Name { get; set; }

        //説明
        public string Description { get; set; }

        //---マイリストサムネイル1---
        public bool ThumbNail1Available { get; set; }
        public string ThumbNail1Url { get; set; }
        public string ThumbNail1ToolTip { get; set; }
        //------

        //---マイリストサムネイル2---
        public bool ThumbNail2Available { get; set; }
        public string ThumbNail2Url { get; set; }
        public string ThumbNail2ToolTip { get; set; }
        //------

        //---マイリストサムネイル3---
        public bool ThumbNail3Available { get; set; }
        public string ThumbNail3Url { get; set; }
        public string ThumbNail3ToolTip { get; set; }
        //------

    }

}
