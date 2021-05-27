namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 作成したDMCのセッション
    /// </summary>
    public class DmcSession {
        /// <summary>
        /// セッションID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 動画URL
        /// </summary>
        public string? ContentUri { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 生のJsonオブジェクト
        /// </summary>
        public string? RawJson { get; set; }

        /// <summary>
        /// API URL
        /// </summary>
        public string? ApiUrl { get; set; }
    }

}
