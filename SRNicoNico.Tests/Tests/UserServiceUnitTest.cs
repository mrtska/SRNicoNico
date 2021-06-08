using System;
using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// IUserServiceのユニットテスト
    /// </summary>
    public class UserServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly IUserService UserService;

        public UserServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            UserService = new NicoNicoUserService(SessionService);
        }

        /// <summary>
        /// 自分がフォローしているユーザーを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetFollowedUsersUnitTest() {

            var result = await UserService.GetFollowedUsersAsync();

            Assert.NotEqual(0, result.Total);

            foreach (var user in result.Entries) {

                Assert.NotNull(user.Id);
                Assert.NotNull(user.NickName);
                Assert.NotNull(user.ThumbnailUrl);
                Assert.NotNull(user.Description);
                Assert.NotNull(user.StrippedDescription);

                // 最初の一つだけ確認する
                break;
            }
        }
        /// <summary>
        /// 自分がフォローしているタグを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetFollowedTagsUnitTest() {

            await foreach (var tag in UserService.GetFollowedTagsAsync()) {

                Assert.NotNull(tag.Name);
                Assert.NotNull(tag.Summary);
                Assert.NotEqual(default, tag.FollowedAt);

                // 最初の一つだけ確認する
                break;
            }
        }
        /// <summary>
        /// 自分がフォローしているマイリストを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetFollowedMylistsUnitTest() {

            await foreach (var mylist in UserService.GetFollowedMylistsAsync()) {

                Assert.NotNull(mylist.Id);
                Assert.NotEqual(default, mylist.CreatedAt);
                Assert.NotNull(mylist.DefaultSortKey);
                Assert.NotNull(mylist.DefaultSortOrder);
                Assert.NotNull(mylist.Description);
                Assert.NotEqual(0, mylist.FollowerCount);
                Assert.True(mylist.IsFollowing);
                Assert.True(mylist.IsPublic);
                Assert.NotNull(mylist.Name);
                Assert.NotNull(mylist.OwnerThumbnailUrl);
                Assert.NotNull(mylist.OwnerId);
                Assert.NotNull(mylist.OwnerName);
                Assert.NotNull(mylist.OwnerType);

                foreach (var video in mylist.SampleItems) {

                    Assert.NotEqual(default, video.AddedAt);
                    Assert.NotNull(video.Description);
                    Assert.NotNull(video.ItemId);
                    Assert.NotNull(video.Id);
                    Assert.NotNull(video.OwnerThumbnailUrl);
                    Assert.NotNull(video.OwnerId);
                    Assert.NotEqual(default, video.RegisteredAt);
                    Assert.NotNull(video.ShortDescription);
                    Assert.NotNull(video.ThumbnailUrl);
                    Assert.NotNull(video.Title);
                    Assert.NotNull(video.Type);
                    // 最初の一つだけ確認する
                    break;
                }
                // 最初の一つだけ確認する
                break;
            }
        }

        /// <summary>
        /// 自分がフォローしているチャンネルを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetFollowedChannelsUnitTest() {

            await foreach (var channel in UserService.GetFollowedChannelsAsync()) {

                Assert.NotNull(channel.Description);
                Assert.NotNull(channel.Id);
                Assert.NotNull(channel.Name);
                Assert.NotNull(channel.OwnerName);
                Assert.NotNull(channel.ScreenName);
                Assert.NotNull(channel.ThumbnailSmallUrl);
                Assert.NotNull(channel.ThumbnailUrl);
                Assert.NotNull(channel.Url);

                // 最初の一つだけ確認する
                break;
            }
        }

        /// <summary>
        /// 自分がフォローしているコミュニティを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetFollowedCommunitiesUnitTest() {

            var result = await UserService.GetFollowedCommunitiesAsync();

            Assert.NotEqual(0, result.Total);

            foreach (var community in result.Entries) {

                Assert.NotEqual(default, community.CreateTime);
                Assert.NotNull(community.Description);
                Assert.NotNull(community.GlobalId);
                Assert.NotNull(community.Id);
                Assert.NotEqual(0, community.Level);
                Assert.NotNull(community.Name);
                Assert.NotNull(community.OwnerId);
                Assert.NotNull(community.Tags);
                Assert.NotNull(community.ThumbnailUrl);
                Assert.NotEqual(0, community.UserCount);

                // 最初の一つだけ確認する
                break;
            }
        }

        /// <summary>
        /// 任意のユーザーの情報が正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetUserUnitTest() {

            var result = await UserService.GetUserAsync("23425727");

            Assert.NotNull(result.UserId);
            Assert.NotNull(result.Description);
            Assert.NotNull(result.StrippedDescription);
            Assert.NotNull(result.RegisteredVersion);
            Assert.NotNull(result.Nickname);
            Assert.NotNull(result.ThumbnailLargeUrl);
            Assert.NotNull(result.ThumbnailSmallUrl);
        }
    }
}
