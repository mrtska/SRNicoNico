using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 動画情報APIのデータ
    /// </summary>
    public class WatchApiData {
        /// <summary>
        /// コメント関連のデータ
        /// </summary>
        public WatchApiDataComment? Comment { get; set; }

        /// <summary>
        /// かんたんコメントのフレーズリスト
        /// </summary>
        public IEnumerable<EasyCommentPhrase>? EasyCommentPhrases { get; set; }

        /// <summary>
        /// ジャンル情報
        /// </summary>
        public WatchApiDataGenre? Genre { get; set; }

        /// <summary>
        /// DMC関連
        /// </summary>
        public WatchApiDataMedia? Media { get; set; }

        public string? OkReason { get; set; }

        /// <summary>
        /// 投稿者の情報
        /// </summary>
        public WatchApiDataOwner? Owner { get; set; }
        /// <summary>
        /// チャンネル情報
        /// </summary>
        public WatchApiDataChannel? Channel { get; set; }

        /// <summary>
        /// 動画プレイヤーの情報
        /// </summary>
        public WatchApiDataPlayer? Player { get; set; }
        
        /// <summary>
        /// ランキング情報
        /// </summary>
        public WatchApiDataRanking? Ranking { get; set; }

        /// <summary>
        /// シリーズ情報
        /// nullの場合もある
        /// </summary>
        public WatchApiDataSeries? Series { get; set; }

        /// <summary>
        /// タグ情報
        /// </summary>
        public WatchApiDataTag? Tag { get; set; }

        /// <summary>
        /// 動画情報
        /// </summary>
        public WatchApiDataVideo Video { get; set; } = default!;

        /// <summary>
        /// 視聴者情報
        /// </summary>
        public WatchApiDataViewer? Viewer { get; set; }
    }

    public class WatchApiDataComment {
        /// <summary>
        /// コメントサーバのURL
        /// </summary>
        public string? ServerUrl { get; set; }

        /// <summary>
        /// ユーザーキー
        /// </summary>
        public string? UserKey { get; set; }

        /// <summary>
        /// コメントレイヤーのリスト
        /// </summary>
        public IEnumerable<CommentLayer>? Layers { get; set; }

        /// <summary>
        /// スレッドのリスト
        /// </summary>
        public IEnumerable<CommentThread>? Threads { get; set; }

        /// <summary>
        /// 動画の長さ
        /// </summary>
        internal int VideoDuration { get; set; }

        /// <summary>
        /// 視聴者のユーザーID
        /// </summary>
        internal string? UserId { get; set; }
    }

    public class CommentLayer {
        /// <summary>
        /// レイヤーインデックス
        /// 数字が小さいものほど上に表示される
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 半透明かどうか
        /// </summary>
        public bool IsTranslucent { get; set; }
        /// <summary>
        /// スレッドIDのリスト
        /// </summary>
        public IEnumerable<CommentLayerThreadId>? ThreadIds { get; set; }
    }
    public class CommentLayerThreadId {
        /// <summary>
        /// スレッドID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// スレッドの種類的なもの
        /// 0: 通常コメント
        /// 1: 投稿者コメント
        /// 2: 簡単コメント
        /// </summary>
        public int Fork { get; set; }
    }
    public class CommentThread {
        /// <summary>
        /// スレッドID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// スレッドの種類
        /// </summary>
        public int Fork { get; set; }
        /// <summary>
        /// スレッドが有効かどうか
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 通常のコメントが投稿されるスレッドかどうか
        /// </summary>
        public bool IsDefaultPostTarget { get; set; }
        /// <summary>
        /// かんたんコメント用のスレッドかどうか
        /// </summary>
        public bool IsEasyCommentPostTarget { get; set; }
        /// <summary>
        /// 葉が必要か 良く分からん
        /// </summary>
        public bool IsLeafRequired { get; set; }
        /// <summary>
        /// 投稿者コメント用のスレッドかどうか
        /// </summary>
        public bool IsOwnerThread { get; set; }
        /// <summary>
        /// スレッドキーが必要かどうか
        /// チャンネルコメントなどで必要
        /// </summary>
        public bool IsThreadKeyRequired { get; set; }
        /// <summary>
        /// スレッドキー
        /// </summary>
        public string? ThreadKey { get; set; }
        /// <summary>
        /// 184コメントを強制されるスレッドかどうか
        /// </summary>
        public bool Is184Forced { get; set; }
        /// <summary>
        /// ニコスクリプトを持てるスレッドかどうか
        /// </summary>
        public bool HasNicoscript { get; set; }
        /// <summary>
        /// ラベル
        /// default, easy, ownerなど
        /// </summary>
        public string Label { get; set; } = default!;
        /// <summary>
        /// ポストキーステータス
        /// </summary>
        public int PostKeyStatus { get; set; }
        /// <summary>
        /// コメントサーバ
        /// </summary>
        public string? Server { get; set; }
    }

    public class EasyCommentPhrase {
        /// <summary>
        /// かんたんコメントのテキスト
        /// </summary>
        public string Text { get; set; } = default!;
        /// <summary>
        /// 大百科のタイトル
        /// </summary>
        public string? NicoDicTitle { get; set; }
        /// <summary>
        /// 大百科の表示タイトル
        /// </summary>
        public string? NicoDicViewTitle { get; set; }
        /// <summary>
        /// 大百科の概要
        /// </summary>
        public string? NicoDicSummary { get; set; }
        /// <summary>
        /// 大百科へのリンク
        /// </summary>
        public string? NicoDicLink { get; set; }
    }

    public class WatchApiDataGenre {
        /// <summary>
        /// ジャンル
        /// </summary>
        public string? Key { get; set; }
        /// <summary>
        /// ジャンル名
        /// </summary>
        public string? Label { get; set; }
        /// <summary>
        /// R18関連？
        /// </summary>
        public bool IsImmoral { get; set; }
        /// <summary>
        /// ジャンルが無効かどうか 多分
        /// </summary>
        public bool IsDisabled { get; set; }
        /// <summary>
        /// ジャンル未設定かどうか 多分
        /// </summary>
        public bool IsNotSet { get; set; }
    }

    public class WatchApiDataMedia {
        /// <summary>
        /// レシピID
        /// </summary>
        public string? RecipeId { get; set; }
        /// <summary>
        /// 暗号化関連
        /// </summary>
        public MediaEncryption? Encryption { get; set; }
        /// <summary>
        /// 動画関連
        /// </summary>
        public MediaMovie? Movie { get; set; }
        /// <summary>
        /// ストーリーボード関連
        /// 現時点ではプレミアム会員のみ
        /// </summary>
        public MediaStoryBoard? StoryBoard { get; set; }

        public string TrackingId { get; set; } = default!;
    }

    public class MediaEncryption {
        /// <summary>
        /// 暗号化キー
        /// </summary>
        public string EncryptedKey { get; set; } = default!;
        /// <summary>
        /// 鍵のURI
        /// </summary>
        public string KeyUri { get; set; } = default!;
    }

    public class MediaMovie {
        public string? ContentId { get; set; }

        public IEnumerable<MediaMovieAudio>? Audios { get; set; }
        public IEnumerable<MediaMovieVideo>? Videos { get; set; }

        public MediaSession? Session { get; set; }
    }
    public class MediaMovieAudio {
        public string? Id { get; set; }
        public bool IsAvailable { get; set; }
        public int Bitrate { get; set; }
        public int SamplingRate { get; set; }
        public double IntegratedLoudness { get; set; }
        public double TruePeak { get; set; }
        public int LevelIndex { get; set; }

        public IDictionary<string, double>? LoudnessCollection { get; set; }
    }
    public class MediaMovieVideo {
        public string? Id { get; set; }
        public bool IsAvailable { get; set; }
        public string? Label { get; set; }
        public int Bitrate { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int LevelIndex { get; set; }
        public int RecommendedHighestAudioLevelIndex { get; set; }
    }

    public class MediaStoryBoard {
        public string ContentId { get; set; } = default!;

        public IEnumerable<string>? ImageIds { get; set; }

        public MediaSession Session { get; set; } = default!;
    }

    public class MediaSession {
        public string? RecipeId { get; set; }
        public string? PlayerId { get; set; }
        public IEnumerable<string>? Videos { get; set; }
        public IEnumerable<string>? Audios { get; set; }
        public IEnumerable<string>? Movies { get; set; }
        public IEnumerable<string>? Protocols { get; set; }

        public string? AuthTypesHttp { get; set; }
        public string? AuthTypesHls { get; set; }
        public string? AuthTypesStoryBoard { get; set; }

        public string? ServiceUserId { get; set; }
        public string? Token { get; set; }
        public string? Signature { get; set; }
        public string? ContentId { get; set; }
        public int HeartbeatLifetime { get; set; }
        public int ContentKeyTimeout { get; set; }
        public double Priority { get; set; }
        public IEnumerable<string>? TransferPresets { get; set; }

        public IEnumerable<MediaSessionUrl>? Urls { get; set; }
    }

    public class MediaSessionUrl {
        public string? Url { get; set; }
        public bool IsWellKnownPort { get; set; }
        public bool IsSsl { get; set; }
    }

    public class WatchApiDataOwner {
        /// <summary>
        /// 投稿者のユーザID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 投稿者名
        /// </summary>
        public string? Nickname { get; set; }
        /// <summary>
        /// 投稿者のサムネイルURL
        /// </summary>
        public string? IconUrl { get; set; }

        public object? Channel { get; set; }

        public object? Live { get; set; }
        /// <summary>
        /// 投稿動画リストが公開されているかどうか
        /// </summary>
        public bool IsVideosPublic { get; set; }
        /// <summary>
        /// マイリストを公開しているかどうか
        /// </summary>
        public bool IsMylistsPublic { get; set; }

        public object? VideoLiveNotice { get; set; }
        /// <summary>
        /// フォローしているかどうか
        /// </summary>
        public bool IsFollowing { get; set; }
    }

    public class WatchApiDataChannel {
        /// <summary>
        /// チャンネルID
        /// </summary>
        public string Id { get; set; } = default!;

        public bool IsDisplayAdBanner { get; set; }
        public bool IsOfficialAnime { get; set; }

        /// <summary>
        /// チャンネル名
        /// </summary>
        public string Name { get; set; } = default!;

        public string ThumbnailUrl { get; set; } = default!;
        /// <summary>
        /// フォローしているかどうか
        /// </summary>
        public bool IsBookmarked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsFollowed { get; set; }

        public string Token { get; set; } = default!;
        public double TokenTimestamp { get; set; }
    }

    public class WatchApiDataPlayer {
        /// <summary>
        /// 再生位置保存タイプ
        /// </summary>
        public string? InitialPlaybackType { get; set; }
        /// <summary>
        /// 再生位置 秒
        /// </summary>
        public int? InitialPlaybackPositionSec { get; set; }
    }

    public class WatchApiDataRanking {
        /// <summary>
        /// ランキング順位
        /// </summary>
        public int? Rank { get; set; }
        /// <summary>
        /// ランキングのジャンル
        /// </summary>
        public string? Genre { get; set; }
        /// <summary>
        /// 日付
        /// </summary>
        public DateTimeOffset? DateTime { get; set; }
        /// <summary>
        /// その他の有名なタグのそれぞれのランキング
        /// </summary>
        public IEnumerable<WatchApiDataRankingPopularTag>? PopularTag { get; set; }
    }
    public class WatchApiDataRankingPopularTag {
        /// <summary>
        /// タグ名
        /// </summary>
        public string? Tag { get; set; }
        /// <summary>
        /// 一般化されたタグ名
        /// </summary>
        public string? RegularizedTag { get; set; }
        /// <summary>
        /// ランキング順位
        /// </summary>
        public int Rank { get; set; }
        /// <summary>
        /// タグのジャンル
        /// </summary>
        public string? Genre { get; set; }
        /// <summary>
        /// 日付
        /// </summary>
        public DateTimeOffset DateTime { get; set; }
    }

    public class WatchApiDataSeries {
        /// <summary>
        /// シリーズのID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// シリーズ名
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// シリーズの説明
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// シリーズのサムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }
        /// <summary>
        /// シリーズの前の動画
        /// </summary>
        public WatchApiDataSeriesVideo? Prev { get; set; }
        /// <summary>
        /// シリーズの次の動画
        /// </summary>
        public WatchApiDataSeriesVideo? Next { get; set; }
        /// <summary>
        /// シリーズの最初の動画
        /// </summary>
        public WatchApiDataSeriesVideo? First { get; set; }
    }
    public class WatchApiDataSeriesVideo {
        /// <summary>
        /// 動画ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// 動画投稿日時
        /// </summary>
        public DateTimeOffset RegisteredAt { get; set; }
        
        /// <summary>
        /// 再生数
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
        /// 良いね数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailMiddleUrl { get; set; }
        public string? ThumbnailLargeUrl { get; set; }
        public string? ThumbnailListingUrl { get; set; }
        public string? ThumbnailnHdUrl { get; set; }

        /// <summary>
        /// 動画の長さ
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 動画説明文
        /// </summary>
        public string? ShortDescription { get; set; }
        /// <summary>
        /// 最近のコメント
        /// </summary>
        public string? LatestCommentSummary { get; set; }
        /// <summary>
        /// チャンネル動画かどうか
        /// </summary>
        public bool IsChannelVideo { get; set; }
        /// <summary>
        /// 課金が必要な動画かどうか
        /// </summary>
        public bool IsPaymentRequired { get; set; }
        /// <summary>
        /// 保存された再生位置
        /// </summary>
        public int? PlaybackPosition { get; set; }
        /// <summary>
        /// 投稿者のサムネイルURL
        /// </summary>
        public string? OwnerIconUrl { get; set; }
        /// <summary>
        /// 投稿者のID
        /// </summary>
        public string? OwnerId { get; set; }
        /// <summary>
        /// 投稿者名
        /// </summary>
        public string? OwnerName { get; set; }
        /// <summary>
        /// 投稿者タイプ
        /// </summary>
        public string? OwnerType { get; set; }
        /// <summary>
        /// センシティブな内容でマスクする必要がある動画かどうか
        /// </summary>
        public bool RequireSensitiveMasking { get; set; }
    }

    public class WatchApiDataTag {
        /// <summary>
        /// タグのリスト
        /// </summary>
        public IEnumerable<WatchApiDataTagItem>? Items { get; set; }

        /// <summary>
        /// R18タグを含むかどうか
        /// </summary>
        public bool HasR18Tag { get; set; }

        public bool IsPublishedNicoscript { get; set; }
        public WatchApiDataTagEdit? Edit { get; set; }
        public WatchApiDataTagEdit? Viewer { get; set; }
    }
    public class WatchApiDataTagItem {
        /// <summary>
        /// タグ名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// カテゴリかどうか
        /// </summary>
        public bool IsCategory { get; set; }
        /// <summary>
        /// カテゴリ候補かどうか
        /// </summary>
        public bool IsCategoryCandidate { get; set; }
        /// <summary>
        /// ニコニコ大百科に記事が存在するかどうか
        /// </summary>
        public bool IsNicodicArticleExists { get; set; }
        /// <summary>
        /// タグロックされているかどうか
        /// </summary>
        public bool IsLocked { get; set; }
    }
    public class WatchApiDataTagEdit {
        /// <summary>
        /// タグを編集可能かどうか
        /// </summary>
        public bool IsEditable { get; set; }
        /// <summary>
        /// 編集できない場合の理由
        /// </summary>
        public object? UneditableReason { get; set; }
        public string? EditKey { get; set; }
    }

    public class WatchApiDataVideo {
        /// <summary>
        /// 動画ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// 短い動画説明文
        /// </summary>
        public string? ShortDescription { get; set; }
        /// <summary>
        /// 動画説明文
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 再生数
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
        /// 良いね数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 動画の長さ
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailMiddleUrl { get; set; }
        public string? ThumbnailLargeUrl { get; set; }
        public string? ThumbnailPlayer { get; set; }
        public string? ThumbnailOgp { get; set; }
        
        public bool IsAdult { get; set; }
        /// <summary>
        /// 動画投稿日時
        /// </summary>
        public DateTimeOffset RegisteredAt { get; set; }
        /// <summary>
        /// 非公開動画かどうか
        /// </summary>
        public bool IsPrivate { get; set; }
        /// <summary>
        /// 削除された動画かどうか
        /// </summary>
        public bool IsDeleted { get; set; }
        public bool IsNoBanner { get; set; }
        public bool IsAuthenticationRequired { get; set; }
        /// <summary>
        /// 外部プレイヤー埋め込みが許可された動画かどうか
        /// </summary>
        public bool IsEmbedPlayerAllowed { get; set; }

        public WatchApiDataVideoViewer? Viewer { get; set; }

        public string? WatchableUserTypeForPayment { get; set; }
        public string? CommentableUserTypeForPayment { get; set; }
    }

    public class WatchApiDataVideoViewer {

        public bool IsOwner { get; set; }
        public bool IsLiked { get; set; }
        public int? LikeCount { get; set; }
    }

    /// <summary>
    /// 視聴者情報
    /// </summary>
    public class WatchApiDataViewer {
        /// <summary>
        /// ユーザーID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// ニックネーム
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// プレミアム会員かどうか
        /// </summary>
        public bool IsPremium { get; set; }
    }
}
