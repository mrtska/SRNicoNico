using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynaJson;
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


        private readonly ISessionService SessionService;


        public NicoNicoVideoService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        
        /// <inheritdoc />
        public async Task<WatchApiData> WatchAsync(string videoId, bool withoutHistory = false) {

            if (string.IsNullOrEmpty(videoId)) {
                throw new ArgumentNullException(nameof(videoId));
            }

            var builder = new GetRequestQueryBuilder(WatchApiUrl + videoId)
                .AddQuery("_frontendId", 6)
                .AddQuery("_frontendVersion", 0)
                .AddQuery("actionTrackId", "1_1")
                .AddQuery("withoutHistory", withoutHistory)
                .AddQuery("additionals", withoutHistory ? "" : "series") // withoutHistoryがfalseの時はシリーズ情報も取得する
                .AddQuery("skips", "harmful");

            var result = await SessionService.GetAsync(builder.Build()).ConfigureAwait(false);
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
                        Bitrate = (int)audio.metadata.bitrate,
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
                        Bitrate = (int)video.metadata.bitrate,
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
            }

            return ret;
        }
    }
}
