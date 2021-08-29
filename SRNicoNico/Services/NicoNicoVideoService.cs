using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DynaJson;
using Microsoft.EntityFrameworkCore;
using SRNicoNico.Entities;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// IVideoServiceの実装
    /// </summary>
    public class NicoNicoVideoService : IVideoService {
        /// <summary>
        /// 動画視聴API
        /// </summary>
        private const string WatchApiUrl = "https://www.nicovideo.jp/api/watch/v3/";
        /// <summary>
        /// 動画再生位置保存API
        /// </summary>
        private const string PlaybackPositionApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/watch/history/playback-position";
        /// <summary>
        /// いいねAPI
        /// </summary>
        private const string LikeApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/likes/items";

        private readonly ISessionService SessionService;
        private readonly ViewerDbContext DbContext;

        public NicoNicoVideoService(ISessionService sessionService, ViewerDbContext dbContext) {

            SessionService = sessionService;
            DbContext = dbContext;
        }

        public Task<WatchApiData> WatchAsync(string videoId, bool withoutHistory = false) {
            return WatchAsyncInternal(videoId, withoutHistory);
        }
        public Task<WatchApiData> WatchContinueAsync(string videoId) {
            return WatchAsyncInternal(videoId, false, true);
        }

        /// <inheritdoc />
        private async Task<WatchApiData> WatchAsyncInternal(string videoId, bool withoutHistory = false, bool isContinueWatching = false) {

            if (string.IsNullOrEmpty(videoId)) {
                throw new ArgumentNullException(nameof(videoId));
            }

            var builder = new GetRequestQueryBuilder(WatchApiUrl + videoId)
                .AddQuery("_frontendId", 6)
                .AddQuery("_frontendVersion", 0)
                .AddQuery("actionTrackId", "1_1")
                .AddQuery("withoutHistory", withoutHistory)
                .AddQuery("isContinueWatching", isContinueWatching)
                .AddQuery("additionals", withoutHistory ? "" : "series") // withoutHistoryがfalseの時はシリーズ情報も取得する
                .AddQuery("skips", "harmful");

            using var result = await SessionService.GetAsync(builder.Build()).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var ret = new WatchApiData();

            // コメントデータ
            {
                var comment = data.comment;
                var layers = new List<CommentLayer>();
                foreach (var layer in comment.layers) {

                    if (layer == null) {
                        continue;
                    }
                    var threadIds = new List<CommentLayerThreadId>();
                    foreach (var threadId in layer.threadIds) {

                        if (threadId == null) {
                            continue;
                        }
                        threadIds.Add(new CommentLayerThreadId {
                            Id = threadId.id.ToString(),
                            Fork = (int)threadId.fork
                        });
                    }
                    layers.Add(new CommentLayer {
                        Index = (int)layer.index,
                        IsTranslucent = layer.isTranslucent,
                        ThreadIds = threadIds
                    });
                }

                var threads = new List<CommentThread>();
                foreach (var thread in comment.threads) {

                    if (thread == null) {
                        continue;
                    }
                    threads.Add(new CommentThread {
                        Id = thread.id.ToString(),
                        Fork = (int)thread.fork,
                        IsActive = thread.isActive,
                        IsDefaultPostTarget = thread.isDefaultPostTarget,
                        IsEasyCommentPostTarget = thread.isEasyCommentPostTarget,
                        IsLeafRequired = thread.isLeafRequired,
                        IsOwnerThread = thread.isOwnerThread,
                        IsThreadKeyRequired = thread.isThreadkeyRequired,
                        ThreadKey = thread.threadkey,
                        Is184Forced = thread.is184Forced,
                        HasNicoscript = thread.hasNicoscript,
                        Label = thread.label,
                        PostKeyStatus = (int)thread.postkeyStatus,
                        Server = thread.server
                    });
                }

                ret.Comment = new WatchApiDataComment {
                    ServerUrl = comment.server.url,
                    UserKey = comment.keys.userKey,
                    Layers = layers,
                    Threads = threads
                };
            }

            // かんたんコメントデータ
            {
                var easyComment = data.easyComment;
                var phrases = new List<EasyCommentPhrase>();

                foreach (var phrase in easyComment.phrases) {

                    if (phrase == null) {
                        continue;
                    }

                    phrases.Add(new EasyCommentPhrase {
                        Text = phrase.text,
                        NicoDicTitle = phrase.nicodic?.title,
                        NicoDicViewTitle = phrase.nicodic?.viewTitle,
                        NicoDicSummary = phrase.nicodic?.summary,
                        NicoDicLink = phrase.nicodic?.link
                    });
                }
                ret.EasyCommentPhrases = phrases;
            }

            // ジャンルデータ
            {
                var genre = data.genre;
                ret.Genre = new WatchApiDataGenre {
                    Key = genre.key,
                    Label = genre.label,
                    IsImmoral = genre.isImmoral,
                    IsDisabled = genre.isDisabled,
                    IsNotSet = genre.isNotSet
                };
            }

            // DMCデータ
            {
                var media = data.media;

                var audios = new List<MediaMovieAudio>();
                foreach (var audio in media.delivery.movie.audios) {

                    if (audio == null) {
                        continue;
                    }

                    var collections = new Dictionary<string, double>();
                    foreach (var lc in audio.metadata.loudnessCollection) {

                        if (lc == null) {
                            continue;
                        }
                        collections[lc.type] = lc.value;
                    }

                    audios.Add(new MediaMovieAudio {
                        Id = audio.id,
                        IsAvailable = audio.isAvailable,
                        Bitrate = (int)audio.metadata.bitrate / 1000,
                        SamplingRate = (int)audio.metadata.samplingRate,
                        IntegratedLoudness = (int)audio.metadata.loudness.integratedLoudness,
                        TruePeak = (int)audio.metadata.loudness.truePeak,
                        LevelIndex = (int)audio.metadata.levelIndex,
                        LoudnessCollection = collections
                    });
                }

                var videos = new List<MediaMovieVideo>();
                foreach (var video in media.delivery.movie.videos) {

                    if (video == null) {
                        continue;
                    }

                    videos.Add(new MediaMovieVideo {
                        Id = video.id,
                        IsAvailable = video.isAvailable,
                        Label = video.metadata.label,
                        Bitrate = (int)video.metadata.bitrate / 1000,
                        Width = (int)video.metadata.resolution.width,
                        Height = (int)video.metadata.resolution.height,
                        LevelIndex = (int)video.metadata.levelIndex,
                        RecommendedHighestAudioLevelIndex = (int)video.metadata.recommendedHighestAudioLevelIndex
                    });
                }

                var movieUrls = new List<MediaSessionUrl>();
                foreach (var url in media.delivery.movie.session.urls) {

                    if (url == null) {
                        continue;
                    }
                    movieUrls.Add(new MediaSessionUrl {
                        Url = url.url,
                        IsWellKnownPort = url.isWellKnownPort,
                        IsSsl = url.isSsl
                    });
                }
                var storyboardUrls = new List<MediaSessionUrl>();
                var storyboardImageIds = new List<string>();
                if (media.delivery.storyboard()) {
                    foreach (var url in media.delivery.storyboard.session.urls) {

                        if (url == null) {
                            continue;
                        }
                        storyboardUrls.Add(new MediaSessionUrl {
                            Url = url.url,
                            IsWellKnownPort = url.isWellKnownPort,
                            IsSsl = url.isSsl
                        });
                    }
                    foreach (var image in media.delivery.storyboard.images) {

                        if (image == null) {
                            continue;
                        }
                        storyboardImageIds.Add(image.id);
                    }
                }

                ret.Media = new WatchApiDataMedia {
                    RecipeId = media.delivery.recipeId,
                    Encryption = media.delivery.encryption,
                    Movie = new MediaMovie {
                        ContentId = media.delivery.movie.contentId,
                        Audios = audios,
                        Videos = videos,
                        Session = new MediaSession {
                            RecipeId = media.delivery.movie.session.recipeId,
                            PlayerId = media.delivery.movie.session.playerId,
                            Videos = JsonObjectExtension.ToStringArray(media.delivery.movie.session.videos),
                            Audios = JsonObjectExtension.ToStringArray(media.delivery.movie.session.audios),
                            Movies = JsonObjectExtension.ToStringArray(media.delivery.movie.session.movies),
                            Protocols = JsonObjectExtension.ToStringArray(media.delivery.movie.session.protocols),
                            AuthTypesHttp = media.delivery.movie.session.authTypes.http,
                            AuthTypesHls = media.delivery.movie.session.authTypes.hls,
                            ServiceUserId = media.delivery.movie.session.serviceUserId,
                            Token = media.delivery.movie.session.token,
                            Signature = media.delivery.movie.session.signature,
                            ContentId = media.delivery.movie.session.contentId,
                            HeartbeatLifetime = (int)media.delivery.movie.session.heartbeatLifetime,
                            ContentKeyTimeout = (int)media.delivery.movie.session.contentKeyTimeout,
                            Priority = media.delivery.movie.session.priority,
                            TransferPresets = JsonObjectExtension.ToStringArray(media.delivery.movie.session.transferPresets),
                            Urls = movieUrls
                        }
                    },
                    StoryBoard = !media.delivery.storyboard() ? null : new MediaStoryBoard {
                        ContentId = media.delivery.storyboard.contentId,
                        ImageIds = storyboardImageIds,
                        Session = new MediaSession {
                            RecipeId = media.delivery.storyboard.session.recipeId,
                            PlayerId = media.delivery.storyboard.session.playerId,
                            Videos = JsonObjectExtension.ToStringArray(media.delivery.storyboard.session.videos),
                            Audios = JsonObjectExtension.ToStringArray(media.delivery.storyboard.session.audios),
                            Movies = JsonObjectExtension.ToStringArray(media.delivery.storyboard.session.movies),
                            Protocols = JsonObjectExtension.ToStringArray(media.delivery.storyboard.session.protocols),
                            AuthTypesStoryBoard = media.delivery.storyboard.session.authTypes.storyboard,
                            ServiceUserId = media.delivery.storyboard.session.serviceUserId,
                            Token = media.delivery.storyboard.session.token,
                            Signature = media.delivery.storyboard.session.signature,
                            ContentId = media.delivery.storyboard.session.contentId,
                            HeartbeatLifetime = (int)media.delivery.storyboard.session.heartbeatLifetime,
                            ContentKeyTimeout = (int)media.delivery.storyboard.session.contentKeyTimeout,
                            Priority = media.delivery.storyboard.session.priority,
                            TransferPresets = JsonObjectExtension.ToStringArray(media.delivery.storyboard.session.transferPresets),
                            Urls = storyboardUrls
                        }
                    },
                    TrackingId = media.delivery.trackingId
                };
            }

            ret.OkReason = data.okReason;

            // 投稿者データ
            {
                var owner = data.owner;
                if (owner != null) {

                    ret.Owner = new WatchApiDataOwner {
                        Id = owner.id.ToString(),
                        Nickname = owner.nickname,
                        IconUrl = owner.iconUrl,
                        Channel = owner.channel,
                        Live = owner.live,
                        IsVideosPublic = owner.isVideosPublic,
                        IsMylistsPublic = owner.isMylistsPublic,
                        VideoLiveNotice = owner.videoLiveNotice,
                        IsFollowing = owner.viewer.isFollowing
                    };
                }
            }

            // プレイヤーデータ
            {
                var player = data.player;
                ret.Player = new WatchApiDataPlayer {
                    InitialPlaybackType = player.initialPlayback?.type,
                    InitialPlaybackPositionSec = (int?)player.initialPlayback?.positionSec,
                };
            }

            // ランキングデータ
            {
                var ranking = data.ranking;
                var tags = new List<WatchApiDataRankingPopularTag>();
                foreach (var tag in ranking.popularTag) {

                    if (tag == null) {
                        continue;
                    }

                    tags.Add(new WatchApiDataRankingPopularTag {
                        Tag = tag.tag,
                        RegularizedTag = tag.regularizedTag,
                        Rank = (int)tag.rank,
                        Genre = tag.genre,
                        DateTime = DateTimeOffset.Parse(tag.dateTime)
                    });
                }

                ret.Ranking = new WatchApiDataRanking {
                    Rank = (int?)ranking.genre?.rank,
                    Genre = ranking.genre?.genre,
                    DateTime = ranking.genre != null ? DateTimeOffset.Parse(ranking.genre.dateTime) : null,
                    PopularTag = tags
                };
            }

            // シリーズデータ
            {
                var series = data.series;
                if (series != null) {
                    var prev = series.video.prev;
                    var next = series.video.next;
                    var first = series.video.first;

                    ret.Series = new WatchApiDataSeries {
                        Id = series.id.ToString(),
                        Title = series.title,
                        Description = series.description,
                        ThumbnailUrl = series.thumbnailUrl,
                        Prev = prev == null ? null : new WatchApiDataSeriesVideo {
                            Id = prev.id,
                            Title = prev.title,
                            RegisteredAt = DateTimeOffset.Parse(prev.registeredAt),
                            ViewCount = (int)prev.count.view,
                            CommentCount = (int)prev.count.comment,
                            MylistCount = (int)prev.count.mylist,
                            LikeCount = (int)prev.count.like,
                            ThumbnailUrl = prev.thumbnail.url,
                            ThumbnailMiddleUrl = prev.thumbnail.middleUrl,
                            ThumbnailLargeUrl = prev.thumbnail.largeUrl,
                            ThumbnailListingUrl = prev.thumbnail.listingUrl,
                            ThumbnailnHdUrl = prev.thumbnail.nHdUrl,
                            Duration = (int)prev.duration,
                            ShortDescription = prev.shortDescription,
                            LatestCommentSummary = prev.latestCommentSummary,
                            IsChannelVideo = prev.isChannelVideo,
                            IsPaymentRequired = prev.isPaymentRequired,
                            PlaybackPosition = (int?)prev.playbackPosition,
                            OwnerType = prev.owner.ownerType,
                            OwnerId = prev.owner.id,
                            OwnerName = prev.owner.name,
                            OwnerIconUrl = prev.owner.iconUrl,
                            RequireSensitiveMasking = prev.requireSensitiveMasking
                        },
                        Next = next == null ? null : new WatchApiDataSeriesVideo {
                            Id = next.id,
                            Title = next.title,
                            RegisteredAt = DateTimeOffset.Parse(next.registeredAt),
                            ViewCount = (int)next.count.view,
                            CommentCount = (int)next.count.comment,
                            MylistCount = (int)next.count.mylist,
                            LikeCount = (int)next.count.like,
                            ThumbnailUrl = next.thumbnail.url,
                            ThumbnailMiddleUrl = next.thumbnail.middleUrl,
                            ThumbnailLargeUrl = next.thumbnail.largeUrl,
                            ThumbnailListingUrl = next.thumbnail.listingUrl,
                            ThumbnailnHdUrl = next.thumbnail.nHdUrl,
                            Duration = (int)next.duration,
                            ShortDescription = next.shortDescription,
                            LatestCommentSummary = next.latestCommentSummary,
                            IsChannelVideo = next.isChannelVideo,
                            IsPaymentRequired = next.isPaymentRequired,
                            PlaybackPosition = (int?)next.playbackPosition,
                            OwnerType = next.owner.ownerType,
                            OwnerId = next.owner.id,
                            OwnerName = next.owner.name,
                            OwnerIconUrl = next.owner.iconUrl,
                            RequireSensitiveMasking = next.requireSensitiveMasking
                        },
                        First = first == null ? null : new WatchApiDataSeriesVideo {
                            Id = first.id,
                            Title = first.title,
                            RegisteredAt = DateTimeOffset.Parse(first.registeredAt),
                            ViewCount = (int)first.count.view,
                            CommentCount = (int)first.count.comment,
                            MylistCount = (int)first.count.mylist,
                            LikeCount = (int)first.count.like,
                            ThumbnailUrl = first.thumbnail.url,
                            ThumbnailMiddleUrl = first.thumbnail.middleUrl,
                            ThumbnailLargeUrl = first.thumbnail.largeUrl,
                            ThumbnailListingUrl = first.thumbnail.listingUrl,
                            ThumbnailnHdUrl = first.thumbnail.nHdUrl,
                            Duration = (int)first.duration,
                            ShortDescription = first.shortDescription,
                            LatestCommentSummary = first.latestCommentSummary,
                            IsChannelVideo = first.isChannelVideo,
                            IsPaymentRequired = first.isPaymentRequired,
                            PlaybackPosition = (int?)first.playbackPosition,
                            OwnerType = first.owner.ownerType,
                            OwnerId = first.owner.id,
                            OwnerName = first.owner.name,
                            OwnerIconUrl = first.owner.iconUrl,
                            RequireSensitiveMasking = first.requireSensitiveMasking
                        }
                    };
                }
            }

            // タグデータ
            {
                var tag = data.tag;
                var items = new List<WatchApiDataTagItem>();
                foreach (var item in tag.items) {

                    if (item == null) {
                        continue;
                    }
                    items.Add(new WatchApiDataTagItem {
                        Name = item.name,
                        IsCategory = item.isCategory,
                        IsCategoryCandidate = item.isCategoryCandidate,
                        IsNicodicArticleExists = item.isNicodicArticleExists,
                        IsLocked = item.isLocked
                    });
                }

                ret.Tag = new WatchApiDataTag {
                    Items = items,
                    HasR18Tag = tag.hasR18Tag,
                    IsPublishedNicoscript = tag.isPublishedNicoscript,
                    Edit = new WatchApiDataTagEdit {
                        IsEditable = tag.edit.isEditable,
                        UneditableReason = tag.edit.uneditableReason,
                        EditKey = tag.edit.editKey
                    },
                    Viewer = new WatchApiDataTagEdit {
                        IsEditable = tag.viewer.isEditable,
                        UneditableReason = tag.viewer.uneditableReason,
                        EditKey = tag.viewer.editKey
                    }
                };
            }

            // 動画データ
            {
                var video = data.video;
                ret.Video = new WatchApiDataVideo {
                    Id = video.id,
                    Title = video.title,
                    Description = video.description,
                    ViewCount = (int)video.count.view,
                    CommentCount = (int)video.count.comment,
                    MylistCount = (int)video.count.mylist,
                    LikeCount = (int)video.count.like,
                    Duration = (int)video.duration,
                    ThumbnailUrl = video.thumbnail.url,
                    ThumbnailMiddleUrl = video.thumbnail.middleUrl,
                    ThumbnailLargeUrl = video.thumbnail.largeUrl,
                    ThumbnailPlayer = video.thumbnail.player,
                    ThumbnailOgp = video.thumbnail.ogp,
                    IsAdult = video.rating.isAdult,
                    RegisteredAt = DateTimeOffset.Parse(video.registeredAt),
                    IsPrivate = video.isPrivate,
                    IsDeleted = video.isDeleted,
                    IsNoBanner = video.isNoBanner,
                    IsAuthenticationRequired = video.isAuthenticationRequired,
                    IsEmbedPlayerAllowed = video.isEmbedPlayerAllowed,
                    Viewer = new WatchApiDataVideoViewer {
                        IsOwner = video.viewer.isOwner,
                        IsLiked = video.viewer.like.isLiked,
                        LikeCount = (int?)video.viewer.like.count,
                    },
                    WatchableUserTypeForPayment = video.watchableUserTypeForPayment,
                    CommentableUserTypeForPayment = video.commentableUserTypeForPayment
                };

                ret.Comment.VideoDuration = ret.Video.Duration;
            }

            // 視聴者データ
            {
                var viewer = data.viewer;
                ret.Viewer = new WatchApiDataViewer {
                    Id = viewer.id.ToString(),
                    Nickname = viewer.nickname,
                    IsPremium = viewer.isPremium
                };

                ret.Comment.UserId = ret.Viewer.Id;
            }

            return ret;
        }


        /// <inheritdoc />
        public async Task<DmcSession> CreateSessionAsync(MediaSession movieSession, string? videoId = null, string? audioId = null) {

            if (movieSession == null) {
                throw new ArgumentNullException(nameof(movieSession));
            }
            if (movieSession.Urls.Count() == 0) {
                throw new ArgumentException("DMCのAPI URLがありません");
            }
            if (movieSession.Videos.Count() == 0) {
                throw new ArgumentException("動画データがありません");
            }
            if (movieSession.Audios.Count() == 0) {
                throw new ArgumentException("音声データがありません");
            }

            string apiUrl = movieSession.Urls.First().Url!;

            string video = videoId ?? movieSession.Videos.First();
            string audio = audioId ?? movieSession.Audios.First();

            var preferedProtocolHttp = movieSession.Protocols.Contains("http");
            var preferedProtocolHls = movieSession.Protocols.Contains("hls");

            // リクエストのjsonを組み立てる
            var payload = JsonObject.Serialize(new {
                session = new {
                    recipe_id = movieSession.RecipeId,
                    content_id = movieSession.ContentId,
                    content_type = "movie",
                    content_src_id_sets = new[] {
                        new {
                            content_src_ids = new[] {
                                new {
                                    src_id_to_mux = new {
                                        video_src_ids = new string[] { video },
                                        audio_src_ids = new string[] { audio }
                                    }
                                }
                            }
                        }
                    },
                    timing_constraint = "unlimited",
                    keep_method = new {
                        heartbeat = new {
                            lifetime = movieSession.HeartbeatLifetime
                        }
                    },
                    protocol = new {
                        name = "http",
                        parameters = new {
                            http_parameters = new {
                                parameters = preferedProtocolHttp ? (object)new {
                                    http_output_download_parameters = new { }
                                } : preferedProtocolHls ? new {
                                    hls_parameters = (object)new {
                                        use_well_known_port = "yes",
                                        use_ssl = "yes",
                                        transfer_preset = movieSession.TransferPresets.First(),
                                        segment_duration = 6000
                                    }
                                } : throw new InvalidOperationException("プロトコルが不明です")
                            }
                        }
                    },
                    content_uri = "",
                    session_operation_auth = new {
                        session_operation_auth_by_signature = new {
                            token = movieSession.Token,
                            signature = movieSession.Signature
                        }
                    },
                    content_auth = new {
                        auth_type = preferedProtocolHttp ? movieSession.AuthTypesHttp :
                                            preferedProtocolHls ? movieSession.AuthTypesHls : throw new InvalidOperationException("authタイプが不明です"),
                        content_key_timeout = movieSession.ContentKeyTimeout,
                        service_id = "nicovideo",
                        service_user_id = movieSession.ServiceUserId
                    },
                    client_info = new {
                        player_id = movieSession.PlayerId
                    },
                    priority = movieSession.Priority
                }
            });

            var builder = new GetRequestQueryBuilder(apiUrl)
                .AddQuery("_format", "json");

            using var result = await SessionService.PostAsync(builder.Build(), payload).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            return new DmcSession {
                Id = data.session.id,
                VideoId = video,
                AudioId = audio,
                ContentUri = data.session.content_uri,
                Version = data.session.version,
                RawJson = data.ToString(),
                ApiUrl = apiUrl,
                CreatedTime = DateTimeOffset.FromUnixTimeMilliseconds((long)data.session.session_operation_auth.session_operation_auth_by_signature.created_time),
                ExpireTime = DateTimeOffset.FromUnixTimeMilliseconds((long)data.session.session_operation_auth.session_operation_auth_by_signature.expire_time)
            };
        }

        /// <inheritdoc />
        public async Task<DmcSession> HeartbeatAsync(DmcSession dmcSession) {

            if (dmcSession == null) {
                throw new ArgumentNullException(nameof(dmcSession));
            }

            var builder = new GetRequestQueryBuilder($"{dmcSession.ApiUrl}/{dmcSession.Id}")
                .AddQuery("_format", "json")
                .AddQuery("_method", "PUT");

            using var result = await SessionService.PostAsync(builder.Build(), dmcSession.RawJson!).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            return new DmcSession {
                Id = data.session.id,
                ContentUri = data.session.content_uri,
                Version = data.session.version,
                RawJson = data.ToString(),
                ApiUrl = dmcSession.ApiUrl
            };
        }

        /// <inheritdoc />
        public async Task<VideoStoryBoard> GetStoryBoardAsync(MediaSession sbSession) {

            if (sbSession == null) {
                throw new ArgumentNullException(nameof(sbSession));
            }
            if (sbSession.Urls.Count() == 0) {
                throw new ArgumentException("DMCのAPI URLがありません");
            }
            string apiUrl = sbSession.Urls.First().Url!;

            var builder = new GetRequestQueryBuilder(apiUrl)
                .AddQuery("_format", "json");

            // リクエストのjsonを組み立てる
            var payload = JsonObject.Serialize(new {
                session = new {
                    recipe_id = sbSession.RecipeId,
                    content_id = sbSession.ContentId,
                    content_type = "video",
                    content_src_id_sets = new[] {
                        new {
                            content_src_ids = sbSession.Videos
                        }
                    },
                    timing_constraint = "unlimited",
                    keep_method = new {
                        heartbeat = new {
                            lifetime = sbSession.HeartbeatLifetime
                        }
                    },
                    protocol = new {
                        name = "http",
                        parameters = new {
                            http_parameters = new {
                                parameters = new {
                                    storyboard_download_parameters = new {
                                        use_well_known_port = "yes",
                                        use_ssl = "yes"
                                    }
                                }
                            }
                        }
                    },
                    content_uri = "",
                    session_operation_auth = new {
                        session_operation_auth_by_signature = new {
                            token = sbSession.Token,
                            signature = sbSession.Signature
                        }
                    },
                    content_auth = new {
                        auth_type = sbSession.AuthTypesStoryBoard,
                        content_key_timeout = sbSession.ContentKeyTimeout,
                        service_id = "nicovideo",
                        service_user_id = sbSession.ServiceUserId
                    },
                    client_info = new {
                        player_id = sbSession.PlayerId
                    },
                    priority = sbSession.Priority
                }
            });

            using var result = await SessionService.PostAsync(builder.Build(), payload).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            using var metadataResult = await SessionService.GetAsync((string)json.data.session.content_uri).ConfigureAwait(false);
            if (!metadataResult.IsSuccessStatusCode) {

                throw new StatusErrorException(metadataResult.StatusCode);
            }
            json = JsonObject.Parse(await metadataResult.Content.ReadAsStringAsync().ConfigureAwait(false));
            var storyboard = json.data.storyboards[0];

            var ret = new VideoStoryBoard {
                Columns = (int)storyboard.columns,
                Interval = (int)storyboard.interval,
                Quality = (int)storyboard.quality,
                Rows = (int)storyboard.rows,
                ThumbnailHeight = (int)storyboard.thumbnail_height,
                ThumbnailWidth = (int)storyboard.thumbnail_width,
                BitmapMap = new Dictionary<int, Bitmap>()
            };

            // 画像1枚にrows * columns個分のストーリーボード画像が入っているので分解してそれぞれ一枚ずつの画像に切り出す
            int bitmapIndex = 0;
            foreach (var image in storyboard.images) {
                if (image == null) {
                    continue;
                }
                using var jpegResult = await SessionService.GetAsync((string)image.uri).ConfigureAwait(false);
                if (!jpegResult.IsSuccessStatusCode) {

                    throw new StatusErrorException(jpegResult.StatusCode);
                }
                var bitmap = new Bitmap(await jpegResult.Content.ReadAsStreamAsync().ConfigureAwait(false));

                for (int i = 0; i < ret.Columns; i++) {
                    for (int j = 0; j < ret.Rows; j++) {

                        ret.BitmapMap[bitmapIndex] = bitmap.Clone(new Rectangle(ret.ThumbnailWidth * j, ret.ThumbnailHeight * i, ret.ThumbnailWidth, ret.ThumbnailHeight), bitmap.PixelFormat);
                        bitmapIndex += ret.Interval;
                    }
                }
            }
            return ret;
        }

        /// <inheritdoc />
        public async Task DeleteSessionAsync(DmcSession dmcSession) {

            if (dmcSession == null) {
                throw new ArgumentNullException(nameof(dmcSession));
            }

            var builder = new GetRequestQueryBuilder($"{dmcSession.ApiUrl}/{dmcSession.Id}")
                .AddQuery("_format", "json")
                .AddQuery("_method", "DELETE");

            using var result = await SessionService.PostAsync(builder.Build(), dmcSession.RawJson!).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
        }

        /// <summary>
        /// 動画の長さに合わせて取得するコメントの量を調整する
        /// </summary>
        /// <param name="duration">動画の長さ</param>
        /// <returns></returns>
        private int GetCommentVolume(int duration) {

            if (duration < 60) {
                return 100;
            }
            if (duration < 300) {
                return 250;
            }
            if (duration < 600) {
                return 500;
            }
            return 1000;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VideoCommentThread>> GetCommentAsync(WatchApiDataComment comment) {

            if (comment == null) {
                throw new ArgumentNullException(nameof(comment));
            }

            var payloadList = new List<dynamic>();
            payloadList.Add(new { ping = new { content = "rs:0" } });

            int i = 0;
            var indexMap = new Dictionary<int, int>();
            foreach (var thread in comment.Threads!) {

                if (thread == null) {
                    continue;
                }
                // アクティブでないスレッドは取得しない
                if (!thread.IsActive) {
                    continue;
                }
                payloadList.Add(new { ping = new { content = $"ps:{i}" } });

                // 投稿者コメントの場合
                if (thread.IsOwnerThread) {

                    payloadList.Add(new {
                        thread = new {
                            fork = thread.Fork,
                            language = 0,
                            nicoru = 3,
                            scores = 1,
                            thread = thread.Id,
                            user_id = comment.UserId,
                            userkey = comment.UserKey,
                            with_global = 1,
                            version = "20061206",
                            res_from = -1000
                        }
                    });
                } else if (thread.IsDefaultPostTarget || thread.IsEasyCommentPostTarget) { // 通常コメント、かんたんコメントの場合

                    payloadList.Add(new {
                        thread = new {
                            fork = thread.Fork,
                            language = 0,
                            nicoru = 3,
                            scores = 1,
                            thread = thread.Id,
                            user_id = comment.UserId,
                            userkey = comment.UserKey,
                            with_global = 1,
                            version = "20090904"
                        }
                    });
                }

                indexMap[i] = thread.Fork;
                payloadList.Add(new { ping = new { content = $"rs:{i++}" } });

                if (thread.IsLeafRequired) {

                    payloadList.Add(new { ping = new { content = $"ps:{i}" } });

                    var leafNum = thread.IsEasyCommentPostTarget ? 25 : 100;
                    var resFrom = GetCommentVolume(comment.VideoDuration);
                    if (thread.IsEasyCommentPostTarget) {
                        resFrom = (int)Math.Floor(resFrom * 0.25);
                    }

                    payloadList.Add(new {
                        thread_leaves = new {
                            content = $"0-{(comment.VideoDuration / 60) + 1}:{leafNum},{resFrom},nicoru:100",
                            fork = thread.Fork,
                            language = 0,
                            nicoru = 3,
                            scores = 1,
                            thread = thread.Id,
                            user_id = comment.UserId,
                            userkey = comment.UserKey
                        }
                    });
                    indexMap[i] = thread.Fork;
                    payloadList.Add(new { ping = new { content = $"rs:{i++}" } });
                }
            }

            payloadList.Add(new { ping = new { content = "rf:0" } });

            using var result = await SessionService.PostAsync(comment.ServerUrl!.Replace("api/", "api.json"), JsonObject.Serialize(payloadList)).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            var ret = new List<VideoCommentThread>();

            var currentFork = -1;
            foreach (var item in json) {

                if (item == null) {
                    continue;
                }
                if (item.ping()) {

                    if (item.ping.content.StartsWith("ps")) {
                        currentFork = indexMap[int.Parse(item.ping.content.Split(':')[1])];
                    }
                    continue;
                }
                if (currentFork == -1) {
                    continue;
                }
                // 対象のスレッドクラスを取得する 無ければ作成する
                var target = ret.SingleOrDefault(s => s.Fork == currentFork);
                if (target == null) {

                    target = new VideoCommentThread {
                        Fork = currentFork,
                        Leaves = new List<VideoCommentLeaf>(),
                        Entries = new List<VideoCommentEntry>(),
                        Label = comment.Threads.Single(s => s.Fork == currentFork).Label
                    };
                    ret.Add(target);
                }

                if (item.thread()) {

                    var thread = item.thread;

                    target.ResultCode = (ThreadResultCode)thread.resultcode;
                    target.Id = thread.thread;
                    target.ServerTime = (long)thread.server_time;
                    target.LastRes = thread.last_res() ? (int)thread.last_res : 0;
                    target.Ticket = thread.ticket;
                    target.Revision = (int)thread.revision;
                    target.ClickRevision = thread.click_revision() ? (int?)thread.click_revision : null;
                    continue;
                }

                if (item.leaf()) {

                    var leaf = item.leaf;

                    target.Leaves!.Add(new VideoCommentLeaf {
                        Fork = leaf.fork() ? (int)leaf.fork : 0,
                        Count = (int)leaf.count,
                        Leaf = leaf.leaf() ? (int)leaf.leaf : 0,
                        ThreadId = leaf.thread
                    });
                    continue;
                }

                if (item.chat()) {

                    var chat = item.chat;

                    var datetime = DateTimeOffset.FromUnixTimeSeconds((long)chat.date);
                    if (chat.date_usec()) {

                        // 1tickは100ナノ秒らしいのでマイクロ秒に10を掛けたtickを追加する
                        datetime.Add(TimeSpan.FromTicks((long)(chat.date_usec * 10)));
                    }

                    target.Entries!.Add(new VideoCommentEntry {
                        ThreadId = chat.thread,
                        Fork = chat.fork() ? (int)chat.fork : 0,
                        Number = (int)chat.no,
                        Vpos = (int)chat.vpos,
                        Leaf = chat.leaf() ? (int)chat.leaf : 0,
                        Anonymity = chat.anonymity(),
                        DateTime = datetime,
                        Content = chat.content() ? chat.content : null,
                        Deleted = chat.deleted(),
                        Mail = chat.mail() ? chat.mail : null,
                        UserId = chat.user_id() ? chat.user_id : null,
                        Nicoru = chat.nicoru() ? (int)chat.nicoru : 0,
                        Premium = chat.premium(),
                        Score = chat.score() ? (int)chat.score : 0,
                        LastNicoruDate = chat.last_nicoru_date() ? chat.last_nicoru_date : null
                    });
                }
            }
            return ret;
        }

        /// <inheritdoc />
        public async Task<bool> SavePlaybackPositionAsync(string watchId, double position) {

            if (watchId == null) {
                throw new ArgumentNullException(nameof(watchId));
            }

            var formData = new Dictionary<string, string> {
                ["watchId"] = watchId,
                ["seconds"] = position.ToString()
            };

            using var result = await SessionService.PutAsync(PlaybackPositionApiUrl, formData, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode || result.StatusCode != HttpStatusCode.NotFound) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.meta.status == 200;
        }

        /// <inheritdoc />
        public async Task<string?> LikeAsync(string videoId) {

            if (string.IsNullOrEmpty(videoId)) {
                throw new ArgumentNullException(nameof(videoId));
            }

            var builder = new GetRequestQueryBuilder(LikeApiUrl)
                .AddQuery("videoId", videoId);

            using var result = await SessionService.PostAsync(builder.Build(), (IDictionary<string, string>?)null, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.data.thanksMessage;
        }

        /// <inheritdoc />
        public async Task<bool> UnlikeAsync(string videoId) {

            if (string.IsNullOrEmpty(videoId)) {
                throw new ArgumentNullException(nameof(videoId));
            }

            var builder = new GetRequestQueryBuilder(LikeApiUrl)
                .AddQuery("videoId", videoId);

            using var result = await SessionService.DeleteAsync(builder.Build(), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.meta.status == 200;
        }

        /// <inheritdoc />
        public async Task<ABRepeatPosition?> GetABRepeatPositionAsync(string videoId) {

            if (videoId == null) {
                throw new ArgumentNullException(nameof(videoId));
            }

            return await DbContext.ABRepeatPositions.SingleOrDefaultAsync(s => s.VideoId == videoId);
        }

        /// <inheritdoc />
        public async Task SaveABRepeatPositionAsync(string videoId, double repeatA, double repeatB) {

            if (videoId == null) {
                throw new ArgumentNullException(nameof(videoId));
            }
            if (repeatA > repeatB - 5) {
                throw new ArgumentException("A地点がB地点-5秒よりも高くなっています");
            }

            var pos = await DbContext.ABRepeatPositions.SingleOrDefaultAsync(s => s.VideoId == videoId).ConfigureAwait(false);
            if (pos == null) {

                pos = new ABRepeatPosition { VideoId = videoId };
                DbContext.ABRepeatPositions.Add(pos);
            }

            pos.RepeatA = repeatA;
            pos.RepeatB = repeatB;

            await DbContext.SaveChangesAsync();
        }
    }
}
