using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities {
    /// <summary>
    /// ミュート設定テーブル
    /// </summary>
    public class MutedAccount {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
        public string AccountId { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public enum AccountType {

        User,

        Channel,

        Community
    }
}
