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





	}


    public class NicoNicoUserEntry {


        //ユーザーID
        public string UserId { get; set; }

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

}
