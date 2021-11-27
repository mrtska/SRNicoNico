using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities {
    /// <summary>
    /// ランキング表示設定テーブル
    /// </summary>
    public class RankingVisibility {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// 動画ID 主キー
        /// </summary>
        [Key]
        public string GenreKey { get; set; }

        /// <summary>
        /// 表示するかどうか
        /// </summary>
        public bool IsVisible { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
