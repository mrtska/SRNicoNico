using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ユーザー情報のリスト
    /// </summary>
    public class UserList {

        /// <summary>
        /// フォローしているユーザー数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 取得しているページ
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// ユーザーリスト
        /// </summary>
        public IEnumerable<UserEntry>? Entries { get; set; }
    }
}
