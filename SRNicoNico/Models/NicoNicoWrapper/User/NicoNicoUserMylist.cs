using Codeplex.Data;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoUserMylist : NotificationObject {

        private static readonly Regex GlobalHashRegex = new Regex(@"Globals\.hash.+'(.+?)';");

        #region Closed変更通知プロパティ
        private bool _Closed;

        public bool Closed {
            get { return _Closed; }
            set {
                if (_Closed == value)
                    return;
                _Closed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region MylistList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoUserMylistEntry> _MylistList;

        public ObservableSynchronizedCollection<NicoNicoUserMylistEntry> MylistList {
            get { return _MylistList; }
            set { 
                if (_MylistList == value)
                    return;
                _MylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly string UserId;

        public NicoNicoUserMylist(string id) {

            UserId = id;
            MylistList = new ObservableSynchronizedCollection<NicoNicoUserMylistEntry>();
        }

        public async Task<string> GetUserMylistAsync() {

            try {

                var query = new GetRequestQuery($"https://nvapi.nicovideo.jp/v1/users/{UserId}/mylists");
                query.AddQuery("sampleItemCount", 3);

                var request = new HttpRequestMessage(HttpMethod.Get, query.TargetUrl);
                request.Headers.Add("X-Frontend-Id", "6");
                request.Headers.Add("X-Frontend-Version", "0");
                request.Headers.Add("X-Niconico-Language", "ja-jp");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var json = DynamicJson.Parse(a);
                if (json.meta.status != 200) {

                    return "ユーザーマイリストの取得に失敗しました";
                }

                foreach (var item in json.data.mylists) {

                    var mylist = new NicoNicoUserMylistEntry {
                        Name = item.name,
                        ContentUrl = $"https://www.nicovideo.jp/user/{UserId}/mylist/{item.id}",
                        Description = item.description
                    };

                    MylistList.Add(mylist);
                }

                return "";
            } catch (RequestFailed) {

                return "ユーザーマイリストの取得に失敗しました";
            }
        }
    }

    public class NicoNicoUserMylistEntry {

        //マイリストURL
        public string ContentUrl { get; set; }

        //マイリストの名前
        public string Name { get; set; }

        //説明
        public string Description { get; set; }

        //---マイリストサムネイル1---
        public bool ThumbNail1Available { get; set; }
        public string ThumbNail1Url { get; set; }
        public string ThumbNail1ToolTip { get; set; }
        //------

        //---マイリストサムネイル2---
        public bool ThumbNail2Available { get; set; }
        public string ThumbNail2Url { get; set; }
        public string ThumbNail2ToolTip { get; set; }
        //------

        //---マイリストサムネイル3---
        public bool ThumbNail3Available { get; set; }
        public string ThumbNail3Url { get; set; }
        public string ThumbNail3ToolTip { get; set; }
        //------
    }
}
