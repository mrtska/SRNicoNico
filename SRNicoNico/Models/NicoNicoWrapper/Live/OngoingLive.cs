namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 放送中の生放送
    /// </summary>
    public class OngoingLive {
        /// <summary>
        /// コミュニティ名 (チャンネル名の時もある)
        /// </summary>
        public string CommunityName { get; set; } = default!;
        /// <summary>
        /// 経過時間
        /// </summary>
        public int ElapsedTime { get; set; }
        /// <summary>
        /// 生放送ID
        /// </summary>
        public string Id { get; set; } = default!;
        /// <summary>
        /// community, channel, officialのどれか
        /// </summary>
        public string ProviderType { get; set; } = default!;
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string Thumbnailurl { get; set; } = default!;
        /// <summary>
        /// 生放送URL
        /// </summary>
        public string ThumbnailLinkUrl { get; set; } = default!;
        /// <summary>
        /// 生放送タイトル
        /// </summary>
        public string Title { get; set; } = default!;
    }
}
