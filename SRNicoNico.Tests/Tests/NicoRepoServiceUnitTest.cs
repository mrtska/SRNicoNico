using System;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// INicoRepoServiceのユニットテスト
    /// </summary>
    public class NicoRepoServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly INicoRepoService NicoRepoService;

        public NicoRepoServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            NicoRepoService = new NicoNicoNicoRepoService(SessionService, null);
        }

        private void AssertNicoRepo(NicoRepoList result) {

            Assert.NotNull(result.Entries);

            foreach (var nicorepo in result.Entries) {

                Assert.NotNull(nicorepo.Id);
                Assert.NotNull(nicorepo.Title);
                Assert.NotEqual(default, nicorepo.UpdatedAt);
                Assert.NotNull(nicorepo.ActorUrl);
                Assert.NotNull(nicorepo.ActorName);
                Assert.NotNull(nicorepo.ActorIconUrl);
                //Assert.NotNull(nicorepo.ObjectType);
                //Assert.NotNull(nicorepo.ObjectUrl);
                //Assert.NotNull(nicorepo.ObjectImageUrl);
                //Assert.NotNull(nicorepo.MuteContext);
                //Assert.NotNull(nicorepo.MuteContext.Task);
                //Assert.NotNull(nicorepo.MuteContext.IdType);
                //Assert.NotNull(nicorepo.MuteContext.Id);
                //Assert.NotNull(nicorepo.MuteContext.Type);
                //Assert.NotNull(nicorepo.MuteContext.Trigger);

                // 最初の一つだけ確認する
                break;
            }
        }

        /// <summary>
        /// ニコレポを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetNicoRepoAllUnitTest() {

            AssertNicoRepo(await NicoRepoService.GetNicoRepoAsync(NicoRepoType.All, NicoRepoFilter.All));
            AssertNicoRepo(await NicoRepoService.GetNicoRepoAsync(NicoRepoType.Self, NicoRepoFilter.All));
            AssertNicoRepo(await NicoRepoService.GetNicoRepoAsync(NicoRepoType.User, NicoRepoFilter.All));
            AssertNicoRepo(await NicoRepoService.GetNicoRepoAsync(NicoRepoType.Channel, NicoRepoFilter.All));
            AssertNicoRepo(await NicoRepoService.GetNicoRepoAsync(NicoRepoType.Community, NicoRepoFilter.All));
            AssertNicoRepo(await NicoRepoService.GetNicoRepoAsync(NicoRepoType.Mylist, NicoRepoFilter.All));
        }

        /// <summary>
        /// ユーザーニコレポを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetUserNicoRepoAllUnitTest() {

            var userId = "23425727";

            AssertNicoRepo(await NicoRepoService.GetUserNicoRepoAsync(userId, NicoRepoFilter.All));
            AssertNicoRepo(await NicoRepoService.GetUserNicoRepoAsync(userId, NicoRepoFilter.Video));
            AssertNicoRepo(await NicoRepoService.GetUserNicoRepoAsync(userId, NicoRepoFilter.VideoUpload));
        }
    }
}
