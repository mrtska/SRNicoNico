using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// ILiveServiceのユニットテスト
    /// </summary>
    public class LiveServiceUnitTest {

        private readonly ILiveService LiveService;

        public LiveServiceUnitTest() {

            LiveService = new NicoNicoLiveService(TestingNicoNicoViewer.Instance.TestSessionService);
        }

        /// <summary>
        /// フォローしているコミュニティやチャンネルが放送中の生放送のリストが正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetAccountHistoryUnitTest() {

            foreach (var history in await LiveService.GetOngoingLivesAsync()) {

                // 値が取れているかの確認
                Assert.NotEmpty(history.CommunityName);
                Assert.NotEmpty(history.ProviderType);
                Assert.NotEmpty(history.Title);
                Assert.NotEmpty(history.ThumbnailLinkUrl);
                Assert.NotEmpty(history.Thumbnailurl);

                break;
            }
        }

    }
}
