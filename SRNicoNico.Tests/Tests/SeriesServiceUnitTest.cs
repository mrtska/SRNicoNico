using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// ISeriesServiceのユニットテスト
    /// </summary>
    public class SeriesServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly ISeriesService SeriesService;

        public SeriesServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            SeriesService = new NicoNicoSeriesService(SessionService);
        }


        /// <summary>
        /// 任意のユーザーのシリーズを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetUserSeriesUnitTest() {

            var result = await SeriesService.GetUserSeriesAsync("23425727");

            Assert.NotEqual(0, result.TotalCount);

            Assert.NotEmpty(result.Items);

            foreach (var item in result.Items) {

                Assert.NotNull(item.Description);
                Assert.NotNull(item.Id);
                Assert.NotNull(item.ThumbnailUrl);
                Assert.NotNull(item.Title);
                Assert.NotNull(item.OwnerId);
                Assert.NotNull(item.OwnerType);
                Assert.NotEqual(0, item.ItemsCount);

                break;
            }
        }

        /// <summary>
        /// 任意のシリーズを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetSeriesUnitTest() {

            var result = await SeriesService.GetSeriesAsync("231356");

            Assert.NotNull(result.SeriesId);
            Assert.NotNull(result.OwnerId);
            Assert.NotNull(result.OwnerName);
            Assert.NotNull(result.OwnerType);
            Assert.NotNull(result.Title);
            Assert.NotNull(result.Description);
            Assert.NotNull(result.ThumbnailUrl);

            Assert.NotEqual(default, result.CreatedAt);
            Assert.NotEqual(default, result.UpdatedAt);

            Assert.NotEqual(0, result.TotalCount);

            Assert.NotEmpty(result.Items);

            foreach (var item in result.Items) {

                //Assert.NotEqual(0, entry.ViewCount);
                //Assert.NotEqual(0, entry.CommentCount);
                //Assert.NotEqual(0, entry.MylistCount);
                //Assert.NotEqual(0, entry.LikeCount);

                Assert.NotEqual(0, item.Duration);
                Assert.NotNull(item.Id);

                //Assert.NotNull(entry.LatestCommentSummary);

                Assert.NotNull(item.OwnerType);
                if (item.OwnerType != "hidden") {

                    Assert.NotNull(item.OwnerIconUrl);
                    Assert.NotNull(item.OwnerId);
                    Assert.NotNull(item.OwnerName);
                }

                Assert.NotEqual(default, item.RegisteredAt);
                Assert.NotNull(item.ShortDescription);
                Assert.NotNull(item.ThumbnailUrl);
                Assert.NotNull(item.Title);
                break;
            }
        }
    }
}
