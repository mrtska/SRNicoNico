using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;

using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoVitaApi : NotificationObject {

        //動画情報取得API コミュ限動画などでも取得できる
        private const string VideoDataApiUrl = "http://api.ce.nicovideo.jp/nicoapi/v1/video.info?__format=json&v=";


        public static NicoNicoVitaApiVideoData GetVideoData(string cmsid) {

            string result = NicoNicoWrapperMain.GetSession().GetAsync(VideoDataApiUrl + cmsid).Result;

            var json = DynamicJson.Parse(result);
            var response = json.nicovideo_video_response;

            NicoNicoVitaApiVideoData ret = new NicoNicoVitaApiVideoData();

            if(!response.video()) {

                ret.Success = false;
                return ret;
            }

            ret.Id = response.video.id;
            ret.Title = response.video.title;
            ret.FirstRetrieve = NicoNicoUtil.DateFromVitaFormatDate(response.video.first_retrieve);
            ret.ViewCounter = int.Parse(response.video.view_counter);
            ret.CommentCounter = int.Parse(response.thread.num_res);
            ret.MylistCounter = int.Parse(response.video.mylist_counter);
            ret.Length = NicoNicoUtil.ConvertTime(long.Parse(response.video.length_in_seconds));
            ret.Description = response.video.description;
            ret.ThumbnailUrl = response.video.thumbnail_url;
            return ret;
        }
    }

    public class NicoNicoVitaApiVideoData : NotificationObject {

        //動画ID
        public string Id { get; set; }

        //タイトル
        public string Title { get; set; }

        //再生数
        public int ViewCounter { get; set; }

        //コメント数
        public int CommentCounter { get; set; }
        
        //マイリスト数
        public int MylistCounter { get; set; }

        //動画時間
        public string Length { get; set; }
        
        //動画投稿時日時
        public string FirstRetrieve { get; set; }
        
        //説明文（タグなし）
        public string Description { get; set; }

        //サムネイル
        public string ThumbnailUrl { get; set; }

        //成功したか否か
        public bool Success { get; set; } = true;

    }

}
