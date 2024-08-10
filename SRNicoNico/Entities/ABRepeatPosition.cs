using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities;

/// <summary>
/// ABリピートのリピート位置を動画単位で持つテーブル
/// </summary>
public record ABRepeatPosition {

    /// <summary>
    /// 動画ID 主キー
    /// </summary>
    [Key]
    public required string VideoId { get; set; }

    /// <summary>
    /// ABリピートのA地点
    /// </summary>
    public double RepeatA { get; set; }

    /// <summary>
    /// ABリピートのB地点
    /// </summary>
    public double RepeatB { get; set; }
}
