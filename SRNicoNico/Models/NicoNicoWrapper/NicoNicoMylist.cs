using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Web;
using Livet;

using Codeplex.Data;

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoMylist : NotificationObject {


        //とりあえずマイリスト取得API
        private const string DefListAPI = "http://www.nicovideo.jp/api/deflist/list";

        //とりあえずマイリスト登録API
        private const string DefListAddAPI = "http://www.nicovideo.jp/api/deflist/add";

        //とりあえずマイリスト複製API
        private const string DefListCopyAPI = "http://www.nicovideo.jp/api/deflist/copy";

        //とりあえずマイリスト移動API
        private const string DefListMoveAPI = "http://www.nicovideo.jp/api/deflist/move";

        //とりあえずマイリスト削除API
        private const string DefListDeleteAPI = "http://www.nicovideo.jp/api/deflist/delete";

        //マイリスト取得API
        private const string MylistGetAPI = "http://www.nicovideo.jp/api/mylist/list?group_id=";

        //マイリスト追加API
        private const string MylistAddAPI = "http://www.nicovideo.jp/api/mylist/add";

        //マイリスト複製API
        private const string MylistCopyAPI = "http://www.nicovideo.jp/api/mylist/copy";

        //マイリスト移動API
        private const string MylistMoveAPI = "http://www.nicovideo.jp/api/mylist/move";

        //マイリスト削除API
        private const string MylistDeleteAPI = "http://www.nicovideo.jp/api/mylist/delete";


        //jsonをパースしてリストにする
        private void StoreItem(dynamic json, List<NicoNicoMylistData> ret) {

            foreach(var entry in json.mylistitem) {

                NicoNicoMylistData data = new NicoNicoMylistData();
                data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                data.Description = entry.description;
                data.ItemId = entry.item_id;

                var item = entry.item_data;
                data.Title = HttpUtility.HtmlDecode(item.title);

                if(entry.item_type is string) {

                    data.Type = int.Parse(entry.item_type);
                } else if(entry.item_type is double) {

                    data.Type = (int)entry.item_type;
                }

                //動画
                if(data.Type == 0) {

                    data.UpdateTime = UnixTime.FromUnixTime((long)item.update_time).ToString();
                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.first_retrieve).ToString();
                    data.Length = NicoNicoUtil.ConvertTime(long.Parse(item.length_seconds));
                    data.Id = item.video_id;
                    data.ViewCounter = int.Parse(item.view_counter);
                    data.CommentCounter = int.Parse(item.num_res);
                    data.MylistCounter = int.Parse(item.mylist_counter);
                    data.ThumbNailUrl = item.thumbnail_url;

                } else if(data.Type == 5) { //静画

                    data.UpdateTime = UnixTime.FromUnixTime((long)item.update_time).ToString();
                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.create_time).ToString();
                    data.Id = item.id.ToString();
                    data.ViewCounter = (int)item.view_count;
                    data.CommentCounter = (int)item.comment_count;
                    data.MylistCounter = (int)item.mylist_count;
                    data.ThumbNailUrl = item.thumbnail_url;

                } else if(data.Type == 6) { //書籍

                    data.UpdateTime = UnixTime.FromUnixTime((long)entry.update_time).ToString();
                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.released_at).ToString();
                    data.Id = "bk" + item.id;
                    data.ViewCounter = (int)item.view_count;
                    data.CommentCounter = (int)item.comment_count;
                    data.MylistCounter = (int)item.mylist_count;
                    data.ThumbNailUrl = item.thumbnail;

                } else if(data.Type == 13) { //ブロマガ

                    data.UpdateTime = UnixTime.FromUnixTime((long)item.commented_time).ToString();
                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.create_time).ToString();
                    data.Id = "ar" + item.id;
                    data.CommentCounter = (int)item.comment_count;
                    data.MylistCounter = int.Parse(item.mylist_count);
                    data.ThumbNailUrl = item.thumbnail_url;
                }
                ret.Add(data);
            }
        }
        //とりあえずマイリストを取得する
        public List<NicoNicoMylistData> GetDefMylist() {

            //とりあえずマイリスト
            var json = DynamicJson.Parse(NicoNicoWrapperMain.GetSession().GetAsync(DefListAPI).Result);

            List<NicoNicoMylistData> ret = new List<NicoNicoMylistData>();

            StoreItem(json, ret);

            return ret;
        }

        //指定したIDのマイリストを取得する
        public List<NicoNicoMylistData> GetMylist(string groupId) {

            //0なら"とりあえずマイリスト"を返す
            if(groupId == "0") {

                return GetDefMylist();
            }

            //指定したマイリスト
            var json = DynamicJson.Parse(NicoNicoWrapperMain.GetSession().GetAsync(MylistGetAPI + groupId).Result);

            List<NicoNicoMylistData> ret = new List<NicoNicoMylistData>();

            StoreItem(json, ret);

            return ret;
        }

        //とりあえずマイリストに登録
        public MylistResult AddDefMylist(string cmsid, string token) {

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["item_id"] = cmsid;
            pair["token"] = token;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, DefListAddAPI);

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;

            var json = DynamicJson.Parse(text);

            //エラー時
            if(json.error()) {

                //もうすでに登録済み
                if(json.error.code == "EXIST") {

                    return MylistResult.EXIST;
                }
            }
            if(json.status()) {

                if(json.status == "ok") {

                    return MylistResult.SUCCESS;
                }
            }
            //失敗
            return MylistResult.FAILED;
        }


        //マイリストに追加
        public MylistResult AddMylist(NicoNicoMylistGroupData group, string cmsid, string desc, string token) {

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = group.Id;
            pair["item_id"] = cmsid;
            pair["description"] = desc;
            pair["token"] = token;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistAddAPI);

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;

            var json = DynamicJson.Parse(text);

            //エラー時
            if(json.error()) {

                //もうすでに登録済み
                if(json.error.code == "EXIST") {

                    return MylistResult.EXIST;
                }
            }
            if(json.status()) {

                if(json.status == "ok") {

                    return MylistResult.SUCCESS;
                }
            }
            //失敗
            return MylistResult.FAILED;
        }


        //マイリストを移動
        public void MoveMylist(MylistListEntryViewModel source, MylistListViewModel dest) {

            string token = GetMylistToken(source.Owner.Group);

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = source.Owner.Group.Id;
            pair["target_group_id"] = dest.Group.Id;
            pair["id_list[0][]"] = source.Entry.ItemId;
            pair["token"] = token;


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistMoveAPI);

            //とりあえずマイリストだったら
            if(source.Owner.Group.Id == "0") {

                request.RequestUri = new Uri(DefListMoveAPI);
            }

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;

            //移動先がとりあえずマイリストだったら
            if(dest.Group.Id == "0") {

                AddDefMylist(source.Entry.Id, token);
            }
        }

        //マイリストを移動
        public void MoveMylist(IEnumerable<MylistListEntryViewModel> source, MylistListViewModel dest) {

            string token = GetMylistToken(source.First().Owner.Group);

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = source.First().Owner.Group.Id;
            pair["target_group_id"] = dest.Group.Id;
            pair["token"] = token;

            //id_list以外のペアを指定
            var encodedContent = new FormUrlEncodedContent(pair);

            //エンコードされたデータを取得
            var text = encodedContent.ReadAsStringAsync().Result;

            //id_listを付け足す
            text += MakeIdList(source);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistMoveAPI);

            //ソースがとりあえずマイリストだったら
            if(source.First().Owner.Group.Id == "0") {

                request.RequestUri = new Uri(DefListMoveAPI);
            }

            //データｗ指定
            request.Content = new StringContent(text);

            //コンテンツタイプを設定
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            //移動先がとりあえずマイリストだったら
            if(dest.Group.Id == "0") {

                foreach(MylistListEntryViewModel vm in source) {

                    AddDefMylist(vm.Entry.Id, token);
                }
            } else {

                var ret = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;
            }

        }

        //マイリストをコピー
        public void CopyMylist(IEnumerable<MylistListEntryViewModel> source, MylistListViewModel dest) {

            string token = GetMylistToken(source.First().Owner.Group);

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = source.First().Owner.Group.Id;
            pair["target_group_id"] = dest.Group.Id;
            pair["token"] = token;

            //フォームエンコード済み文字列を取得
            var encodedContent = new FormUrlEncodedContent(pair);
            var text = encodedContent.ReadAsStringAsync().Result;

            //IDリスト作成
            text += MakeIdList(source);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistCopyAPI);

            if(source.First().Owner.Group.Id == "0") {

                request.RequestUri = new Uri(DefListCopyAPI);
            }

            request.Content = new StringContent(text);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");


            if(dest.Group.Id == "0") {

                foreach(MylistListEntryViewModel vm in source) {

                    AddDefMylist(vm.Entry.Id, token);
                }
            } else {

                var ret = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;
            }



        }
        //マイリストを削除
        public void DeleteMylist(IEnumerable<MylistListEntryViewModel> source) {

            string token = GetMylistToken(source.First().Owner.Group);

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = source.First().Owner.Group.Id;
            pair["token"] = token;

            var encodedContent = new FormUrlEncodedContent(pair);

            var text = encodedContent.ReadAsStringAsync().Result;

            text += MakeIdList(source);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistDeleteAPI);

            if(source.First().Owner.Group.Id == "0") {

                request.RequestUri = new Uri(DefListDeleteAPI);
            }

            request.Content = new StringContent(text);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var ret = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;
        }

        //マイリストページからCSRFトークンを取得する
        public string GetMylistToken(NicoNicoMylistGroupData Group) {
            
            var api = "http://www.nicovideo.jp/my/mylist/#/" + Group.Id;

            var result = NicoNicoWrapperMain.GetSession().GetAsync(api).Result;

            return result.Substring(result.IndexOf("NicoAPI.token = \"") + 17, 60);
        }
        public string GetMylistToken() {

            var api = "http://www.nicovideo.jp/my/mylist";

            var result = NicoNicoWrapperMain.GetSession().GetAsync(api).Result;

            return result.Substring(result.IndexOf("NicoAPI.token = \"") + 17, 60);
        }

        //IDリストを作成
        private string MakeIdList(IEnumerable<MylistListEntryViewModel> source) {

            string text = "&";

            foreach(MylistListEntryViewModel vm in source) {

                text += "id_list[0][]=" + vm.Entry.ItemId + "&";
            }

            return text.Substring(0, text.Length - 1);
        }
    }

    //マイリスト結果
    public enum MylistResult {

        SUCCESS,
        EXIST,
        FAILED
    }


    //マイリストエントリ
    public class NicoNicoMylistData {

        //エントリタイプ 0が動画 13がブロマガ
        public int Type { get; set; }

        //追加日時 Unixタイム
        public string CreateTime { get; set; }

        //コメントアップデートタイム
        public string UpdateTime { get; set; }

        //マイリストコメント
        public string Description { get; set; }

        //投稿日時 Unixタイム
        public string FirstRetrieve { get; set; }

        //動画時間 秒
        public string Length { get; set; }

        //ID
        public string Id { get; set; }
        
        //マイリストアイテムID
        public string ItemId { get; set; }

        //タイトル
        public string Title { get; set; }

        //再生数
        public int ViewCounter { get; set; }

        //コメント数
        public int CommentCounter { get; set; }

        //マイリスト数
        public int MylistCounter { get; set; }

        //サムネイルURL
        public string ThumbNailUrl { get; set; }

        public override string ToString() {
            return "Create:" + CreateTime + " Comment:" + Description + " FirstRetrive:" + FirstRetrieve + " Length:" + Length + " Title:" + Title + " ViewCounter:" + ViewCounter + " CommentCounter:" + CommentCounter + " MylistCounter:" + MylistCounter; 
        }
    }

}
