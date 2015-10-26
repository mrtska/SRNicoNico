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

        //とりあえずマイリストコピーAPI
        private const string DefListCopyAPI = "http://www.nicovideo.jp/api/deflist/copy";

        //とりあえずマイリスト移動API
        private const string DefListMoveAPI = "http://www.nicovideo.jp/api/deflist/move";

        //とりあえずマイリスト削除API
        private const string DefListDeleteAPI = "http://www.nicovideo.jp/api/deflist/delete";

        //マイリスト一覧取得API
        private const string MylistGroupAPI = "http://www.nicovideo.jp/api/mylistgroup/list";

        //マイリスト作成API
        private const string MylistGroupCreateAPI = "http://www.nicovideo.jp/api/mylistgroup/add";

        //マイリスト更新API
        private const string MylistGroupUpdateAPI = "http://www.nicovideo.jp/api/mylistgroup/update";

        //マイリスト削除API
        private const string MylistGroupDeleteAPI = "http://www.nicovideo.jp/api/mylistgroup/delete";

        private const string MylistAddAPI = "http://www.nicovideo.jp/api/mylist/add";

        //マイリストコピーAPI
        private const string MylistCopyAPI = "http://www.nicovideo.jp/api/mylist/copy";

        //マイリスト移動API
        private const string MylistMoveAPI = "http://www.nicovideo.jp/api/mylist/move";

        //マイリスト削除API
        private const string MylistDeleteAPI = "http://www.nicovideo.jp/api/mylist/delete";

        public NicoNicoMylist() {


        }

        //jsonをパースしてリストにする
        private void StoreItem(dynamic json, List<NicoNicoMylistData> ret) {

            foreach(var entry in json.mylistitem) {

                NicoNicoMylistData data = new NicoNicoMylistData();
                data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                data.Description = entry.description;
                data.ItemId = entry.item_id;

                var item = entry.item_data;

                if(entry.item_type is string) {

                    data.Type = int.Parse(entry.item_type);
                } else if(entry.item_type is double) {

                    data.Type = (int)entry.item_type;
                }


                //動画
                if(data.Type == 0) {

                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.first_retrieve).ToString();
                    data.Length = NicoNicoUtil.GetTimeFromLong(long.Parse(item.length_seconds));
                    data.Id = item.video_id;
                    data.Title = item.title;
                    data.ViewCounter = int.Parse(item.view_counter);
                    data.CommentCounter = int.Parse(item.num_res);
                    data.MylistCounter = int.Parse(item.mylist_counter);
                    data.ThumbNailUrl = item.thumbnail_url;

                } else if(data.Type == 5) { //静画

                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.create_time).ToString();
                    data.Id = item.id.ToString();
                    data.Title = item.title;
                    data.ViewCounter = (int)item.view_count;
                    data.CommentCounter = (int)item.comment_count;
                    data.MylistCounter = (int)item.mylist_count;
                    data.ThumbNailUrl = item.thumbnail_url;

                } else if(data.Type == 6) { //書籍

                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.released_at).ToString();
                    data.Id = "bk" + item.id;
                    data.Title = item.title;
                    data.ViewCounter = (int)item.view_count;
                    data.CommentCounter = (int)item.comment_count;
                    data.MylistCounter = (int)item.mylist_count;
                    data.ThumbNailUrl = item.thumbnail;

                } else if(data.Type == 13) { //ブロマガ

                    data.FirstRetrieve = UnixTime.FromUnixTime((long)item.create_time).ToString();
                    data.Id = "ar" + item.id;
                    data.Title = item.title;
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

            const string api = "http://www.nicovideo.jp/api/mylist/list?group_id=";

            //指定したマイリスト
            var json = DynamicJson.Parse(NicoNicoWrapperMain.GetSession().GetAsync(api + groupId).Result);

            List<NicoNicoMylistData> ret = new List<NicoNicoMylistData>();

            StoreItem(json, ret);

            return ret;
        }

        //自分のマイリストを取得
        public List<NicoNicoMylistGroupData> GetMylistGroup() {

            List<NicoNicoMylistGroupData> ret = new List<NicoNicoMylistGroupData>();

            //マイリスト取得
            var json = DynamicJson.Parse(NicoNicoWrapperMain.GetSession().GetAsync(MylistGroupAPI).Result);
            foreach(var entry in json.mylistgroup) {

                NicoNicoMylistGroupData data = new NicoNicoMylistGroupData();
                data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                data.Description = entry.description;

                data.Id = entry.id;
                data.Name = entry.name;
                data.IsPublic = entry.@public == "0" ? false : true;
                data.SortOrder = int.Parse(entry.sort_order);

                ret.Add(data);
            }
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

        //マイリストを作成
        public void CreateMylistGroup(string name, string desc) {

            string token = GetMylistToken();

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["name"] = name;
            pair["default_sort"] = "1";
            pair["description"] = desc;
            pair["token"] = token;


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistGroupCreateAPI);

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;
            
        }

        //マイリストの情報を更新
        public void UpdateMylistGroup(NicoNicoMylistGroupData group) {


            string token = GetMylistToken(group);

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = group.Id;
            pair["name"] = group.Name;
            pair["default_sort"] = "1";
            pair["description"] = group.Description;
            pair["token"] = token;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistGroupUpdateAPI);

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;
        }

        //指定したマイリストを削除
        public void DeleteMylistGroup(string groupId) {

            string token = GetMylistToken();

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = groupId;
            pair["token"] = token;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistGroupDeleteAPI);

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;
        }

        //マイリストに追加
        public MylistResult AddMylist(NicoNicoMylistGroupData group, string cmsid, string token) {

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = group.Id;
            pair["item_id"] = cmsid;
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

            var encodedContent = new FormUrlEncodedContent(pair);

            var text = encodedContent.ReadAsStringAsync().Result;

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

    //マイリストグループエントリ
    public class NicoNicoMylistGroupData {


        //追加日時 Unixタイム
        public string CreateTime { get; set; }

        //マイリストグループコメント
        public string Description { get; set; }

        //グループID
        public string Id { get; set; }

        //名前
        public string Name { get; set; }

        //is公開
        public bool IsPublic { get; set; }

        //ソートオーダー
        public int SortOrder { get; set; }


        public string BeforeName { get; set; }
        public string BeforeDescription { get; set; }
    }

    //マイリストエントリ
    public class NicoNicoMylistData {

        //エントリタイプ 0が動画 13がブロマガ
        public int Type { get; set; }

        //追加日時 Unixタイム
        public string CreateTime { get; set; }

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
