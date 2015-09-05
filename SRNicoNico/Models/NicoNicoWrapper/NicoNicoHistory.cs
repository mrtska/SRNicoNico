using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoHistory : NotificationObject {
        
        
        //視聴履歴を返すAPI
        private const string HistroyApiUrl = "http://www.nicovideo.jp/api/videoviewhistory/list";

        //たまに失敗するから注意
        public List<NicoNicoHistoryData> GetHistroyData() {


            App.ViewModelRoot.StatusBar.Status = "視聴履歴取得中";

            //このAPIだけでは再生数やコメント数などが取得できないので
            string result = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(HistroyApiUrl).Result;

            List<NicoNicoHistoryData> ret = new List<NicoNicoHistoryData>();

            var json = DynamicJson.Parse(result);

            foreach(var entry in json.history) {

                NicoNicoHistoryData data = new NicoNicoHistoryData() {

                    Id = entry.video_id,
                    Length = entry.length,
                    ThumbnailUrl = entry.thumbnail_url,
                    Title = entry.title,
                    WatchCount = (int) entry.watch_count,
                    WatchDate = (long) entry.watch_date
                };









                ret.Add(data);
            }

            App.ViewModelRoot.StatusBar.Status = "視聴履歴取得完了";


            return ret;
        }
    }

    public class NicoNicoHistoryData {

        //動画ID
        public string Id { get; set; }

        //動画サムネイル
        public string ThumbnailUrl { get; set; }

        //動画タイトル
        public string Title { get; set; }

        //動画の長さ
        public string Length { get; set; }

        //視聴日時 Unix時間
        public long WatchDate { get; set; }

        //視聴回数
        public int WatchCount { get; set; }

        //再生数
        public int ViewCounter { get; set; }

        //コメント数
        public int CommentCounter { get; set; }

        //マイリスト数
        public int MylistCounter { get; set; }
    }

}
