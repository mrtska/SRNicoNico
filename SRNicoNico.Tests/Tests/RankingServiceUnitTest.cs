using System;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// IRankingServiceのユニットテスト
    /// </summary>
    public class RankingServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly IRankingService RankingService;

        public RankingServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            RankingService = new NicoNicoRankingService(SessionService);
        }

        /// <summary>
        /// カスタムランキングの設定を取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetRankingSettingsUnitTest() {

            var result = await RankingService.GetCustomRankingSettingsAsync();

            Assert.NotEmpty(result.GenreMap);
            Assert.NotEmpty(result.Settings);

            foreach (var entry in result.Settings) {

                Assert.NotNull(entry.Title);
                Assert.NotNull(entry.Type);
            }
        }

        /// <summary>
        /// カスタムランキングを取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetRankingUnitTest() {

            var result = await RankingService.GetCustomRankingAsync(1);

            Assert.NotNull(result.LaneType);
            Assert.NotNull(result.Title);
            Assert.NotNull(result.CustomType);
            Assert.NotNull(result.ChannelVideoListingStatus);
            Assert.NotNull(result.DefaultTitle);

            Assert.NotEmpty(result.VideoList);
        }
    }
}
