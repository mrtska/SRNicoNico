using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Http;
using Codeplex.Data;

using System.Web.Script.Serialization;

using Livet;

using SRNicoNico.ViewModels;

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

        private SearchResultViewModel SearchResult;



		public NicoNicoSearch(SearchResultViewModel vm, string keyword, string sort) {

            SearchResult = vm;
			this.Keyword = keyword;

			this.Sort = "&sort=" + sort.Split(':')[0];
			this.Order = "&order=" + sort.Split(':')[1];


		}

		//検索結果 json デシリアライズはこのクラスでは行わない
		private string Response() {

			//URLに検索キーワードを入れる
			string search = SearchURL + "search/" + this.Keyword + "?mode=watch" + this.Sort + this.Order + "&page=" + CurrentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.GetSession().HttpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}

		public void DoSearch() {


			//検索結果をUIに
			NicoNicoSearchResult result = NicoNicoSearchResult.Deserialize(Response());


			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				SearchResult.Total = String.Format("{0:#,0}", result.Total) + "件の動画";

				SearchResult.List.Clear();
				foreach (NicoNicoSearchResultNode node in result.List) {

					SearchResultEntryViewModel vm = new SearchResultEntryViewModel();
					vm.Node = node;
				    SearchResult.List.Add(vm);
				}
			}));
		}


		private string Next() {

			string search = SearchURL + "search/" + this.Keyword + "?mode=watch" + this.Sort + this.Order + "&page=" + CurrentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.GetSession().HttpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}

		public void SearchNext() {

			//検索結果をUIに
			NicoNicoSearchResult result = NicoNicoSearchResult.Deserialize(Next());

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				foreach (NicoNicoSearchResultNode node in result.List) {

					SearchResultEntryViewModel vm = new SearchResultEntryViewModel();
					vm.Node = node;
					SearchResult.List.Add(vm);
				}

			}));

		}




	}
}
