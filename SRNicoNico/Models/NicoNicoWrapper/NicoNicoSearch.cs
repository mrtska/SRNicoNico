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

		private readonly string searchURL = "http://ext.nicovideo.jp/api/search/";

		//検索キーワード
		private string keyword;

		//ソート
		private string sort;

		//オーダー
		private string order;

		//取得しているページ
		private byte currentPage = 1;




		public NicoNicoSearch(string keyword, string sort) {

			this.keyword = keyword;

			this.sort = "&sort=" + sort.Split(':')[0];
			this.order = "&order=" + sort.Split(':')[1];


		}

		//検索結果 json デシリアライズはこのクラスでは行わない
		public string response() {

			//URLに検索キーワードを入れる
			string search = searchURL + "search/" + this.keyword + "?mode=watch" + this.sort + this.order + "&page=" + currentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.getSession().httpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}

		public string next() {

			string search = searchURL + "search/" + this.keyword + "?mode=watch" + this.sort + this.order + "&page=" + currentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.getSession().httpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}






	}
}
