using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities {
    /// <summary>
    /// 検索履歴テーブル
    /// </summary>
    public class SearchHistory {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// 検索文字列
        /// </summary>
        [Key]
        public string Query { get; set; }

        /// <summary>
        /// 順番
        /// </summary>
        public int Order { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
