using System;
using System.Collections.Generic;
using System.Text;
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

        public NicoNicoHistoryService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<HistoryEntry>? GetAccountHistoryAsync() {

            var result = await SessionService.GetAsync(HistoryApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync());
            
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
                    ViewCount = (int) count.view,
                    CommentCount = (int)count.comment,
                    MylistCount = (int)count.mylist,
                    WatchCount = (int)entry.views,
                    Duration = (int)video.duration,
                    PlaybackPosition = (int)entry.playbackPosition
                };
            }
        }
    }
}
