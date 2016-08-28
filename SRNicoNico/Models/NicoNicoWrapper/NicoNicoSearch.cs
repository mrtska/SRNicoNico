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
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoSearch : NotificationObject {

        //検索API
        private const string SearchURL = "http://ext.nicovideo.jp/api/search/";

        //取得しているページ
        private byte CurrentPage = 1;

        private SearchViewModel Owner;


        public NicoNicoSearch(SearchViewModel vm) {

            Owner = vm;


        }
        //リクエストのレスポンスを返す
        public async Task<NicoNicoSearchResult> Search(string keyword, SearchType type, string Sort, int page = 1) {

            
            Owner.Status = "検索中:" +  keyword;
            
            Sort = "&sort=" + Sort.Split(':')[0] + "&order=" + Sort.Split(':')[1];
            
            string typestr;
            if(type == SearchType.Keyword) {

                typestr = "search";
            } else {

                typestr = "tag";
            }

            var result = new NicoNicoSearchResult();

            try {

                var a = await NicoNicoWrapperMain.Session.GetAsync(SearchURL + typestr + "/" + keyword + "?mode=watch" + Sort + "&page=" + page);

                //取得したJsonの全体
                var json = DynamicJson.Parse(a);

                //検索結果総数
                if(json.count()) {
                    result.Total = (ulong)json.count;
                } else {

                    //連打するとエラーになる
                    Owner.Status = "アクセスしすぎです。1分ほど時間を置いてください。";
                    return null;
                }

                //Jsonからリストを取得、データを格納
                if(json.list()) {
                    foreach(var entry in json.list) {

                        var node = new NicoNicoVideoInfoEntry();

                        node.Cmsid = entry.id;
                        node.Title = entry.title_short;
                        node.ViewCounter = (int)entry.view_counter;
                        node.CommentCounter = (int)entry.num_res;
                        node.MylistCounter = (int)entry.mylist_counter;
                        node.ThumbnailUrl = entry.thumbnail_url;
                        node.Length = entry.length;
                        node.FirstRetrieve = entry.first_retrieve;

                        result.List.Add(node);
                    }
                }


                Owner.Status = "";


                return result;

            } catch(RequestTimeout) {

                Owner.Status = "検索がタイムアウトしました";
                return null;
            }
        }
    }


    public class NicoNicoSearchResult : NotificationObject {

        //検索結果の総数
        public ulong Total { get; set; }
        public List<NicoNicoVideoInfoEntry> List { get; set; } = new List<NicoNicoVideoInfoEntry>();

    }


    public enum SearchType {

        Keyword,
        Tag
    }

}
