using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastEnumUtility;
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
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報詳細</returns>
        Task<UserDetails> GetUserAsync(string userId);

        /// <summary>
        /// 指定したユーザーがフォローしているユーザーのリストを返す
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="cursor">位置</param>
        /// <param name="pageSIze">1ページのアイテムの数</param>
        /// <returns>ユーザーリスト</returns>
        Task<UserFollowList> GetUserFollowingAsync(string userId, string? cursor = null, int pageSIze = 100);

        /// <summary>
        /// 指定したユーザーをフォローしているユーザーのリストを返す
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="cursor">位置</param>
        /// <param name="pageSIze">1ページのアイテムの数</param>
        /// <returns>ユーザーリスト</returns>
        Task<UserFollowList> GetUserFollowerAsync(string userId, string? cursor = null, int pageSIze = 100);

        /// <summary>
        /// 指定したユーザーの投稿動画を取得する
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="sortKey">ソートキー</param>
        /// <param name="page">ページ</param>
        /// <param name="pageSize">1ページのアイテムの数</param>
        /// <returns>投稿動画のリスト</returns>
        Task<VideoList> GetUserVideosAsync(string userId, VideoSortKey sortKey, int page = 1, int pageSize = 100);

        Task TestAsync();
    }


    /// <summary>
    /// 投稿動画のソート順のキー
    /// </summary>
    public enum VideoSortKey {
        /// <summary>
        /// 投稿日時が新しい順
        /// </summary>
        [Display(Name = "投稿日時が新しい順")]
        [Label("sortKey=registeredAt&sortOrder=desc")]
        RegisteredAtDesc,
        /// <summary>
        /// 投稿日時が古い順
        /// </summary>
        [Display(Name = "投稿日時が古い順")]
        [Label("sortKey=registeredAt&sortOrder=asc")]
        RegisteredAtAsc,
        /// <summary>
        /// 再生数が多い順
        /// </summary>
        [Display(Name = "再生数が多い順")]
        [Label("sortKey=viewCount&sortOrder=desc")]
        ViewCountDesc,
        /// <summary>
        /// 再生数が少ない順
        /// </summary>
        [Display(Name = "再生数が少ない順")]
        [Label("sortKey=viewCount&sortOrder=asc")]
        ViewCountAsc,
        /// <summary>
        /// コメントが新しい順
        /// </summary>
        [Display(Name = "コメントが新しい順")]
        [Label("sortKey=lastCommentTime&sortOrder=desc")]
        LastCommentTimeDesc,
        /// <summary>
        /// コメントが古い順
        /// </summary>
        [Display(Name = "コメントが古い順")]
        [Label("sortKey=lastCommentTime&sortOrder=asc")]
        LastCommentTimeAsc,
        /// <summary>
        /// コメント数が多い順
        /// </summary>
        [Display(Name = "コメント数が多い順")]
        [Label("sortKey=commentCount&sortOrder=desc")]
        CommentCountDesc,
        /// <summary>
        /// コメント数が少ない順
        /// </summary>
        [Display(Name = "コメント数が少ない順")]
        [Label("sortKey=commentCount&sortOrder=asc")]
        CommentCountAsc,
        /// <summary>
        /// マイリスト数が多い順
        /// </summary>
        [Display(Name = "マイリスト数が多い順")]
        [Label("sortKey=mylistCount&sortOrder=desc")]
        MylistCountDesc,
        /// <summary>
        /// マイリスト数が少ない順
        /// </summary>
        [Display(Name = "マイリスト数が少ない順")]
        [Label("sortKey=mylistCount&sortOrder=asc")]
        MylistCountAsc,
        /// <summary>
        /// 再生時間が多い順
        /// </summary>
        [Display(Name = "再生時間が多い順")]
        [Label("sortKey=duration&sortOrder=desc")]
        DurationDesc,
        /// <summary>
        /// 再生時間が短い順
        /// </summary>
        [Display(Name = "再生時間が短い順")]
        [Label("sortKey=duration&sortOrder=asc")]
        DurationAsc
    }
}
