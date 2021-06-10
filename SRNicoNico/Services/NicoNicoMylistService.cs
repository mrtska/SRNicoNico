using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DynaJson;
using FastEnumUtility;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// あとで見るとマイリストの実装
    /// </summary>
    public class NicoNicoMylistService : IMylistService {
        /// <summary>
        /// あとで見る取得API
        /// </summary>
        private const string WatchLaterApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/watch-later";
        /// <summary>
        /// マイリスト取得API
        /// </summary>
        private const string MylistApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/mylists";
        /// <summary>
        /// 公開マイリスト取得API
        /// </summary>
        private const string PublicMylistApiUrl = "https://nvapi.nicovideo.jp/v1/users/{0}/mylists";

        private readonly ISessionService SessionService;

        public NicoNicoMylistService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<WatchLaterList> GetWatchLaterAsync(MylistSortKey sortKey, int page) {

            var builder = new GetRequestQueryBuilder(WatchLaterApiUrl)
                .AddRawQuery(sortKey.GetLabel()!)
                .AddQuery("pageSize", 100)
                .AddQuery("page", page);

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

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
            var entries = new List<MylistEntry>();

            foreach (var entry in watchLater.items) {

                if (entry == null) {
                    continue;
                }
                var video = entry.video;
                entries.Add(new MylistEntry {
                    AddedAt = DateTimeOffset.Parse(entry.addedAt),
                    ItemId = entry.itemId.ToString(),
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
                    ThumbnailUrl = video.thumbnail.listingUrl,
                    Title = video.title,
                    Type =video.type,

                    WatchId = entry.watchId,
                    VideoUrl = $"https://www.nicovideo.jp/watch/{video.id}"
                });
            }
            ret.Entries = entries;

            return ret;
        }

        /// <inheritdoc />
        public async Task<bool> AddWatchLaterAsync(string watchId, string? memo) {

            if (string.IsNullOrEmpty(watchId)) {

                throw new ArgumentNullException(nameof(watchId));
            }

            var formData = new Dictionary<string, string> {
                ["watchId"] = watchId,
                ["memo"] = memo ?? string.Empty
            };
            var result = await SessionService.PostAsync(WatchLaterApiUrl, formData, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            // 200番台か409以外だったらエラーとする
            if (!result.IsSuccessStatusCode && result.StatusCode != HttpStatusCode.Conflict) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            // 201なら成功
            return json.meta.status == 201;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteWatchLaterAsync(params string[] itemIds) {

            var builder = new GetRequestQueryBuilder(WatchLaterApiUrl)
                .AddQuery("itemIds", string.Join(',', itemIds.Distinct()));

            var result = await SessionService.DeleteAsync(builder.Build(), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            // 200番台か404以外だったらエラーとする
            if (!result.IsSuccessStatusCode && result.StatusCode != HttpStatusCode.NotFound) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            // 200なら成功
            return json.meta.status == 200;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<MylistListEntry> GetMylistsAsync(int sampleItemCount = 0) {

            var builder = new GetRequestQueryBuilder(MylistApiUrl)
                .AddQuery("sampleItemCount", sampleItemCount);

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            foreach (var mylist in json.data.mylists) {

                if (mylist == null) {
                    continue;
                }
                var sampleItems = new List<MylistEntry>();
                foreach (var sampleItem in mylist.sampleItems) {

                    if (sampleItem == null) {
                        continue;
                    }
                    var video = sampleItem.video;
                    sampleItems.Add(new MylistEntry {
                        AddedAt = DateTimeOffset.Parse(sampleItem.addedAt),
                        ItemId = sampleItem.itemId.ToString(),
                        Memo = sampleItem.description, // あとで見るはmemoだけどマイリストはdescription
                        Status = sampleItem.status,

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
                        ThumbnailUrl = video.thumbnail.listingUrl,
                        Title = video.title,
                        Type = video.type,

                        WatchId = sampleItem.watchId,
                        VideoUrl = $"https://www.nicovideo.jp/watch/{video.id}"
                    });
                }
                yield return new MylistListEntry {
                    CreatedAt = DateTimeOffset.Parse(mylist.createdAt),
                    DefaultSortKey = mylist.defaultSortKey,
                    DefaultSortOrder = mylist.defaultSortOrder,
                    Description = mylist.description,
                    FollowerCount = (int)mylist.followerCount,
                    Id = mylist.id.ToString(),
                    IsFollowing = mylist.isFollowing,
                    IsPublic = mylist.isPublic,
                    ItemsCount = (int)mylist.itemsCount,
                    Name = mylist.name,
                    OwnerIconUrl = mylist.owner.iconUrl,
                    OwnerId = mylist.owner.id,
                    OwnerName = mylist.owner.name,
                    OwnerType = mylist.owner.ownerType,
                    SampleItems = sampleItems
                };
            }
        }

        /// <inheritdoc />
        public async Task<Mylist> GetMylistAsync(string mylistId, MylistSortKey sortKey, int page) {

            if (string.IsNullOrEmpty(mylistId)) {
                throw new ArgumentNullException(nameof(mylistId));
            }

            var builder = new GetRequestQueryBuilder($"{MylistApiUrl}/{mylistId}")
                .AddRawQuery(sortKey.GetLabel()!)
                .AddQuery("pageSize", 100)
                .AddQuery("page", page);

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var mylist = json.data.mylist;


            var items = new List<MylistEntry>();
            foreach (var item in mylist.items) {

                if (item == null) {
                    continue;
                }
                var video = item.video;
                items.Add(new MylistEntry {
                    AddedAt = DateTimeOffset.Parse(item.addedAt),
                    ItemId = item.itemId.ToString(),
                    Memo = item.description, // あとで見るはmemoだけどマイリストはdescription
                    Status = item.status,

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
                    ThumbnailUrl = video.thumbnail.listingUrl,
                    Title = video.title,
                    Type = video.type,

                    WatchId = item.watchId,
                    VideoUrl = $"https://www.nicovideo.jp/watch/{video.id}"
                });
            }
            return new Mylist {
                DefaultSortKey = mylist.defaultSortKey,
                DefaultSortOrder = mylist.defaultSortOrder,
                Description = mylist.description,
                FollowerCount = (int)mylist.followerCount,
                HasInvisibleItems = mylist.hasInvisibleItems,
                HasNext = mylist.hasNext,
                Id = mylist.id.ToString(),
                IsFollowing = mylist.isFollowing,
                IsPublic = mylist.isPublic,
                Name = mylist.name,
                OwnerIconUrl = mylist.owner.iconUrl,
                OwnerId = mylist.owner.id,
                OwnerName = mylist.owner.name,
                OwnerType = mylist.owner.ownerType,
                TotalItemCount = (int)mylist.totalItemCount,
                Entries = items
            };
        }

        /// <inheritdoc />
        public async Task<bool> AddMylistItemAsync(string mylistId, string itemId, string? memo) {

            if (string.IsNullOrEmpty(mylistId)) {

                throw new ArgumentNullException(nameof(mylistId));
            }
            if (string.IsNullOrEmpty(itemId)) {

                throw new ArgumentNullException(nameof(itemId));
            }

            var builder = new GetRequestQueryBuilder($"{MylistApiUrl}/{mylistId}/items")
                .AddQuery("itemId", itemId)
                .AddQuery("description", memo ?? string.Empty);
            // (IDictionary<string, string>?)null はちょっとダサいのでどうにかしたい
            var result = await SessionService.PostAsync(builder.Build(), (IDictionary<string, string>?)null, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            // 200番台か409以外だったらエラーとする
            if (!result.IsSuccessStatusCode && result.StatusCode != HttpStatusCode.Conflict) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            // 201なら成功
            return json.meta.status == 201;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteMylistItemAsync(string mylistId, params string[] itemIds) {

            if (string.IsNullOrEmpty(mylistId)) {

                throw new ArgumentNullException(nameof(mylistId));
            }

            var builder = new GetRequestQueryBuilder($"{MylistApiUrl}/{mylistId}/items")
                .AddQuery("itemIds", string.Join(',', itemIds.Distinct()));

            var result = await SessionService.DeleteAsync(builder.Build(), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            // 200番台か404以外だったらエラーとする
            if (!result.IsSuccessStatusCode && result.StatusCode != HttpStatusCode.NotFound) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            // 200なら成功
            return json.meta.status == 200;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<MylistListEntry> GetUserPublicMylistAsync(string userId, int sampleItemCount = 0) {

            if (string.IsNullOrEmpty(userId)) {

                throw new ArgumentNullException(nameof(userId));
            }

            var builder = new GetRequestQueryBuilder(string.Format(PublicMylistApiUrl, userId))
                .AddQuery("sampleItemCount", sampleItemCount);

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            foreach (var mylist in json.data.mylists) {

                if (mylist == null) {
                    continue;
                }
                var sampleItems = new List<MylistEntry>();
                foreach (var sampleItem in mylist.sampleItems) {

                    if (sampleItem == null) {
                        continue;
                    }
                    var video = sampleItem.video;
                    sampleItems.Add(new MylistEntry {
                        AddedAt = DateTimeOffset.Parse(sampleItem.addedAt),
                        ItemId = sampleItem.itemId.ToString(),
                        Memo = sampleItem.description, // あとで見るはmemoだけどマイリストはdescription
                        Status = sampleItem.status,

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
                        ThumbnailUrl = video.thumbnail.listingUrl,
                        Title = video.title,
                        Type = video.type,

                        WatchId = sampleItem.watchId,
                        VideoUrl = $"https://www.nicovideo.jp/watch/{video.id}"
                    });
                }
                yield return new MylistListEntry {
                    CreatedAt = DateTimeOffset.Parse(mylist.createdAt),
                    DefaultSortKey = mylist.defaultSortKey,
                    DefaultSortOrder = mylist.defaultSortOrder,
                    Description = mylist.description,
                    FollowerCount = (int)mylist.followerCount,
                    Id = mylist.id.ToString(),
                    IsFollowing = mylist.isFollowing,
                    IsPublic = mylist.isPublic,
                    ItemsCount = (int)mylist.itemsCount,
                    Name = mylist.name,
                    OwnerIconUrl = mylist.owner.iconUrl,
                    OwnerId = mylist.owner.id,
                    OwnerName = mylist.owner.name,
                    OwnerType = mylist.owner.ownerType,
                    SampleItems = sampleItems
                };
            }
        }
    }
}
