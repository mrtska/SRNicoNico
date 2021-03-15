namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// チャンネル情報
    /// </summary>
    public class ChannelEntry {

        /// <summary>
        /// チャンネルの税抜価格(多分)
        /// </summary>
        public int BodyPrice { get; set; }

        /// <summary>
        /// チャンネルに入会可能かどうか
        /// </summary>
        public bool CanAdmit { get; set; }

        /// <summary>
        /// チャンネルの説明文
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// チャンネルのID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 成人向けのチャンネルかどうか
        /// </summary>
        public bool IsAdult { get; set; }

        /// <summary>
        /// 無料かどうか
        /// </summary>
        public bool IsFree { get; set; }

        /// <summary>
        /// チャンネル名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// チャンネルの提供者または所有者
        /// </summary>
        public string? OwnerName { get; set; }

        /// <summary>
        /// チャンネルの税込価格(多分)
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// チャンネルのURLの部分
        /// </summary>
        public string? ScreenName { get; set; }

        /// <summary>
        /// チャンネルに入会しているかどうか
        /// </summary>
        public bool IsJoining { get; set; }

        /// <summary>
        /// チャンネルのサムネイルURL(小さい版)
        /// </summary>
        public string? ThumbnailSmallUrl { get; set; }

        /// <summary>
        /// チャンネルのサムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// チャンネルのパーマリンク
        /// </summary>
        public string? Url { get; set; }
    }
}
