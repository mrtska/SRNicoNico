using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// シリーズ
    /// </summary>
    public class Series {
        /// <summary>
        /// シリーズID
        /// </summary>
        public string? SeriesId { get; set; }
        /// <summary>
        /// オーナータイプ
        /// </summary>
        public string? OwnerType { get; set; }
        /// <summary>
        /// オーナーID
        /// </summary>
        public string? OwnerId { get; set; }
        /// <summary>
        /// オーナー名
        /// </summary>
        public string? OwnerName { get; set; }
        /// <summary>
        /// シリーズ名
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// シリーズ説明文
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }
        /// <summary>
        /// リストに表示されるかどうか
        /// </summary>
        public bool IsListed { get; set; }
        /// <summary>
        /// シリーズ作成日時
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
        /// <summary>
        /// シリーズ更新日時
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// シリーズの中の動画数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// シリーズのリスト
        /// </summary>
        public IEnumerable<VideoListItem>? Items { get; set; }
    } 
}
