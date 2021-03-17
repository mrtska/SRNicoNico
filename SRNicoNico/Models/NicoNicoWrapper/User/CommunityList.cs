using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// コミュニティ情報のリスト
    /// </summary>
    public class CommunityList {

        /// <summary>
        /// フォローしているコミュニティ数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 取得しているページ
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// コミュニティリスト
        /// </summary>
        public IEnumerable<CommunityEntry>? Entries { get; set; }
    }
}
