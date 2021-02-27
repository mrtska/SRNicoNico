using System;
using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// IHistoryServiceのユニットテスト
    /// </summary>
    public class HistoryServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly IHistoryService HistoryService;

        public HistoryServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            HistoryService = new NicoNicoHistoryService(SessionService);
        }

        /// <summary>
        /// アカウントの視聴履歴が正しく取れることのテスト
        /// </summary>
        [Fact]
        public async Task GetAccountHistoryUnitTest() {

            await foreach (var history in HistoryService.GetAccountHistoryAsync()) {

                // 値が取れているかの確認
                Assert.False(string.IsNullOrEmpty(history.VideoId));
                Assert.False(string.IsNullOrEmpty(history.Title));
                Assert.NotNull(history.ShortDescription);
                Assert.False(string.IsNullOrEmpty(history.ThumbnailUrl));
                Assert.True(history.PostedAt != default);
                Assert.True(history.WatchedAt != default);
                Assert.True(history.WatchCount > 0);
                Assert.True(history.ViewCount > 0);
                Assert.True(history.Duration > 0);

                // 最初の1つ目だけ確認する
                break;
            }
        }

        /// <summary>
        /// アカウントの視聴履歴が削除出来ることのテスト
        /// </summary>
        [Fact]
        public async Task DeleteAccountHistoryUnitTest() {

            // sm9を視聴履歴に追加する 無くても削除API自体は成功するっぽいけど
            await SessionService.GetAsync("https://www.nicovideo.jp/watch/sm9");

            // ちょっと待つ
            await Task.Delay(1000);

            var result = await HistoryService.DeleteAccountHistoryAsync("sm9");

            // trueなら成功
            Assert.True(result);
        }
    }
}
