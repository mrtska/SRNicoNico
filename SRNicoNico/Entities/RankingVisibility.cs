using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities;

/// <summary>
/// ランキング表示設定テーブル
/// </summary>
public record RankingVisibility {
    /// <summary>
    /// 動画ID 主キー
    /// </summary>
    [Key]
    public required string GenreKey { get; set; }

    /// <summary>
    /// 表示するかどうか
    /// </summary>
    public bool IsVisible { get; set; }
}
