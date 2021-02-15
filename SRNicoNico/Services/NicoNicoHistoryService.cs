using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコニコの視聴履歴の処理の実装
    /// </summary>
    public class NicoNicoHistoryService : IHistoryService {

        /// <summary>
        /// 視聴履歴を取得するAPI
        /// </summary>
        private const string HistoryApiUrl = "https://nvapi.nicovideo.jp/v1/users/me/watch/history";


        private readonly ISessionService SessionService;

        public NicoNicoHistoryService(ISessionService sessionService) {

            SessionService = sessionService;
        }

        /// <inheritdoc />
        public async Task<List<HistoryEntry>?> GetAccountHistoryAsync() {

            var result = await SessionService.GetAsync(HistoryApiUrl, NicoNicoSessionService.ApiHeaders);

            if (!result.IsSuccessStatusCode) {

                return null;
            }
            
            throw new NotImplementedException();
        }
    }
}
