using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastEnumUtility;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ランキング関連の処理を提供するサービス
    /// </summary>
    public interface IRankingService {

        /// <summary>
        /// 話題ランキングを取得する
        /// </summary>
        /// <param name="term">集計期間</param>
        /// <param name="genre">ジャンル</param>
        /// <param name="popularTag">人気のタグで絞る場合に指定するキー</param>
        /// <param name="page">ページ</param>
        /// <returns>ランキング情報 genreの指定やpupularTagの指定が間違っていた場合はnull</returns>
        Task<RankingDetails?> GetRankingAsync(RankingTerm term, string genre, string? popularTag = null, int page = 1);

        /// <summary>
        /// 指定したジャンルの人気のタグを取得する
        /// </summary>
        /// <param name="genreKey">ジャンルのキー名</param>
        /// <returns>人気のタグ 指定したジャンルが無効な場合はnull</returns>
        Task<PopularTags?> GetPopularTagsAsync(string genreKey);

        /// <summary>
        /// 指定したレーンIDのランキングを取得する
        /// </summary>
        /// <param name="laneId">レーンID</param>
        /// <returns>ランキング詳細</returns>
        Task<CustomRankingDetails> GetCustomRankingAsync(int laneId);

        /// <summary>
        /// カスタムランキングの設定を取得する
        /// </summary>
        /// <returns>ランキング設定</returns>
        Task<RankingSettings> GetCustomRankingSettingsAsync();

        /// <summary>
        /// 指定したレーンのカスタムランキングの設定を取得する
        /// </summary>
        /// <param name="laneId">レーンID</param>
        /// <returns>ランキング設定</returns>
        Task<RankingSetting> GetCustomRankingSettingAsync(int laneId);

        /// <summary>
        /// 指定したレーンのカスタムランキングの設定をリセットする
        /// </summary>
        /// <param name="laneId">レーンID</param>
        /// <returns>リセット後のランキング設定</returns>
        Task<RankingSetting> ResetCustomRankingSettingAsync(int laneId);

        /// <summary>
        /// 指定したレーンのカスタムランキングの設定を保存する
        /// </summary>
        /// <param name="laneId">レーンID</param>
        /// <returns>保存後のランキング設定</returns>
        Task<RankingSetting> SaveCustomRankingSettingAsync(RankingSettingsEntry setting);

        /// <summary>
        /// 話題ランキングを取得する
        /// </summary>
        /// <param name="term">集計期間</param>
        /// <param name="key">ジャンルで絞る場合に指定するキー 絞らない場合はall</param>
        /// <param name="page">ページ</param>
        /// <returns>ランキング情報 keyの指定が間違っていた場合はnull</returns>
        Task<RankingDetails?> GetHotTopicRankingAsync(RankingTerm term, string key, int page = 1);

        /// <summary>
        /// 話題のジャンルとタグを取得する
        /// </summary>
        /// <returns>話題</returns>
        Task<HotTopics> GetHotTopicsAsync();

        /// <summary>
        /// 現在使われているジャンルを取得する
        /// </summary>
        /// <returns>ジャンルのキーとラベルのリスト</returns>
        Task<Dictionary<string, string>> GetGenresAsync();

        /// <summary>
        /// ランキングのジャンルごとの表示設定を返す
        /// Trueで表示
        /// </summary>
        /// <returns>ジャンルのキーと設定のリスト</returns>
        Task<Dictionary<string, bool>> GetRankingVisibilityAsync();

        /// <summary>
        /// ランキングのジャンル表示設定を保存する
        /// </summary>
        /// <param name="genreKey">ジャンルのキー</param>
        /// <param name="isVisible">表示設定</param>
        Task SaveRankingVisibilityAsync(string genreKey, bool isVisible);
    }

    /// <summary>
    /// ランキングの集計期間
    /// </summary>
    public enum RankingTerm {
        /// <summary>
        /// 毎時ランキング
        /// </summary>
        [Display(Name = "毎時")]
        [Label("hour")]
        Hour,
        /// <summary>
        /// 24時間ランキング
        /// </summary>
        [Display(Name = "24時間")]
        [Label("24h")]
        Day,
        /// <summary>
        /// 週間ランキング
        /// 話題ランキングでは使用不可
        /// </summary>
        [Display(Name = "週間")]
        [Label("week")]
        Week,
        /// <summary>
        /// 月間ランキング
        /// 話題ランキングでは使用不可
        /// </summary>
        [Display(Name = "月間")]
        [Label("month")]
        Month,
        /// <summary>
        /// 全期間ランキング
        /// 話題ランキングでは使用不可
        /// </summary>
        [Display(Name = "全期間")]
        [Label("total")]
        Total
    }
}
