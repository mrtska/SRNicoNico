namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ユーザー情報
    /// </summary>
    public class UserEntry {

        /// <summary>
        /// ニコニコのユーザーID
        /// </summary>
        public string Id { get; set; } = default!;

        /// <summary>
        /// ニコニコのユーザー名
        /// </summary>
        public string NickName { get; set; } = default!;

        /// <summary>
        /// ユーザーのサムネイルのURL
        /// </summary>
        public string ThumbnailUrl { get; set; } = default!;

        /// <summary>
        /// ユーザーの説明文 htmlタグ付き
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// ユーザーの説明文 htmlタグ無し
        /// </summary>
        public string StrippedDescription { get; set; } = default!;

        /// <summary>
        /// プレミアム会員かどうか
        /// </summary>
        public bool IsPremium { get; set; }
    }
}
