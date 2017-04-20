using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using SRNicoNico.ViewModels;
using Codeplex.Data;
using System.Web;
using System.Net;
using System.Net.Http;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoWatchApi {

        //投稿者をフォローしているか確認するにはこれを叩かないといけない
        private const string UploaderInfoApi = "https://public.api.nicovideo.jp/v1/user/followees/niconico-users/{0}.json";

        //
        private const string RecordPositionApi = "http://flapi.nicovideo.jp/api/record_current_playback_position";

        private const string ChannelFollowApi = "http://ch.nicovideo.jp/api/addbookmark";
        private const string ChannelUnFollowApi = "http://ch.nicovideo.jp/api/deletebookmark";


        private VideoViewModel Owner;

        public NicoNicoWatchApi(VideoViewModel vm) {

            Owner = vm;
        }

        public  async Task<NicoNicoWatchApiData> GetWatchApiDataAsync() {

            try {

                var res = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(Owner.VideoUrl);

                //チャンネル動画などでリダイレクトされるURLの可能性があるのでリダイレクトされた場合はリダイレクト先を読み込む
                if(res.StatusCode == HttpStatusCode.MovedPermanently) {

                    res = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(res.Headers.Location.OriginalString);
                }

                //削除された動画は404を律儀に返してくる
                if(res.StatusCode == HttpStatusCode.NotFound) {

                    Owner.Status = "動画が削除されている可能性があります";
                    return null;
                }

                //鯖落ちかな？
                if(res.StatusCode == HttpStatusCode.ServiceUnavailable) {

                    Owner.Status = "サーバーが混み合っているようです";
                    return null;
                }


                //Cookieを取得してWebBrowserの方に反映させる
                //これがないと動画を取得しようとしても403が返ってくる
                IEnumerable<string> cookie;
                if(res.Headers.TryGetValues("Set-Cookie", out cookie)) {

                    foreach(string s in cookie) {

                        foreach(string ss in s.Split(';')) {

                            App.SetCookie(new Uri("http://nicovideo.jp/"), ss);
                        }
                    }
                }

                //html
                var a = await res.Content.ReadAsStringAsync();

                //Responsehはもう必要ない
                res.Dispose();

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var container = doc.DocumentNode.SelectSingleNode("//div[@id='js-initial-watch-data']");

                if (container == null) {

                    container = doc.DocumentNode.SelectSingleNode("//div[@id='watchAPIDataContainer']");

                    //html5版ページで視聴できなかった場合
                    if(container != null) {

                        var json = DynamicJson.Parse(HttpUtility.HtmlDecode(container.InnerText));

                        var flash = json.flashvars;

                        var flv = HttpUtility.UrlDecode(HttpUtility.UrlDecode(flash.flvInfo));

                        var getFlv = HttpUtility.ParseQueryString(flv);
                        
                        var videoDetail = json.videoDetail;

                        var ret = new NicoNicoWatchApiData();
                        //動画情報
                        ret.Video = new NicoNicoVideo() {

                            Id = videoDetail.id,
                            SmileInfo = new NicoNicoVideo.NicoNicoSmileInfo() {
                                Url = getFlv["url"]
                            },
                            Title = videoDetail.title,
                            OriginalTitle = videoDetail.title_original,
                            Description = videoDetail.description,
                            OriginalDescription = videoDetail.description_original,
                            ThumbNailUrl = videoDetail.thumbnail,
                            PostedDateTime = DateTime.Parse(videoDetail.postedAt),
                            OriginalPostedDateTime = DateTime.MinValue,
                            Duration = (int) videoDetail.length,
                            ViewCount = (int) videoDetail.viewCount,
                            MylistCount = (int) videoDetail.mylistCount
                        };

                        ret.Thread = new NicoNicoThread() {

                            CommentCount = (int)videoDetail.commentCount,
                            HasOwnerThread = videoDetail.has_owner_thread,
                            ServerUrl = getFlv["ms"].Replace("api", "api.json"),
                            SubServerurl = getFlv["ms_sub"],
                            ThreadIds = new NicoNicoThreadIds() {

                                Default = getFlv["thread_id"],
                                Nicos = getFlv["thread_id"],
                                Community = getFlv["thread_id"]
                            }
                        };

                        ret.Tags = new List<NicoNicoVideoTag>();

                        foreach(var tag in videoDetail.tagList) {

                            var entry = new NicoNicoVideoTag() {

                                Id = tag.id,
                                Tag = HttpUtility.HtmlDecode(tag.tag),
                                Dic = tag.dic(),
                                Lck = tag.lck == "1" ? true : false,
                                Cat = tag.cat()
                            };
                            entry.Url = "http://dic.nicovideo.jp/a/" + entry.Tag;
                            ret.Tags.Add(entry);
                        }

                        if(json.uploaderInfo()) {
                            var owner = json.uploaderInfo;
                            ret.Owner = new NicoNicoVideoOwner() {

                                Id = owner.id,
                                UserUrl = "http://www.nicovideo.jp/user/" + owner.id,
                                Nickname = owner.nickname,

                                IconUrl = owner.icon_url,
                                IsUserVideoPublic = owner.is_uservideo_public,
                                IsUserMyVideoPublic = owner.is_user_myvideo_public,
                                IsUserOpenListPublic = owner.is_user_openlist_public
                            };

                            //投稿者をお気に入り登録しているか調べる
                            var follow = await App.ViewModelRoot.CurrentUser.Session.GetAsync(string.Format(UploaderInfoApi, ret.Owner.Id));

                            ret.IsUploaderFollowed = DynamicJson.Parse(follow).data.following;
                        }

                        if(json.channelInfo()) {
                            var channel = json.channelInfo;
                            ret.Channel = new NicoNicoVideoChannel() {

                                Id = channel.id,
                                Name = channel.name,
                                IconUrl = channel.icon_url,
                                FavoriteToken = channel.favorite_token,
                                FavoriteTokenTime = (long)channel.favorite_token_time,
                                IsFavorited = channel.is_favorited == 1D
                            };
                            ret.Channel.ChannelUrl = "http://ch.nicovideo.jp/ch" + ret.Channel.Id;

                            ret.IsChannelVideo = true;
                            ret.IsUploaderFollowed = ret.Channel.IsFavorited;
                        }

                        var viewer = json.viewerInfo;
                        ret.Viewer = new NicoNicoVideoViewer() {

                            Id = Convert.ToString(viewer.id),
                            Nickname = viewer.nickname,
                            IsPremium = viewer.isPremium,
                            IsPrivileged = viewer.isPrivileged,
                        };

                        ret.Context = new NicoNicoVideoContext() {

                            HighestRank = videoDetail.highest_rank != null ? int.Parse(videoDetail.highest_rank) : null,
                            YesterdayRank = videoDetail.yesterday_rank != null ? int.Parse(videoDetail.yesterday_rank) : null,
                            CsrfToken = flash.csrfToken,
                            UserKey = getFlv["userkey"]
                        };
                        
                        ret.FmsToken = getFlv["fmst"];

                        return ret;
                    }
                    Owner.Status = "動画の読み込みに失敗しました";
                    return null;
                } else {

                    //htmlから取得したJsonをhtmlデコードしてdynamic型に変える
                    var json = DynamicJson.Parse(HttpUtility.HtmlDecode(container.Attributes["data-api-data"].Value.Trim()));

                    var ret = new NicoNicoWatchApiData();

                    //動画情報
                    var video = json.video;

                    ret.Video = new NicoNicoVideo() {

                        Id = video.id,
                        SmileInfo = new NicoNicoVideo.NicoNicoSmileInfo() {

                            Url = video.smileInfo.url,
                            IsSlowLine = video.smileInfo.isSlowLine,
                            CurrentQualityId = video.smileInfo.currentQualityId
                        },
                        Title = HttpUtility.HtmlDecode(video.title),
                        OriginalTitle = video.originalTitle,
                        Description = HyperLinkReplacer.Replace(video.description),
                        OriginalDescription = video.originalDescription,
                        ThumbNailUrl = video.thumbnailURL,
                        PostedDateTime = DateTime.Parse(video.postedDateTime),
                        OriginalPostedDateTime = video.originalPostedDateTime() && video.originalPostedDateTime != null ? DateTime.Parse(video.originalPostedDateTime) : DateTime.MinValue,
                        Width = (int?)video.width,
                        Height = (int?)video.height,
                        Duration = (int)video.duration,
                        ViewCount = (int)video.viewCount,
                        MylistCount = (int)video.mylistCount,
                        Translation = video.translation,
                        Translator = video.translator,
                        MovieType = video.movieType,
                        Badges = video.badges,
                        IntroducedNicoliveDJInfo = video.introducedNicoliveDJInfo,
                        DmcInfo = video.dmcInfo != null ? new NicoNicoDmc() {
                            Time = (long)video.dmcInfo.time,
                            TimeMs = (long)video.dmcInfo.time_ms,
                            VideoId = video.dmcInfo.video.video_id,
                            LengthSeconds = (int)video.dmcInfo.video.length_seconds,
                            Deleted = (int)video.dmcInfo.video.deleted,
                            ServerUrl = video.dmcInfo.thread.server_url,
                            SubServerUrl = video.dmcInfo.thread.sub_server_url,
                            ThreadId = Convert.ToString(video.dmcInfo.thread.thread_id),
                            NicosThreadId = Convert.ToString(video.dmcInfo.thread.nicos_thread_id),
                            OptionalThreadId = Convert.ToString(video.dmcInfo.thread.optional_thread_id),
                            ThreadKeyRequired = video.dmcInfo.thread.thread_key_required,
                            ChannelNGWords = video.dmcInfo.thread.channel_ng_words,
                            OwnerNGWords = video.dmcInfo.thread.owner_ng_words,
                            MaintenancesNg = video.dmcInfo.thread.maintenances_ng,
                            PostkeyAvailable = video.dmcInfo.thread.postkey_available,
                            NgRevision = (int)video.dmcInfo.thread.ng_revision,

                            ApiUrls = video.dmcInfo.session_api.api_urls,
                            RecipeId = video.dmcInfo.session_api.recipe_id,
                            PlayerId = video.dmcInfo.session_api.player_id,
                            Videos = video.dmcInfo.session_api.videos,
                            Audios = video.dmcInfo.session_api.audios,
                            Movies = video.dmcInfo.session_api.movies,
                            Protocols = video.dmcInfo.session_api.protocols,
                            AuthType = video.dmcInfo.session_api.auth_types.http,
                            ServiceUserId = video.dmcInfo.session_api.service_user_id,
                            Token = video.dmcInfo.session_api.token,
                            Signature = video.dmcInfo.session_api.signature,
                            ContentId = video.dmcInfo.session_api.content_id,
                            HeartbeatLifeTime = (int)video.dmcInfo.session_api.heartbeat_lifetime,
                            ContentKeyTimeout = (int)video.dmcInfo.session_api.content_key_timeout,
                            Priority = video.dmcInfo.session_api.priority
                        } : null,
                        BackCommentType = video.backCommentType,
                        IsCommentExpired = video.isCommentExpired,
                        IsWide = video.isWide,
                        IsOfficialAnime = (int?)video.isOfficialAnime,
                        IsNoBanner = video.isNoBanner,
                        IsDeleted = video.isDeleted,
                        IsTranslated = video.isTranslated,
                        IsR18 = video.isR18,
                        IsAdult = video.isAdult,
                        IsNicowari = video.isNicowari,
                        IsPublic = video.isPublic,
                        IsPublishedNicoscript = video.isPublishedNicoscript,
                        IsNoNGS = video.isNoNGS,
                        IsCommunityMemberOnly = video.isCommunityMemberOnly,
                        IsCommonsTreeExists = video.isCommonsTreeExists,
                        IsNoIchiba = video.isNoIchiba,
                        IsOfficial = video.isOfficial,
                        IsMonetized = video.isMonetized
                    };

                    var thread = json.thread;
                    ret.Thread = new NicoNicoThread() {

                        CommentCount = (int)thread.commentCount,
                        HasOwnerThread = thread.hasOwnerThread,
                        MymemoryLanguage = thread.mymemoryLanguage,
                        ServerUrl = thread.serverUrl.Replace("api", "api.json"),
                        SubServerurl = thread.subServerUrl,
                        ThreadIds = new NicoNicoThreadIds() {

                            Default = thread.ids.@default,
                            Nicos = thread.ids.nicos,
                            Community = thread.ids.community
                        }
                    };

                    //タグを走査
                    ret.Tags = new List<NicoNicoVideoTag>();
                    foreach(var tag in json.tags) {

                        var entry = new NicoNicoVideoTag() {

                            Id = tag.id,
                            Tag = HttpUtility.HtmlDecode(tag.name),
                            Dic = tag.isDictionaryExists,
                            Lck = tag.isLocked,
                            Cat = tag.isCategory
                        };
                        entry.Url = "http://dic.nicovideo.jp/a/" + entry.Tag;
                        ret.Tags.Add(entry);
                    }

                    var owner = json.owner;
                    if(owner != null) {
                        ret.Owner = new NicoNicoVideoOwner() {

                            Id = owner.id,
                            UserUrl = "http://www.nicovideo.jp/user/" + owner.id,
                            Nickname = owner.nickname,

                            IconUrl = owner.iconURL,
                            NicoliveInfo = owner.nicoliveInfo,
                            ChannelInfo = owner.channelInfo,
                            IsUserVideoPublic = owner.isUserVideoPublic,
                            IsUserMyVideoPublic = owner.isUserMyVideoPublic,
                            IsUserOpenListPublic = owner.isUserOpenListPublic
                        };

                        //投稿者をお気に入り登録しているか調べる
                        var follow = await App.ViewModelRoot.CurrentUser.Session.GetAsync(string.Format(UploaderInfoApi, ret.Owner.Id));

                        ret.IsUploaderFollowed = DynamicJson.Parse(follow).data.following;
                    }

                    var channel = json.channel;
                    if(channel != null) {

                        ret.Channel = new NicoNicoVideoChannel() {

                            Id = channel.id,
                            Name = channel.name,
                            IconUrl = channel.iconURL,
                            FavoriteToken = channel.favoriteToken,
                            FavoriteTokenTime = (long)channel.favoriteTokenTime,
                            IsFavorited = channel.isFavorited,
                            NGFilters = new List<NicoNicoVideoNGFilter>(),
                            ThreadType = channel.threadType,
                            GlobalId = channel.globalId
                        };
                        ret.Channel.ChannelUrl = "http://ch.nicovideo.jp/" + ret.Channel.GlobalId;

                        foreach(var filter in channel.ngFilters) {

                            ret.Channel.NGFilters.Add(new NicoNicoVideoNGFilter() { Source = filter.source, Destination = filter.destination });
                        }

                        ret.IsChannelVideo = true;
                        ret.IsUploaderFollowed = ret.Channel.IsFavorited;
                    }

                    var viewer = json.viewer;
                    ret.Viewer = new NicoNicoVideoViewer() {

                        Id = Convert.ToString(viewer.id),
                        Nickname = viewer.nickname,
                        Prefecture = (int)viewer.prefecture,
                        Sex = (int)viewer.sex,
                        Age = (int)viewer.age,
                        IsPremium = viewer.isPremium,
                        IsPrivileged = viewer.isPrivileged,
                        IsPostLocked = viewer.isPostLocked,
                        IsHtrzm = viewer.isHtrzm,
                        IsTwitterConnection = viewer.isTwitterConnection
                    };

                    var context = json.context;
                    ret.Context = new NicoNicoVideoContext() {

                        PlayFrom = context.playFrom,
                        InitialPlaybackPosition = (int?)context.initialPlaybackPosition,
                        InitialPlaybackType = context.initialPlaybackType,
                        PlayLength = context.playLength,
                        ReturnId = context.returnId,
                        ReturnTo = context.returnTo,
                        ReturnMsg = context.returnMsg,
                        WatchId = context.watchId,
                        IsNoMovie = context.isNoMovie,
                        IsNoRelatedVideo = context.isNoRelatedVideo,
                        IsDownloadCompleteWait = context.isDownloadCompleteWait,
                        IsNoNicotic = context.isNoNicotic,
                        IsNeedPayment = context.isNeedPayment,
                        IsAdultRatingNG = context.isAdultRatingNG,
                        IsPlayable = context.isPlayable,
                        IsTranslatable = context.isTranslatable,
                        IsTagUneditable = context.isTagUneditable,
                        IsVideoOwner = context.isVideoOwner,
                        IsThreadOwner = context.isThreadOwner,
                        IsOwnerThreadEditable = context.isOwnerThreadEditable,
                        UseChecklistCache = context.useChecklistCache,
                        IsDisabledMarquee = context.isDisabledMarquee,
                        IsDictionaryDisplayable = context.isDictionaryDisplayable,
                        IsDefaultCommentInvisible = context.isDefaultCommentInvisible,
                        AccessFrom = context.accessFrom,
                        CsrfToken = context.csrfToken,
                        TranslationVersionJsonUpdateTime = (long)context.translationVersionJsonUpdateTime,
                        UserKey = context.userkey,
                        WatchAuthKey = context.watchAuthKey,
                        WatchTrackId = context.watchTrackId,
                        WatchPageServerTime = (long)context.watchPageServerTime,
                        IsAuthenticationRequired = context.isAuthenticationRequired,
                        IsPeakTime = context.isPeakTime,
                        NgRevision = (int)context.ngRevision,
                        CategoryName = context.categoryName,
                        CategoryKey = context.categoryKey,
                        CategoryGroupName = context.categoryGroupName,
                        CategoryGroupKey = context.categoryGroupKey,
                        YesterdayRank = (int?)context.yesterdayRank,
                        HighestRank = (int?)context.highestRank,
                        IsMyMemory = context.isMyMemory,
                        OwnerNGFilters = new List<NicoNicoVideoNGFilter>()
                    };

                    //垢消しうp主
                    if(ret.Channel == null && ret.Owner == null) {

                        ret.IsOwnerDeleted = true;
                    }

                    if(!Settings.Instance.UseResumePlay) {

                        ret.Context.InitialPlaybackPosition = 0;
                    }

                    foreach(var filter in context.ownerNGList) {

                        ret.Context.OwnerNGFilters.Add(new NicoNicoVideoNGFilter() { Source = filter.source, Destination = filter.destination });
                    }
                    return ret;
                }
            } catch(RequestFailed) {

                Owner.Status = "動画情報の取得に失敗しました";
                return null;
            }
        }

        public async void RecordPlaybackPositionAsync(NicoNicoWatchApiData apiData, double pos) {

            try {

                var form = new Dictionary<string, string> {
                    ["watch_id"] = apiData.Video.Id,
                    ["playback_position"] = pos.ToString(),
                    ["csrf_token"] = apiData.Context.CsrfToken
                };
                var request = new HttpRequestMessage(HttpMethod.Post, RecordPositionApi) {
                    Content = new FormUrlEncodedContent(form)
                };

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);
                var json = DynamicJson.Parse(a);


                if(json.status == "ok") {

                    //再生位置を記録出来た、と
                    //Owner.Status = "再生位置を保存しました。";
                } else {

                    //Owner.Status = "再生位置の保存に失敗しました";
                }
            } catch(RequestFailed) {

                Owner.Status = "再生位置の保存に失敗しました";
            }
        }

        public async Task<bool> ToggleFollowOwnerAsync(NicoNicoWatchApiData apiData) {

            try {

                if(apiData.IsChannelVideo) {


                    GetRequestQuery query = null;
                    if (apiData.IsUploaderFollowed) {

                        query = new GetRequestQuery(ChannelUnFollowApi);
                    } else {

                        query = new GetRequestQuery(ChannelFollowApi);
                    }

                    query.AddQuery("channel_id", apiData.Channel.Id);
                    query.AddQuery("return_json", 1);
                    query.AddQuery("time", apiData.Channel.FavoriteTokenTime);
                    query.AddQuery("key", apiData.Channel.FavoriteToken);

                    var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);

                    var json = DynamicJson.Parse(a);

                    if(json.status == "succeed") {

                        if(apiData.IsUploaderFollowed) {

                            Owner.Status = "フォローを解除しました";
                        } else {

                            Owner.Status = "フォローしました";
                        }
                        return true;
                    }
                } else {

                    if(apiData.IsUploaderFollowed) {

                        var request = new HttpRequestMessage(HttpMethod.Delete, string.Format(UploaderInfoApi, apiData.Owner.Id)) {
                            Content = new FormUrlEncodedContent(new Dictionary<string, string>())
                        };
                        request.Headers.Add("x-request-with", Owner.VideoUrl);

                        var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);
                        
                        var json = DynamicJson.Parse(a);

                        if(json.meta.status == 200D) {

                            Owner.Status = "フォローを解除しました";
                            return true;
                        } else {

                            Owner.Status = "フォロー解除に失敗しました";
                            return false;
                        }

                    } else {
                        var request = new HttpRequestMessage(HttpMethod.Post, string.Format(UploaderInfoApi, apiData.Owner.Id)) {
                            Content = new FormUrlEncodedContent(new Dictionary<string, string>())
                        };
                        request.Headers.Add("x-request-with", Owner.VideoUrl);

                        var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                        var json = DynamicJson.Parse(a);
                        if (json.meta.status == 200D) {

                            Owner.Status = "フォローしました";
                            return true;
                        } else {

                            Owner.Status = "フォローに失敗しました";
                            return false;
                        }
                    }
                }
                return false;
            } catch(RequestFailed) {

                return false;
            }
        }
    }
    public enum PlayBackRateAvalilableReason {

        Available,
        NotInitialized,
        PremiumOnly,
        NotSupportVideo
    }

    public class NicoNicoWatchApiData : NotificationObject {

        //GetFlvAPIの結果 Obsolete
        //public NicoNicoGetFlv GetFlv { get; set; }
        public NicoNicoVideo Video { get; set; }

        //コメント関連
        public NicoNicoThread Thread { get; set; }

        //動画についているタグ
        public List<NicoNicoVideoTag> Tags { get; set; }

        //動画投稿者 人ではない場合null
        public NicoNicoVideoOwner Owner { get; set; }

        //動画投稿チャンネル チャンネルじゃない場合はnull
        public NicoNicoVideoChannel Channel { get; set; }

        //うp主をフォローしているか

        #region IsUploaderFollowed変更通知プロパティ
        private bool _IsUploaderFollowed;

        public bool IsUploaderFollowed {
            get { return _IsUploaderFollowed; }
            set { 
                if (_IsUploaderFollowed == value)
                    return;
                _IsUploaderFollowed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //動画を見ている人 つまり自分
        public NicoNicoVideoViewer Viewer { get; set; }

        //チャンネル動画か否か
        public bool IsChannelVideo { get; set; }

        //投稿者失踪
        public bool IsOwnerDeleted { get; set; }

        //プレイヤーコンテキスト
        public NicoNicoVideoContext Context { get; set; }

        public NicoNicoVideoType VideoType { get; set; }

        //RTMPの時に使う
        public string FmsToken { get; set; }
    }

    public class NicoNicoVideo {

        //VideoId
        public string Id { get; set; }


        public class NicoNicoSmileInfo {

            //動画URL
            public string Url { get; set; }

            public bool IsSlowLine { get; set; }

            public string CurrentQualityId { get; set; }



        }

        public NicoNicoSmileInfo SmileInfo { get; set; }



        //動画タイトル
        public string Title { get; set; }
        public string OriginalTitle { get; set; }

        //動画説明文
        public string Description { get; set; }
        public string OriginalDescription { get; set; }

        //動画サムネイルURL
        public string ThumbNailUrl { get; set; }

        //動画投稿日時
        public DateTime PostedDateTime { get; set; }
        public DateTime OriginalPostedDateTime { get; set; }

        //動画の横幅
        public int? Width { get; set; }

        //動画の縦幅
        public int? Height { get; set; }

        //動画の長さ
        public int Duration { get; set; }

        //再生数
        public int ViewCount { get; set; }

        //マイリスト数
        public int MylistCount { get; set; }

        public bool Translation { get; set; }

        public object Translator { get; set; }

        //動画タイプ mp4かflv
        public string MovieType { get; set; }

        public object Badges { get; set; }

        public object IntroducedNicoliveDJInfo { get; set; }

        public NicoNicoDmc DmcInfo { get; set; }

        public object BackCommentType { get; set; }

        public bool IsCommentExpired { get; set; }

        public string IsWide { get; set; }

        //なんでboolじゃないの・・・ 1=公式アニメ null=それ以外
        public int? IsOfficialAnime { get; set; }

        public string IsNoBanner { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsTranslated { get; set; }

        public bool IsR18 { get; set; }

        public bool IsAdult { get; set; }

        public bool? IsNicowari { get; set; }

        public bool IsPublic { get; set; }

        public bool? IsPublishedNicoscript { get; set; }

        public string IsNoNGS { get; set; }

        public string IsCommunityMemberOnly { get; set; }

        public bool? IsCommonsTreeExists { get; set; }

        public bool IsNoIchiba { get; set; }

        //公式か
        public bool IsOfficial { get; set; }

        public bool IsMonetized { get; set; }
    }

    public class NicoNicoDmc {

        //何の時間だよ
        public long Time { get; set; }
        public long TimeMs { get; set; }

        public string VideoId { get; set; }
        public int LengthSeconds { get; set; }
        public int Deleted { get; set; }

        public string ServerUrl { get; set; }
        public string SubServerUrl { get; set; }
        public string ThreadId { get; set; }
        public string NicosThreadId { get; set; }
        public string OptionalThreadId { get; set; }
        public bool ThreadKeyRequired { get; set; }
        public object ChannelNGWords { get; set; }
        public object OwnerNGWords { get; set; }
        public bool MaintenancesNg { get; set; }
        public bool PostkeyAvailable { get; set; }
        public int NgRevision { get; set; }

        //session API
        public List<string> ApiUrls { get; set; }

        public string RecipeId { get; set; }
        public string PlayerId { get; set; }

        //
        public List<string> Videos { get; set; }

        //オーディオコーデック
        public List<string> Audios { get; set; }

        //
        public List<string> Movies { get; set; }

        //http以外あるのか謎
        public List<string> Protocols { get; set; }

        //認証タイプ
        public string AuthType { get; set; }

        //どうみても自分のユーザーID
        public string ServiceUserId { get; set; }

        //
        public string Token { get; set; }

        //
        public string Signature { get; set; }

        //まだ分からぬ
        public string ContentId { get; set; }

        //
        public int HeartbeatLifeTime { get; set; }

        //
        public int ContentKeyTimeout { get; set; }
        //
        public double Priority { get; set; }
    }


    public class NicoNicoThread {

        //コメント数
        public int CommentCount { get; set; }

        //投稿者コメントがあるか否か
        public string HasOwnerThread { get; set; }

        public object MymemoryLanguage { get; set; }

        //コメントサーバーURL
        public string ServerUrl { get; set; }

        //サブサーバーURL だいたい本番サーバーと同じなので意味をなしてない
        public string SubServerurl { get; set; }

        public NicoNicoThreadIds ThreadIds { get; set; }
    }
    public class NicoNicoThreadIds {

        //デフォルトスレッドID
        public string Default { get; set; }

        //ニコスクリプトスレッドID
        public string Nicos { get; set; }

        //コミュニティスレッドID
        public string Community { get; set; }
    }

    public class NicoNicoVideoContext {

        public object PlayFrom { get; set; }

        //動画再生開始位置
        public int? InitialPlaybackPosition { get; set;}

        //動画再生開始位置タイプ
        public string InitialPlaybackType { get; set; }

        public object PlayLength { get; set; }

        public object ReturnId { get; set; }

        public object ReturnTo { get; set; }

        public object ReturnMsg { get; set; }

        //動画ID
        public string WatchId { get; set; }

        public bool? IsNoMovie { get; set; }

        public string IsNoRelatedVideo { get; set; }

        public bool? IsDownloadCompleteWait { get; set; }

        public bool? IsNoNicotic { get; set; }

        //課金が必要な動画か
        public bool IsNeedPayment { get; set; }

        public bool IsAdultRatingNG { get; set; }

        public bool? IsPlayable { get; set; }
        
        //翻訳可能か
        public bool IsTranslatable { get; set; } 

        //タグが編集不能か
        public bool IsTagUneditable { get; set; }

        public bool IsVideoOwner { get; set; }

        public bool IsThreadOwner { get; set; }

        public string IsOwnerThreadEditable { get; set; }

        public string UseChecklistCache { get; set; }

        public bool? IsDisabledMarquee { get; set; }

        public bool IsDictionaryDisplayable { get; set; }

        public bool IsDefaultCommentInvisible { get; set; }

        public object AccessFrom { get; set; }

        //CSRF対策トークン
        public string CsrfToken { get; set; }

        public long TranslationVersionJsonUpdateTime { get; set; }

        //スレッドユーザーキー
        public string UserKey { get; set; }

        public string WatchAuthKey { get; set; }

        public string WatchTrackId { get; set; }

        //ミリ秒
        public long WatchPageServerTime { get; set; }

        public bool IsAuthenticationRequired { get; set; }

        public bool? IsPeakTime { get; set; }

        public int NgRevision { get; set; }

        //動画カテゴリの名前
        public string CategoryName { get; set; }

        //カテゴリのキー
        public string CategoryKey { get; set; }

        public string CategoryGroupName { get; set; }

        public string CategoryGroupKey { get; set; }

        //前日ランキング順位
        public int? YesterdayRank { get; set; }

        //過去最高順位
        public int? HighestRank { get; set; }

        //マイメモリーか
        public bool IsMyMemory { get; set; }

        //投稿者NGフィルター
        public List<NicoNicoVideoNGFilter> OwnerNGFilters { get; set; }
    }

    public class NicoNicoVideoOwner {

        //投稿者ID
        public string Id { get; set; }

        //ユーザーページURL
        public string UserUrl { get; set; }

        //投稿者の名前
        public string Nickname { get; set; }

        //アイコンURL
        public string IconUrl { get; set; }

        
        public object NicoliveInfo { get; set; }

        public object ChannelInfo { get; set; }

        //投稿動画が公開しているか
        public bool IsUserVideoPublic { get; set; }

        //
        public bool IsUserMyVideoPublic { get; set; }

        //公開マイリストが公開されているか
        public bool IsUserOpenListPublic { get; set; }

    }

    public class NicoNicoVideoChannel {

        //チャンネルID
        public string Id { get; set; }

        //チャンネル名
        public string Name { get; set; }

        //チャンネルURL
        public string ChannelUrl { get; set; }

        //チャンネルアイコンURL
        public string IconUrl { get; set; }

        public string FavoriteToken { get; set; }

        //UnixTimeぽい
        public long FavoriteTokenTime { get; set; }

        //フォローしているかどうか
        public bool IsFavorited { get; set; }

        //チャンネルNGフィルター
        public List<NicoNicoVideoNGFilter> NGFilters { get; set; }

        public string ThreadType { get; set; }

        public string GlobalId { get; set; }

    }

    public class NicoNicoVideoViewer {

        //視聴者ID
        public string Id { get; set; }

        //視聴者の名前
        public string Nickname { get; set; }

        //住んでいる地域？
        public int Prefecture { get; set; }

        //0 = Male, 1 = Female
        public int Sex { get; set; }

        //年齢
        public int Age { get; set; }

        //プレミアム会員か
        public bool IsPremium { get; set; }

        public bool IsPrivileged { get; set; }

        //コメント規制されているか
        public bool IsPostLocked { get; set; }

        public bool IsHtrzm { get; set; }

        public bool IsTwitterConnection { get; set; }
    }


    //タグ情報
    public class NicoNicoVideoTag {

        //タグID
        public string Id { get; set; }

        //タグ名
        public string Tag { get; set; }

        //大百科URL
        public string Url { get; set; }

        //大百科が存在するか
        public bool Dic { get; set; }

        //ロックされているか
        public bool Lck { get; set; }

        //カテゴリタグか否か
        public bool Cat { get; set; }
    }
    public class NicoNicoVideoNGFilter {

        //コメントを変換するもとの文字列
        public string Source;

        //対象のコメントをこれに置換する
        public string Destination;

        public bool UseRegex {
            get {
                return Source != null ? Source.StartsWith("/") : false;
            }
        }
        public bool WholeReplace {
            get {
                return Source != null ? Source.StartsWith("*") : false;
            }
        }
    }

    public enum NicoNicoVideoType {

        MP4,
        FLV,
        SWF,
        RTMP,
        New
    }
}
