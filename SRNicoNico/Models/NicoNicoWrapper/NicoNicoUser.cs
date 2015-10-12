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

        //ユーザーID
        public uint UserId { get; private set; }

        //ユーザーネーム
        public string UserName { get; private set; }

		public NicoNicoUser(uint userId) {

            UserId = userId;
            UserName = LookupUserName(UserId);
		}

        public NicoNicoUser(uint userId, string userName) {

            UserId = userId;
            UserName = userName;
        }

        public static string LookupUserName(uint userId) {

			string uri = UserLookUpURL + userId;

			string json = NicoNicoUtil.XmlToJson(NicoNicoWrapperMain.GetSession().GetAsync(uri).Result);
            
			return DynamicJson.Parse(json).response.user.nickname;
		}

	}
}
