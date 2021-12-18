using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// タグ情報
    /// </summary>
    public class TagEntry {

        /// <summary>
        /// タグ名
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// タグの概要文
        /// </summary>
        public string Summary { get; set; } = default!;

        /// <summary>
        /// タグをフォローした時間
        /// </summary>
        public DateTimeOffset FollowedAt { get; set; }
    }
}
