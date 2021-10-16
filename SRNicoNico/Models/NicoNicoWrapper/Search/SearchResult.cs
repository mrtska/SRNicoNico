using System.Collections.Generic;
using SRNicoNico.Services;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class SearchResult {
        /// <summary>
        /// タグかキーワードか
        /// </summary>
        public SearchType SearchType { get; set; }
        /// <summary>
        /// 検索タグ
        /// </summary>
        public string Value { get; set; } = default!;
        /// <summary>
        /// 検索結果の数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 次のページがあるかどうか
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// 検索結果
        /// </summary>
        public IEnumerable<VideoListItem> Items { get; set; } = default!;
    }
}
