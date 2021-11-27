using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 各APIで取得出来る動画情報の共通部分
    /// </summary>
    public class VideoItem {
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
        public string Id { get; set; } = default!;
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
        public string OwnerType { get; set; } = default!;
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
        public string ShortDescription { get; set; } = default!;
        /// <summary>
        /// 動画のサムネイルURL
        /// </summary>
        public string ThumbnailUrl { get; set; } = default!;
        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// jsonから値を取り出す
        /// </summary>
        /// <param name="video">json</param>
        /// <returns>this</returns>
        public virtual VideoItem Fill(dynamic video) {

            CommentCount = (int)video.count.comment;
            LikeCount = (int)video.count.like;
            MylistCount = (int)video.count.mylist;
            ViewCount = (int)video.count.view;
            Duration = (int)video.duration;
            Id = video.id;
            IsChannelVideo = video.isChannelVideo;
            IsPaymentRequired = video.isPaymentRequired;
            LatestCommentSummary = video.latestCommentSummary;
            OwnerIconUrl = video.owner.iconUrl;
            OwnerId = video.owner.id;
            OwnerName = video.owner.name;
            OwnerType = video.owner.ownerType;
            PlaybackPosition = (int?)video.playbackPosition;
            RegisteredAt = DateTimeOffset.Parse(video.registeredAt);
            RequireSensitiveMasking = video.requireSensitiveMasking;
            ShortDescription = video.shortDescription;
            ThumbnailUrl = video.thumbnail.listingUrl;
            Title = video.title;

            return this;
        }
    }
}
