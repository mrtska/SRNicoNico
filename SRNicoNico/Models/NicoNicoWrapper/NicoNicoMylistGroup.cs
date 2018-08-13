using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoMylistGroup {

        //マイリスト一覧取得Api
        private const string MylistGroupApi = "http://www.nicovideo.jp/api/mylistgroup/list";

        //マイリスト作成Api
        private const string MylistGroupCreateApi = "http://www.nicovideo.jp/api/mylistgroup/add";

        //マイリスト更新Api
        private const string MylistGroupUpdateApi = "http://www.nicovideo.jp/api/mylistgroup/update";

        //マイリスト削除Api
        private const string MylistGroupDeleteApi = "http://www.nicovideo.jp/api/mylistgroup/delete";

        //とりあえずマイリスト取得Api
        private const string DefListGetApi = "http://www.nicovideo.jp/api/deflist/list";

        private TabItemViewModel Owner;

        public NicoNicoMylistGroup(TabItemViewModel owner) {

            Owner = owner;
        }

        //マイリスト一覧を取得
        public async Task<List<NicoNicoMylistGroupEntry>> GetMylistGroupAsync() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(MylistGroupApi);
                dynamic json = DynamicJson.Parse(a);

                var ret = new List<NicoNicoMylistGroupEntry>();

                foreach(var entry in json.mylistgroup) {

                    var data = new NicoNicoMylistGroupEntry();

                    data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                    data.Description = data.DescriptionOriginal = HttpUtility.HtmlDecode(entry.description);
                    data.Id = entry.id;
                    data.Name = data.NameOriginal = HttpUtility.HtmlDecode(entry.name);
                    data.IsPublic = entry.@public != "0";
                    data.SortOrder = int.Parse(entry.sort_order);

                    ret.Add(data);
                }
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "マイリストグループの取得に失敗しました";
                return null;
            }
        }

        //マイリストを作成
        public async Task<bool> CreateMylistAsync(string title, string desc, string token) {

            try {

                var pair = new Dictionary<string, string>();
                pair["name"] = title;
                pair["default_sort"] = "1";
                pair["description"] = desc;
                pair["token"] = token;

                var request = new HttpRequestMessage(HttpMethod.Post, MylistGroupCreateApi);
                request.Content = new FormUrlEncodedContent(pair);

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                dynamic json = DynamicJson.Parse(a);
                if(json.status == "ok") {

                    return true;
                } else {

                    Owner.Status = "マイリストの作成に失敗しました";
                    return false;
                }
            } catch(RequestFailed) {

                Owner.Status = "マイリストの作成に失敗しました";
                return false;
            }
        }

        //マイリストを編集
        public async Task<bool> UpdateMylistAsync(NicoNicoMylistGroupEntry group, string token) {

            try {

                Owner.Status = "マイリスト情報を更新中";

                var pair = new Dictionary<string, string>();
                pair["group_id"] = group.Id;
                pair["name"] = group.Name;
                pair["default_sort"] = "1";
                pair["description"] = group.Description;
                pair["token"] = token;

                var request = new HttpRequestMessage(HttpMethod.Post, MylistGroupUpdateApi);
                request.Content = new FormUrlEncodedContent(pair);

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                dynamic json = DynamicJson.Parse(a);
                if(json.status == "ok") {

                    Owner.Status = "マイリスト情報を更新しました";
                    return true;
                } else {

                    Owner.Status = "マイリストの更新に失敗しました";
                    return false;
                }
            } catch(RequestFailed) {

                Owner.Status = "マイリストの更新に失敗しました";
                return false;
            }
        }

        //マイリストを削除
        public async Task<bool> DeleteMylistAsync(NicoNicoMylistGroupEntry group, string token) {

            try {

                Owner.Status = group.Name + " を削除中";

                var pair = new Dictionary<string, string>();
                pair["group_id"] = group.Id;
                pair["token"] = token;

                var request = new HttpRequestMessage(HttpMethod.Post, MylistGroupDeleteApi);
                request.Content = new FormUrlEncodedContent(pair);

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                dynamic json = DynamicJson.Parse(a);
                if(json.status == "ok") {

                    Owner.Status = "マイリストを削除しました";
                    return true;
                } else {

                    Owner.Status = "マイリストの削除に失敗しました";
                    return false;
                }
            } catch(RequestFailed) {

                Owner.Status = "マイリストの削除に失敗しました";
                return false;
            }
        }
    }
    //マイリストグループ
    public class NicoNicoMylistGroupEntry {

        //追加日時 Unixタイム
        public string CreateTime { get; set; }

        //マイリストグループコメント
        public string Description { get; set; }
        public string DescriptionOriginal { get; set; }

        //グループID
        public string Id { get; set; }

        //名前
        public string Name { get; set; }
        public string NameOriginal { get; set; }

        //is公開
        public bool IsPublic { get; set; }

        //ソートオーダー
        public int SortOrder { get; set; }
    }
}
