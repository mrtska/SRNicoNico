using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynaJson;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ISeriesServiceの実装
    /// </summary>
    public class NicoNicoSeriesService : ISeriesService {
        /// <summary>
        /// ユーザーシリーズ取得API
        /// </summary>
        private const string UserSeriesApiUrl = "https://nvapi.nicovideo.jp/v1/users/{0}/series";
        /// <summary>
        /// シリーズ取得API
        /// </summary>
        private const string SeriesApiUrl = "https://nvapi.nicovideo.jp/v1/series/";



        private readonly ISessionService SessionService;

        public NicoNicoSeriesService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<SeriesList> GetUserSeriesAsync(string userId, int page = 1, int pageSize = 100) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var builder = new GetRequestQueryBuilder(string.Format(UserSeriesApiUrl, userId))
                .AddQuery("pageSize", pageSize)
                .AddQuery("page", page);

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var items = new List<SeriesListItem>();

            foreach (var item in data.items) {

                if (item == null) {
                    continue;
                }
                items.Add(new SeriesListItem {
                    Description = item.description,
                    Id = item.id.ToString(),
                    IsListed = item.isListed,
                    ItemsCount = (int)item.itemsCount,
                    OwnerId = item.owner.id,
                    OwnerType = item.owner.type,
                    ThumbnailUrl = item.thumbnailUrl,
                    Title = item.title
                });
            }

            return new SeriesList {
                Items = items,
                TotalCount = (int)data.totalCount
            };
        }

        /// <inheritdoc />
        public async Task<Series> GetSeriesAsync(string seriesId) {

            if (seriesId == null) {
                throw new ArgumentNullException(nameof(seriesId));
            }

            var result = await SessionService.GetAsync(SeriesApiUrl + seriesId, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;
            var detail = data.detail;

            var items = new List<VideoListItem>();

            foreach (var item in data.items) {

                if (item == null) {
                    continue;
                }
                var video = item.video;

                items.Add(new VideoListItem {
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

            return new Series {
                SeriesId = detail.id.ToString(),
                OwnerId = detail.owner.id,
                OwnerType = detail.owner.type,
                OwnerName = detail.owner.user.nickname,
                Title = detail.title,
                Description = detail.description,
                ThumbnailUrl = detail.thumbnailUrl,
                IsListed = detail.isListed,
                CreatedAt = DateTimeOffset.Parse(detail.createdAt),
                UpdatedAt = DateTimeOffset.Parse(detail.updatedAt),
                TotalCount = (int)data.totalCount,
                Items = items
            };
        }
    }
}
