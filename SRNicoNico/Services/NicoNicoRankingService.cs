using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DynaJson;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// IRankingServiceの実装
    /// </summary>
    public class NicoNicoRankingService : IRankingService {
        /// <summary>
        /// カスタムランキング設定取得API
        /// </summary>
        private const string CustomRankingSettingsApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/custom-ranking/settings";
        /// <summary>
        /// カスタムランキング取得API
        /// </summary>
        private const string CustomRankingApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/custom-ranking/ranking/";
        /// <summary>
        /// ランキング取得API
        /// </summary>
        private const string RankingApiUrl = "https://nvapi.nicovideo.jp/v1/ranking";
        /// <summary>
        /// ジャンル取得API
        /// </summary>
        private const string GenreApiUrl = "https://nvapi.nicovideo.jp/v1/genres";
        /// <summary>
        /// 人気のタグ取得API
        /// </summary>
        private const string PopularTagApiUrl = "https://nvapi.nicovideo.jp/v1/genres/{0}/popular-tags";
        /// <summary>
        /// 話題のジャンルとタグ取得API
        /// </summary>
        private const string HotTopicsApiUrl = "https://nvapi.nicovideo.jp/v1/hot-topics";


        private readonly ISessionService SessionService;

        public NicoNicoRankingService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<RankingDetails> GetCustomRankingAsync(int laneId) {
            
            if (laneId <= 0 || laneId > 5) {
                throw new ArgumentException("laneIdは1から5の間のみ有効");
            }

            var result = await SessionService.GetAsync(CustomRankingApiUrl + laneId, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

              throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var videoList = new List<VideoListItem>();
            foreach (var video in data.videoList) {

                if (video == null) {
                    continue;
                }

                videoList.Add(new VideoListItem {
                    CommentCount = (int)video.count.comment,
                    LikeCount = (int)video.count.like,
                    MylistCount = (int)video.count.mylist,
                    ViewCount = (int)video.count.view,
                    Duration = (int)video.duration,
                    Id = video.id,
                    IsChannelVideo = video.isChannelVideo,
                    IsPaymentRequired = video.isPaymentRequired,
                    LatestCommentSummary = video.latestCommentSummary,
                    OwnerIconUrl = video.owner.iconUrl,
                    OwnerId = video.owner.id,
                    OwnerName = video.owner.name,
                    OwnerType = video.owner.ownerType,
                    PlaybackPosition = (int?)video.playbackPosition,
                    RegisteredAt = DateTimeOffset.Parse(video.registeredAt),
                    RequireSensitiveMasking = video.requireSensitiveMasking,
                    ShortDescription = video.shortDescription,
                    ThumbnailUrl = video.thumbnail.listingUrl,
                    Title = video.title
                });
            }

            var genreMap = new Dictionary<string, string>();
            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                genreMap[genre.key] = genre.label;
            }

            return new RankingDetails {
                LaneId = (int)data.laneId,
                LaneType = data.laneType,
                Title = data.title,
                CustomType = data.customType,
                Genres = genreMap,
                Tags = JsonObjectExtension.ToStringArray(data.tags),
                ChannelVideoListingStatus = data.channelVideoListingStatus,
                IsDefault = data.isDefault,
                DefaultTitle = data.defaultTitle,
                HasNext = data.hasNext,
                VideoList = videoList
            };
        }

        /// <inheritdoc />
        public async Task<RankingSettings> GetCustomRankingSettingsAsync() {

            using var result = await SessionService.GetAsync(CustomRankingSettingsApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var genreMap = new Dictionary<string, string>();
            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                genreMap[genre.key] = genre.label;
            }
            
            var settings = new List<RankingSettingsEntry>();
            foreach (var entry in data.settings) {

                if (entry == null) {
                    continue;
                }

                settings.Add(new RankingSettingsEntry {
                    LaneId = (int)entry.laneId,
                    Title = entry.title,
                    Type = entry.type,
                    IsDefault = entry.isDefault,
                    ChannelVideoListingStatus = entry.channelVideoListingStatus,
                    GenreKeys = JsonObjectExtension.ToStringArray(entry.genreKeys),
                    Tags = JsonObjectExtension.ToStringArray(entry.tags)
                });
            }

            return new RankingSettings {
                GenreMap = genreMap,
                Settings = settings
            };
        }

        /// <inheritdoc />
        public async Task<HotTopics> GetHotTopicsAsync() {

            using var result = await SessionService.GetAsync(HotTopicsApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var items = new List<HotTopicsItem>();

            foreach (var item in data.hotTopics) {

                if (item == null) {
                    continue;
                }
                items.Add(new HotTopicsItem {
                    Key = item.key,
                    Label = item.label,
                    Tag = item.conditions[0].tag,
                    GenreKey = item.conditions[0].genre.key,
                    GenreLabel = item.conditions[0].genre.label,
                });
            }

            return new HotTopics {
                StartAt = DateTimeOffset.Parse(data.startAt),
                Items = items
            };
        }

        /// <inheritdoc />
        public async Task<PopularTags?> GetPopularTagsAsync(string genreKey) {

            if (string.IsNullOrEmpty(genreKey)) {

                throw new ArgumentNullException(nameof(genreKey));
            }

            using var result = await SessionService.GetAsync(string.Format(PopularTagApiUrl, genreKey), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            // 404だった場合はnullを返す
            if (result.StatusCode == HttpStatusCode.NotFound) {

                return null;
            }
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;
            
            return new PopularTags {
                StartAt = DateTimeOffset.Parse(data.startAt),
                Tags = JsonObjectExtension.ToStringArray(data.tags)
            };
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> GetGenresAsync() {

            using var result = await SessionService.GetAsync(GenreApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var map = new Dictionary<string, string>();

            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                map[genre.key] = genre.label;
            }
            return map;
        }
    }
}
