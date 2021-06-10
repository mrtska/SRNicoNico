using System;
using System.Collections.Generic;
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
        private const string RankingSettingsApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/custom-ranking/settings";
        /// <summary>
        /// カスタムランキング取得API
        /// </summary>
        private const string RankingApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/custom-ranking/ranking/";

        private readonly ISessionService SessionService;

        public NicoNicoRankingService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<RankingDetails> GetCustomRankingAsync(int laneId) {

            if (laneId <= 0 || laneId > 5) {
                throw new ArgumentException("laneIdは1 から5の間のみ有効");
            }

            var result = await SessionService.GetAsync(RankingApiUrl + laneId, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

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

            return new RankingDetails {
                LaneId = (int)data.laneId,
                LaneType = data.laneType,
                Title = data.title,
                CustomType = data.customType,
                Genres = JsonObjectExtension.ToStringArray(data.genres),
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

            var result = await SessionService.GetAsync(RankingSettingsApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

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
    }
}
