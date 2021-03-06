using System;
using System.Threading.Tasks;
using SRNicoNico.Services;
using Xunit;

namespace SRNicoNico.Tests {
    /// <summary>
    /// IUserServiceのユニットテスト
    /// </summary>
    public class UserServiceUnitTest {

        private readonly ISessionService SessionService;
        private readonly IUserService UserService;

        public UserServiceUnitTest() {

            SessionService = TestingNicoNicoViewer.Instance.TestSessionService;
            UserService = new NicoNicoUserService(SessionService);
        }

        /// <summary>
        /// 自分がフォローしているユーザーを正しく取得出来ることのテスト
        /// </summary>
        [Fact]
        public async Task GetFollowedUsersUnitTest() {

            var result = await UserService.GetFollowedUsersAsync();

            Assert.NotEqual(0, result.Total);

            foreach (var user in result.Entries) {

                Assert.NotNull(user.Id);
                Assert.NotNull(user.NickName);
                Assert.NotNull(user.ThumbnailUrl);
                Assert.NotNull(user.Description);
                Assert.NotNull(user.StrippedDescription);

                // 最初の一つだけ確認する
                break;
            }
        }
    }
}
