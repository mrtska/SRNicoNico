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
			Keyword = keyword;

			Sort = "&sort=" + sort.Split(':')[0];
			Order = "&order=" + sort.Split(':')[1];


		}

		//検索結果 json デシリアライズはこのクラスでは行わない
		private string Response() {

			//URLに検索キーワードを入れる
			string search = SearchURL + "search/" + Keyword + "?mode=watch" + Sort + Order + "&page=" + CurrentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.GetSession().HttpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}

		public void DoSearch() {

            App.ViewModelRoot.StatusBar.Status = "検索中(" + Keyword + ")";
			//検索結果をUIに
			NicoNicoSearchResult result = NicoNicoSearchResult.Deserialize(Response());


			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				SearchResult.Total = string.Format("{0:#,0}", result.Total) + "件の動画";

				SearchResult.List.Clear();
				foreach (NicoNicoSearchResultNode node in result.List) {

					SearchResult.List.Add(new SearchResultEntryViewModel(node));
				}
                App.ViewModelRoot.StatusBar.Status = "検索完了(" + Keyword + ")";

            }));
		}


		private string Next() {

			string search = SearchURL + "search/" + Keyword + "?mode=watch" + Sort + Order + "&page=" + CurrentPage++;

			HttpResponseMessage response = NicoNicoWrapperMain.GetSession().HttpClient.GetAsync(search).Result;
			return response.Content.ReadAsStringAsync().Result;
		}

		public void SearchNext() {

            App.ViewModelRoot.StatusBar.Status = "さらに検索中(" + Keyword + ")";

            //検索結果をUIに
            NicoNicoSearchResult result = NicoNicoSearchResult.Deserialize(Next());

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				foreach (NicoNicoSearchResultNode node in result.List) {

					SearchResult.List.Add(new SearchResultEntryViewModel(node));
				}
                App.ViewModelRoot.StatusBar.Status = "検索完了(" + Keyword + ")";
            }));
		}
	}
}
