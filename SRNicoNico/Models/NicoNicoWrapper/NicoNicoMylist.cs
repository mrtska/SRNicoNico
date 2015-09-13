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


        public NicoNicoMylist() {


        }


        //とりあえずマイリストを取得する
        public List<NicoNicoMylistData> GetDefMylist() {


            //とりあえずマイリスト
            var json = DynamicJson.Parse(NicoNicoWrapperMain.GetSession().GetAsync(DefListAPI).Result);

            List<NicoNicoMylistData> ret = new List<NicoNicoMylistData>();

            foreach(var entry in json.mylistitem) {

                NicoNicoMylistData data = new NicoNicoMylistData();
                data.CreateTime = UnixTime.FromUnixTime((long)entry.create_time).ToString();
                data.Description = entry.description;

                var item = entry.item_data;

                data.FirstRetrieve =UnixTime.FromUnixTime((long)item.first_retrieve).ToString();
                data.Length = NicoNicoUtil.GetTimeFromLong(long.Parse(item.length_seconds));
                data.Id = item.video_id;
                data.Title = item.title;
                data.ViewCounter = int.Parse(item.view_counter);
                data.CommentCounter = int.Parse(item.num_res);
                data.MylistCounter = int.Parse(item.mylist_counter);
                data.ThumbNailUrl = item.thumbnail_url;

                ret.Add(data);
            }

            return ret;

        }



    }

    public class NicoNicoMylistData {

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
