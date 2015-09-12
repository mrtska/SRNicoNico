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
		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */



		public const string UserLookUpURL = "http://seiga.nicovideo.jp/api/user/info?id=";

		public uint UserId { get; internal set; }

		public string UserName { get; internal set; }

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
