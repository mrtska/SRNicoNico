using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Codeplex.Data;

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

        private string UserPage = "http://www.nicovideo.jp/user/";

        public NicoNicoUser(string id) {

            UserPage += id;
        }

        public NicoNicoUserEntry GetUserInfo() {

            var ret = new NicoNicoUserEntry();

            var a = NicoNicoWrapperMain.Session;






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
