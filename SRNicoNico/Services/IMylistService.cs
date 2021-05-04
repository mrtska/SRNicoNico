using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastEnumUtility;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// あとで見るとマイリストを操作する処理を提供するサービス
    /// </summary>
    public interface IMylistService {
        /// <summary>
        /// あとで見るを取得する
        /// </summary>
        /// <param name="sortKey">並び順</param>
        /// <param name="page">ページ番号</param>
        /// <returns>あとで見るのリスト</returns>
        Task<WatchLaterList> GetWatchLaterAsync(MylistSortKey sortKey, int page);

        /// <summary>
        /// 指定した動画IDの動画をあとで見るに登録する
        /// </summary>
        /// <param name="watchId">動画ID</param>
        /// <param name="memo">マイリストメモ</param>
        /// <returns>成功したらTrue</returns>
        Task<bool> AddWatchLaterAsync(string watchId, string? memo);

        /// <summary>
        /// 指定したアイテムIDのあとで見るに登録された動画を削除する
        /// </summary>
        /// <param name="itemIds">削除したい動画のアイテムID</param>
        /// <returns>成功したらTrue</returns>
        Task<bool> DeleteWatchLaterAsync(params string[] itemIds);

        /// <summary>
        /// マイリストの一覧を取得する
        /// </summary>
        /// <param name="sampleItemCount">含めたい動画数 デフォルトは0</param>
        /// <returns>マイリストの一覧</returns>
        IAsyncEnumerable<MylistListEntry> GetMylistsAsync(int sampleItemCount = 0);

        /// <summary>
        /// 指定したマイリストIDのマイリストを取得する
        /// </summary>
        /// <param name="mylistId">マイリストID</param>
        /// <param name="sortKey">並び順</param>
        /// <param name="page">ページ</param>
        /// <returns>マイリストの詳細</returns>
        Task<Mylist> GetMylistAsync(string mylistId, MylistSortKey sortKey, int page);

        /// <summary>
        /// 指定したマイリストに動画を追加する
        /// </summary>
        /// <param name="mylistId">動画を追加したいマイリストのID</param>
        /// <param name="watchId">動画ID</param>
        /// <param name="memo">メモ 任意</param>
        /// <returns>成功したらTrue</returns>
        Task<bool> AddMylistItemAsync(string mylistId, string watchId, string? memo);

        /// <summary>
        /// 指定したマイリストから動画を削除する
        /// 複数動画を指定可
        /// </summary>
        /// <param name="mylistId">削除対象があるマイリストのID</param>
        /// <param name="itemIds">削除対象の動画のアイテムID</param>
        /// <returns>成功したらTrue</returns>
        Task<bool> DeleteMylistItemAsync(string mylistId, params string[] itemIds);

    }
    /// <summary>
    /// マイリストやあとで見るのソート順のキー
    /// </summary>
    public enum MylistSortKey {
        /// <summary>
        /// 追加が新しい順
        /// </summary>
        [Display(Name = "追加が新しい順")]
        [Label("sortKey=addedAt&sortOrder=desc")]
        AddedAtDesc,
        /// <summary>
        /// 追加が古い順
        /// </summary>
        [Display(Name = "追加が古い順")]
        [Label("sortKey=addedAt&sortOrder=asc")]
        AddedAtAsc,
        /// <summary>
        /// タイトル昇順
        /// </summary>
        [Display(Name = "タイトル昇順")]
        [Label("sortKey=title&sortOrder=asc")]
        TitleAsc,
        /// <summary>
        /// タイトル降順
        /// </summary>
        [Display(Name = "タイトル降順")]
        [Label("sortKey=title&sortOrder=desc")]
        TitleDesc,
        /// <summary>
        /// メモ昇順
        /// </summary>
        [Display(Name = "メモ昇順")]
        [Label("sortKey=memo&sortOrder=asc")]
        MemoAsc,
        /// <summary>
        /// メモ降順
        /// </summary>
        [Display(Name = "メモ降順")]
        [Label("sortKey=memo&sortOrder=desc")]
        MemoDesc,
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
