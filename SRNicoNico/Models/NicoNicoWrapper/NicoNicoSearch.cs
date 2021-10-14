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
using SRNicoNico.Views.Converter;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoSearch : NotificationObject {

        //検索API
        private const string SearchURL = "https://ext.nicovideo.jp/api/search/";
        private const string VideoSearchApiUrl = "https://nvapi.nicovideo.jp/v1/search/video";

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
        public async Task<string> Search(string keyword, SearchType type, string sort, int page = 1) {

            SearchResult.Clear();

            string typestr;
            if (type == SearchType.Keyword) {

                typestr = "keyword=" + HttpUtility.UrlEncode(keyword);
            } else {

                typestr = "tag=" + HttpUtility.UrlEncode(keyword);
            }
            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync($"{VideoSearchApiUrl}?_frontendId=6&{typestr}&{sort}&page={page}&pageSize=30");

                //取得したJsonの全体
                var json = DynamicJson.Parse(a);

                //検索結果総数
                if (json.meta.status != 200) {

                    return "エラーが発生しました";
                }
                Total = (int)json.data.totalCount;

                var mssConverter = new SecondsToMSSConverter();

                foreach (var entry in json.data.items) {

                    var node = new NicoNicoSearchResultEntry {
                        Cmsid = entry.id,
                        Title = entry.title,
                        ViewCounter = (int)entry.count.view,
                        CommentCounter = (int)entry.count.comment,
                        MylistCounter = (int)entry.count.mylist,
                        ThumbnailUrl = entry.thumbnail.listingUrl,
                        Length = mssConverter.Convert(entry.duration, null, null, null),
                        FirstRetrieve = DateTimeOffset.Parse(entry.registeredAt).ToString("yyyy/MM/dd HH:mm:ss")
                    };

                    node.ContentUrl = "https://www.nicovideo.jp/watch/" + node.Cmsid;

                    NicoNicoUtil.ApplyLocalHistory(node);

                    SearchResult.Add(node);
                }
                return "";

            } catch (RequestFailed) {

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
