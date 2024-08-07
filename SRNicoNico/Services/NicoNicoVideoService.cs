﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
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
        /// <summary>
        /// 2ab0cbaaとは
        /// </summary>
        private const string TrackingApiUrl = "https://nvapi.nicovideo.jp/v1/2ab0cbaa/watch";
        /// <summary>
        /// ユーザーキーAPI
        /// </summary>
        private const string GetUserKeyApiUrl = "https://flapi.nicovideo.jp/api/getuserkey";
        /// <summary>
        /// スレッドキーAPI
        /// </summary>
        private const string GetThreadKeyApiUrl = "https://flapi.nicovideo.jp/api/getthreadkey";
        /// <summary>
        /// ポストキーAPI
        /// </summary>
        private const string GetPostKeyApiUrl = "https://flapi.nicovideo.jp/api/getpostkey";

        private const string NvCommentEastPostKeyApiUrl = "https://nvapi.nicovideo.jp/v1/comment/keys/post-easy";
        private const string NvCommentPostKeyApiUrl = "https://nvapi.nicovideo.jp/v1/comment/keys/post";

        /// <summary>
        /// コメントAPIベース
        /// </summary>
        private const string CommentApiUrl = "https://nv-comment.nicovideo.jp/v1/threads/";

        /// <summary>
        /// Dowango Media Service API
        /// </summary>
        private const string AccessRightsUrl = "https://nvapi.nicovideo.jp/v1/watch/{videoId}/access-rights/";

        private readonly ISessionService SessionService;
        private readonly ISettings Settings;
        private readonly ViewerDbContext DbContext;

        public NicoNicoVideoService(ISessionService sessionService, ISettings settings, ViewerDbContext dbContext) {

            SessionService = sessionService;
            Settings = settings;
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
                            Fork = (int)threadId.fork,
                            ForkLabel = threadId.forkLabel,
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
                        ForkLabel = thread.forkLabel,
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
                    ServerUrl = comment.nvComment.server + "/v1/threads",
                    ThreadKey = comment.nvComment.threadKey,
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
                if (media.domand != null) {
                    var accessRightsBaseUrl = AccessRightsUrl.Replace("{videoId}", videoId);

                    var audios = new List<MediaMovieAudio>();
                    foreach (var audio in media.domand.audios) {

                        if (audio == null) {
                            continue;
                        }

                        var collections = new Dictionary<string, double>();
                        foreach (var lc in audio.loudnessCollection) {

                            if (lc == null) {
                                continue;
                            }
                            collections[lc.type] = lc.value;
                        }

                        audios.Add(new MediaMovieAudio {
                            Id = audio.id,
                            IsAvailable = audio.isAvailable,
                            Bitrate = (int)audio.bitRate / 1000,
                            SamplingRate = (int)audio.samplingRate,
                            IntegratedLoudness = (int)audio.integratedLoudness,
                            TruePeak = (int)audio.truePeak,
                            LoudnessCollection = collections
                        });
                    }

                    var videos = new List<MediaMovieVideo>();
                    foreach (var video in media.domand.videos) {

                        if (video == null) {
                            continue;
                        }

                        videos.Add(new MediaMovieVideo {
                            Id = video.id,
                            IsAvailable = video.isAvailable,
                            Label = video.label,
                            Bitrate = (int)video.bitRate / 1000,
                            Width = (int)video.width,
                            Height = (int)video.height,
                            RecommendedHighestAudioQualityLevel = (int)video.recommendedHighestAudioQualityLevel
                        });
                    }

                    ret.Media = new WatchApiDataMedia {
                        Movie = new MediaMovie {
                            ApiUrl = accessRightsBaseUrl + "hls?actionTrackId=1_1",
                            Audios = audios,
                            Videos = videos,
                            AccessRightKey = media.domand.accessRightKey,
                        },
                        StoryBoard = media.domand.isStoryboardAvailable ? new MediaStoryBoard {
                            ApiUrl = accessRightsBaseUrl + "storyboard?actionTrackId=1_1",
                            AccessRightKey = media.domand.accessRightKey,
                        } : null,
                    };
                } else if (media.delivery != null) {

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
                            RecommendedHighestAudioQualityLevel = (int)video.metadata.recommendedHighestAudioLevelIndex
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
                    if (media.delivery.storyboard != null) {
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
                        Encryption = media.delivery.encryption != null ? new MediaEncryption {
                            EncryptedKey = media.delivery.encryption.encryptedKey,
                            KeyUri = media.delivery.encryption.keyUri
                        } : null,
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
                                AuthTypesHttp = media.delivery.movie.session.authTypes.http() ? media.delivery.movie.session.authTypes.http : null,
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
                        StoryBoard = media.delivery.storyboard == null ? null : new MediaStoryBoard {
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

            // チャンネルデータ
            {
                var channel = data.channel;
                if (channel != null) {

                    var follow = channel.viewer.follow;
                    ret.Channel = new WatchApiDataChannel {
                        Id = channel.id.ToString(),
                        Name = channel.name,
                        ThumbnailUrl = channel.thumbnail.url,
                        IsDisplayAdBanner = channel.isDisplayAdBanner,
                        IsOfficialAnime = channel.isOfficialAnime,
                        IsBookmarked = follow.isBookmarked,
                        IsFollowed = follow.isFollowed,
                        Token = follow.token,
                        TokenTimestamp = follow.tokenTimestamp
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
                var description = (string)video.description;
                var replaced = HttpUtility.HtmlDecode(((string)video.description).Replace("<br>", " "));
                ret.Video = new WatchApiDataVideo {
                    Id = video.id,
                    Title = video.title,
                    Description = description,
                    ShortDescription = replaced.Substring(0, Math.Min(replaced.Length, 50)),
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
        public async Task<bool> SendTrackingAsync(string trackingId) {

            if (string.IsNullOrEmpty(trackingId)) {
                throw new ArgumentNullException(nameof(trackingId));
            }

            var builder = new GetRequestQueryBuilder(TrackingApiUrl)
                .AddQuery("t", trackingId);

            using var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.meta.status == 200;
        }

        /// <inheritdoc />
        public async Task<DmcSession> CreateSessionAsync(MediaMovie movie, MediaEncryption? encryption = null, string? videoId = null, string? audioId = null) {

            if (movie == null) {
                throw new ArgumentNullException(nameof(movie));
            }
            if (!movie.Videos.Any()) {
                throw new ArgumentException("動画データがありません");
            }
            if (!movie.Audios.Any()) {
                throw new ArgumentException("音声データがありません");
            }

            if (movie.AccessRightKey != null) {
                var domandHeaders = new Dictionary<string, string>(NicoNicoSessionService.AjaxApiHeaders) {
                    ["X-Access-Right-Key"] = movie.AccessRightKey,
                };

                var actualVideoId = videoId ?? movie.Videos.First().Id;
                var actualAudioId = audioId ?? movie.Audios.First().Id;

                // リクエストのjsonを組み立てる
                var payload = JsonObject.Serialize(new {
                    outputs = new[] { new[] { actualVideoId, actualAudioId } }
                });

                using var result = await SessionService.PostAsync(movie.ApiUrl, payload, domandHeaders).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode) {
                    throw new StatusErrorException(result.StatusCode);
                }

                var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
                var data = json.data;

                var setCookie = result.Headers.GetValues("set-cookie").FirstOrDefault();
                Cookie? cookie = null;

                if (setCookie != null) {
                    cookie = new Cookie();
                    var parts = setCookie.Split(';').Select(s => {
                        var pair = s.Split('=');
                        return new KeyValuePair<string, string?>(pair[0].Trim(), pair.Length == 1 ? null : pair[1]);
                    });
                    cookie.Name = parts.First().Key;
                    cookie.Value = parts.First().Value;
                    cookie.Expires = DateTime.Parse(parts.First(p => p.Key == "expires").Value!);
                    cookie.Path = parts.First(p => p.Key == "path").Value;
                    cookie.Domain = ".nicovideo.jp";
                    cookie.Secure = true;
                    cookie.HttpOnly = true;
                }

                return new DmcSession {
                    VideoId = actualVideoId,
                    AudioId = actualAudioId,
                    ContentUri = data.contentUrl,
                    RawJson = data.ToString(),
                    CreatedTime = DateTimeOffset.Parse(data.createTime),
                    ExpireTime = DateTimeOffset.Parse(data.expireTime),
                    DmsCookie = cookie,
                };
            } else {
                var movieSession = movie.Session!;
                var apiUrl = movieSession.Urls.First().Url!;

                var video = videoId ?? movieSession.Videos.First();
                var audio = audioId ?? movieSession.Audios.First();

                var preferedProtocolHttp = movieSession.Protocols.Contains("http");
                var preferedProtocolHls = movieSession.Protocols.Contains("hls");

                object hlsParameter;
                if (encryption != null) {
                    hlsParameter = new {
                        use_well_known_port = "yes",
                        use_ssl = "yes",
                        transfer_preset = movieSession.TransferPresets.FirstOrDefault(""),
                        segment_duration = 6000,
                        encryption = new {
                            hls_encryption_v1 = new {
                                encrypted_key = encryption.EncryptedKey,
                                key_uri = encryption.KeyUri
                            }
                        },
                    };
                } else {
                    hlsParameter = new {
                        use_well_known_port = "yes",
                        use_ssl = "yes",
                        transfer_preset = movieSession.TransferPresets.FirstOrDefault(""),
                        segment_duration = 6000
                    };
                }

                // HLSが無効でない場合はHTTPを無効にする
                if (!Settings.DisableHls) {
                    preferedProtocolHttp = false;
                }

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
                                        http_output_download_parameters = new {
                                            use_well_known_port = "yes",
                                            use_ssl = "yes",
                                            transfer_preset = movieSession.TransferPresets.FirstOrDefault("")
                                        }
                                    } : preferedProtocolHls ? new {
                                        hls_parameters = hlsParameter
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
        public async Task<VideoStoryBoard> GetStoryBoardAsync(MediaStoryBoard storyBoard) {

            if (storyBoard == null) {
                throw new ArgumentNullException(nameof(storyBoard));
            }

            if (storyBoard.AccessRightKey != null) {
                var domandHeaders = new Dictionary<string, string>(NicoNicoSessionService.AjaxApiHeaders) {
                    ["X-Access-Right-Key"] = storyBoard.AccessRightKey,
                };
                using var result = await SessionService.PostAsync(storyBoard.ApiUrl, (IDictionary<string, string>?)null, domandHeaders).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode) {
                    throw new StatusErrorException(result.StatusCode);
                }
                var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

                var assetsUrl = (string)json.data.contentUrl;
                using var assetsResult = await SessionService.GetAsync(assetsUrl).ConfigureAwait(false);
                if (!assetsResult.IsSuccessStatusCode) {
                    throw new StatusErrorException(assetsResult.StatusCode);
                }

                var assetsJson = JsonObject.Parse(await assetsResult.Content.ReadAsStringAsync().ConfigureAwait(false));
                var data = assetsJson;

                var ret = new VideoStoryBoard {
                    Columns = (int)data.columns,
                    Interval = (int)data.interval,
                    Rows = (int)data.rows,
                    ThumbnailHeight = (int)data.thumbnailHeight,
                    ThumbnailWidth = (int)data.thumbnailWidth,
                    BitmapMap = new Dictionary<int, Bitmap>()
                };

                // 画像1枚にrows * columns個分のストーリーボード画像が入っているので分解してそれぞれ一枚ずつの画像に切り出す
                int bitmapIndex = 0;
                foreach (var image in data.images) {
                    if (image == null) {
                        continue;
                    }

                    var url = assetsUrl.Replace("storyboard.json", image.url);
                    using var jpegResult = await SessionService.GetAsync(url).ConfigureAwait(false);
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
            } else {
                var sbSession = storyBoard.Session!;
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
        }

        /// <inheritdoc />
        public async Task DeleteSessionAsync(DmcSession dmcSession) {

            if (dmcSession == null) {
                throw new ArgumentNullException(nameof(dmcSession));
            }
            // 新サーバでは特に何もしない
            if (dmcSession.DmsCookie != null) {
                return;
            }

            var builder = new GetRequestQueryBuilder($"{dmcSession.ApiUrl}/{dmcSession.Id}")
                .AddQuery("_format", "json")
                .AddQuery("_method", "DELETE");

            using var result = await SessionService.PostAsync(builder.Build(), dmcSession.RawJson!).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VideoCommentThread>> GetCommentAsync(WatchApiDataComment comment) {

            if (comment == null) {
                throw new ArgumentNullException(nameof(comment));
            }

            var payload = new {
                @params = new {
                    language = "ja-jp",
                    targets = comment.Threads?.Select(s => new { id = s.Id, fork = s.ForkLabel }),
                },
                threadKey = comment.ThreadKey,
                additionals = new { }
            };

            using var result = await SessionService.PostAsync(comment.ServerUrl!, JsonObject.Serialize(payload), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {
                var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            var ret = new List<VideoCommentThread>();

            foreach (var thread in json.data.threads) {
                var comments = new List<VideoCommentEntry>();

                foreach (var item in thread.comments) {
                    var commands = (string[]) item.commands;
                    comments.Add(new VideoCommentEntry {
                        Id = item.id,
                        Number = (int)item.no,
                        Vpos = ((int)item.vposMs) / 10,
                        Content = item.body,
                        Mail = string.Join(" ", commands),
                        UserId = item.userId,
                        Premium = item.isPremium,
                        Score = (int)item.score,
                        Nicoru = (int)item.nicoruCount,
                        DateTime = DateTimeOffset.Parse(item.postedAt),
                        Fork = thread.fork,
                    });
                }

                var data = new VideoCommentThread {
                    Id = thread.id.ToString(),
                    Entries = comments,
                    Label = thread.fork,
                    ForkLabel = thread.fork,
                };
                ret.Add(data);
            }

            return ret;
        }

        /// <inheritdoc />
        public async Task<int?> PostEasyCommentAsync(string videoId, EasyCommentPhrase phrase, string threadId, int vpos) {

            var builder = new GetRequestQueryBuilder(NvCommentEastPostKeyApiUrl)
                .AddQuery("threadId", threadId);

            using var postKeyResult = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            var json = JsonObject.Parse(await postKeyResult.Content.ReadAsStringAsync().ConfigureAwait(false));
            var postKey = json.data.postEasyKey;

            var payload = new {
                body = phrase.Text,
                postEasyKey = postKey,
                videoId = videoId,
                vposMs = vpos * 10,
            };

            var api = CommentApiUrl + threadId + "/easy-comments";

            using var result = await SessionService.PostAsync(api, JsonObject.Serialize(payload), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return (int) json.data.no;
        }

        /// <inheritdoc />
        public Task PostCommentAsync(string comment, string threadId, string ticket, int vpos) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<string> GetPostKeyAsync(string threadId, int blockNo) {

            var builder = new GetRequestQueryBuilder(GetPostKeyApiUrl)
                .AddQuery("thread", threadId)
                .AddQuery("block_no", blockNo)
                .AddQuery("device", 1)
                .AddQuery("version", 1)
                .AddQuery("version_sub", 6);

            using var result = await SessionService.GetAsync(builder.Build()).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var key = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            return key[8..];
        }

        /// <inheritdoc />
        public async Task<string> GetUserKeyAsync() {

            using var result = await SessionService.GetAsync(GetUserKeyApiUrl).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var key = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            return key[8..];
        }

        /// <inheritdoc />
        public async Task<string?> GetThreadKeyAsync(string threadId) {

            var builder = new GetRequestQueryBuilder(GetThreadKeyApiUrl)
                .AddQuery("thread", threadId);

            using var result = await SessionService.GetAsync(builder.Build()).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var key = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(key.Trim())) {
                return null;
            }
            return key[10..];
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
            if (!result.IsSuccessStatusCode && result.StatusCode != HttpStatusCode.NotFound) {

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
