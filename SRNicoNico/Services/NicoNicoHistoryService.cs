﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynaJson;
using Microsoft.EntityFrameworkCore;
using SRNicoNico.Entities;
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
        private readonly ViewerDbContext ReadOnlyDbContext;

        public NicoNicoHistoryService(ISessionService sessionService, ViewerDbContext dbContext) {

            SessionService = sessionService;
            ReadOnlyDbContext = dbContext;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<HistoryEntry> GetAccountHistoryAsync() {

            var query = new GetRequestQueryBuilder(HistoryApiUrl)
                .AddQuery("pageSize", 200)
                .AddQuery("page", 1);

            var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

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

        /// <inheritdoc />
        public IAsyncEnumerable<LocalHistory> GetLocalHistoryAsync() {

            return ReadOnlyDbContext.LocalHistories.AsNoTracking().OrderByDescending(o => o.LastWatchedAt).AsAsyncEnumerable();
        }

        /// <inheritdoc />
        public async Task<bool> DeleteLocalHistoryAsync(string videoId) {

            using var dbContext = new ViewerDbContext();
            var history = await dbContext.LocalHistories.SingleOrDefaultAsync(s => s.VideoId == videoId).ConfigureAwait(false);
            // 履歴がDBに存在しなかったらそのまま終了する
            if (history == null) {

                return false;
            }
            dbContext.LocalHistories.Remove(history);

            await dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> SyncLocalHistoryAsync(IEnumerable<HistoryEntry> histories) {

            using var dbContext = new ViewerDbContext();
            var dic = await dbContext.LocalHistories.AsNoTracking().ToDictionaryAsync(t => t.VideoId).ConfigureAwait(false);

            foreach (var history in histories) {

                if (dic.ContainsKey(history.VideoId!)) {

                    dbContext.LocalHistories.Update(new LocalHistory {

                        VideoId = history.VideoId!,
                        Title = history.Title!,
                        ShortDescription = history.ShortDescription!,
                        ThumbnailUrl = history.ThumbnailUrl!,
                        Duration = history.Duration,
                        WatchCount = history.WatchCount,
                        PostedAt = history.PostedAt,
                        LastWatchedAt = history.WatchedAt
                    });
                } else {

                    dbContext.LocalHistories.Add(new LocalHistory {

                        VideoId = history.VideoId!,
                        Title = history.Title!,
                        ShortDescription = history.ShortDescription!,
                        ThumbnailUrl = history.ThumbnailUrl!,
                        Duration = history.Duration,
                        WatchCount = history.WatchCount,
                        PostedAt = history.PostedAt,
                        LastWatchedAt = history.WatchedAt
                    });
                }
            }
            await dbContext.SaveChangesAsync();

            return false;
        }

        /// <inheritdoc />
        public Task<bool> HasWatchedAsync(string videoId) {

            return ReadOnlyDbContext.LocalHistories.AsNoTracking().AnyAsync(a => a.VideoId == videoId);
        }
    }
}
