using System;
using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// IHistoryServiceのユニットテスト
    /// </summary>
    public class HistoryServiceUnitTest {

        private readonly IHistoryService HistoryService;

        public HistoryServiceUnitTest() {

            HistoryService = new NicoNicoHistoryService(TestingNicoNicoViewer.Instance.TestSessionService);
        }

        /// <summary>
        /// アカウントの視聴履歴が正しく取れることのテスト
        /// </summary>
        [Fact]
        public async Task GetAccountHistoryUnitTest() {

            var result = await HistoryService.GetAccountHistoryAsync();

            Assert.NotEmpty(result);

            // 最初の1つ目だけ確認する
            var history = result[0];

            // 値が取れているかの確認
            Assert.False(string.IsNullOrEmpty(history.VideoId));
            Assert.False(string.IsNullOrEmpty(history.Title));
            Assert.NotNull(history.ShortDescription);
            Assert.False(string.IsNullOrEmpty(history.ThmbnailUrl));
            Assert.True(history.PostedAt != default);
            Assert.True(history.WatchedAt != default);
            Assert.True(history.WatchCount > 0);
            Assert.True(history.ViewCount > 0);
        }
    }
}
