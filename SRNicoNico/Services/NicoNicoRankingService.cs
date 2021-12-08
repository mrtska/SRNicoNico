using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DynaJson;
using FastEnumUtility;
using Microsoft.EntityFrameworkCore;
using SRNicoNico.Entities;
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
        /// カスタムランキング設定取得API
        /// </summary>
        private const string CustomRankingSettingApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/custom-ranking/setting/";
        /// <summary>
        /// カスタムランキング取得API
        /// </summary>
        private const string CustomRankingApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/custom-ranking/ranking/";
        /// <summary>
        /// ランキング取得API
        /// </summary>
        private const string RankingApiUrl = "https://nvapi.nicovideo.jp/v1/ranking/genre/";
        /// <summary>
        /// 話題ランキング取得API
        /// </summary>
        private const string HotTopicRankingApiUrl = "https://nvapi.nicovideo.jp/v1/ranking/hot-topic";
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
        private readonly ViewerDbContext DbContext;

        public NicoNicoRankingService(ISessionService sessionService, ViewerDbContext dbContext) {

            SessionService = sessionService;
            DbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<RankingDetails?> GetRankingAsync(RankingTerm term, string genre, string? popularTag = null, int page = 1) {

            if (string.IsNullOrEmpty(genre)) {

                throw new ArgumentNullException(nameof(genre));
            }

            var query = new GetRequestQueryBuilder(RankingApiUrl + genre)
                .AddQuery("term", term.GetLabel()!)
                .AddQuery("page", page);
            if (!string.IsNullOrEmpty(popularTag)) {
                query.AddQuery("tag", popularTag);
            }

            using var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            // genreかpupularTagが見つからなかった場合
            if (result.StatusCode == HttpStatusCode.NotFound) {
                return null;
            }
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var items = new List<RankingVideoItem>();
            var rank = 1 + ((page - 1) * 100);
            foreach (var video in data.items) {

                if (video == null) {
                    continue;
                }
                items.Add(new RankingVideoItem { Rank = rank++ }.Fill(video));
            }

            return new RankingDetails {
                HasNext = data.hasNext,
                VideoList = items
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
                Tags = JsonObjectExtension.ToStringArray(data.tags, "すべて")
            };
        }

        /// <inheritdoc />
        public async Task<CustomRankingDetails> GetCustomRankingAsync(int laneId) {
            
            if (laneId <= 0 || laneId >= 6) {
                throw new ArgumentException("laneIdは1から5の間のみ有効");
            }

            using var result = await SessionService.GetAsync(CustomRankingApiUrl + laneId, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

              throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var videoList = new List<RankingVideoItem>();
            var rank = 1;
            foreach (var video in data.videoList) {

                if (video == null) {
                    continue;
                }
                videoList.Add(new RankingVideoItem { Rank = rank++ }.Fill(video));
            }

            var genreMap = new Dictionary<string, string>();
            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                genreMap[genre.key] = genre.label;
            }

            return new CustomRankingDetails {
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

            // 特別扱いジャンルを追加する
            genreMap["all"] = "全ジャンル";
            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                genreMap[genre.key] = genre.label;
            }
            genreMap["r18"] = "R-18";

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
        public async Task<RankingSetting> GetCustomRankingSettingAsync(int laneId) {

            if (laneId <= 0 || laneId >= 6) {
                throw new ArgumentException("laneIdは1から5の間のみ有効");
            }

            using var result = await SessionService.GetAsync(CustomRankingSettingApiUrl + laneId, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;
            var entry = data.setting;

            var genreMap = new Dictionary<string, string>();
            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                genreMap[genre.key] = genre.label;
            }

            return new RankingSetting {
                GenreMap = genreMap,
                Setting = new RankingSettingsEntry {
                    LaneId = (int)entry.laneId,
                    Title = entry.title,
                    Type = entry.type,
                    IsAllGenre = entry.isAllGenre,
                    IsDefault = entry.isDefault,
                    ChannelVideoListingStatus = entry.channelVideoListingStatus,
                    GenreKeys = JsonObjectExtension.ToStringArray(entry.genreKeys),
                    Tags = JsonObjectExtension.ToStringArray(entry.tags)
                }
            };
        }

        /// <inheritdoc />
        public async Task<RankingSetting> ResetCustomRankingSettingAsync(int laneId) {

            if (laneId <= 0 || laneId >= 6) {
                throw new ArgumentException("laneIdは1から5の間のみ有効");
            }

            using var result = await SessionService.DeleteAsync(CustomRankingSettingApiUrl + laneId, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;
            var entry = data.setting;

            var genreMap = new Dictionary<string, string>();
            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                genreMap[genre.key] = genre.label;
            }

            return new RankingSetting {
                GenreMap = genreMap,
                Setting = new RankingSettingsEntry {
                    LaneId = (int)entry.laneId,
                    Title = entry.title,
                    Type = entry.type,
                    IsAllGenre = entry.isAllGenre,
                    IsDefault = entry.isDefault,
                    ChannelVideoListingStatus = entry.channelVideoListingStatus,
                    GenreKeys = JsonObjectExtension.ToStringArray(entry.genreKeys),
                    Tags = JsonObjectExtension.ToStringArray(entry.tags)
                }
            };
        }

        /// <inheritdoc />
        public async Task<RankingSetting> SaveCustomRankingSettingAsync(RankingSettingsEntry setting) {

            if (setting == null) {
                throw new ArgumentNullException(nameof(setting));
            }

            if (setting.LaneId <= 0 || setting.LaneId >= 6) {
                throw new ArgumentException("laneIdは1から5の間のみ有効");
            }

            var formData = new Dictionary<string, string> {
                ["title"] = setting.Title,
                ["type"] = setting.Type,
                ["isAllGenre"] = setting.IsAllGenre.ToString().ToLower(),
                ["genreKeys"] = setting.IsAllGenre ? string.Empty : string.Join(',', setting.GenreKeys),
                ["tags"] = string.Join(',', setting.Tags),
                ["channelVideoListingStatus"] = setting.ChannelVideoListingStatus,
            };

            using var result = await SessionService.PutAsync(CustomRankingSettingApiUrl + setting.LaneId, formData, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;
            var entry = data.setting;

            var genreMap = new Dictionary<string, string>();
            foreach (var genre in data.genres) {

                if (genre == null) {
                    continue;
                }
                genreMap[genre.key] = genre.label;
            }

            return new RankingSetting {
                GenreMap = genreMap,
                Setting = new RankingSettingsEntry {
                    LaneId = (int)entry.laneId,
                    Title = entry.title,
                    Type = entry.type,
                    IsDefault = entry.isDefault,
                    ChannelVideoListingStatus = entry.channelVideoListingStatus,
                    GenreKeys = JsonObjectExtension.ToStringArray(entry.genreKeys),
                    Tags = JsonObjectExtension.ToStringArray(entry.tags)
                }
            };
        }

        /// <inheritdoc />
        public async Task<RankingDetails?> GetHotTopicRankingAsync(RankingTerm term, string key, int page = 1) {

            if (string.IsNullOrEmpty(key)) {

                throw new ArgumentNullException(nameof(key));
            }

            var query = new GetRequestQueryBuilder(HotTopicRankingApiUrl)
                .AddQuery("term", term.GetLabel()!)
                .AddQuery("key", key)
                .AddQuery("page", page);

            using var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            // キーが存在しなかった場合
            if (result.StatusCode == HttpStatusCode.NotFound) {
                return null;
            }
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var items = new List<RankingVideoItem>();
            var rank = 1 + ((page - 1) * 100);
            foreach (var video in data.items) {

                if (video == null) {
                    continue;
                }
                items.Add(new RankingVideoItem { Rank = rank++ }.Fill(video));
            }

            return new RankingDetails {

                HasNext = data.hasNext,
                VideoList = items
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

            // allを先に入れておく
            items.Add(new HotTopicsItem {
                Key = "all",
                Label = "すべて",
                GenreKey = string.Empty,
                GenreLabel = string.Empty,
                Tag = string.Empty
            });

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

        /// <inheritdoc />
        public Task<Dictionary<string, bool>> GetRankingVisibilityAsync() {

            return DbContext.RankingVisibilities.AsNoTracking().ToDictionaryAsync(d => d.GenreKey, d => d.IsVisible);
        }

        /// <inheritdoc />
        public async Task SaveRankingVisibilityAsync(string genreKey, bool isVisible) {

            var setting = await DbContext.RankingVisibilities.SingleOrDefaultAsync(s => s.GenreKey == genreKey);
            if (setting == null) {

                setting = new RankingVisibility {
                    GenreKey = genreKey
                };
                DbContext.RankingVisibilities.Add(setting);
            }
            setting.IsVisible = isVisible;

            await DbContext.SaveChangesAsync();
        }
    }
}
