using System.Collections.Generic;
using System.Threading.Tasks;
using DynaJson;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ILiveServiceの実装
    /// </summary>
    public class NicoNicoLiveService : ILiveService {
        /// <summary>
        /// 現在放送中の生放送を取得するAPI
        /// </summary>
        private const string GetOngoingLiveApiUrl = "https://papi.live.nicovideo.jp/api/relive/notifybox.content";

        private readonly ISessionService SessionService;

        public NicoNicoLiveService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OngoingLive>> GetOngoingLivesAsync() {

            var query = new GetRequestQueryBuilder(GetOngoingLiveApiUrl)
                .AddQuery("row", 100);

            using var result = await SessionService.GetAsync(query.Build()).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode) {

                throw new StatusErrorException(result.StatusCode);
            }

            var json = JsonObject.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            var data = json.data;
            var ret = new List<OngoingLive>();

            foreach (var item in data.notifybox_content) {

                if (item == null) {
                    continue;
                }
                ret.Add(new OngoingLive {
                    CommunityName = item.community_name,
                    ElapsedTime = (int)item.elapsed_time,
                    Id = item.id,
                    ProviderType = item.provider_type,
                    ThumbnailLinkUrl = item.thumbnail_link_url,
                    Thumbnailurl = item.thumbnail_url,
                    Title = item.title
                });
            }

            return ret;
        }
    }
}
