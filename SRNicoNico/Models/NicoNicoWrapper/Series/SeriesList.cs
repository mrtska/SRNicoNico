using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// シリーズリスト
    /// </summary>
    public class SeriesList {
        /// <summary>
        /// シリーズのリスト
        /// </summary>
        public IEnumerable<SeriesListItem>? Items { get; set; }
        /// <summary>
        /// シリーズの総数
        /// </summary>
        public int TotalCount { get; set; }
    } 

    public class SeriesListItem {
        /// <summary>
        /// シリーズID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// シリーズ名
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// シリーズ説明文
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 含まれる動画の数
        /// </summary>
        public int ItemsCount { get; set; }
        /// <summary>
        /// サムネイルURL
        /// </summary>
        public string? ThumbnailUrl { get; set; }
        /// <summary>
        /// リストに表示されるかどうか
        /// </summary>
        public bool IsListed { get; set; }
        /// <summary>
        /// オーナータイプ
        /// </summary>
        public string? OwnerType { get; set; }
        /// <summary>
        /// オーナーID
        /// </summary>
        public string? OwnerId { get; set; }
    }
}
