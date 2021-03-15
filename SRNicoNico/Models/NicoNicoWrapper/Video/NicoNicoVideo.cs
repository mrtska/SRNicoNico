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

    /// <summary>
    /// 動画再生に係る諸々のバックエンド処理
    /// </summary>
    public class NicoNicoVideo : NotificationObject {

        /// <summary>
        /// チャンネルをフォローする時に使うAPI
        /// </summary>
        private const string FavChannelApiUrl = "https://ch.nicovideo.jp/api/addbookmark";

        /// <summary>
        /// チャンネルのフォローを外す時に使うAPI
        /// </summary>
        private const string UnfavChannelApiUrl = "https://ch.nicovideo.jp/api/deletebookmark";

        /// <summary>
        /// ユーザーをフォローしたりフォロー解除したりする時に使うAPI
        /// {0}は対象のユーザーID
        /// </summary>
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

        /// <summary>
        /// 対象の動画URL
        /// </summary>
        private readonly string VideoUrl;

        public NicoNicoVideo(string url) {

            if (string.IsNullOrEmpty(url)) {

                throw new ArgumentNullException(nameof(url));
            }

            VideoUrl = url;
        }

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <returns>失敗した場合のエラーメッセージ</returns>
        public async Task<string> GetVideoDataAsync() {

            try {
                var response = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(VideoUrl);

                //チャンネル動画などでリダイレクトされるURLの可能性があるのでリダイレクトされた場合はリダイレクト先を読み込む
                if (response.StatusCode == HttpStatusCode.MovedPermanently) {

                    response = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(response.Headers.Location.OriginalString);
                }

                //削除された動画は404が返ってくる
                if (response.StatusCode == HttpStatusCode.NotFound) {

                    return "動画が削除されている可能性があります";
                }

                // 鯖落ちかな？
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable) {

                    return "サーバーが混み合っているか、メンテナンス中です";
                }

                //Cookieを取得してWebBrowserの方に反映させる
                //これがないと動画を取得しようとしても403が返ってくる(smilevideoの場合)
                if (response.Headers.TryGetValues("Set-Cookie", out var cookie)) {

                    var nicovideo = new Uri("http://nicovideo.jp/");

                    foreach (string s in cookie) {

                        foreach (string ss in s.Split(';')) {

                            App.SetCookie(nicovideo, ss);
                        }
                    }
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());

                var container = doc.DocumentNode.SelectSingleNode("//div[@id='js-initial-watch-data']");

                // Flashじゃないと再生出来ない動画は弾く
                if (container == null) {

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
                api.Title = video.title;
                api.ThumbnailUrl = video.thumbnail.url;
                api.Description = HyperLinkReplacer.Replace(video.description);
                api.PostedAt = DateTime.Parse(video.registeredAt);
                api.Duration = (int)video.duration;
                api.ViewCount = (int)video.count.view;
                api.MylistCount = (int)video.count.mylist;
                api.CommentCount = (int)video.count.comment;

                api.HighestRank = null;//(int?)json.context.highestGenreRanking?.rank ?? null;

                api.Tags = new List<VideoTag>();
                foreach (var jsontag in json.tag.items) {

                    var tag = new VideoTag {
                        Name = jsontag.name,
                        IsCategory = jsontag.isCategory,
                        IsCategoryCandidate = jsontag.isCategoryCandidate,
                        IsDictionaryExists = jsontag.isNicodicArticleExists,
                        IsLocked = jsontag.isLocked
                    };
                    tag.Url = "http://dic.nicovideo.jp/a/" + tag.Name;

                    api.Tags.Add(tag);
                }

                api.ViewerInfo = new ViewerInfo() {

                    Id = json.viewer.id.ToString(),
                    Nickname = json.viewer.nickname.ToString(),
                    IsPremium = json.viewer.isPremium,
                };


                if (json.owner != null) {

                    api.UploaderInfo = new UploaderInfo {
                        Id = json.owner.id.ToString(),
                        ThumbnailUrl = json.owner.iconUrl,
                        UploaderUrl = "https://www.nicovideo.jp/user/" + json.owner.id,
                        IsChannel = false
                    };
                    api.UploaderInfo.Name = $"<a href='{api.UploaderInfo.UploaderUrl}'>{json.owner.nickname}</a>";
                    api.UploaderInfo.Followed = json.owner.viewer.isFollowing;

                } else if (json.channel != null) {

                    api.UploaderInfo = new UploaderInfo {
                        Id = json.channel.id,
                        ThumbnailUrl =json.channel.thumbnail.url,
                        UploaderUrl = "http://ch.nicovideo.jp/ch" + json.channel.id,
                        IsChannel = true,
                        Followed = json.channel.viewer.follow.isFollowed
                    };
                    api.UploaderInfo.Name = $"<a href='{api.UploaderInfo.UploaderUrl}'>{json.channel.name}</a>";
                }

                var thread = new ThreadInfo {
                    ServerUrl = json.comment.server.url.Replace("api/", "api.json/"),
                    Composites = new List<CommentComposite>()
                };
                foreach (var composite in json.comment.threads) {

                    thread.Composites.Add(new CommentComposite() {

                        Id = composite.id.ToString(),
                        Fork = composite.fork == 1,
                        IsActive = composite.isActive,
                        PostKeyStatus = (int)composite.postkeyStatus,
                        IsDefaultPostTarget = composite.isDefaultPostTarget,
                        IsThreadKeyRequired = composite.isThreadkeyRequired,
                        IsLeafRequired = composite.isLeafRequired,
                        Label = composite.label,
                        IsOwnerThread = composite.isOwnerThread,
                        HasNicoscript = composite.hasNicoscript
                    });
                }
                api.ThreadInfo = thread;

                //api.CsrfToken = json.context.csrfToken;
                api.UserKey = json.comment.keys.userKey;
                api.IsNeedPayment = video.isAuthenticationRequired;
                api.InitialPlaybackPosition = (int?)json.player.initialPlayback?.positionSec ?? 0;

                //要課金動画はdmcInfoがないのでここで終了させる
                if (api.IsNeedPayment) {

                    ApiData = api;
                    return "";
                }

                if (json.media != null) {

                    api.DmcInfo = new NicoNicoDmc(json.media);
                    api.DmcHeartbeatRequired = true;

                    if (json.media.delivery.storyboard != null) {

                        // ストーリーボードを非同期で取得
                        _ = Task.Run(async () => {

                            api.StoryBoard = new NicoNicoStoryBoard(json.media.delivery.storyboard);
                            await api.StoryBoard.GetStoryBoardAsync();
                        });
                    }

                    // トラッキングIDを送る
                    // 送らないと暗号化済み動画で複合キーが正しく取得出来ない
                    var query = new GetRequestQuery("https://nvapi.nicovideo.jp/v1/2ab0cbaa/watch");
                    query.AddQuery("t", Uri.EscapeDataString(json.media.delivery.trackingId));

                    var request = new HttpRequestMessage(HttpMethod.Get, query.TargetUrl);
                    request.Headers.Add("Origin", "https://www.nicovideo.jp");
                    request.Headers.Referrer = new Uri(VideoUrl);
                    request.Headers.Add("X-Frontend-Id", "6");
                    request.Headers.Add("X-Frontend-Version", "0");

                    await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(request);
                    //------

                } else {

                    api.VideoUrl = video.smileInfo.url;
                }
                ApiData = api;
                return "";
            } catch (RequestFailed) {

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
            } catch (RequestFailed) {

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
