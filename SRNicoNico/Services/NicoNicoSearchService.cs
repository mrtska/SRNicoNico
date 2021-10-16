using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DynaJson;
using FastEnumUtility;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ISearchServiceの実装
    /// </summary>
    public class NicoNicoSearchService : ISearchService {
        /// <summary>
        /// 検索API
        /// </summary>
        private const string VideoSearchApiUrl = "https://nvapi.nicovideo.jp/v1/search/video";
        /// <summary>
        /// ジャンル情報取得API
        /// </summary>
        private const string GetGenreFacetApiUrl = "https://nvapi.nicovideo.jp/v1/search/facet";

        private readonly ISessionService SessionService;

        public NicoNicoSearchService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SearchGenreFacet>> GetGenreFacetAsync(SearchType type, string value) {

            if (string.IsNullOrEmpty(value)) {

                throw new ArgumentNullException(nameof(value));
            }

            var builder = new GetRequestQueryBuilder(GetGenreFacetApiUrl);
            if (type == SearchType.Keyword) {

                builder.AddQuery("keyword", value);
            } else {

                builder.AddQuery("tag", value);
            }

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var ret = new List<SearchGenreFacet>();
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            foreach (var item in json.data.items) {

                if (item == null) {
                    continue;
                }
                ret.Add(new SearchGenreFacet {
                    Count = (int)item.count,
                    Key = item.genre.key,
                    Label = item.genre.label
                });
            }
            return ret;
        }

        /// <inheritdoc />
        public async Task<SearchResult> SearchAsync(SearchSortKey sortKey, SearchType type, string value, int page = 1, string? genre = null, int pageSize = 30) {

            if (string.IsNullOrEmpty(value)) {

                throw new ArgumentNullException(nameof(value));
            }

            var builder = new GetRequestQueryBuilder(VideoSearchApiUrl)
                .AddQuery("pageSize", pageSize)
                .AddQuery("page", page)
                .AddRawQuery(sortKey.GetLabel()!);

            if (!string.IsNullOrEmpty(genre)) {

                builder.AddQuery("genres", genre);
            }
            if (type == SearchType.Keyword) {

                builder.AddQuery("keyword", value);
            } else {

                builder.AddQuery("tag", value);
            }

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var ret = new SearchResult();
            var json = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            ;

            return ret;
        }
    }
}
