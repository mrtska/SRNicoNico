using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// カスタムランキングの設定
    /// </summary>
    public class RankingSetting {

        /// <summary>
        /// 設定
        /// </summary>
        public RankingSettingsEntry Setting { get; set; } = default!;

        /// <summary>
        /// ジャンル情報
        /// </summary>
        public IDictionary<string, string> GenreMap { get; set; } = default!;
    }

}
