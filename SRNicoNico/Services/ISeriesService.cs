using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// シリーズ関連の処理を提供するサービス
    /// </summary>
    public interface ISeriesService {

        /// <summary>
        /// 指定したユーザーIDのシリーズを取得する
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="page">ページ</param>
        /// <param name="pageSize">ページ数</param>
        /// <returns>シリーズのリスト</returns>
        Task<SeriesList> GetUserSeriesAsync(string userId, int page = 1, int pageSize = 100);


    }
}
