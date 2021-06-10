using System;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;
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
    }
}
