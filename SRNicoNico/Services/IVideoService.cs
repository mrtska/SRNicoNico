using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// 動画関連の処理を提供するサービス
    /// </summary>
    public interface IVideoService {

        /// <summary>
        /// 動画を視聴するために必要な情報を取得する
        /// </summary>
        /// <param name="videoId">動画ID sm9など</param>
        /// <param name="withoutHistory">視聴履歴に追加したくない場合はtrueにする</param>
        /// <returns>動画情報</returns>
        Task<WatchApiData> WatchAsync(string videoId, bool withoutHistory = false);

    }
}
