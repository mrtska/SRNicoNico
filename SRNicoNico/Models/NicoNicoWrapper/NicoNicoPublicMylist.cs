using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Livet;
using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using System.Web;

using SRNicoNico.ViewModels;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoPublicMylist : NotificationObject {

        private string MylistUrl;

        public NicoNicoPublicMylist(string url) {

            MylistUrl = url;
        }

        public NicoNicoPublicMylistEntry GetMylist() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(MylistUrl).Result;

                //該当JavaScriptの部分から取得
                var globals = a.Substring(a.IndexOf("Jarty.globals("));

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

                var ret = new NicoNicoPublicMylistEntry();

                ret.NickName = @"<a href=""http://www.nicovideo.jp/user/" + userid + @""">" +  nickname + "</a> さんの公開マイリスト";
                ret.MylistName = mylistname;
                ret.Description = description;

                //\nを改行に置換
                ret.Description = ret.Description.Replace("\\n", "<br>").Replace("\\r", "");

                ret.Description = HyperLinkParser.Parse(ret.Description); 

                var list = new List<MylistListEntryViewModel>();

                var data = DynamicJson.Parse(json);

                StoreItem(data, list);

                ret.Data = list;

                return ret;
            } catch(RequestTimeout) {

                return null;
            }



        }
        //jsonをパースしてリストにする
        private void StoreItem(dynamic json, List<MylistListEntryViewModel> ret) {

            foreach(var entry in json) {

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
                ret.Add(new MylistListEntryViewModel(data));
            }
        }

    }

    public class NicoNicoPublicMylistEntry {

        //マイリスおオーダーのユーザーID
        public Uri MylistOwnerUri { get; set; }

        //マイリストオーナーのニックネーム
        public string NickName { get; set; }

        //マイリストの名前
        public string MylistName { get; set; }

        //マイリストの説明
        public string Description { get; set; }

        //マイリスト
        public List<MylistListEntryViewModel> Data { get; set; }

    }


}
