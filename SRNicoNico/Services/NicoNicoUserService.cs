﻿using System;
using System.Collections.Generic;
using System.Text;
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


        private readonly ISessionService SessionService;

        public NicoNicoUserService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<UserList> GetFollowedUsersAsync(int page = 1, int pageSize = 100) {

            if (page < 1) {

                throw new ArgumentException("pageに0以下を指定しないで");
            }
            var ret = new UserList();

            var query = new GetRequestQueryBuilder(FollowingUsersApiUrl)
                .AddQuery("pageSize", pageSize)
                .AddQuery("page", page);

            var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            ret.Page = page;
            ret.Total = (int)json.data.summary.followees;

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
            ret.Entries = list;
            return ret;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TagEntry> GetFollowedTagsAsync() {

            var result = await SessionService.GetAsync(FollowingTagsApiUrl, NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

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
        public async IAsyncEnumerable<MylistEntry> GetFollowedMylistsAsync() {

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

                    yield return new MylistEntry {
                        Status = status,
                        Id = entry.id
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

                yield return new MylistEntry {
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
    }
}
