using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// タグ情報
    /// </summary>
    public class TagEntry {

        /// <summary>
        /// ニコニコのユーザーID
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// タグの概要文
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// タグをフォローした時間
        /// </summary>
        public DateTimeOffset FollowedAt { get; set; }
    }
}
