using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 人気のタグ
    /// </summary>
    public class PopularTags {
        /// <summary>
        /// 人気のタグ
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = default!;
        /// <summary>
        /// 人気のタグ更新日？
        /// </summary>
        public DateTimeOffset StartAt { get; set; }
    }
}
