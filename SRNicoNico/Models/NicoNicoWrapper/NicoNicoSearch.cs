using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using SRNicoNico.ViewModels;
using Codeplex.Data;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoSearch {

        //検索API
        private const string SearchURL = "http://ext.nicovideo.jp/api/search/";

        private SearchViewModel Owner;


        public NicoNicoSearch(SearchViewModel vm) {

            Owner = vm;
        }

        //リクエストのレスポンスを返す
        public async Task<NicoNicoSearchResult> Search(string keyword, SearchType type, string Sort, int page = 1) {


            Owner.Status = "検索中:" + keyword;

            Sort = "&sort=" + Sort.Split(':')[0] + "&order=" + Sort.Split(':')[1];

            string typestr;
            if(type == SearchType.Keyword) {

                typestr = "search";
            } else {

                typestr = "tag";
            }

            var result = new NicoNicoSearchResult();

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(SearchURL + typestr + "/" + HttpUtility.UrlEncode(keyword) + "?mode=watch" + Sort + "&page=" + page);

                //取得したJsonの全体
                var json = DynamicJson.Parse(a);

                //検索結果総数
                if(json.count()) {

                    result.Total = (int)json.count;
                } else {

                    //連打するとエラーになる
                    Owner.Status = "アクセスしすぎです。1分ほど時間を置いてください。";
                    return null;
                }

                //Jsonからリストを取得、データを格納
                if(json.list()) {
                    foreach(var entry in json.list) {

                        var node = new NicoNicoSearchResultEntry() {
                            Cmsid = entry.id,
                            Title = HttpUtility.HtmlDecode(entry.title_short),
                            ViewCounter = (int)entry.view_counter,
                            CommentCounter = (int)entry.num_res,
                            MylistCounter = (int)entry.mylist_counter,
                            ThumbnailUrl = entry.thumbnail_url,
                            Length = entry.length,
                            FirstRetrieve = entry.first_retrieve
                        };

                        node.FirstRetrieve = node.FirstRetrieve.Replace('-', '/');
                        node.ContentUrl = "http://www.nicovideo.jp/watch/" + node.Cmsid;

                        NicoNicoUtil.ApplyLocalHistory(node);

                        result.List.Add(node);
                    }
                }

                Owner.Status = "";

                return result;

            } catch(RequestFailed) {

                Owner.Status = "検索がタイムアウトしました";
                return null;
            }
        }
    }

    public class NicoNicoSearchResult : NotificationObject {

        //検索結果の総数
        public int Total { get; set; }
        public List<NicoNicoSearchResultEntry> List { get; set; } = new List<NicoNicoSearchResultEntry>();

    }

    public enum SearchType {

        Keyword,
        Tag
    }
    public class NicoNicoSearchResultEntry : IWatchable {

        //ID sm9みたいな
        public string Cmsid { get; set; }

        //タイトル
        public string Title { get; set; }

        //再生回数
        public int ViewCounter { get; set; }

        //コメント数
        public int CommentCounter { get; set; }

        //マイリスト数
        public int MylistCounter { get; set; }

        //サムネイルURL
        public string ThumbnailUrl { get; set; }

        //再生時間
        public string Length { get; set; }

        //動画投稿時日時
        public string FirstRetrieve { get; set; }

        public bool IsWatched { get; set; }

        public string ContentUrl { get; set; }
    }
}
