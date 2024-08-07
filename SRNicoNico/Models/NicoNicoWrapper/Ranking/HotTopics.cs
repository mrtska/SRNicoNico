using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 話題の一覧
    /// </summary>
    public class HotTopics {
        /// <summary>
        /// 要素
        /// </summary>
        public IEnumerable<HotTopicsItem> Items { get; set; } = default!;
        /// <summary>
        /// ジャンル更新日？
        /// </summary>
        public DateTimeOffset StartAt { get; set; }
    }

    public class HotTopicsItem {
        /// <summary>
        /// キー
        /// </summary>
        public string Key { get; set; } = default!;
        /// <summary>
        /// ラベル
        /// </summary>
        public string Label { get; set; } = default!;
        /// <summary>
        /// タグ
        /// </summary>
        public string Tag { get; set; } = default!;
        /// <summary>
        /// ジャンルのキー
        /// </summary>
        public string GenreKey { get; set; } = default!;
        /// <summary>
        /// ジャンルのラベル
        /// </summary>
        public string GenreLabel { get; set; } = default!;
    }
}
