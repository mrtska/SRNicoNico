using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Http;
using Codeplex.Data;

using System.Web.Script.Serialization;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoSearch : NotificationObject {
		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */

		private readonly string SearchURL = "http://ext.nicovideo.jp/api/search/";

		//検索キーワード
		private string Keyword;

		//ソート
		private string Sort;

		//オーダー
		private string Order;

		//取得しているページ
		private byte CurrentPage = 1;




		public NicoNicoSearch(string keyword, string sort) {

			this.Keyword = keyword;

			this.Sort = "&sort=" + sort.Split(':')[0];
			this.Order = "&order=" + sort.Split(':')[1];


		}

		//検索結果 json デシリアライズはこのクラスでは行わない
		public string Response() {

			//URLに検索キーワードを入れる
			string search = SearchURL + "search/" + this.Keyword + "?mode=watch" + this.Sort + this.Order + "&page=" + CurrentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.getSession().HttpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}

		public string Next() {

			string search = SearchURL + "search/" + this.Keyword + "?mode=watch" + this.Sort + this.Order + "&page=" + CurrentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.getSession().HttpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}






	}
}
