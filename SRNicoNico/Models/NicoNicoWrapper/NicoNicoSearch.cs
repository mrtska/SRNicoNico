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
            if(Type == SearchType.Keyword) {

                typeString = "テキスト";
                type = "search";
            } else {

                typeString = "タグ";
                type = "tag";
            }
            SearchVM.Status = "検索中(" + typeString + ":" + Keyword + ")";

            NicoNicoSearchResult result = new NicoNicoSearchResult();

            //テキスト検索のとき、urlの場合はそれも検索結果に表示する
            if(Type == SearchType.Keyword) {

                Match match = Regex.Match(Keyword, @"^(:?http://(:?www.nicovideo.jp/watch/|nico.ms/))?(?<cmsid>\w{0,2}\d+).*?$");
                if(match.Success) {

                    NicoNicoVitaApiVideoData data = NicoNicoVitaApi.GetVideoData(match.Groups["cmsid"].Value);
                    NicoNicoVideoInfoEntry node = new NicoNicoVideoInfoEntry();
                    node.Cmsid = data.Id;
                    node.Title = HttpUtility.HtmlDecode(data.Title);
                    node.ViewCounter = data.ViewCounter;
                    node.CommentCounter = data.CommentCounter;
                    node.MylistCounter = data.MylistCounter;
                    node.ThumbnailUrl = data.ThumbnailUrl;
                    node.Length = data.Length;
                    node.FirstRetrieve = data.FirstRetrieve.Replace('-', '/');

                    result.List.Add(node);

                    result.Total++;
                }
            }

            //URLに検索キーワードやその他いろいろをGETリクエストする
            string search = SearchURL + type + "/" + Keyword + "?mode=watch" + Sort + Order + "&page=" + CurrentPage++;

            string jsonString = NicoNicoWrapperMain.GetSession().GetAsync(search).Result;

            //取得したJsonの全体
            var json = DynamicJson.Parse(jsonString);

            //検索結果総数
            if(json.count()) {
                result.Total += (ulong)json.count;
            }

            //Jsonからリストを取得、データを格納
            if(json.list()) {
                foreach(var entry in json.list) {

                    NicoNicoVideoInfoEntry node = new NicoNicoVideoInfoEntry();

                    node.Cmsid = entry.id;
                    node.Title = entry.title_short;
                    node.ViewCounter = (int) entry.view_counter;
                    node.CommentCounter = (int) entry.num_res;
                    node.MylistCounter = (int) entry.mylist_counter;
                    node.ThumbnailUrl = entry.thumbnail_url;
                    node.Length = entry.length;
                    node.FirstRetrieve = entry.first_retrieve;

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
        public List<NicoNicoVideoInfoEntry> List { get; set; }

        public NicoNicoSearchResult() {

            List = new List<NicoNicoVideoInfoEntry>();
        }
    }


    public enum SearchType {

        Keyword,
        Tag
    }

}
