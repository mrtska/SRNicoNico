using System.Collections.Generic;
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

        /// <summary>
        /// 話題のジャンルとタグを取得する
        /// </summary>
        /// <returns>話題</returns>
        Task<HotTopics> GetHotTopicsAsync();

        /// <summary>
        /// 指定したジャンルの人気のタグを取得する
        /// </summary>
        /// <param name="genreKey">ジャンルのキー名</param>
        /// <returns>人気のタグ 指定したジャンルが無効な場合はnull</returns>
        Task<PopularTags?> GetPopularTagsAsync(string genreKey);

        /// <summary>
        /// 現在使われているジャンルを取得する
        /// </summary>
        /// <returns>ジャンルのキーとラベルのリスト</returns>
        Task<Dictionary<string, string>> GetGenresAsync();
    }
}
