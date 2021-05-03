using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 「あとで見る」のリスト
    /// </summary>
    public class WatchLaterList {
        /// <summary>
        /// 不可視のアイテムがあるかどうか
        /// 謎
        /// </summary>
        public bool HasInvisibleItems { get; set; }
        /// <summary>
        /// 次のページがあるかどうか
        /// </summary>
        public bool HasNext { get; set; }
        /// <summary>
        /// あとで見るの総数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// あとで見るのリスト
        /// TotalCountと同じ数とは限らない
        /// </summary>
        public IEnumerable<MylistEntry>? Entries { get; set; } 
    }
}
