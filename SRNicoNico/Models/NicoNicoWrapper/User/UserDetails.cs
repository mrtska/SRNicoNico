namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ユーザー情報詳細
    /// </summary>
    public class UserDetails {
        /// <summary>
        /// ユーザーID
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// 説明文
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// HTMLタグを削除した説明文
        /// </summary>
        public string? StrippedDescription { get; set; }
        /// <summary>
        /// プレミアム会員かどうか
        /// </summary>
        public bool IsPremium { get; set; }
        /// <summary>
        /// ニコニコにアカウント登録した時のバージョン
        /// </summary>
        public string? RegisteredVersion { get; set; }
        /// <summary>
        /// フォローしている数
        /// </summary>
        public int FolloweeCount { get; set; }
        /// <summary>
        /// フォローされている数
        /// </summary>
        public int FollowerCount { get; set; }
        /// <summary>
        /// レベル
        /// </summary>
        public int CurrentLevel { get; set; }
        /// <summary>
        /// 次のレベルになるために必要な経験値量
        /// </summary>
        public int NextLevelThresholdExperience { get; set; }
        /// <summary>
        /// 次のレベルになるために必要な経験値量
        /// </summary>
        public int NextLevelExperience { get; set; }
        /// <summary>
        /// 現在の経験値量
        /// </summary>
        public int CurrentLevelExperience { get; set; }
        /// <summary>
        /// ユーザーチャンネル
        /// 常にnullっぽい
        /// </summary>
        public object? UserChannel { get; set; }
        /// <summary>
        /// ニコレポが公開されているかどうか
        /// </summary>
        public bool IsNicorepoReadable { get; set; }
        /// <summary>
        /// ユーザー名
        /// </summary>
        public string? Nickname { get; set; }
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailLargeUrl { get; set; }
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailSmallUrl { get; set; }
        /// <summary>
        /// 自分がこのユーザーをフォローしているかどうか
        /// </summary>
        public bool IsFollowing { get; set; }
        /// <summary>
        /// このユーザーが自分かどうか
        /// </summary>
        public bool IsMe { get; set; }
    }
}
