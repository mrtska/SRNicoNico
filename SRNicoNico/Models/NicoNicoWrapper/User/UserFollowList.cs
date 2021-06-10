using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// フォローしている/されているユーザーのリスト
    /// </summary>
    public class UserFollowList {
        /// <summary>
        /// ユーザーリスト
        /// </summary>
        public IEnumerable<UserFollowItem>? Items { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string? Cursor { get; set; }
        /// <summary>
        /// フォローしている数
        /// </summary>
        public int Followees { get; set; }
        /// <summary>
        /// フォローされている数
        /// </summary>
        public int Followers { get; set; }
        /// <summary>
        /// 次があるかどうか
        /// </summary>
        public bool HasNext { get; set; }
    }

    public class UserFollowItem {
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
        /// ユーザー名
        /// </summary>
        public string? Nickname { get; set; }
        /// <summary>
        /// プレミアム会員かどうか
        /// </summary>
        public bool IsPremium { get; set; }
        /// <summary>
        /// 自分がフォローしているかどうか
        /// </summary>
        public bool IsFollowing { get; set; }
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailLargeUrl { get; set; }
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailSmallUrl { get; set; }
    }
}
