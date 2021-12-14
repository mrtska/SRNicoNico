using System.Collections.Generic;
using System.Threading.Tasks;
using SRNicoNico.Entities;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// アカウントサービス
    /// </summary>
    public interface IAccountService {

        /// <summary>
        /// 指定した動画がミュート対象かを返す
        /// </summary>
        /// <param name="item">確認対象</param>
        /// <returns>ミュート対象ならTrue</returns>
        bool IsMuted(VideoItem item);

        /// <summary>
        /// 指定したユーザがミュート対象かを返す
        /// </summary>
        /// <param name="userId">確認対象</param>
        /// <returns>ミュート対象ならTrue</returns>
        bool IsMutedUser(string userId);

        /// <summary>
        /// 指定したチャンネルがミュート対象かを返す
        /// </summary>
        /// <param name="channelId">確認対象</param>
        /// <returns>ミュート対象ならTrue</returns>
        bool IsMutedChannel(string channelId);

        /// <summary>
        /// 指定したコミュニティがミュート対象かを返す
        /// </summary>
        /// <param name="communityId">確認対象</param>
        /// <returns>ミュート対象ならTrue</returns>
        bool IsMutedCommunity(string communityId);

        /// <summary>
        /// ミュートアカウントのキャッシュをフラッシュする
        /// </summary>
        void FlushCache();

        /// <summary>
        /// 全てのミュート設定を取得する
        /// </summary>
        /// <returns>ミュート設定</returns>
        Task<IEnumerable<MutedAccount>> GetMutedAccountsAsync();

        /// <summary>
        /// 指定した条件でミュート設定を追加する
        /// </summary>
        /// <param name="type">アカウントタイプ</param>
        /// <param name="accountId">ID</param>
        /// <returns>既に登録済みならfalseを返す</returns>
        Task<bool> AddMutedAccountAsync(AccountType type, string accountId);

        /// <summary>
        /// 指定した条件のミュート設定を削除する
        /// </summary>
        /// <param name="type">アカウントタイプ</param>
        /// <param name="accountId">ID</param>
        Task RemoveMutedAccountAsync(AccountType type, string accountId);
    }
}
