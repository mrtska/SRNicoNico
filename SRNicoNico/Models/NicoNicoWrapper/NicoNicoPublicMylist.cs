using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoPublicMylist {

        private string MylistId;

        private TabItemViewModel Owner;

        public NicoNicoPublicMylist(TabItemViewModel vm, string mylistId) {

            Owner = vm;
            MylistId = Regex.Match(mylistId, @"(\d+)$").Groups[1].Value;
        }


        public async Task<NicoNicoPublicMylistGroupEntry> GetMylistAsync() {

            try {

                var query = new GetRequestQuery($"https://nvapi.nicovideo.jp/v2/mylists/{MylistId}");
                query.AddQuery("page", 1);
                query.AddQuery("pageSize", 300);

                var request = new HttpRequestMessage(HttpMethod.Get, query.TargetUrl);
                request.Headers.Add("X-Frontend-Id", "6");
                request.Headers.Add("X-Frontend-Version", "0");
                request.Headers.Add("X-Niconico-Language", "ja-jp");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request).ConfigureAwait(false);

                var json = DynamicJson.Parse(a);
                if (json.meta.status != 200) {

                    return null;
                }

                var userId = json.data.mylist.owner.id;
                var userName = json.data.mylist.owner.name;
                var mylistName = json.data.mylist.name;
                var description = json.data.mylist.description;
                var ret = new NicoNicoPublicMylistGroupEntry();
                ret.OwnerName = @"<a href=""https://www.nicovideo.jp/user/" + userId + @""">" + userName + "</a> さんの公開マイリスト";
                ret.Name = mylistName;
                ret.Description = description;
                ret.Description = HyperLinkReplacer.Replace(ret.Description);

                var list = new List<NicoNicoMylistEntry>();
                StoreItems(json.data.mylist.items, list);

                ret.Data = list;

                return ret;
            } catch (RequestFailed) {

                Owner.Status = "公開マイリストの取得に失敗しました";
                return null;
            }
        }

        //jsonをパースしてリストに入れる
        private void StoreItems(dynamic json, List<NicoNicoMylistEntry> ret) {

            foreach (var entry in json) {


                var data = new NicoNicoMylistVideoEntry();

                data.ItemId = entry.itemId.ToString();
                data.Description = entry.description;
                data.CreateTime = UnixTime.ToUnixTime(DateTime.Parse(entry.addedAt));

                data.VideoId = entry.watchId;
                data.ContentUrl = "https://www.nicovideo.jp/watch/" + data.VideoId;
                data.Title = entry.video.title;
                data.ThumbNailUrl = entry.video.thumbnail.url;
                data.FirstRetrieve = UnixTime.ToUnixTime(DateTime.Parse(entry.video.registeredAt));
                data.ViewCount = (int)entry.video.count.view;
                data.MylistCount = (int)entry.video.count.mylist;
                data.CommentCount = (int)entry.video.count.comment;
                data.Length = (int)entry.video.duration;

                ret.Add(data);
            }
        }
    }

    //マイリストグループ
    public class NicoNicoPublicMylistGroupEntry {

        //追加日時 Unixタイム
        public string CreateTime { get; set; }

        //マイリストグループコメント
        public string Description { get; set; }

        //グループID
        public string Id { get; set; }

        //名前
        public string Name { get; set; }

        //マイリストオーナーの名前
        public string OwnerName { get; set; }

        //ソートオーダー
        public int SortOrder { get; set; }

        //マイリスト
        public List<NicoNicoMylistEntry> Data { get; set; }
    }
}
