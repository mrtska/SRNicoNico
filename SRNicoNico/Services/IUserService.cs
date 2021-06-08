using System.Collections.Generic;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ユーザーを操作する処理を提供するサービス
    /// </summary>
    public interface IUserService {

        /// <summary>
        /// 自分がフォローしているユーザーを返す
        /// </summary>
        /// <param name="page">ページ数</param>
        /// <param name="pageSize">1ページのアイテムの数</param>
        /// <returns>ユーザーリスト 総数が多い場合はpageを使って取得する</returns>
        /// <exception cref="Models.StatusErrorException">取得に失敗した場合</exception>
        Task<UserList> GetFollowedUsersAsync(int page = 1, int pageSize = 100);
        
        /// <summary>
        /// 自分がフォローしているタグを返す
        /// </summary>
        /// <returns>タグのリスト</returns>
        /// <exception cref="Models.StatusErrorException">取得に失敗した場合</exception>
        IAsyncEnumerable<TagEntry> GetFollowedTagsAsync();

        /// <summary>
        /// 自分がフォローしているマイリストを返す
        /// </summary>
        /// <returns>マイリストのリスト</returns>
        /// <exception cref="Models.StatusErrorException">取得に失敗した場合</exception>
        IAsyncEnumerable<UserMylistEntry> GetFollowedMylistsAsync();

        /// <summary>
        /// 自分がフォローしているチャンネルを返す
        /// </summary>
        /// <returns>チャンネルのリスト</returns>
        /// <exception cref="Models.StatusErrorException">取得に失敗した場合</exception>
        IAsyncEnumerable<ChannelEntry> GetFollowedChannelsAsync();

        /// <summary>
        /// 自分がフォローしているコミュニティを返す
        /// </summary>
        /// <param name="page">ページ数</param>
        /// <param name="pageSize">1ページのアイテムの数</param>
        /// <returns>コミュニティリスト 総数が多い場合はpageを使って取得する</returns>
        /// <exception cref="Models.StatusErrorException">取得に失敗した場合</exception>
        Task<CommunityList> GetFollowedCommunitiesAsync(int page = 1, int pageSize = 100);

        /// <summary>
        /// 指定したユーザーIDのユーザー情報を取得する
        /// </summary>
        /// <param name="userId">ユーザーいD</param>
        /// <returns>ユーザー情報詳細</returns>
        Task<UserDetails> GetUserAsync(string userId);
    }
}
