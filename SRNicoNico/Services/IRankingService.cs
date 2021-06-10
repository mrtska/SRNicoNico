using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ランキング関連の処理を提供するサービス
    /// </summary>
    public interface IRankingService {

        /// <summary>
        /// 指定したレーンIDのランキングを取得する
        /// </summary>
        /// <param name="laneId">レーンID</param>
        /// <returns>ランキング詳細</returns>
        Task<RankingDetails> GetCustomRankingAsync(int laneId);

        /// <summary>
        /// カスタムランキングの設定を取得する
        /// </summary>
        /// <returns>ランキング設定</returns>
        Task<RankingSettings> GetCustomRankingSettingsAsync();

    }
}
