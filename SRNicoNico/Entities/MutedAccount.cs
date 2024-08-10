using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities;

/// <summary>
/// ミュート設定テーブル
/// </summary>
public record MutedAccount {

    /// <summary>
    /// 主キー サロゲートキー
    /// </summary>
    [Key]
    public int Key { get; set; }

    /// <summary>
    /// アカウントタイプ
    /// </summary>
    public AccountType AccountType { get; set; }

    /// <summary>
    /// アカウントID
    /// </summary>
    public required string AccountId { get; set; }

}

public enum AccountType {
    [Display(Name = "ユーザー")]
    User,
    [Display(Name = "チャンネル")]
    Channel,
    [Display(Name = "コミュニティ")]
    Community
}
