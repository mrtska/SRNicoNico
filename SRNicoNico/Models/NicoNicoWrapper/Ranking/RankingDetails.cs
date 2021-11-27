using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ランキング情報
    /// </summary>
    public class RankingDetails {
        /// <summary>
        /// 次があるか
        /// </summary>
        public bool HasNext { get; set; }
        /// <summary>
        /// 動画リスト
        /// </summary>
        public IEnumerable<RankingVideoItem> VideoList { get; set; } = default!;
    }

    public class RankingVideoItem : VideoItem {
        /// <summary>
        /// 順位
        /// </summary>
        public int Rank { get; set; }
    }
}
