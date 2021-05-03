using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ユーザーがフォローしているマイリスト情報
    /// </summary>
    public class UserMylistEntry {

        /// <summary>
        /// マイリストの公開状態
        /// ステータスがPublic以外の場合はId以外の全てのプロパティは意味を成さない
        /// </summary>
        public MylistStatus Status { get; set; }

        /// <summary>
        /// マイリストのID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// マイリストの作成日時
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// マイリストのソートルール
        /// addedAt固定っぽい
        /// </summary>
        public string? DefaultSortKey { get; set; }

        /// <summary>
        /// マイリストのソート順
        /// asc固定っぽい
        /// </summary>
        public string? DefaultSortOrder { get; set; }

        /// <summary>
        /// マイリストの説明文
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// マイリストをフォローしているユーザーの数
        /// </summary>
        public int FollowerCount { get; set; }

        /// <summary>
        /// マイリストをフォローしているか(フォローしているマイリストを取得してきているんだからTrue以外有り得ないと思うが、、、)
        /// </summary>
        public bool IsFollowing { get; set; }

        /// <summary>
        /// マイリストが公開されているか(公開されていなかったらId以外の値が取れなくなるからTrue以外有り得ないと思う)
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// マイリストに含まれている動画の数
        /// </summary>
        public int ItemsCount { get; set; }
        
        /// <summary>
        /// マイリスト名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// マイリスト所有者のサムネイルURL
        /// </summary>
        public string? OwnerThumbnailUrl { get; set; }

        /// <summary>
        /// マイリスト所有者のID
        /// </summary>
        public string? OwnerId { get; set; }

        /// <summary>
        /// マイリスト所有者名
        /// </summary>
        public string? OwnerName { get; set; }

        /// <summary>
        /// マイリスト所有者の種類
        /// user以外になにかあるかな
        /// </summary>
        public string? OwnerType { get; set; }

        /// <summary>
        /// マイリスト内にある動画のサンプル
        /// </summary>
        public IEnumerable<MylistSampleVideo>? SampleItems { get; set; }
    }

    /// <summary>
    /// マイリストの動画サンプル
    /// </summary>
    public class MylistSampleVideo {

        /// <summary>
        /// マイリストに登録された日時
        /// </summary>
        public DateTimeOffset AddedAt { get; set; }

        /// <summary>
        /// 動画に付けられたマイリストメモ
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// マイリスト上の動画に付けられたID 動画IDとは別物
        /// </summary>
        public string? ItemId { get; set; }

        /// <summary>
        /// 動画のステータス
        /// </summary>
        public SampleVideoStatus Status { get; set; }

        /// <summary>
        /// 動画再生数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// コメント数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// マイリスト数
        /// </summary>
        public int MylistCount { get; set; }

        /// <summary>
        /// いいね数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 動画の長さ 動画のステータスがHiddenの時は0になるっぽい
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
        /// 有料動画かどうか
        /// </summary>
        public bool IsPaymentRequired { get; set; }

        /// <summary>
        /// 最近のコメント
        /// </summary>
        public string? LatestCommentSummary { get; set; }

        /// <summary>
        /// 動画投稿者のサムネイルURL
        /// </summary>
        public string? OwnerThumbnailUrl { get; set; }

        /// <summary>
        /// 動画投稿者のID
        /// </summary>
        public string? OwnerId { get; set; }

        /// <summary>
        /// 動画投稿者の種類
        /// </summary>
        public SampleVideoOwnerType OwnerType { get; set; }

        /// <summary>
        /// 動画再生位置 nullの場合は未再生
        /// </summary>
        public int? PlaybackPosition { get; set; }

        /// <summary>
        /// 動画投稿日時
        /// </summary>
        public DateTimeOffset RegisteredAt { get; set; }

        /// <summary>
        /// なんだろうこれ
        /// </summary>
        public bool RequireSensitiveMasking { get; set; }

        /// <summary>
        /// 動画説明文
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
        /// essential以外なにかあるかな
        /// </summary>
        public string? Type { get; set; }
    }

    public enum SampleVideoOwnerType {
        /// <summary>
        /// 動画投稿者が一般ユーザー
        /// </summary>
        User,
        /// <summary>
        /// 動画投稿者がチャンネル
        /// </summary>
        Channel,
        /// <summary>
        /// 非公開動画の場合
        /// </summary>
        Hidden
    }

    public enum SampleVideoStatus {
        /// <summary>
        /// 公開動画
        /// </summary>
        Public,
        /// <summary>
        /// 非公開動画
        /// </summary>
        Hidden,
        /// <summary>
        /// 削除された動画
        /// </summary>
        Deleted,
        /// <summary>
        /// コミュ限動画
        /// </summary>
        MemberOnly
    }


    public enum MylistStatus {
        /// <summary>
        /// 公開されているマイリスト
        /// </summary>
        Public,
        /// <summary>
        /// 非公開のマイリスト
        /// </summary>
        Private,
        /// <summary>
        /// 削除されたマイリスト
        /// </summary>
        Deleted
    }
}
