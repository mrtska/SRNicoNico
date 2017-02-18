using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;
using Codeplex.Data;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoMylistItem {


        //とりあえずマイリスト取得Api
        private const string DefListGetApi = "http://www.nicovideo.jp/api/deflist/list";

        //とりあえずマイリスト登録Api
        private const string DefListAddApi = "http://www.nicovideo.jp/api/deflist/add";

        //とりあえずマイリスト複製Api
        private const string DefListCopyApi = "http://www.nicovideo.jp/api/deflist/copy";

        //とりあえずマイリスト移動Api
        private const string DefListMoveApi = "http://www.nicovideo.jp/api/deflist/move";

        //とりあえずマイリストコメント更新Api
        private const string DefListUpdateApi = "http://www.nicovideo.jp/api/deflist/update";

        //とりあえずマイリスト削除Api
        private const string DefListDeleteApi = "http://www.nicovideo.jp/api/deflist/delete";

        //マイリスト取得Api
        private const string MylistGetApi = "http://www.nicovideo.jp/api/mylist/list";

        //マイリスト追加Api
        private const string MylistAddApi = "http://www.nicovideo.jp/api/mylist/add";

        //マイリスト複製Api
        private const string MylistCopyApi = "http://www.nicovideo.jp/api/mylist/copy";

        //マイリスト移動Api
        private const string MylistMoveApi = "http://www.nicovideo.jp/api/mylist/move";

        //マイリストコメント更新Api
        private const string MylistUpdateApi = "http://www.nicovideo.jp/api/mylist/update";

        //マイリスト削除Api
        private const string MylistDeleteApi = "http://www.nicovideo.jp/api/mylist/delete";

        private TabItemViewModel Owner;

        public NicoNicoMylistItem(TabItemViewModel owner) {

            Owner = owner;
        }


        //とりあえずマイリストを取得
        public async Task<List<NicoNicoMylistEntry>> GetDefListAsync() {

            try {

                Owner.Status = "とりあえずマイリスト取得中";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(DefListGetApi);

                var ret = new List<NicoNicoMylistEntry>();
                StoreItems(a, ret);

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "とりあえずマイリストの取得に失敗しました";
                return null;
            }

        }

        //とりあえずマイリストに追加
        public async Task<bool> AddDefListAsync(string cmsid, string token) {

            try {

                Owner.Status = "とりあえずマイリスト登録中";

                var pair = new Dictionary<string, string>();
                pair["item_id"] = cmsid;
                pair["token"] = token;

                var request = new HttpRequestMessage(HttpMethod.Post, DefListAddApi) {
                    Content = new FormUrlEncodedContent(pair)
                };
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var json = DynamicJson.Parse(a);

                if(json.error()) {

                    if(json.error.code == "EXIST") {

                        Owner.Status = "既に登録済みです";
                        return false;
                    }
                }
                if(json.status()) {

                    if(json.status == "ok") {

                        Owner.Status = "とりあえずマイリストに登録しました";
                        return true;
                    }
                }
                Owner.Status = "登録に失敗しました";
                return false;
            } catch(RequestFailed) {

                Owner.Status = "登録に失敗しました";
                return false;
            }
        }

        //マイリストコメントを更新
        public async Task<bool> UpdateDescriptionAsync(NicoNicoMylistGroupEntry group, NicoNicoMylistEntry entry, string token) {

            try {

                Owner.Status = "マイリストコメント更新中";

                var pair = new Dictionary<string, string>();

                //とりあえずマイリストはgroupがnullなので
                if(group != null) {

                    pair["group_id"] = group.Id;
                }

                pair["item_id"] = entry.ItemId;
                pair["description"] = entry.Description;
                pair["token"] = token;

                //マイリストタイプによって変わる
                if(entry is NicoNicoMylistVideoEntry) {

                    pair["item_type"] = "0";
                } else if(entry is NicoNicoMylistMangaEntry) {

                    pair["item_type"] = "5";
                } else if(entry is NicoNicoMylistBookEntry) {

                    pair["item_type"] = "6";
                } else if(entry is NicoNicoMylistArticleEntry) {

                    pair["item_type"] = "13";
                }

                var request = new HttpRequestMessage(HttpMethod.Post, MylistUpdateApi);

                if(group == null) {

                    request.RequestUri = new Uri(DefListUpdateApi);
                }

                request.Content = new FormUrlEncodedContent(pair);

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                dynamic json = DynamicJson.Parse(a);
                if(json.status == "ok") {

                    Owner.Status = "マイリストコメントを更新しました";
                    return true;
                } else {

                    Owner.Status = "マイリストコメントの更新に失敗しました";
                    return false;
                }
            } catch(RequestFailed) {

                Owner.Status = "マイリストコメントの更新に失敗しました";
                return false;
            }
        }


        //マイリストを取得
        public async Task<List<NicoNicoMylistEntry>> GetMylistAsync(NicoNicoMylistGroupEntry group) {

            try {

                Owner.Status = "マイリスト " + group.Name + " 取得中";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(MylistGetApi + "?group_id=" + group.Id);

                var ret = new List<NicoNicoMylistEntry>();
                StoreItems(a, ret);

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "マイリストの取得に失敗しました";
                return null;
            }
        }

        //マイリストに追加
        public async Task<bool> AddMylistAsync(NicoNicoMylistGroupEntry group, string cmsid, string desc, string token) {

            try {

                var pair = new Dictionary<string, string> {
                    ["group_id"] = group.Id,
                    ["item_id"] = cmsid,
                    ["description"] = desc,
                    ["token"] = token
                };
                var request = new HttpRequestMessage(HttpMethod.Post, MylistAddApi);
                request.Content = new FormUrlEncodedContent(pair);

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var json = DynamicJson.Parse(a);

                if(json.error()) {

                    if(json.error.code == "EXIST") {

                        Owner.Status = "既に登録済みです";
                        return false;
                    }
                }
                if(json.status()) {

                    if(json.status == "ok") {

                        Owner.Status = group.Name + " に登録しました";
                        return true;
                    }
                }

                Owner.Status = "登録に失敗しました";
                return false;
            } catch(RequestFailed) {

                Owner.Status = "登録に失敗しました";
                return false;
            }
        }

        //マイリストをコピーする
        public async Task<bool> CopyMylistAsync(IEnumerable<MylistResultEntryViewModel> source, NicoNicoMylistGroupEntry target, string token) {

            try {

                Owner.Status = "マイリストコピー中";

                var form = new Dictionary<string, string>();
                form["group_id"] = source.First().Owner.Group.Id;
                form["target_group_id"] = target.Id;
                form["token"] = token;

                //Dictionaryをテキストに変換
                var text = await new FormUrlEncodedContent(form).ReadAsStringAsync();
                text += MakeIdList(source);

                var request = new HttpRequestMessage(HttpMethod.Post, MylistCopyApi);

                //とりあえずマイリストだったらApiが変わってくる
                if(source.First().Owner.IsDefList) {

                    request.RequestUri = new Uri(DefListCopyApi);
                }

                //Dictionaryで扱えないのでrawなstringで渡す
                request.Content = new StringContent(text);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                dynamic json = DynamicJson.Parse(a);
                if(json.status == "ok") {

                    Owner.Status = "マイリストをコピーしました";
                    return true;
                } else {

                    Owner.Status = "マイリストのコピーに失敗しました";
                    return false;
                }
            } catch(RequestFailed) {

                Owner.Status = "マイリストのコピーに失敗しました";
                return false;
            }
        }

        //マイリスト移動
        public async Task<bool> MoveMylistAsync(IEnumerable<MylistResultEntryViewModel> source, NicoNicoMylistGroupEntry target, string token) {

            try {

                Owner.Status = "マイリスト移動中";

                var form = new Dictionary<string, string>();
                form["group_id"] = source.First().Owner.Group.Id;
                form["target_group_id"] = target.Id;
                form["token"] = token;

                //Dictionaryをテキストに変換
                var text = await new FormUrlEncodedContent(form).ReadAsStringAsync();
                text += MakeIdList(source);

                var request = new HttpRequestMessage(HttpMethod.Post, MylistMoveApi);

                //とりあえずマイリストだったらApiが変わってくる
                if(source.First().Owner.IsDefList) {

                    request.RequestUri = new Uri(DefListMoveApi);
                }

                //Dictionaryで扱えないのでrawなstringで渡す
                request.Content = new StringContent(text);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                dynamic json = DynamicJson.Parse(a);
                if(json.status == "ok") {

                    Owner.Status = "マイリストを移動しました";
                    return true;
                } else {

                    Owner.Status = "マイリストの移動に失敗しました";
                    return false;
                }
            } catch(RequestFailed) {

                Owner.Status = "マイリストの移動に失敗しました";
                return false;
            }
        }

        //マイリスト移動
        public async Task<bool> DeleteMylistAsync(IEnumerable<MylistResultEntryViewModel> source, string token) {

            try {

                Owner.Status = "マイリスト削除中";

                var form = new Dictionary<string, string>();

                //とりあえずマイリストだったらパラメータが変わってくる
                if(!source.First().Owner.IsDefList) {

                    form["group_id"] = source.First().Owner.Group.Id;
                }
                form["token"] = token;

                //Dictionaryをテキストに変換
                var text = await new FormUrlEncodedContent(form).ReadAsStringAsync();
                text += MakeIdList(source);

                var request = new HttpRequestMessage(HttpMethod.Post, MylistDeleteApi);

                //とりあえずマイリストだったらApiが変わってくる
                if(source.First().Owner.IsDefList) {

                    request.RequestUri = new Uri(DefListDeleteApi);
                }

                //Dictionaryで扱えないのでrawなstringで渡す
                request.Content = new StringContent(text);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

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

        //IDリストを作成
        //何このクソ仕様 酷くない？
        private string MakeIdList(IEnumerable<MylistResultEntryViewModel> source) {

            var text = "&";

            foreach(MylistResultEntryViewModel vm in source) {

                if(vm.Item is NicoNicoMylistVideoEntry) {

                    text += "id_list[0][]=" + vm.Item.ItemId + "&";
                } else if(vm.Item is NicoNicoMylistMangaEntry) {

                    text += "id_list[5][]=" + vm.Item.ItemId + "&";
                } else if(vm.Item is NicoNicoMylistBookEntry) {

                    text += "id_list[6][]=" + vm.Item.ItemId + "&";
                } else if(vm.Item is NicoNicoMylistArticleEntry) {

                    text += "id_list[13][]=" + vm.Item.ItemId + "&";
                }
            }

            //末尾に&が残ってるはずなので消す
            return text.Substring(0, text.Length - 1);
        }


        //jsonをパースしてリストに入れる
        private void StoreItems(string str, List<NicoNicoMylistEntry> ret) {

            var json = DynamicJson.Parse(str);

            foreach(var entry in json.mylistitem) {

                //普通のマイリストはなぜかdoubleでとりあえずマイリストはstringなんだよね 意味分からん
                if(entry.item_type is double) {

                    entry.item_type = entry.item_type.ToString();
                }

                if(entry.item_type is string) {

                    var type = entry.item_type as string;
                    var item = entry.item_data;
                    switch(type) {
                        case "0": {

                                var data = new NicoNicoMylistVideoEntry();

                                data.ItemId = entry.item_id;
                                data.Description = entry.description;
                                data.CreateTime = (long)entry.create_time;
                                data.UpdateTime = (long)entry.update_time;

                                data.VideoId = item.video_id;
                                data.ContentUrl = "http://www.nicovideo.jp/watch/" + data.VideoId;
                                data.Title = HttpUtility.HtmlDecode(item.title);
                                data.ThumbNailUrl = item.thumbnail_url;
                                data.FirstRetrieve = (long)item.first_retrieve;
                                data.ViewCount = int.Parse(item.view_counter);
                                data.MylistCount = int.Parse(item.mylist_counter);
                                data.CommentCount = int.Parse(item.num_res);
                                data.Length = int.Parse(item.length_seconds);

                                ret.Add(data);
                                break;
                            }
                        case "5": {

                                var data = new NicoNicoMylistMangaEntry();

                                data.ItemId = entry.item_id;
                                data.Description = entry.description;
                                data.CreateTime = (long)entry.create_time;
                                data.UpdateTime = (long)entry.update_time;

                                data.MangaId = "mg" + item.id;
                                data.ContentUrl = "http://seiga.nicovideo.jp/watch/" + data.MangaId;
                                data.Title = HttpUtility.HtmlDecode(item.title);
                                data.PictureCount = (int)item.res_count;
                                data.ViewCount = (int)item.view_count;
                                data.CommentCount = (int)item.comment_count;
                                data.MylistCount = (int)item.mylist_count;
                                data.ThumbNailUrl = item.thumbnail_url;
                                data.FirstRetrieve = (long)item.create_time;

                                ret.Add(data);
                                break;
                            }
                        case "6": {

                                var data = new NicoNicoMylistBookEntry();

                                data.ItemId = entry.item_id;
                                data.Description = entry.description;
                                data.CreateTime = (long)entry.create_time;
                                data.UpdateTime = (long)entry.update_time;

                                data.BookId = "bk" + item.id;
                                data.ContentUrl = "http://seiga.nicovideo.jp/watch/" + data.BookId;
                                data.Title = HttpUtility.HtmlDecode(item.title);
                                data.ThumbNailUrl = item.thumbnail_url;
                                data.ViewCount = (int)item.view_count;
                                data.CommentCount = (int)item.comment_count;
                                data.MylistCount = (int)item.mylist_count;
                                data.FirstRetrieve = (long)item.released_at;

                                ret.Add(data);
                                break;
                            }
                        case "13": {

                                var data = new NicoNicoMylistArticleEntry();

                                data.ItemId = entry.item_id;
                                data.Description = entry.description;
                                data.CreateTime = (long)entry.create_time;
                                data.UpdateTime = (long)entry.update_time;

                                data.ArticleId = "ar" + item.id;
                                data.ScreenName = item.screen_name;
                                data.ContentUrl = "http://ch.nicovideo.jp/" + data.ScreenName + "/blomaga/" + data.ArticleId;
                                data.Title = HttpUtility.HtmlDecode(item.title);
                                data.ThumbNailUrl = item.thumbnail_url;
                                data.CommentCount = (int)item.comment_count;
                                data.MylistCount = int.Parse(item.mylist_count);
                                data.FirstRetrieve = (long)item.create_time;

                                ret.Add(data);
                                break;
                            }
                        default:
                            //な、なんだって～
                            break;
                    }
                }
            }
        }
    }


    //マイリストエントリ
    public abstract class NicoNicoMylistEntry {

        //マイリストアイテムID
        public string ItemId { get; set; }

        //タイトル
        public string Title { get; set; }

        //マイリストコメント
        public string Description { get; set; }

        //追加日時 Unixタイム
        public long CreateTime { get; set; }

        //コメントアップデートタイム
        public long UpdateTime { get; set; }

        //投稿日時 Unixタイム
        public long FirstRetrieve { get; set; }

        //サムネイルURL
        public string ThumbNailUrl { get; set; }

        //再生数
        public int ViewCount { get; set; }

        //マイリスト数
        public int MylistCount { get; set; }

        //コメント数
        public int CommentCount { get; set; }

        //コンテンツへのURL
        public string ContentUrl { get; set; }
    }

    //マイリストのタイプごとにクラスを作ってDataTemplateで頑張るのが良いのかな
    public class NicoNicoMylistVideoEntry : NicoNicoMylistEntry {

        //動画ID
        public string VideoId { get; set; }

        //動画時間 秒
        public int Length { get; set; }
    }
    public class NicoNicoMylistMangaEntry : NicoNicoMylistEntry {

        //静画ID
        public string MangaId { get; set; }

        //画数
        public int PictureCount { get; set; }
    }
    public class NicoNicoMylistBookEntry : NicoNicoMylistEntry {

        //静画ID
        public string BookId { get; set; }
    }

    //ブロマガ
    public class NicoNicoMylistArticleEntry : NicoNicoMylistEntry {

        //静画ID
        public string ArticleId { get; set; }

        //ブロマガオーナーの名前
        public string ScreenName { get; set; }
    }
}
