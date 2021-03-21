using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ニコレポ情報
    /// </summary>
    public class NicoRepoEntry {
        /// <summary>
        /// ニコレポのID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// ニコレポのタイトル
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// ニコレポが作られた日時
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
        /// <summary>
        /// ニコレポが作成された人、チャンネルなどのURL
        /// </summary>
        public string? ActorUrl { get; set; }
        /// <summary>
        /// ニコレポが作成された人、チャンネルなどの名前
        /// </summary>
        public string? ActorName { get; set; }
        /// <summary>
        /// ニコレポが作成された人、チャンネルなどのサムネイルURL
        /// </summary>
        public string? ActorIconUrl { get; set; }
        /// <summary>
        /// ニコレポの内容の種類
        /// videoやprogramなど
        /// </summary>
        public string? ObjectType { get; set; }
        /// <summary>
        /// ニコレポの内容のURL
        /// </summary>
        public string? ObjectUrl { get; set; }
        /// <summary>
        /// ニコレポの内容の名前
        /// 動画名など
        /// </summary>
        public string? ObjectName { get; set; }
        /// <summary>
        /// ニコレポの内容のサムネイルURL
        /// 動画や生放送のサムネイルなど
        /// </summary>
        public string? ObjectImageUrl { get; set; }
        /// <summary>
        /// ミュート
        /// </summary>
        public NicoRepoMuteContext? MuteContext { get; set; }
    }
    /// <summary>
    /// ニコレポのミュートに使用する何か
    /// </summary>
    public class NicoRepoMuteContext {
        /// <summary>
        /// nicorepo固定かな？
        /// </summary>
        public string? Task { get; set; }
        /// <summary>
        /// Idのタイプ userやchannelなど
        /// </summary>
        public string? IdType { get; set; }
        /// <summary>
        /// Id IdTypeがuserの場合はuserのID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// IdTypeと同じ？
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// ニコレポのトリガー名
        /// </summary>
        public string? Trigger { get; set; }
    }

}
