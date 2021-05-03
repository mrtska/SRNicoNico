using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// マイリストの情報
    /// </summary>
    public class Mylist {
        /// <summary>
        /// デフォルトの並び順のキー
        /// </summary>
        public string? DefaultSortKey { get; set; }
        /// <summary>
        /// デフォルトの並び順
        /// ascかdesc
        /// </summary>
        public string? DefaultSortOrder { get; set; }
        /// <summary>
        /// マイリストの説明文
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// マイリストをフォローしている人の数
        /// </summary>
        public int FollowerCount { get; set; }
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
        /// マイリストをフォローしているかどうか
        /// </summary>
        public bool IsFollowing { get; set; }
        /// <summary>
        /// マイリストが公開されているかどうか
        /// </summary>
        public bool IsPublic { get; set; }
        /// <summary>
        /// マイリストに含まれている動画の総数
        /// </summary>
        public int TotalItemCount { get; set; }
        /// <summary>
        /// あとで見るのリスト
        /// TotalCountと同じ数とは限らない
        /// </summary>
        public IEnumerable<MylistEntry>? Entries { get; set; } 
    }
}
