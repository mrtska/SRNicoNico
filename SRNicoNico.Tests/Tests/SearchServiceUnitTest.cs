using System;
using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// ISearchServiceのユニットテスト
    /// </summary>
    public class SearchServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly ISearchService SearchService;

        public SearchServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            SearchService = new NicoNicoSearchService(SessionService);
        }

        /// <summary>
        /// 検索が正しく出来ることのテスト
        /// </summary>
        [Fact]
        public async Task SearchUnitTest() {

            var result = await SearchService.SearchAsync(SearchSortKey.ViewCountDesc, SearchType.Tag, "Factorio");

            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Items);
        }

        /// <summary>
        /// ジャンル情報が取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetGenreFacetUnitTest() {

            var result = await SearchService.GetGenreFacetAsync(SearchType.Tag, "Factorio");
            Assert.NotEmpty(result);
            
            foreach (var facet in result) {

                Assert.NotNull(facet.Key);
                Assert.NotNull(facet.Label);

                Assert.NotEqual(0, facet.Count);
                break;
            }
        }
    }
}
