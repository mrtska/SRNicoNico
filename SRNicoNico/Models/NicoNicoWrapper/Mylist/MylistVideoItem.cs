using System;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// あとで見るやマイリストの動画情報
    /// </summary>
    public class MylistVideoItem : VideoItem {
        /// <summary>
        /// あとで見るやマイリストに追加された日時
        /// </summary>
        public DateTimeOffset AddedAt { get; set; }
        /// <summary>
        /// アイテムID
        /// </summary>
        public string ItemId { get; set; } = default!;
        /// <summary>
        /// メモ
        /// マイリストコメント的なもの
        /// </summary>
        public string Memo { get; set; } = default!;
        /// <summary>
        /// ステータス
        /// public, hidden, memberOnly それ以外もあるかも
        /// </summary>
        public string Status { get; set; } = default!;
        /// <summary>
        /// 動画のIDかな？
        /// </summary>
        public string WatchId { get; set; } = default!;

        public override VideoItem Fill(dynamic item) {

            AddedAt = DateTimeOffset.Parse(item.addedAt);
            ItemId = item.itemId.ToString();
            Memo = item.memo() ? item.memo : item.description; // あとで見るはmemoだけどマイリストはdescription
            Status = item.status;
            WatchId = item.watchId;
            
            return base.Fill((object)item.video);
        }
    }
}
