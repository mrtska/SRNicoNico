using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 動画コメント
    /// </summary>
    public class VideoCommentThread {
        /// <summary>
        /// スレッドID
        /// </summary>
        public string? Id { get; set; }

        public ThreadResultCode ResultCode { get; set; }

        public int Revision { get; set; }
        public int? ClickRevision { get; set; }

        public long ServerTime { get; set; }

        public string? Ticket { get; set; }

        public int Fork { get; set; }

        public int LastRes { get; set; }

        /// <summary>
        /// 葉のリスト
        /// </summary>
        public List<VideoCommentLeaf>? Leaves { get; set; }

        /// <summary>
        /// コメントリスト
        /// </summary>
        public List<VideoCommentEntry>? Entries { get; set; }
    }

    public class VideoCommentLeaf {

        /// <summary>
        /// スレッドID
        /// </summary>
        public string? ThreadId { get; set; }

        public int Fork { get; set; }

        public int Leaf { get; set; }
        public int Count { get; set; }
    }

    public class VideoCommentEntry {

        /// <summary>
        /// スレッドID
        /// </summary>
        public string? ThreadId { get; set; }

        /// <summary>
        /// コメント番号
        /// </summary>
        public long Number { get; set; }

        /// <summary>
        /// コメント再生位置 センチ秒だったはず
        /// </summary>
        public int Vpos { get; set; }

        /// <summary>
        /// 葉
        /// </summary>
        public int Leaf { get; set; }

        public int Fork { get; set; }

        /// <summary>
        /// コメント投稿時間 Unixタイム
        /// </summary>
        public long Date { get; set; }

        /// <summary>
        /// コメント投稿時間の小数点以下マイクロ秒
        /// </summary>
        public int DateUsec { get; set; }

        /// <summary>
        /// NGスコア 健全な人は0, NG登録されるとどんどんマイナスになっていく
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// ニコられた数
        /// </summary>
        public int Nicoru { get; set; }

        /// <summary>
        /// 最後にニコられた時間
        /// </summary>
        public string? LastNicoruDate { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// プレミアム会員フラグ
        /// </summary>
        public bool Premium { get; set; }

        /// <summary>
        /// 匿名フラグ
        /// 匿名コメントならTrue
        /// </summary>
        public bool Anonymity { get; set; }

        /// <summary>
        /// ユーザーID
        /// 削除フラグがTrueの時はnull
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// コメントの装飾
        /// 削除フラグがTrueの時はnull
        /// </summary>
        public string? Mail { get; set; }

        /// <summary>
        /// コメントテキスト
        /// 削除フラグがTrueの時はnull
        /// </summary>
        public string? Content { get; set; }
    }

    public enum ThreadResultCode {
        ThreadFound = 0,
        ThreadNotFound = 1,
        ThreadInvalid = 2,
        ThreadVersion = 3,
        ThreadInvalidWaybackKey = 4,
        ThreadTooOldWaybackKey = 5,
        ThreadInvalidAdminKey = 6,
        ThreadTooOldAdminKey = 7,
        ThreadInvalidThreadKey = 8,
        ThreadTooOldThreadKey = 9,
        ThreadAdminConflict = 10,
        ThreadLeafNotActivated = 11,
        ThreadLeafLoading = 12,
        ThreadInvalidLanguage = 13,
        ThreadInvalidUserKey = 14,
        ThreadTooOldUserKey = 15,
        ThreadConflictUserKeyAndOtherKey = 16,
        ThreadUserConflict = 17
    }

}
