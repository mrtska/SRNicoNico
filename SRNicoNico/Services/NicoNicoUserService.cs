using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DynaJson;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコニコのユーザー関連の機能の実装
    /// </summary>
    public class NicoNicoUserService : IUserService {

        /// <summary>
        /// 自分がフォローしているユーザーを取得するAPI
        /// </summary>
        private const string FollowingUsersApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/following/users";


        private readonly ISessionService SessionService;


        public NicoNicoUserService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<UserList> GetFollowedUsersAsync(int page = 1, int pageSize = 100) {

            if (page < 1) {

                throw new ArgumentException("pageに0以下を指定しないで");
            }

            var ret = new UserList();

            var query = new GetRequestQueryBuilder(FollowingUsersApiUrl)
                .AddQuery("pageSize", pageSize)
                .AddQuery("page", page);

            var result = await SessionService.GetAsync(query.Build(), NicoNicoSessionService.ApiHeaders).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            ret.Page = page;
            ret.Total = (int)json.data.summary.followees;

            var list = new List<UserEntry>();
            foreach (var user in json.data.items) {

                if (user == null) {
                    continue;
                }
                list.Add(new UserEntry {
                    Id = user.id.ToString(),
                    Description = user.description,
                    StrippedDescription = user.strippedDescription,
                    NickName = user.nickname,
                    ThumbnailUrl = user.icons.large,
                    IsPremium = user.isPremium
                });
            }

            ret.Entries = list;
            return ret;
        }
    }
}
