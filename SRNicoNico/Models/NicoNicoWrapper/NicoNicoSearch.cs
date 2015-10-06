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

        private SearchViewModel SearchVM;


		public NicoNicoSearch(SearchViewModel vm, string keyword, string sort) {

            SearchVM = vm;
			Keyword = keyword;

			Sort = "&sort=" + sort.Split(':')[0];
			Order = "&order=" + sort.Split(':')[1];


		}
        //リクエストのレスポンスを返す
        public NicoNicoSearchResult Search() {

            SearchVM.Status = "検索中(" + Keyword + ")";

            //URLに検索キーワードやその他いろいろをGETリクエストする
            string search = SearchURL + "search/" + Keyword + "?mode=watch" + Sort + Order + "&page=" + CurrentPage++;

            string jsonString = NicoNicoWrapperMain.GetSession().GetAsync(search).Result;

            NicoNicoSearchResult result = new NicoNicoSearchResult();

            //取得したJsonの全体
            var json = DynamicJson.Parse(jsonString);

            //検索結果総数
            result.Total = (ulong)json.count;

            //Jsonからリストを取得、データを格納
            foreach(var entry in json.list) {

                NicoNicoSearchResultEntry node = new NicoNicoSearchResultEntry(entry.id, entry.title_short, (ulong)entry.view_counter, (ulong)entry.num_res,
                                                                                (ulong)entry.mylist_counter, entry.thumbnail_url, entry.length, entry.first_retrieve);

                result.List.Add(node);
            }
            SearchVM.Status = "検索完了(" + Keyword + ")";


            return result;
        }
	}


    public class NicoNicoSearchResult : NotificationObject {
        /*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */


        //検索結果の総数
        public ulong Total { get; set; }
        public List<NicoNicoSearchResultEntry> List { get; set; }

        public NicoNicoSearchResult() {

            List = new List<NicoNicoSearchResultEntry>();

        }


    }

    public class NicoNicoSearchResultEntry {

        //ID sm9みたいな
        public string Cmsid { get; private set; }

        //タイトル
        public string Title { get; private set; }

        //再生回数
        public ulong ViewCounter { get; private set; }

        //コメント数
        public ulong CommentCounter { get; private set; }

        //マイリスト数
        public ulong MylistCounter { get; private set; }

        //サムネイルURL
        public string ThumbnailUrl { get; private set; }

        //再生時間
        public string Length { get; private set; }

        //動画投稿時日時
        public string FirstRetrieve { get; private set; }


        //コンストラクタ
        public NicoNicoSearchResultEntry(string cmsid, string title, ulong view_counter, ulong comment_counter, ulong mylist_counter, string thumbnail_url, string length, string first_retrieve) {

            Cmsid = cmsid;
            Title = title;
            ViewCounter = view_counter;
            CommentCounter = comment_counter;
            MylistCounter = mylist_counter;
            ThumbnailUrl = thumbnail_url;
            Length = length;
            FirstRetrieve = first_retrieve.Replace('-', '/');
        }

    }
}
