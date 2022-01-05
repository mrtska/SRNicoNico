using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// 視聴履歴の動画情報
    /// </summary>
    public class HistoryVideoItem : VideoItem {
        /// <summary>
        /// 最終視聴日時
        /// </summary>
        public DateTimeOffset WatchedAt { get; set; }

        /// <summary>
        /// 視聴回数
        /// </summary>
        public int WatchCount { get; set; }

        /// <summary>
        /// 動画のIDかな？
        /// </summary>
        public string WatchId { get; set; } = default!;

        public HistoryVideoItem() {
        }

        public HistoryVideoItem(dynamic item) {
            Fill(item);
        }

        public override VideoItem Fill(dynamic item) {

            WatchedAt = DateTimeOffset.Parse(item.lastViewedAt);
            WatchCount = (int) item.views;
            WatchId = item.watchId;
            
            return base.Fill((object)item.video);
        }
    }
}
