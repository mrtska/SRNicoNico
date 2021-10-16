namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ジャンル情報
    /// </summary>
    public class SearchGenreFacet {
        /// <summary>
        /// ジャンルのキー
        /// </summary>
        public string Key { get; set; } = default!;
        /// <summary>
        /// ジャンルのラベル
        /// </summary>
        public string Label { get; set; } = default!;
        /// <summary>
        /// 動画数
        /// </summary>
        public int Count { get; set; }
    }
}
