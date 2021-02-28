using System;
using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities {
    /// <summary>
    /// ローカル視聴履歴テーブル
    /// </summary>
    public class LocalHistory {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// 動画ID 主キー
        /// </summary>
        [Key]
        public string VideoId { get; set; }

        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 動画説明文
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// 動画のサムネイルURL
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 動画の長さ 秒単位
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 動画視聴回数
        /// </summary>
        public int WatchCount { get; set; }

        /// <summary>
        /// 動画投稿日時
        /// </summary>
        public DateTime PostedAt { get; set; }

        /// <summary>
        /// 最終視聴日 INDEX
        /// </summary>
        public DateTime LastWatchedAt { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
