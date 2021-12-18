using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynaJson;
using FastEnumUtility;
using Microsoft.EntityFrameworkCore;
using SRNicoNico.Entities;
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
        /// <summary>
        /// お気に入り登録したタグ取得API
        /// </summary>
        private const string GetFavTagApiUrl = "https://www.nicovideo.jp/api/favtag/list";
        /// <summary>
        /// サジェスト取得API
        /// </summary>
        private const string GetTagSuggestionApiUrl = "https://sug.search.nicovideo.jp/suggestion/expand/";

        private readonly ISessionService SessionService;
        private readonly IHistoryService HistoryService;
        private readonly IAccountService AccountService;
        private readonly ViewerDbContext DbContext;

        public NicoNicoSearchService(ISessionService sessionService, IHistoryService historyService, IAccountService accountService, ViewerDbContext dbContext) {

            SessionService = sessionService;
            HistoryService = historyService;
            AccountService = accountService;
            DbContext = dbContext;
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

            using var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var ret = new List<SearchGenreFacet>();
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            foreach (var item in json.data.items) {

                if (item == null) {
                    continue;
                }
                ret.Add(new SearchGenreFacet {
                    Count = (int)item.count,
                    Key = item.genre.key,
                    Label = item.genre.label,
                    Time = time
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

            using var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            
            var ret = new SearchResult {
                SearchType = type,
                TotalCount = (int)json.data.totalCount,
                HasNext = json.data.hasNext,
                Value = value,
                Time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            var items = new List<VideoItem>();
            foreach (var video in json.data.items) {

                if (video == null) {
                    continue;
                }
                var item = new VideoItem().Fill(video);
                item.IsMuted = AccountService.IsMuted(item);
                items.Add(item);
            }
            ret.Items = items;
            Parallel.ForEach(ret.Items, async item => item.HasWatched = await HistoryService.HasWatchedAsync(item.Id));
            return ret;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetFavoriteTagsAsync() {

            using var result = await SessionService.GetAsync(GetFavTagApiUrl).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            var ret = new List<string>();

            foreach (var item in json.favtag_items) {

                if (item == null) {
                    continue;
                }
                ret.Add(item.tag);
            }

            return ret;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetTagSuggestionAsync(string tag) {

            if (string.IsNullOrEmpty(tag)) {

                throw new ArgumentNullException(nameof(tag));
            }

            using var result = await SessionService.GetAsync(GetTagSuggestionApiUrl + tag).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return JsonObjectExtension.ToStringArray(json.candidates);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetSearchHistoriesAsync() {
            
            return await DbContext.SearchHistories.AsNoTracking().OrderBy(o => o.Order).Select(s => s.Query).ToListAsync();
        }

        /// <inheritdoc />
        public async Task SaveSearchHistoriesAsync(IEnumerable<string> searchHistories) {

            // 全て削除してから追加する
            DbContext.SearchHistories.RemoveRange(DbContext.SearchHistories);

            for (int i = 0; i < searchHistories.Count(); i++) {

                DbContext.SearchHistories.Add(new SearchHistory {
                    Query = searchHistories.ElementAt(i),
                    Order = i
                });
            }

            await DbContext.SaveChangesAsync();
        }
    }
}
