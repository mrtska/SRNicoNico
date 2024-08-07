using System.Collections.Generic;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// 生放送関連の処理を提供するサービス
    /// </summary>
    public interface ILiveService {

        /// <summary>
        /// 現在放送中の生放送を取得する
        /// </summary>
        /// <returns>放送中の生放送のリスト</returns>
        Task<IEnumerable<OngoingLive>> GetOngoingLivesAsync();
    }
}
