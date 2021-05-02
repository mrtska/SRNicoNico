using System;
using System.Linq;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;
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
                Assert.NotEqual(0, entry.ItemId);
                Assert.NotNull(entry.Memo);
                Assert.NotNull(entry.Status);

                //Assert.NotEqual(0, entry.ViewCount);
                //Assert.NotEqual(0, entry.CommentCount);
                //Assert.NotEqual(0, entry.MylistCount);
                //Assert.NotEqual(0, entry.LikeCount);
                
                Assert.NotEqual(0, entry.Duration);
                Assert.NotNull(entry.Id);

                //Assert.NotNull(entry.LatestCommentSummary);

                Assert.NotNull(entry.OwnerIconUrl);
                Assert.NotNull(entry.OwnerId);
                Assert.NotNull(entry.OwnerName);
                Assert.NotNull(entry.OwnerType);

                Assert.NotEqual(default, entry.RegisteredAt);
                Assert.NotNull(entry.ShortDescription);
                Assert.NotNull(entry.ThumbnailUrl);
                Assert.NotNull(entry.Title);
                Assert.NotNull(entry.Type);
                Assert.NotNull(entry.WatchId);
            }


        }
    }
}
