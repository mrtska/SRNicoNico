using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// あとで見るやマイリストの動画情報
    /// </summary>
    public class MylistEntry {
        /// <summary>
        /// あとで見るやマイリストに追加された日時
        /// </summary>
        public DateTimeOffset AddedAt { get; set; }
        /// <summary>
        /// アイテムID
        /// </summary>
        public string? ItemId { get; set; }
        /// <summary>
        /// メモ
        /// マイリストコメント的なもの
        /// </summary>
        public string? Memo { get; set; }
        /// <summary>
        /// ステータス
        /// public, hidden, memberOnly それ以外もあるかも
        /// </summary>
        public string? Status { get; set; }

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
        /// essential
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// 動画のIDかな？
        /// </summary>
        public string? WatchId { get; set; }
        /// <summary>
        /// 動画URL
        /// </summary>
        public string? VideoUrl { get; set; }
    }
}
