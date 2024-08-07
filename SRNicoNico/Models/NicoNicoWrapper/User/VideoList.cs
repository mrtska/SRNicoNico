using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ユーザーの投稿動画
    /// </summary>
    public class VideoList {
        /// <summary>
        /// 動画リスト
        /// </summary>
        public IEnumerable<VideoListItem>? Items { get; set; }
        /// <summary>
        /// 投稿動画数
        /// </summary>
        public int TotalCount { get; set; }
    }

    public class VideoListItem {
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
        /// いいね数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 動画の長さ 秒単位
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 動画ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// チャンネル動画かどうか
        /// </summary>
        public bool IsChannelVideo { get; set; }
        /// <summary>
        /// 要課金動画かどうか
        /// </summary>
        public bool IsPaymentRequired { get; set; }
        /// <summary>
        /// 最新コメント
        /// </summary>
        public string? LatestCommentSummary { get; set; }
        /// <summary>
        /// 動画投稿者のサムネイルURL
        /// </summary>
        public string? OwnerIconUrl { get; set; }
        /// <summary>
        /// 動画投稿者のID
        /// </summary>
        public string? OwnerId { get; set; }
        /// <summary>
        /// 動画投稿者の名前
        /// </summary>
        public string? OwnerName { get; set; }
        /// <summary>
        /// 動画投稿者の種類
        /// userかchannelかhidden
        /// </summary>
        public string? OwnerType { get; set; }
        /// <summary>
        /// 再生位置
        /// </summary>
        public int? PlaybackPosition { get; set; }
        /// <summary>
        /// 動画投稿日
        /// </summary>
        public DateTimeOffset RegisteredAt { get; set; }
        /// <summary>
        /// マスクが必要か
        /// 良く分からん
        /// </summary>
        public bool RequireSensitiveMasking { get; set; }
        /// <summary>
        /// 動画説明文の一部
        /// </summary>
        public string? ShortDescription { get; set; }
        /// <summary>
        /// 動画のサムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }
        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// シリーズID
        /// </summary>
        public string? SeriesId { get; set; }
        /// <summary>
        /// シリーズの中の位置
        /// </summary>
        public int? SeriesOrder { get; set; }
        /// <summary>
        /// シリーズ名
        /// </summary>
        public string? SeriesTitle { get; set; }
    }
}
