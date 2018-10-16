using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoPublicMylist {

        private string MylistUrl;

        private TabItemViewModel Owner;

        public NicoNicoPublicMylist(TabItemViewModel vm, string mylistUrl) {

            Owner = vm;
            MylistUrl = mylistUrl;
        }


        public async Task<NicoNicoPublicMylistGroupEntry> GetMylistAsync() {

            try {


                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(MylistUrl);

                var index = a.IndexOf("Jarty.globals(");

                if(index == -1) {

                    return null;
                }

                //該当JavaScriptの部分から取得
                var globals = a.Substring(index);

                //改行で分割
                var splitted = globals.Split('\n');

                //正規表現でダブルクォーテ内の名前を取得
                var regex = new Regex("\"(.*)\"");

                string nickname = null;
                string userid = null;
                string mylistname = null;
                string description = null;

                string json = null;

                foreach(var text in splitted) {

                    //マイリストオーナーだったら
                    if(text.Contains("mylist_owner:")) {

                        //マイリストオーナーのニックネームを取得
                        nickname = text.Substring(text.IndexOf("nickname: "));

                        var match = regex.Match(nickname);

                        //グループから取得
                        nickname = match.Groups[1].Value;
                        continue;
                    }

                    if(nickname != null && text.Contains("user_id:")) {

                        userid = new Regex(@"\d+").Match(text).Value;
                        continue;
                    }

                    if(userid != null && text.Contains("name:")) {

                        mylistname = regex.Match(text).Groups[1].Value;
                        continue;
                    }
                    if(mylistname != null && text.Contains("description:")) {

                        description = regex.Match(text).Groups[1].Value;
                        continue;
                    }
                    if(description != null && text.Contains("Mylist.preload(")) {

                        //Json取得
                        json = text.Substring(text.IndexOf(",") + 1, text.Length - text.IndexOf(",") - 3);
                        break;
                    }
                }

                var ret = new NicoNicoPublicMylistGroupEntry();

                ret.OwnerName = @"<a href=""https://www.nicovideo.jp/user/" + userid + @""">" + nickname + "</a> さんの公開マイリスト";
                ret.Name = mylistname;
                ret.Description = description;

                //\nを改行に置換
                ret.Description = ret.Description.Replace("\\n", "<br>").Replace("\\r", "");

                ret.Description = HyperLinkReplacer.Replace(ret.Description);

                var list = new List<NicoNicoMylistEntry>();

                if(json != null) {

                    StoreItems(json, list);
                }


                ret.Data = list;

                return ret;
            } catch(RequestFailed) {

                Owner.Status = "公開マイリストの取得に失敗しました";
                return null;
            }

        }


        //jsonをパースしてリストに入れる
        private void StoreItems(string str, List<NicoNicoMylistEntry> ret) {

            var json = DynamicJson.Parse(str);

            foreach(var entry in json) {

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
                                data.ContentUrl = "https://www.nicovideo.jp/watch/" + data.VideoId;
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
