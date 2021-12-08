﻿using System;
using System.Collections.Generic;
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
        /// <summary>
        /// お気に入り登録したタグ取得API
        /// </summary>
        private const string GetFavTagApiUrl = "https://www.nicovideo.jp/api/favtag/list";
        /// <summary>
        /// サジェスト取得API
        /// </summary>
        private const string GetTagSuggestionApiUrl = "https://sug.search.nicovideo.jp/suggestion/expand/";

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

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var ret = new SearchResult();
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            ret.SearchType = type;
            ret.TotalCount = (int)json.data.totalCount;
            ret.HasNext = json.data.hasNext;
            ret.Value = value;
            ret.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var items = new List<VideoListItem>();
            foreach (var video in json.data.items) {

                if (video == null) {
                    continue;
                }
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
            ret.Items = items;
            return ret;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetFavoriteTagsAsync() {

            var result = await SessionService.GetAsync(GetFavTagApiUrl).ConfigureAwait(false);
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

            var result = await SessionService.GetAsync(GetTagSuggestionApiUrl + tag).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return JsonObjectExtension.ToStringArray(json.candidates);
        }
    }
}
