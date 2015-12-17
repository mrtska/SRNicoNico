using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using System.Web;
using System.Net;
using System.Net.Http;

using Livet;
using Livet.Messaging;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

using Codeplex.Data;
using System.Collections.ObjectModel;
using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {


    //動画を見るうえで必要な情報をこのAPIだけで全て取得できるヤバいAPI
    public sealed class NicoNicoWatchApi : NotificationObject {

        //動画ページを指定
        public static WatchApiData GetWatchApiData(string videoPage) {

            //動画ページのhtml取得
            var response = NicoNicoWrapperMain.GetSession().GetResponseAsync(videoPage).Result;

            //チャンネル、公式動画
            if(response.StatusCode == HttpStatusCode.MovedPermanently) {
                
                response = NicoNicoWrapperMain.GetSession().GetResponseAsync(response.Headers.Location.OriginalString).Result;
            }
            //削除された動画
            if(response.StatusCode == HttpStatusCode.NotFound) {

                return null;
            }
            //混雑中
            if(response.StatusCode == HttpStatusCode.ServiceUnavailable) {

                return null;
            }

            string html = response.Content.ReadAsStringAsync().Result;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml2(html);

            //htmlからAPIデータだけを綺麗に抜き出す すごい
            var container = doc.DocumentNode.QuerySelector("#watchAPIDataContainer");

            if(container == null) {

                return null;
            }

            var data = container.InnerHtml;

            //html特殊文字をデコードする
            data = HttpUtility.HtmlDecode(data);

            //jsonとしてAPIデータを展開していく
            var json = DynamicJson.Parse(data);

            //GetFlvの結果
             string flv = json.flashvars.flvInfo;

            //2重にエンコードされてるので二回
            flv = HttpUtility.UrlDecode(flv);
            flv = HttpUtility.UrlDecode(flv);


            WatchApiData ret = new WatchApiData();

            //&で繋がれているので剥がす
            var getFlv = flv.Split(new char[] { '&' }).ToDictionary(source => source.Substring(0, source.IndexOf('=')),
            source => Uri.UnescapeDataString(source.Substring(source.IndexOf('=') + 1)));

            ret.GetFlv = new NicoNicoGetFlvData(getFlv);

            //動画情報
            var videoDetail = json.videoDetail;

            //---情報を詰める---
            ret.Cmsid = videoDetail.id;
            ret.MovieType = json.flashvars.movie_type;
            ret.Title = HttpUtility.HtmlDecode(videoDetail.title);  //html特殊文字をデコード
            ret.Thumbnail = videoDetail.thumbnail;
            ret.Description = videoDetail.description;
            ret.PostedAt = videoDetail.postedAt;
            ret.Length = (int) videoDetail.length;
            ret.ViewCounter = (int) videoDetail.viewCount;
            ret.CommentCounter = (int) videoDetail.commentCount;
            ret.MylistCounter = (int) videoDetail.mylistCount;
            ret.YesterdayRank = videoDetail.yesterday_rank == null ? "圏外" : videoDetail.yesterday_rank + "位";
            ret.HighestRank = videoDetail.highest_rank == null ? "圏外" : videoDetail.highest_rank + "位";
            ret.Token = json.flashvars.csrfToken;

            if(json.uploaderInfo()) {

                //投稿者情報
                var uploaderInfo = json.uploaderInfo;

                ret.UploaderId = uploaderInfo.id;
                ret.UploaderIconUrl = uploaderInfo.icon_url;
                ret.UploaderName = uploaderInfo.nickname;
                ret.UploaderIsFavorited = uploaderInfo.is_favorited;

            } else if(json.channelInfo()) {

                //投稿者情報
                var channelInfo = json.channelInfo;

                ret.UploaderId = channelInfo.id;
                ret.UploaderIconUrl = channelInfo.icon_url;
                ret.UploaderName = channelInfo.name;
                ret.UploaderIsFavorited = channelInfo.is_favorited == 1 ? true : false;

                ret.IsChannelVideo = true;
            }


            ret.Description = HyperLinkParser.Parse(ret.Description);

            ret.TagList = new ObservableCollection<VideoTagViewModel>();

            foreach(var tag in videoDetail.tagList) {

                NicoNicoTag entry = new NicoNicoTag() {

                    Id = tag.id,
                    Tag = tag.tag,
                    Dic = tag.dic(),
                    Lck = tag.lck == "1" ? true : false,
                    Cat = tag.cat()
                };
                ret.TagList.Add(new VideoTagViewModel(entry)); 
            }
            //------

            //有料動画
            if(ret.GetFlv.VideoUrl == null || ret.GetFlv.VideoUrl.Count() == 0) {

                ret.IsPaidVideo = true;
                return ret;
            }

            //念のためCookieを設定
            var cookie = response.Headers.GetValues("Set-Cookie");
            foreach(string s in cookie) {

                foreach(string ss in s.Split(';')) {

                    App.SetCookie(new Uri("http://nicovideo.jp/"), ss);
                }
            }
            return ret;
        }

        public static Status AddFavorite(VideoViewModel vm, string userId, string token) {

            vm.Status = "お気に入りに登録中";


            var add = "http://www.nicovideo.jp/api/watchitem/add";

            var formData = new Dictionary<string, string>();

            formData["item_type"] = "1";
            formData["item_id"] = userId;
            formData["token"] = token;
            try {

                var request = new HttpRequestMessage(HttpMethod.Post, add);

                request.Content = new FormUrlEncodedContent(formData);


                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;

                vm.VideoData.ApiData.UploaderIsFavorited = true;

                vm.Status = "";

                return Status.Success;
            } catch(RequestTimeout) {

                vm.Status = "お気に入り登録に失敗しました。";
                return Status.Failed;
            }
            

        }

        public static Status DeleteFavorite(VideoViewModel vm, string userId, string token) {

            vm.Status = "お気に入り解除中";
            

            var del = "http://www.nicovideo.jp/api/watchitem/delete";

            var formData = new Dictionary<string, string>();

            formData["id_list[1][]"] = userId;
            formData["token"] = token;

            try {

                var request = new HttpRequestMessage(HttpMethod.Post, del);

                request.Content = new FormUrlEncodedContent(formData);

                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;

                vm.VideoData.ApiData.UploaderIsFavorited = false;
                vm.Status = "";

                return Status.Success;

            } catch(RequestTimeout) {

                vm.Status = "お気に入り解除に失敗しました。";
                return Status.Failed;
            }
        }
    }


    public class WatchApiData : NotificationObject {

        //有料動画か否か
        public bool IsPaidVideo { get; set; }

        //GetFlvAPIの結果
        public NicoNicoGetFlvData GetFlv { get; set; }

        public string Cmsid { get; set; }

        //動画タイプ mp4かflv
        public string MovieType { get; set; }

        //動画タイトル
        public string Title { get; set; }

        //動画サムネイルURL
        public string Thumbnail { get; set; }

        //動画説明文
        public string Description { get; set; }

        //動画投稿日時
        public string PostedAt { get; set; }

        //動画時間
        public int Length { get; set; }

        //再生数
        public int ViewCounter { get; set; }

        //コメント数
        public int CommentCounter { get; set; }

        //マイリスト数
        public int MylistCounter { get; set; }

        //昨日のランキング
        public string YesterdayRank { get; set; }

        //最高ランキング
        public string HighestRank { get; set; }

        //CSRFトークン
        public string Token { get; set; }

        //タグリスト
        #region TagList変更通知プロパティ
        private ObservableCollection<VideoTagViewModel> _TagList;

        public ObservableCollection<VideoTagViewModel> TagList {
            get { return _TagList; }
            set { 
                if(_TagList == value)
                    return;
                _TagList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //うｐ主又はチャンネルのID
        public string UploaderId { get; set; }

        //うｐ主又はチャンネルアイコンURL
        public string UploaderIconUrl { get; set; }

        //うｐ主又はチャンネルの名前
        public string UploaderName { get; set; }

        //うｐ主又はチャンネルをお気に入り登録しているかどうか
        #region UploaderIsFavorited変更通知プロパティ
        private bool _UploaderIsFavorited;

        public bool UploaderIsFavorited {
            get { return _UploaderIsFavorited; }
            set { 
                if(_UploaderIsFavorited == value)
                    return;
                _UploaderIsFavorited = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //チャンネル動画か否か
        public bool IsChannelVideo { get; set; }
    }

    //タグ情報
    public class NicoNicoTag : NotificationObject {

        //タグID
        public string Id { get; set; }

        //タグ名
        public string Tag { get; set; }

        //大百科が存在するか
        public bool Dic { get; set; }

        //ロックされているか
        public bool Lck { get; set; }

        //カテゴリタグか否か
        public bool Cat { get; set; }
    }


    public class NicoNicoGetFlvData : NotificationObject {

        //getflvAPIで取得できるデータ

        //スレッドID
        public string ThreadID { get; private set; }

        //長さ
        public uint Length { get; private set; }

        //動画URL
        public string VideoUrl { get; private set; }

        //コメントサーバーURL
        public Uri CommentServerUrl { get; private set; }

        //サブコメントサーバーURL
        public Uri SubCommentServerUrl { get; private set; }
        
		//ユーザーID
		public string UserId { get; private set; }
        
		//プレミアムか否か 
		public bool IsPremium { get; private set; }
         
        //FMSトークン
        public string FmsToken { get; private set; } 
         
        //非公開理由
        public int ClosedReason { get; private set; }   

        public NicoNicoGetFlvData(Dictionary<string, string> wwwData) {

            ThreadID = wwwData["thread_id"];
            Length = uint.Parse(wwwData["l"]);
            VideoUrl = wwwData["url"];
            CommentServerUrl = new Uri(wwwData["ms"]);
            SubCommentServerUrl = new Uri(wwwData["ms_sub"]);
			UserId = wwwData["user_id"];
			IsPremium = wwwData["is_premium"] == "1" ? true : false;
            FmsToken = wwwData.ContainsKey("fmst") ? wwwData["fmst"] : null;
        }
    }
    
    public enum NicoNicoVideoType {

        MP4,
        FLV,
        SWF,
        RTMP
    }

    public enum Status {

        Success,
        Failed
    }

}
