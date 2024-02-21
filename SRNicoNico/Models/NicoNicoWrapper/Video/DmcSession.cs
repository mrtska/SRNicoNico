using System;
using System.Net;

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
        /// 動画ID
        /// </summary>
        public string? VideoId { get; set; }
        /// <summary>
        /// オーディオID
        /// </summary>
        public string? AudioId { get; set; }

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

        /// <summary>
        /// セッション作成時間
        /// </summary>
        public DateTimeOffset CreatedTime { get;  set; }

        /// <summary>
        /// セッション有効期限
        /// </summary>
        public DateTimeOffset ExpireTime { get;  set; }

        /// <summary>
        /// DMS用Cookie
        /// </summary>
        public Cookie? DmsCookie { get; set; }
    }

}
