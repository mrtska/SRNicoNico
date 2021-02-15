using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 視聴履歴のPOCO
    /// </summary>
    public class HistoryEntry {

        /// <summary>
        /// 動画ID
        /// </summary>
        public string? VideoId { get; set; }

        /// <summary>
        /// 動画説明文
        /// 短い
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// 動画のサムネイルURL
        /// </summary>
        public string? ThmbnailUrl { get; set; }

        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 動画投稿日時
        /// </summary>
        public DateTime PostedAt { get; set; }

        /// <summary>
        /// 視聴日時
        /// </summary>
        public DateTime WatchedAt { get; set; }

        /// <summary>
        /// 動画の再生数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 動画のコメント数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 動画のマイリスト数
        /// </summary>
        public int MylistCount { get; set; }

        /// <summary>
        /// 視聴回数
        /// </summary>
        public int WatchCount { get; set; }
    }
}
