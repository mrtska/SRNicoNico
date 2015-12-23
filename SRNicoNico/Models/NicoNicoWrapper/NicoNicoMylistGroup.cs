using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using System.Web;
using System.Net.Http;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoMylistGroup : NotificationObject {

        //マイリスト一覧取得API
        private const string MylistGroupAPI = "http://www.nicovideo.jp/api/mylistgroup/list";

        //マイリスト作成API
        private const string MylistGroupCreateAPI = "http://www.nicovideo.jp/api/mylistgroup/add";

        //マイリスト更新API
        private const string MylistGroupUpdateAPI = "http://www.nicovideo.jp/api/mylistgroup/update";

        //マイリスト削除API
        private const string MylistGroupDeleteAPI = "http://www.nicovideo.jp/api/mylistgroup/delete";


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


        //自分のマイリストを取得
        public List<NicoNicoMylistGroupData> GetMylistGroup() {

            List<NicoNicoMylistGroupData> ret = new List<NicoNicoMylistGroupData>();

            try {

                //マイリスト取得
                var json = DynamicJson.Parse(NicoNicoWrapperMain.Session.GetAsync(MylistGroupAPI).Result);
                foreach(var entry in json.mylistgroup) {

                    NicoNicoMylistGroupData data = new NicoNicoMylistGroupData();
                    data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                    data.Description = HttpUtility.HtmlDecode(entry.description);

                    data.Id = entry.id;
                    data.Name = HttpUtility.HtmlDecode(entry.name);
                    data.IsPublic = entry.@public == "0" ? false : true;
                    data.SortOrder = int.Parse(entry.sort_order);

                    ret.Add(data);
                }
                return ret;
            } catch(RequestTimeout) {

                return null;
            }
        }


        //マイリストを作成
        public void CreateMylistGroup(string name, string desc, string token) {

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["name"] = name;
            pair["default_sort"] = "1";
            pair["description"] = desc;
            pair["token"] = token;


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistGroupCreateAPI);

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;

        }
        public void CreateMylistGroup(string name, string desc) {

            CreateMylistGroup(name, desc, GetMylistToken());
        }

        //マイリストの情報を更新
        public void UpdateMylistGroup(NicoNicoMylistGroupData group, string token) {

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
        public void UpdateMylistGroup(NicoNicoMylistGroupData group) {

            UpdateMylistGroup(group, GetMylistToken(group));
        }


        //指定したマイリストを削除
        public void DeleteMylistGroup(string groupId, string token) {

            Dictionary<string, string> pair = new Dictionary<string, string>();
            pair["group_id"] = groupId;
            pair["token"] = token;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MylistGroupDeleteAPI);

            request.Content = new FormUrlEncodedContent(pair);

            var text = NicoNicoWrapperMain.GetSession().GetAsync(request).Result;
        }
        public void DeleteMylistGroup(string groupId) {

            DeleteMylistGroup(groupId, GetMylistToken());
        }
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
}
