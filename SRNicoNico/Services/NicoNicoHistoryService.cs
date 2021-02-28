using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynaJson;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコニコの視聴履歴の処理の実装
    /// </summary>
    public class NicoNicoHistoryService : IHistoryService {

        /// <summary>
        /// 視聴履歴を取得するAPI
        /// </summary>
        private const string HistoryApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/watch/history";


        private readonly ISessionService SessionService;
        private readonly ViewerDbContext DbContext;

        public NicoNicoHistoryService(ISessionService sessionService, ViewerDbContext dbContext) {

            SessionService = sessionService;
            DbContext = dbContext;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<HistoryEntry> GetAccountHistoryAsync() {

            var result = await SessionService.GetAsync(HistoryApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            foreach (var entry in json.data.items) {

                if (entry == null) {
                    continue;
                }
                var video = entry.video;
                var count = video.count;
                yield return new HistoryEntry {

                    VideoId = video.id,
                    ShortDescription = video.shortDescription,
                    ThumbnailUrl = video.thumbnail.listingUrl,
                    Title = video.title,
                    PostedAt = DateTimeOffset.Parse(video.registeredAt),
                    WatchedAt = DateTimeOffset.Parse(entry.lastViewedAt),
                    ViewCount = (int)count.view,
                    CommentCount = (int)count.comment,
                    MylistCount = (int)count.mylist,
                    WatchCount = (int)entry.views,
                    Duration = (int)video.duration,
                    PlaybackPosition = (int)entry.playbackPosition
                };
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAccountHistoryAsync(string videoId) {

            var query = new GetRequestQueryBuilder(HistoryApiUrl)
                .AddQuery("target", videoId);

            var result = await SessionService.DeleteAsync(query.Build(), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            if (result.IsSuccessStatusCode) {

                var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = JsonObject.Parse(content);
                return json.meta.status == 200;
            }
            return false;
        }
    }
}
