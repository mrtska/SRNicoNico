using System.ComponentModel.DataAnnotations;

namespace SRNicoNico.Entities {
    /// <summary>
    /// ABリピートのリピート位置を動画単位で持つテーブル
    /// </summary>
    public class ABRepeatPosition {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// 動画ID 主キー
        /// </summary>
        [Key]
        public string VideoId { get; set; }

        /// <summary>
        /// ABリピートのA地点
        /// </summary>
        public double RepeatA { get; set; }

        /// <summary>
        /// ABリピートのB地点
        /// </summary>
        public double RepeatB { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
