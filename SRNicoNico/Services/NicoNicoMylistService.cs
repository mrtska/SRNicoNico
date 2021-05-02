using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DynaJson;
using FastEnumUtility;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper.WatchLater;

namespace SRNicoNico.Services {
    /// <summary>
    /// あとで見るとマイリストの実装
    /// </summary>
    public class NicoNicoMylistService : IMylistService {

        private const string WatchLaterApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/watch-later";


        private readonly ISessionService SessionService;

        public NicoNicoMylistService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<WatchLaterList> GetWatchLaterAsync(MylistSortKey sortKey, int page) {

            var builder = new GetRequestQueryBuilder(WatchLaterApiUrl)
                .AddRawQuery(sortKey.GetLabel()!)
                .AddQuery("pageSize", 10)
                .AddQuery("page", page);

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var watchLater = json.data.watchLater;

            var ret = new WatchLaterList {
                HasInvisibleItems = watchLater.hasInvisibleItems,
                HasNext = watchLater.hasNext,
                TotalCount = (int)watchLater.totalCount
            };
            var entries = new List<WatchLaterEntry>();

            foreach (var entry in watchLater.items) {

                if (entry == null) {
                    continue;
                }
                var video = entry.video;
                entries.Add(new WatchLaterEntry {
                    AddedAt = DateTimeOffset.Parse(entry.addedAt),
                    ItemId = (int)entry.itemId,
                    Memo = entry.memo,
                    Status = entry.status,

                    ViewCount = (int)video.count.view,
                    CommentCount = (int)video.count.comment,
                    MylistCount = (int)video.count.mylist,
                    LikeCount = (int)video.count.like,

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
                    ThumbnailUrl = video.thumbnail.url,
                    Title = video.title,
                    Type =video.type,

                    WatchId = entry.watchId
                });
            }
            ret.Entries = entries;

            return ret;
        }
    }
}
