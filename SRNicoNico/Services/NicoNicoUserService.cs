using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DynaJson;
using FastEnumUtility;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコニコのユーザー関連の機能の実装
    /// </summary>
    public class NicoNicoUserService : IUserService {
        /// <summary>
        /// 自分がフォローしているユーザーを取得するAPI
        /// </summary>
        private const string FollowingUsersApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/following/users";
        /// <summary>
        /// 自分がフォローしているタグを取得するAPI
        /// </summary>
        private const string FollowingTagsApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/following/tags";
        /// <summary>
        /// 自分がフォローしているマイリストを取得するAPI
        /// </summary>
        private const string FollowingMylistsApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/following/mylists";
        /// <summary>
        /// 自分がフォローしているチャンネルを取得するAPI
        /// </summary>
        private const string FollowingChannelsApiUrl = "https://public.api.nicovideo.jp/v1/user/followees/channels.json";
        /// <summary>
        /// 自分がフォローしているコミュニティを取得するAPI
        /// </summary>
        private const string FollowingCommunitiesApiUrl = "https://public.api.nicovideo.jp/v1/user/followees/communities.json";
        /// <summary>
        /// ユーザー詳細を取得するAPI
        /// </summary>
        private const string UserDetailsApiUrl = "https://nvapi.nicovideo.jp/v1/users/";
        /// <summary>
        /// ユーザーがフォローしているユーザーを取得するAPI
        /// </summary>
        private const string UserFollowingApiUrl = "https://nvapi.nicovideo.jp/v1/users/{0}/following/users";
        /// <summary>
        /// ユーザーをフォローしているユーザーを取得するAPI
        /// </summary>
        private const string UserFollowerApiUrl = "https://nvapi.nicovideo.jp/v1/users/{0}/followed-by/users";
        /// <summary>
        /// ユーザーの投稿動画を取得するAPI
        /// </summary>
        private const string UserVideoApiUrl = "https://nvapi.nicovideo.jp/v2/users/{0}/videos";
        /// <summary>
        /// ユーザーをフォロー/フォロー解除するAPI
        /// </summary>
        private const string UserFollowApiUrl = "https://user-follow-api.nicovideo.jp/v1/user/followees/niconico-users/{0}.json";

        private readonly ISessionService SessionService;

        public NicoNicoUserService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<UserList> GetFollowedUsersAsync(int page = 1, int pageSize = 100) {

            if (page < 1) {

                throw new ArgumentException("pageに0以下を指定しないで");
            }

            var query = new GetRequestQueryBuilder(FollowingUsersApiUrl)
                .AddQuery("pageSize", pageSize)
                .AddQuery("page", page);

            using var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            var list = new List<UserEntry>();
            foreach (var user in json.data.items) {

                if (user == null) {
                    continue;
                }
                list.Add(new UserEntry {
                    Id = user.id.ToString(),
                    Description = user.description,
                    StrippedDescription = user.strippedDescription,
                    NickName = user.nickname,
                    ThumbnailUrl = user.icons.large,
                    IsPremium = user.isPremium
                });
            }

            return new UserList {
                Page = page,
                Total = (int)json.data.summary.followees,
                Entries = list
            };
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TagEntry> GetFollowedTagsAsync() {

            using var result = await SessionService.GetAsync(FollowingTagsApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            foreach (var entry in json.data.tags) {

                if (entry == null) {
                    continue;
                }
                yield return new TagEntry {
                    Name = entry.name,
                    Summary = $"{entry.nicodicSummary}…",
                    FollowedAt = DateTimeOffset.Parse(entry.followedAt)
                };
            }
        }

        /// <inheritdoc />
        public async Task<bool> FollowTagAsync(string tag) {

            if (string.IsNullOrEmpty(tag)) {
                throw new ArgumentNullException(nameof(tag));
            }

            var query = new GetRequestQueryBuilder(FollowingTagsApiUrl)
                .AddQuery("tag", tag);

            using var result = await SessionService.PostAsync(query.Build(), (IDictionary<string, string>?)null, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.meta.status == 201;
        }

        /// <inheritdoc />
        public async Task<bool> UnfollowTagAsync(string tag) {

            if (string.IsNullOrEmpty(tag)) {
                throw new ArgumentNullException(nameof(tag));
            }

            var query = new GetRequestQueryBuilder(FollowingTagsApiUrl)
                .AddQuery("tag", tag);

            using var result = await SessionService.DeleteAsync(query.Build(), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.meta.status == 200;
        }


        /// <inheritdoc />
        public async IAsyncEnumerable<UserMylistEntry> GetFollowedMylistsAsync() {

            var query = new GetRequestQueryBuilder(FollowingMylistsApiUrl)
                .AddQuery("sampleItemCount", 3);

            var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            foreach (var entry in json.data.mylists) {

                if (entry == null) {
                    continue;
                }
                var status = FastEnum.Parse<MylistStatus>(entry.status, true);
                if (status != MylistStatus.Public) {

                    yield return new UserMylistEntry {
                        Status = status,
                        Id = entry.id.ToString()
                    };
                    continue;
                }
                var detail = entry.detail;
                var videos = new List<MylistSampleVideo>();

                foreach (var video in detail.sampleItems) {

                    if (video == null) {
                        continue;
                    }
                    videos.Add(new MylistSampleVideo {

                        AddedAt = DateTimeOffset.Parse(video.addedAt),
                        Description = video.description,
                        ItemId = video.itemId.ToString(),
                        Status = FastEnum.Parse<SampleVideoStatus>(video.status, true),
                        ViewCount = (int)video.video.count.view,
                        CommentCount = (int)video.video.count.comment,
                        MylistCount = (int)video.video.count.mylist,
                        LikeCount = (int)video.video.count.like,
                        Duration = (int)video.video.duration,
                        Id = video.video.id,
                        IsChannelVideo = video.video.isChannelVideo,
                        IsPaymentRequired = video.video.isPaymentRequired,
                        LatestCommentSummary = video.video.latestCommentSummary,
                        OwnerThumbnailUrl = video.video.owner.iconUrl,
                        OwnerId = video.video.owner.id,
                        OwnerType = FastEnum.Parse<SampleVideoOwnerType>(video.video.owner.ownerType, true),
                        PlaybackPosition = video.video.playbackPosition,
                        RegisteredAt = DateTimeOffset.Parse(video.video.registeredAt),
                        RequireSensitiveMasking = video.video.requireSensitiveMasking,
                        ShortDescription = video.video.shortDescription,
                        ThumbnailUrl = video.video.thumbnail.url,
                        Title = video.video.title,
                        Type = video.video.type
                    });
                }

                yield return new UserMylistEntry {
                    Status = status,
                    Id = entry.id.ToString(),
                    CreatedAt = DateTimeOffset.Parse(detail.createdAt),
                    DefaultSortKey = detail.defaultSortKey,
                    DefaultSortOrder = detail.defaultSortOrder,
                    Description = detail.description,
                    FollowerCount = (int)detail.followerCount,
                    IsFollowing = detail.isFollowing,
                    IsPublic = detail.isPublic,
                    ItemsCount = (int)detail.itemsCount,
                    Name = detail.name,
                    OwnerThumbnailUrl = detail.owner.iconUrl,
                    OwnerId = detail.owner.id,
                    OwnerName = detail.owner.name,
                    OwnerType = detail.owner.ownerType,
                    SampleItems = videos
                };
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ChannelEntry> GetFollowedChannelsAsync() {

            var query = new GetRequestQueryBuilder(FollowingChannelsApiUrl)
                .AddQuery("limit", 100);

            var result = await SessionService.GetAsync(query.Build()).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            foreach (var entry in json.data) {

                if (entry == null) {
                    continue;
                }
                yield return new ChannelEntry {
                    BodyPrice = (int)entry.bodyPrice,
                    CanAdmit = entry.canAdmit,
                    Description = entry.description.Replace("<br>", ""), // タグを削除する
                    Id = entry.id.ToString(),
                    IsAdult = entry.isAdult,
                    IsFree = entry.isFree,
                    Name = entry.name,
                    OwnerName = entry.ownerName,
                    Price = (int)entry.price,
                    ScreenName = entry.screenName,
                    IsJoining = entry.session.joining,
                    ThumbnailSmallUrl = entry.thumbnailSmallUrl,
                    ThumbnailUrl = entry.thumbnailUrl,
                    Url = entry.url
                };
            }
        }

        /// <inheritdoc />
        public async Task<CommunityList> GetFollowedCommunitiesAsync(int page = 1, int pageSize = 100) {

            if (page < 1) {

                throw new ArgumentException("pageに0以下を指定しないで");
            }
            var ret = new CommunityList();

            var query = new GetRequestQueryBuilder(FollowingCommunitiesApiUrl)
                .AddQuery("limit", pageSize)
                .AddQuery("offset", (page - 1) * pageSize);

            var result = await SessionService.GetAsync(query.Build()).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            ret.Page = page;
            ret.Total = (int)json.meta.total;

            var list = new List<CommunityEntry>();
            foreach (var community in json.data) {

                if (community == null) {
                    continue;
                }
                var entry = new CommunityEntry {
                    CreateTime = DateTimeOffset.Parse(community.createTime),
                    Description = Regex.Replace(community.description, "<.*?>", string.Empty).Trim(), // タグを削除する
                    GlobalId = community.globalId,
                    Id = community.id.ToString(),
                    Level = (int)community.level,
                    Name = community.name,
                    CommunityAutoAcceptEntry = community.optionFlags.communityAutoAcceptEntry,
                    CommunityBlomaga = community.optionFlags.communityBlomaga,
                    CommunityHideLiveArchives = community.optionFlags.communityHideLiveArchives,
                    CommunityIconInspectionMobile = community.optionFlags.communityIconInspectionMobile() ? community.optionFlags.communityIconInspectionMobile : null,
                    CommunityInvalidBbs = community.optionFlags.communityInvalidBbs,
                    CommunityPrivLiveBroadcastNew = community.optionFlags.communityPrivLiveBroadcastNew,
                    CommunityPrivUserAuth = community.optionFlags.communityPrivUserAuth,
                    CommunityPrivVideoPost = community.optionFlags.communityPrivVideoPost,
                    CommunityShownNewsNum = (int)community.optionFlags.communityShownNewsNum,
                    CommunityUserInfoRequired = community.optionFlags.communityUserInfoRequired,
                    OwnerId = community.ownerId.ToString(),
                    Status = FastEnum.Parse<CommunityStatus>(community.status, true),
                    ThreadCount = (int)community.threadCount,
                    ThreadMax = (int)community.threadMax,
                    ThumbnailUrl = community.thumbnailUrl.normal,
                    UserCount = (int)community.userCount
                };
                var tags = new List<string>();
                foreach (var tag in community.tags) {

                    if (tag == null) {
                        continue;
                    }
                    tags.Add(tag.text);
                }
                entry.Tags = tags;

                list.Add(entry);
            }
            ret.Entries = list;
            return ret;
        }

        /// <inheritdoc />
        public async Task<UserDetails> GetUserAsync(string userId) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var result = await SessionService.GetAsync(UserDetailsApiUrl + userId, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var a = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonObject.Parse(a);
            var data = json.data;

            return new UserDetails {
                UserId = data.user.id.ToString(),
                Description = data.user.description,
                StrippedDescription = data.user.strippedDescription,
                IsPremium = data.user.isPremium,
                RegisteredVersion = data.user.registeredVersion,
                FolloweeCount = (int)data.user.followeeCount,
                FollowerCount = (int)data.user.followerCount,
                CurrentLevel = (int)data.user.userLevel.currentLevel,
                NextLevelThresholdExperience = (int)data.user.userLevel.nextLevelThresholdExperience,
                NextLevelExperience = (int)data.user.userLevel.nextLevelExperience,
                CurrentLevelExperience = (int)data.user.userLevel.currentLevelExperience,
                UserChannel = data.user.userChannel,
                IsNicorepoReadable = data.user.isNicorepoReadable,
                Nickname = data.user.nickname,
                ThumbnailLargeUrl = data.user.icons.large,
                ThumbnailSmallUrl = data.user.icons.small,
                IsFollowing = data.relationships.sessionUser.isFollowing,
                IsMe = data.relationships.isMe
            };
        }

        private UserFollowList ParseFollowList(dynamic data) {

            var summary = data.summary;

            var items = new List<UserFollowItem>();

            foreach (var item in data.items) {

                if (item == null) {
                    continue;
                }

                items.Add(new UserFollowItem {
                    UserId = item.id.ToString(),
                    Description = item.description,
                    IsFollowing = item.relationships.sessionUser.isFollowing,
                    IsPremium = item.isPremium,
                    Nickname = item.nickname,
                    StrippedDescription = item.strippedDescription,
                    ThumbnailLargeUrl = item.icons.large,
                    ThumbnailSmallUrl = item.icons.small
                });
            }

            return new UserFollowList {
                Items = items,
                Cursor = summary.cursor,
                Followees = (int)summary.followees,
                Followers = (int)summary.followers,
                HasNext = summary.hasNext
            };
        }

        /// <inheritdoc />
        public async Task<UserFollowList> GetUserFollowingAsync(string userId, string? cursor = null, int pageSIze = 100) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var query = new GetRequestQueryBuilder(string.Format(UserFollowingApiUrl, userId))
                .AddQuery("pageSize", pageSIze);

            if (!string.IsNullOrEmpty(cursor)) {
                query.AddQuery("cursor", cursor);
            }

            var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return ParseFollowList(json.data);
        }

        /// <inheritdoc />
        public async Task<UserFollowList> GetUserFollowerAsync(string userId, string? cursor = null, int pageSIze = 100) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var query = new GetRequestQueryBuilder(string.Format(UserFollowerApiUrl, userId))
                .AddQuery("pageSize", pageSIze);

            if (!string.IsNullOrEmpty(cursor)) {
                query.AddQuery("cursor", cursor);
            }

            var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            return ParseFollowList(json.data);
        }

        /// <inheritdoc />
        public async Task<VideoList> GetUserVideosAsync(string userId, VideoSortKey sortKey, int page = 1, int pageSize = 100) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var builder = new GetRequestQueryBuilder(string.Format(UserVideoApiUrl, userId))
                .AddRawQuery(sortKey.GetLabel()!)
                .AddQuery("pageSize", pageSize)
                .AddQuery("page", page);

            var result = await SessionService.GetAsync(builder.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;

            var items = new List<VideoListItem>();

            foreach (var item in data.items) {

                if (item == null) {
                    continue;
                }
                var essential = item.essential;
                var series = item.series;

                items.Add(new VideoListItem {
                    CommentCount = (int)essential.count.comment,
                    LikeCount = (int)essential.count.like,
                    MylistCount = (int)essential.count.mylist,
                    ViewCount = (int)essential.count.view,
                    Duration = (int)essential.duration,
                    Id = essential.id,
                    IsChannelVideo = essential.isChannelVideo,
                    IsPaymentRequired = essential.isPaymentRequired,
                    LatestCommentSummary = essential.latestCommentSummary,
                    OwnerIconUrl = essential.owner.iconUrl,
                    OwnerId = essential.owner.id,
                    OwnerName = essential.owner.name,
                    OwnerType = essential.owner.ownerType,
                    PlaybackPosition = (int?)essential.playbackPosition,
                    RegisteredAt = DateTimeOffset.Parse(essential.registeredAt),
                    RequireSensitiveMasking = essential.requireSensitiveMasking,
                    ShortDescription = essential.shortDescription,
                    ThumbnailUrl = essential.thumbnail.listingUrl,
                    Title = essential.title,

                    SeriesId = series?.id.ToString(),
                    SeriesOrder = (int?)series?.order,
                    SeriesTitle = series?.title
                });
            }

            return new VideoList {
                Items = items,
                TotalCount = (int)data.totalCount
            };
        }

        /// <inheritdoc />
        public async Task<bool> FollowUserAsync(string userId) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var result = await SessionService.PostAsync(string.Format(UserFollowApiUrl, userId), (IDictionary<string, string>?)null, NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            return json.meta.status == 200;
        }

        /// <inheritdoc />
        public async Task<bool> UnfollowUserAsync(string userId) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var result = await SessionService.DeleteAsync(string.Format(UserFollowApiUrl, userId), NicoNicoSessionService.AjaxApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            return json.meta.status == 200;
        }

        /// <inheritdoc />
        public async Task<bool> IsFollowUserAsync(string userId) {

            if (userId == null) {
                throw new ArgumentNullException(nameof(userId));
            }

            var result = await SessionService.GetAsync(string.Format(UserFollowApiUrl, userId), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            return json.data.following;
        }

    }
}
