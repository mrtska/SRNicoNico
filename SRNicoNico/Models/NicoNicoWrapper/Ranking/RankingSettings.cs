using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// カスタムランキングの設定
    /// </summary>
    public class RankingSettings {

        /// <summary>
        /// 設定のリスト
        /// </summary>
        public IEnumerable<RankingSettingsEntry> Settings { get; set; } = default!;

        /// <summary>
        /// ジャンル情報
        /// </summary>
        public IDictionary<string, string> GenreMap { get; set; } = default!;
    }

    public class RankingSettingsEntry {
        /// <summary>
        /// ランキングのレーンID
        /// </summary>
        public int LaneId { get; set; }
        /// <summary>
        /// レーンタイトル
        /// </summary>
        public string Title { get; set; } = default!;
        /// <summary>
        /// ランキングタイプ tagかgenre
        /// </summary>
        public string Type { get; set; } = default!;
        /// <summary>
        /// ランキングを構成しているジャンル
        /// Typeがgenreの時のみ有効
        /// </summary>
        public IEnumerable<string>? GenreKeys { get; set; }
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
    }
}
