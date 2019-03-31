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
    public class NicoNicoSearch : NotificationObject {

        //検索API
        private const string SearchURL = "https://ext.nicovideo.jp/api/search/";

        //検索結果
        #region SearchResult変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoSearchResultEntry> _SearchResult;

        public ObservableSynchronizedCollection<NicoNicoSearchResultEntry> SearchResult {
            get { return _SearchResult; }
            set { 
                if (_SearchResult == value)
                    return;
                _SearchResult = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Total変更通知プロパティ
        private int _Total;

        public int Total {
            get { return _Total; }
            set { 
                if (_Total == value)
                    return;
                _Total = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoSearch() {

            SearchResult = new ObservableSynchronizedCollection<NicoNicoSearchResultEntry>();
        }

        //リクエストのレスポンスを返す
        public async Task<string> Search(string keyword, SearchType type, string Sort, int page = 1) {

            SearchResult.Clear();

            Sort = "&sort=" + Sort.Split(':')[0] + "&order=" + Sort.Split(':')[1];

            string typestr;
            if(type == SearchType.Keyword) {

                typestr = "search";
            } else {

                typestr = "tag";
            }
            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(SearchURL + typestr + "/" + HttpUtility.UrlEncode(keyword) + "?mode=watch" + Sort + "&page=" + page);

                //取得したJsonの全体
                var json = DynamicJson.Parse(a);

                //検索結果総数
                if (json.count()) {

                    Total = (int)json.count;
                } else {
                    //連打するとエラーになる
                    return "アクセスしすぎです。1分ほど時間を置いてください。";
                }
                //Jsonからリストを取得、データを格納
                if(json.list()) {
                    foreach(var entry in json.list) {

                        var node = new NicoNicoSearchResultEntry() {
                            Cmsid = entry.id,
                            Title = HttpUtility.HtmlDecode(entry.title),
                            ViewCounter = (int)entry.view_counter,
                            CommentCounter = (int)entry.num_res,
                            MylistCounter = (int)entry.mylist_counter,
                            ThumbnailUrl = entry.thumbnail_url,
                            Length = entry.length,
                            FirstRetrieve = entry.first_retrieve
                        };

                        node.FirstRetrieve = node.FirstRetrieve.Replace('-', '/');
                        node.ContentUrl = "https://www.nicovideo.jp/watch/" + node.Cmsid;

                        NicoNicoUtil.ApplyLocalHistory(node);

                        SearchResult.Add(node);
                    }
                }
                return "";

            } catch(RequestFailed) {

                return "検索がタイムアウトしました";
            }
        }
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
