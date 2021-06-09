using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DynaJson;
using FastEnumUtility;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコレポを取得する処理の実装
    /// </summary>
    public class NicoNicoNicoRepoService : INicoRepoService {
        /// <summary>
        /// ニコレポを取得するAPI
        /// </summary>
        private const string NicoRepoApiUrl = "https://public.api.nicovideo.jp/v1/timelines/nicorepo/last-1-month/my/pc/entries.json";
        /// <summary>
        /// ユーザーニコレポを取得するAPI
        /// </summary>
        private const string NicoRepoUserApiUrl = "https://public.api.nicovideo.jp/v1/timelines/nicorepo/last-6-months/users/{0}/pc/entries.json";

        private readonly ISessionService SessionService;

        public NicoNicoNicoRepoService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        private async Task<NicoRepoList> GetNicoRepoAsync(string? userId, NicoRepoType type, NicoRepoFilter filter, string? untilId = null) {

            var query = new GetRequestQueryBuilder(userId == null ? NicoRepoApiUrl : string.Format(NicoRepoUserApiUrl, userId));
            if (type != NicoRepoType.All) {

                query.AddQuery("list", type.GetLabel()!);
            }
            if (filter != NicoRepoFilter.All) {

                query.AddRawQuery(filter.GetLabel()!);
            }
            if (!string.IsNullOrEmpty(untilId)) {

                query.AddQuery("untilId", untilId);
            }

            var result = await SessionService.GetAsync(query.Build()).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }
            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            var ret = new NicoRepoList {
                HasNext = json.meta.hasNext,
                MinId = json.meta.minId() ? json.meta.minId : null,
                MaxId = json.meta.maxId() ? json.meta.maxId : null
            };

            var entries = new List<NicoRepoEntry>();
            foreach (var nicorepo in json.data) {

                if (nicorepo == null) {
                    continue;
                }
                entries.Add(new NicoRepoEntry {
                    Id = nicorepo.id,
                    Title = nicorepo.title,
                    UpdatedAt = DateTimeOffset.Parse(nicorepo.updated),
                    ActorUrl = nicorepo.actor.url,
                    ActorName = nicorepo.actor.name,
                    ActorIconUrl = nicorepo.actor.icon,
                    ObjectType = nicorepo.@object() ? nicorepo.@object.type : null,
                    ObjectUrl = nicorepo.@object() ? nicorepo.@object?.url : null,
                    ObjectName = nicorepo.@object() ? nicorepo.@object?.name : null,
                    ObjectImageUrl = nicorepo.@object() ? nicorepo.@object?.image : null,
                    MuteContext = nicorepo.muteContext() ? new NicoRepoMuteContext {
                        Task = nicorepo.muteContext.task,
                        Id = nicorepo.muteContext.sender.id,
                        IdType = nicorepo.muteContext.sender.idType,
                        Type = nicorepo.muteContext.sender.type,
                        Trigger = nicorepo.muteContext.trigger,
                    } : null
                });
            }
            ret.Entries = entries;

            return ret;
        }

        /// <inheritdoc />
        public Task<NicoRepoList> GetNicoRepoAsync(NicoRepoType type, NicoRepoFilter filter, string? untilId = null) {
            return GetNicoRepoAsync(null, type, filter, untilId);
        }

        /// <inheritdoc />
        public Task<NicoRepoList> GetUserNicoRepoAsync(string userId, NicoRepoType type, NicoRepoFilter filter, string? untilId = null) {
            return GetNicoRepoAsync(userId, type, filter, untilId);
        }
    }
}
