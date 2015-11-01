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

        public NicoNicoUser(string pageUrl) {

            UserPage = pageUrl;
        }

        public NicoNicoUserEntry GetUserInfo() {

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
            ret.Description = profile.SelectSingleNode("child::ul[@class='userDetailComment channel_open_mt0']/li/p/span").InnerHtml;
            ret.Id = account.SelectSingleNode("child::p[@class='accountNumber']").InnerText.Trim();
            ret.Gender = account.SelectSingleNode("child::p[2]").InnerText.Trim();
            ret.BirthDay = account.SelectSingleNode("child::p[3]").InnerText.Trim();
            ret.Region = account.SelectSingleNode("child::p[4]").InnerText.Trim();

            return ret;
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
