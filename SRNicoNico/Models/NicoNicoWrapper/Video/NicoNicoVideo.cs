using Codeplex.Data;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoVideo : NotificationObject {

        private const string FavChannelApiUrl = "https://ch.nicovideo.jp/api/addbookmark";
        private const string UnfavChannelApiUrl = "https://ch.nicovideo.jp/api/deletebookmark";

        private const string FavUserApiUrl = "https://public.api.nicovideo.jp/v1/user/followees/niconico-users/{0}.json";

        #region ApiData変更通知プロパティ
        private NicoNicoWatchApi _ApiData;

        public NicoNicoWatchApi ApiData {
            get { return _ApiData; }
            set { 
                if (_ApiData == value)
                    return;
                _ApiData = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly string VideoUrl;

        public NicoNicoVideo(string url) {

            VideoUrl = url;
        }

        public async Task<string> GetVideoDataAsync() {

            try {
                var response = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(VideoUrl);

                //チャンネル動画などでリダイレクトされるURLの可能性があるのでリダイレクトされた場合はリダイレクト先を読み込む
                if (response.StatusCode == HttpStatusCode.MovedPermanently) {

                    response = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(response.Headers.Location.OriginalString);
                }

                //削除された動画は404を律儀に返してくる
                if (response.StatusCode == HttpStatusCode.NotFound) {

                    return "動画が削除されている可能性があります";
                }

                // 鯖落ちかな？
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable) {

                    return "サーバーが混み合っているか、メンテナンス中です";
                }

                //Cookieを取得してWebBrowserの方に反映させる
                //これがないと動画を取得しようとしても403が返ってくる
                if (response.Headers.TryGetValues("Set-Cookie", out var cookie)) {

                    foreach (string s in cookie) {

                        foreach (string ss in s.Split(';')) {

                            App.SetCookie(new Uri("http://nicovideo.jp/"), ss);
                        }
                    }
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());

                var container = doc.DocumentNode.SelectSingleNode("//div[@id='js-initial-watch-data']");

                if(container == null) {

                    return "この動画はNicoNicoViewerでは再生できません";
                }

                var env = DynamicJson.Parse(HttpUtility.HtmlDecode(container.Attributes["data-environment"].Value.Trim()));
                var json = DynamicJson.Parse(HttpUtility.HtmlDecode(container.Attributes["data-api-data"].Value.Trim()));
                var api = new NicoNicoWatchApi {
                    RootJson = json,
                    PlaylistToken = env.playlistToken
                };
                //動画情報
                var video = json.video;

                api.VideoId = video.id;
                api.Title = video.originalTitle;
                api.ThumbnailUrl = video.largeThumbnailURL ?? video.thumbnailURL;
                api.Description = HyperLinkReplacer.Replace(video.description);
                api.PostedAt = DateTime.Parse(video.postedDateTime);
                api.Duration = (int) video.duration;
                api.ViewCount = (int) video.viewCount;
                api.MylistCount = (int) video.mylistCount;
                api.CommentCount = (int) json.thread.commentCount;

                api.HighestRank = (int?) json.context.highestRank;
                api.YesterdayRank = (int?) json.context.yesterdayRank;

                api.Tags = new List<VideoTag>();
                foreach(var jsontag in json.tags) {

                    var tag = new VideoTag {
                        Id = jsontag.id,
                        Name = jsontag.name,
                        IsCategory = jsontag.isCategory,
                        IsCategoryCandidate = jsontag.isCategoryCandidate,
                        IsDictionaryExists = jsontag.isDictionaryExists,
                        IsLocked = jsontag.isLocked
                    };
                    tag.Url = "http://dic.nicovideo.jp/a/" + tag.Name;

                    api.Tags.Add(tag);
                }

                api.ViewerInfo = new ViewerInfo() {

                    Id = json.viewer.id.ToString(),
                    Nickname = json.viewer.nickname.ToString(),
                    IsPremium = json.viewer.isPremium,
                    NicosId = json.viewer.nicosid
                };

                
                if(json.owner != null) {

                    api.UploaderInfo = new UploaderInfo {
                        Id = json.owner.id,
                        ThumbnailUrl = json.owner.iconURL,
                        UploaderUrl = "http://www.nicovideo.jp/user/" + json.owner.id,
                        IsChannel = false
                    };
                    api.UploaderInfo.Name = $"<a href='{api.UploaderInfo.UploaderUrl}'>{json.owner.nickname}</a>";

                    var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(string.Format(FavUserApiUrl, api.UploaderInfo.Id));
                    var ret = DynamicJson.Parse(a);
                    api.UploaderInfo.Followed = ret.data.following;

                } else if(json.channel != null) {

                    api.UploaderInfo = new UploaderInfo {
                        Id = json.channel.id,
                        // https://public.api.nicovideo.jp/v1/channels.json?channelIds=2633724&responseGroup=detail もしURLハードコーディングでアイコンが出なくなったら諦めて素直にAPI叩く
                        ThumbnailUrl = "https://secure-dcdn.cdn.nimg.jp/comch/channel-icon/128x128/ch" + json.channel.id + ".jpg",
                        UploaderUrl = "http://ch.nicovideo.jp/ch" + json.channel.id,
                        IsChannel = true,
                        Followed = json.channel.isFavorited
                    };
                    api.UploaderInfo.Name = $"<a href='{api.UploaderInfo.UploaderUrl}'>{json.channel.name}</a>";
                }

                var thread = new ThreadInfo {
                    ServerUrl = json.thread.serverUrl.Replace("api/", "api.json/"),
                    Composites = new List<CommentComposite>()
                };
                foreach (var composite in json.commentComposite.threads) {

                    thread.Composites.Add(new CommentComposite() {

                        Id = composite.id.ToString(),
                        Fork = composite.fork == 1,
                        IsActive = composite.isActive,
                        PostKeyStatus = (int) composite.postkeyStatus,
                        IsDefaultPostTarget = composite.isDefaultPostTarget,
                        IsThreadKeyRequired = composite.isThreadkeyRequired,
                        IsLeafRequired = composite.isLeafRequired,
                        Label = composite.label,
                        IsOwnerThread = composite.isOwnerThread,
                        HasNicoscript = composite.hasNicoscript
                    });
                }
                api.ThreadInfo = thread;

                api.CsrfToken = json.context.csrfToken;
                api.UserKey = json.context.userkey;
                api.IsNeedPayment = json.context.isNeedPayment;
                api.InitialPlaybackPosition =  json.context.initialPlaybackPosition != null ? (int) json.context.initialPlaybackPosition : 0;

                if(video.dmcInfo != null) {

                    api.DmcInfo = new NicoNicoDmc(video.dmcInfo.session_api);
                    api.DmcHeartbeatRequired = true;

                    if (video.dmcInfo.storyboard_session_api != null) {

                        // ストーリーボード取得
                        await Task.Run(async () => {

                            api.StoryBoard = new NicoNicoStoryBoard(video.dmcInfo.storyboard_session_api);
                            await api.StoryBoard.GetStoryBoardAsync();
                        });
                    }
                } else {

                    api.VideoUrl = video.smileInfo.url;
                }
                ApiData = api;
                return "";
            } catch(RequestFailed) {

                return "動画情報の取得に失敗しました";
            }
        }

        public async Task<bool> FavChannelAsync(bool fav) {

            try {
                var query = new GetRequestQuery(fav ? FavChannelApiUrl : UnfavChannelApiUrl);
                query.AddQuery("channel_id", ApiData.RootJson.channel.id);
                query.AddQuery("return_json", 1);
                query.AddQuery("time", (int)ApiData.RootJson.channel.favoriteTokenTime);
                query.AddQuery("key", ApiData.RootJson.channel.favoriteToken);

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);

                var ret = DynamicJson.Parse(a);
                return ret.status == "succeed";
            } catch(RequestFailed) {

                return false;
            }
        }

        public async Task<bool> FavUserAsync(bool fav) {

            try {

                var request = new HttpRequestMessage(fav ? HttpMethod.Post : HttpMethod.Delete, string.Format(FavUserApiUrl, ApiData.UploaderInfo.Id));
                request.Headers.Add("X-Request-With", VideoUrl);
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var ret = DynamicJson.Parse(a);
                return ret.meta.status == 200;
            } catch (RequestFailed) {

                return false;
            }
        }


    }
}
