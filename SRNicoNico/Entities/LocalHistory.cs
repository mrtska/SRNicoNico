using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities;

/// <summary>
/// ローカル視聴履歴テーブル
/// </summary>
public record LocalHistory {

    /// <summary>
    /// 動画ID 主キー
    /// </summary>
    [Key]
    public required string VideoId { get; set; }

    /// <summary>
    /// 動画タイトル
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// 動画説明文
    /// </summary>
    public required string ShortDescription { get; set; }

    /// <summary>
    /// 動画のサムネイルURL
    /// </summary>
    public required string ThumbnailUrl { get; set; }

    /// <summary>
    /// 動画の長さ 秒単位
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 動画視聴回数
    /// </summary>
    public int WatchCount { get; set; }

    /// <summary>
    /// 動画投稿日時
    /// </summary>
    public DateTimeOffset PostedAt { get; set; }

    /// <summary>
    /// 最終視聴日 INDEX
    /// </summary>
    public DateTimeOffset LastWatchedAt { get; set; }
}
