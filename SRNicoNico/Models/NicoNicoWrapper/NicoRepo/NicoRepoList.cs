using System;
using System.Collections.Generic;
using System.Text;

namespace SRNicoNico.Models.NicoNicoWrapper {
    /// <summary>
    /// ニコレポ情報
    /// </summary>
    public class NicoRepoList {
        /// <summary>
        /// さらに読み込むニコレポがあるかどうか
        /// </summary>
        public bool HasNext { get; set; }
        
        public string? MaxId { get; set; }
        
        public string? MinId { get; set; }

        /// <summary>
        /// ニコレポリスト
        /// </summary>
        public IEnumerable<NicoRepoEntry>? Entries { get; set; }
    }
}
