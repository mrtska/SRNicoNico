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
            RankingService = new NicoNicoRankingService(SessionService, null);
        }

        /// <summary>
        /// ランキングを取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetRankingUnitTest() {

            var result = await RankingService.GetRankingAsync(RankingTerm.Hour, "all");

            Assert.NotEmpty(result.VideoList);
        }

        /// <summary>
        /// 人気のタグを取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetPopularTagsUnitTest() {

            var result = await RankingService.GetPopularTagsAsync("anime");

            Assert.NotEqual(default, result.StartAt);
            Assert.NotEmpty(result.Tags);
        }

        /// <summary>
        /// カスタムランキングの設定を取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetCustomRankingSettingsUnitTest() {

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
        public async Task GetCustomRankingUnitTest() {

            var result = await RankingService.GetCustomRankingAsync(1);

            Assert.NotNull(result.LaneType);
            Assert.NotNull(result.Title);
            Assert.NotNull(result.CustomType);
            Assert.NotNull(result.ChannelVideoListingStatus);
            Assert.NotNull(result.DefaultTitle);

            Assert.NotEmpty(result.VideoList);
        }

        /// <summary>
        /// 話題ランキングを取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetHotTopicRankingUnitTest() {

            var result = await RankingService.GetHotTopicRankingAsync(RankingTerm.Hour, "all");

            Assert.NotEmpty(result.VideoList);
        }

        /// <summary>
        /// 話題のジャンルを取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetHotTopicsUnitTest() {

            var result = await RankingService.GetHotTopicsAsync();

            Assert.NotEqual(default, result.StartAt);
            Assert.NotEmpty(result.Items);

            foreach (var item in result.Items) {

                Assert.NotNull(item.Key);
                Assert.NotNull(item.Label);
                Assert.NotNull(item.Tag);
                Assert.NotNull(item.GenreKey);
                Assert.NotNull(item.GenreLabel);
            }
        }

        /// <summary>
        /// ジャンルを取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetGenresUnitTest() {

            var result = await RankingService.GetGenresAsync();

            Assert.NotEmpty(result);
        }
    }
}
