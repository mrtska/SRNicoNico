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



		public const string user_lookup_url = "http://seiga.nicovideo.jp/api/user/info?id=";

		public uint UserId { get; internal set; }

		public string UserName { get; internal set; }

		public NicoNicoUser(uint user_id) {

			this.UserId = user_id;

			this.UserName = lookupUserName(this.UserId);

			
		}

		public static string lookupUserName(uint user_id) {

			string uri = user_lookup_url + user_id;

			Task<string> task = NicoNicoWrapperMain.getSession().httpClient.GetStringAsync(uri);

			string json = NicoNicoUtil.xmlToJson(task.Result);

			return DynamicJson.Parse(json).response.user.nickname;
		}

	}
}
