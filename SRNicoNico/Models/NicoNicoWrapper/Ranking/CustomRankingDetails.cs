using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// カスタムランキング情報
    /// </summary>
    public class CustomRankingDetails : RankingDetails {
        /// <summary>
        /// レーンID
        /// </summary>
        public int LaneId { get; set; }
        /// <summary>
        /// レーンタイプ custom固定のはず
        /// </summary>
        public string LaneType { get; set; } = default!;
        /// <summary>
        /// カスタムランキング名
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// ランキングタイプ tagかgenre
        /// </summary>
        public string? CustomType { get; set; }
        /// <summary>
        /// ランキングを構成しているジャンル
        /// Typeがgenreの時のみ有効
        /// </summary>
        public IDictionary<string, string>? Genres { get; set; }
        /// <summary>
        /// ランキングを構成しているタグ
        /// Typeがtagの時のみ有効
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }
        /// <summary>
        /// チャンネル動画をランキングに含むかどうか
        /// </summary>
        public string? ChannelVideoListingStatus { get; set; }
        /// <summary>
        /// デフォルトで用意されているランキングかどうか
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// デフォルトのランキング名
        /// </summary>
        public string? DefaultTitle { get; set; }
    }
}
