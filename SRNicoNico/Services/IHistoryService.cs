using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// 視聴履歴を操作する処理を提供するサービス
    /// </summary>
    public interface IHistoryService {

        /// <summary>
        /// アカウントの視聴履歴を取得できる分だけ返す
        /// </summary>
        /// <returns>アカウントの視聴履歴</returns>
        Task<List<HistoryEntry>> GetAccountHistoryAsync();



    }
}
