using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.Web;

using Codeplex.Data;

using System.Web.Script.Serialization;

using Livet;

using SRNicoNico.ViewModels;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoSearch : NotificationObject {

        //検索API
        private const string SearchURL = "http://ext.nicovideo.jp/api/search/";

        //検索の種類
        private SearchType Type;
        
        //検索キーワード
		private string Keyword;

		//ソート
		private string Sort;

		//オーダー
		private string Order;

		//取得しているページ
		private byte CurrentPage = 1;

        private SearchViewModel SearchVM;


		public NicoNicoSearch(SearchViewModel vm, string keyword, SearchType type, string sort) {

            SearchVM = vm;
            Type = type;
			Keyword = keyword;

			Sort = "&sort=" + sort.Split(':')[0];
			Order = "&order=" + sort.Split(':')[1];


		}
        //リクエストのレスポンスを返す
        public NicoNicoSearchResult Search() {

            string typeString;  //表示文字列
            string type;        //クエリ文字列
            if (Type == SearchType.Keyword) {

                typeString = "テキスト";
                type = "search";
            } else {

                typeString = "タグ";
                type = "tag";
            }
            SearchVM.Status = "検索中(" + typeString + ":" + Keyword + ")";
            
            NicoNicoSearchResult result = new NicoNicoSearchResult();

            //テキスト検索のとき、urlの場合はそれも検索結果に表示する
            if (Type == SearchType.Keyword)
            {
                Match match = Regex.Match(Keyword, @"^(:?http://(:?www.nicovideo.jp/watch/|nico.ms/))?(?<cmsid>\w{0,2}\d+).*?$");
                if (match.Success)
                {
                    NicoNicoVitaApiVideoData data = NicoNicoVitaApi.GetVideoData(match.Groups["cmsid"].Value);
                    NicoNicoSearchResultEntry node = new NicoNicoSearchResultEntry(data.Id, data.Title, (ulong)data.ViewCounter, (ulong)data.CommentCounter,
                                                                                    (ulong)data.MylistCounter, data.ThumbnailUrl, data.Length, data.FirstRetrieve);
                    result.List.Add(node);
                }
            }

            //URLに検索キーワードやその他いろいろをGETリクエストする
            string search = SearchURL + type + "/" + Keyword + "?mode=watch" + Sort + Order + "&page=" + CurrentPage++;

            string jsonString = NicoNicoWrapperMain.GetSession().GetAsync(search).Result;

            //取得したJsonの全体
            var json = DynamicJson.Parse(jsonString);

            //検索結果総数
            if (json.IsDefined("count")) {
                result.Total = (ulong)json.count;
            }

            //Jsonからリストを取得、データを格納
            if (json.IsDefined("list")) {
                foreach (var entry in json.list) {

                    NicoNicoSearchResultEntry node = new NicoNicoSearchResultEntry(entry.id, entry.title_short, (ulong)entry.view_counter, (ulong)entry.num_res,
                                                                                (ulong)entry.mylist_counter, entry.thumbnail_url, entry.length, entry.first_retrieve);

                    result.List.Add(node);
                }
            }
            SearchVM.Status = "検索完了(" + typeString + ":" + Keyword + ")";


            return result;
        }
	}


    public class NicoNicoSearchResult : NotificationObject {


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
            Title = HttpUtility.HtmlDecode(title);
            ViewCounter = view_counter;
            CommentCounter = comment_counter;
            MylistCounter = mylist_counter;
            ThumbnailUrl = thumbnail_url;
            Length = length;
            FirstRetrieve = first_retrieve.Replace('-', '/');
        }

    }

    public enum SearchType {

        Keyword,
        Tag
    }

}
