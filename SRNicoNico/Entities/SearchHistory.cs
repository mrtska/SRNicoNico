using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities;

/// <summary>
/// 検索履歴テーブル
/// </summary>
public record SearchHistory {

    /// <summary>
    /// 検索文字列
    /// </summary>
    [Key]
    public required string Query { get; set; }

    /// <summary>
    /// 順番
    /// </summary>
    public int Order { get; set; }
}
