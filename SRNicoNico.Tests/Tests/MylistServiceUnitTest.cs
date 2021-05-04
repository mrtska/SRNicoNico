using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// IMylistServiceのユニットテスト
    /// </summary>
    public class MylistServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly IMylistService MylistService;

        public MylistServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            MylistService = new NicoNicoMylistService(SessionService);
        }

        /// <summary>
        /// あとで見るを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetWatchLaterUnitTest() {

            var result = await MylistService.GetWatchLaterAsync(MylistSortKey.AddedAtDesc, 1);

            Assert.NotEqual(0, result.TotalCount);
            Assert.NotEmpty(result.Entries);

            foreach (var entry in result.Entries) {

                Assert.NotEqual(default, entry.AddedAt);
                Assert.NotNull(entry.ItemId);
                Assert.NotNull(entry.Memo);
                Assert.NotNull(entry.Status);

                //Assert.NotEqual(0, entry.ViewCount);
                //Assert.NotEqual(0, entry.CommentCount);
                //Assert.NotEqual(0, entry.MylistCount);
                //Assert.NotEqual(0, entry.LikeCount);

                Assert.NotEqual(0, entry.Duration);
                Assert.NotNull(entry.Id);

                //Assert.NotNull(entry.LatestCommentSummary);

                Assert.NotNull(entry.OwnerType);
                if (entry.OwnerType != "hidden") {

                    Assert.NotNull(entry.OwnerIconUrl);
                    Assert.NotNull(entry.OwnerId);
                    Assert.NotNull(entry.OwnerName);
                }

                Assert.NotEqual(default, entry.RegisteredAt);
                Assert.NotNull(entry.ShortDescription);
                Assert.NotNull(entry.ThumbnailUrl);
                Assert.NotNull(entry.Title);
                Assert.NotNull(entry.Type);
                Assert.NotNull(entry.WatchId);
            }
        }

        /// <summary>
        /// あとで見るに追加と削除が出来ることのテスト
        /// </summary>
        [Fact]
        public async Task AddAndDeleteWatchLaterUnitTest() {

            var result = await MylistService.AddWatchLaterAsync("sm9", null);
            Assert.True(result);

            result = await MylistService.DeleteWatchLaterAsync("1173108780");
            Assert.True(result);
        }

        /// <summary>
        /// マイリストの一覧が取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetMylistsUnitTest() {

            await foreach (var result in MylistService.GetMylistsAsync(1)) {

                Assert.NotEqual(default, result.CreatedAt);
                Assert.NotNull(result.DefaultSortKey);
                Assert.NotNull(result.DefaultSortOrder);
                Assert.NotNull(result.Description);
                Assert.NotNull(result.Id);
                Assert.NotNull(result.Name);

                Assert.NotNull(result.OwnerType);
                Assert.NotNull(result.OwnerIconUrl);
                Assert.NotNull(result.OwnerId);
                Assert.NotNull(result.OwnerName);

                Assert.NotNull(result.SampleItems);

                // 最初の一つだけを確認する
                break;
            }
        }

        /// <summary>
        /// 特定のマイリストの詳細が取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetMylistUnitTest() {

            // mrtskaのテスト用公開マイリスト
            // mrtskaのアカウントでないとこのテストは動かないと思われる
            var result = await MylistService.GetMylistAsync("70670861", MylistSortKey.AddedAtDesc, 1);

            Assert.NotNull(result.DefaultSortKey);
            Assert.NotNull(result.DefaultSortOrder);
            Assert.NotNull(result.Description);
            Assert.NotNull(result.Id);
            Assert.NotNull(result.Name);

            Assert.NotNull(result.OwnerType);
            Assert.NotNull(result.OwnerIconUrl);
            Assert.NotNull(result.OwnerId);
            Assert.NotNull(result.OwnerName);

            Assert.NotNull(result.Entries);

            foreach (var entry in result.Entries) {

                Assert.NotEqual(default, entry.AddedAt);
                Assert.NotNull(entry.ItemId);
                Assert.NotNull(entry.Memo);
                Assert.NotNull(entry.Status);

                //Assert.NotEqual(0, entry.ViewCount);
                //Assert.NotEqual(0, entry.CommentCount);
                //Assert.NotEqual(0, entry.MylistCount);
                //Assert.NotEqual(0, entry.LikeCount);

                Assert.NotEqual(0, entry.Duration);
                Assert.NotNull(entry.Id);

                //Assert.NotNull(entry.LatestCommentSummary);

                Assert.NotNull(entry.OwnerType);
                if (entry.OwnerType != "hidden") {

                    Assert.NotNull(entry.OwnerIconUrl);
                    Assert.NotNull(entry.OwnerId);
                    Assert.NotNull(entry.OwnerName);
                }

                Assert.NotEqual(default, entry.RegisteredAt);
                Assert.NotNull(entry.ShortDescription);
                Assert.NotNull(entry.ThumbnailUrl);
                Assert.NotNull(entry.Title);
                Assert.NotNull(entry.Type);
                Assert.NotNull(entry.WatchId);
            }
        }

        /// <summary>
        /// マイリストに追加と削除が出来ることのテスト
        /// </summary>
        [Fact]
        public async Task AddAndDeleteMylistUnitTest() {

            // mrtskaのテスト用公開マイリスト
            // mrtskaのアカウントでないとこのテストは動かないと思われる
            var result = await MylistService.AddMylistItemAsync("70670861", "sm9", null);
            Assert.True(result);

            result = await MylistService.DeleteMylistItemAsync("70670861", "1173108780");
            Assert.True(result);
        }
    }
}
