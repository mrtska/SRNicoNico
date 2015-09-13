using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;

using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoMylist : NotificationObject {


        //とりあえずマイリスト取得API
        private const string DefListAPI = "http://www.nicovideo.jp/api/deflist/list";

        //マイリスト一覧取得API
        private const string MylistGroupAPI = "http://www.nicovideo.jp/api/mylistgroup/list";

        public NicoNicoMylist() {


        }

        //jsonをパースしてリストにする
        private void StoreItem(dynamic json, List<NicoNicoMylistData> ret) {

            foreach(var entry in json.mylistitem) {

                NicoNicoMylistData data = new NicoNicoMylistData();
                data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                data.Description = entry.description;

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
        public List<NicoNicoMylistData> GetMylist(int groupId) {


            const string api = "http://www.nicovideo.jp/api/mylist/list?group_id=";

            //指定したマイリスト
            var json = DynamicJson.Parse(NicoNicoWrapperMain.GetSession().GetAsync(api + groupId).Result);

            List<NicoNicoMylistData> ret = new List<NicoNicoMylistData>();

            StoreItem(json, ret);

            return ret;
        }


        public List<NicoNicoMylistGroupData> GetMylistGroup() {

            List<NicoNicoMylistGroupData> ret = new List<NicoNicoMylistGroupData>();


            //指定した
            var json = DynamicJson.Parse(NicoNicoWrapperMain.GetSession().GetAsync(MylistGroupAPI).Result);
            foreach(var entry in json.mylistgroup) {

                NicoNicoMylistGroupData data = new NicoNicoMylistGroupData();
                data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                data.Description = entry.description;

                data.Id = int.Parse(entry.id);
                data.Name = entry.name;
                data.IsPublic = entry.@public == "0" ? false : true;
                data.SortOrder = int.Parse(entry.sort_order);

                ret.Add(data);
            }


            return ret;
        }






    }

    //マイリストグループエントリ
    public class NicoNicoMylistGroupData {


        //追加日時 Unixタイム
        public string CreateTime { get; set; }

        //マイリストグループコメント
        public string Description { get; set; }

        //グループID
        public int Id { get; set; }

        //名前
        public string Name { get; set; }

        //is公開
        public bool IsPublic { get; set; }

        //ソートオーダー
        public int SortOrder { get; set; }
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
