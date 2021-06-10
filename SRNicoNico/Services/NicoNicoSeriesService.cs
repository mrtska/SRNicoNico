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
        /// シリーズ取得API
        /// </summary>
        private const string SeriesApiUrl = "https://nvapi.nicovideo.jp/v1/users/{0}/series";

        private readonly ISessionService SessionService;

        public NicoNicoSeriesService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<SeriesList> GetUserSeriesAsync(string userId, int page = 1, int pageSize = 100) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var builder = new GetRequestQueryBuilder(string.Format(SeriesApiUrl, userId))
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
    }
}
