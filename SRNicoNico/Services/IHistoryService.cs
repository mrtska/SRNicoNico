using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SRNicoNico.Entities;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// 視聴履歴を操作する処理を提供するサービス
    /// </summary>
    public interface IHistoryService {

        /// <summary>
        /// アカウントの視聴履歴を取得できる分だけ返す
        /// </summary>
        /// <returns>アカウントの視聴履歴 取得に失敗した場合はnull</returns>
        /// <exception cref="Models.StatusErrorException">取得に失敗した場合</exception>
        IAsyncEnumerable<HistoryVideoItem> GetAccountHistoryAsync();

        /// <summary>
        /// ローカルの視聴履歴を取得できる分だけ返す
        /// </summary>
        /// <returns>ローカルの視聴履歴</returns>
        IAsyncEnumerable<LocalHistory> GetLocalHistoryAsync();

        /// <summary>
        /// アカウントの視聴履歴を削除する
        /// </summary>
        /// <param name="videoId">削除したい動画ID</param>
        /// <returns>削除に成功したらTrue</returns>
        Task<bool> DeleteAccountHistoryAsync(string videoId);

        /// <summary>
        /// ローカルの視聴履歴を削除する
        /// 視聴時間でソートされる
        /// </summary>
        /// <param name="videoId">削除したい動画ID</param>
        /// <returns>削除に成功したらTrue</returns>
        Task<bool> DeleteLocalHistoryAsync(string videoId);

        /// <summary>
        /// 渡した視聴履歴をローカル視聴履歴に同期させる
        /// </summary>
        /// <param name="histories">アカウントの視聴履歴</param>
        /// <returns>成功したらTrue</returns>
        Task<bool> SyncLocalHistoryAsync(IEnumerable<HistoryVideoItem> histories);

        /// <summary>
        /// 指定した動画が視聴済みかどうかを返す
        /// </summary>
        /// <param name="videoId">確認したい動画のID</param>
        /// <returns>視聴済みならTrue</returns>
        Task<bool> HasWatchedAsync(string videoId);
    }
}
