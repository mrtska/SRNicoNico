using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using FastEnumUtility;
using SRNicoNico.Models.NicoNicoWrapper.WatchLater;

namespace SRNicoNico.Services {
    /// <summary>
    /// あとで見るとマイリストを操作する処理を提供するサービス
    /// </summary>
    public interface IMylistService {

        Task<WatchLaterList> GetWatchLaterAsync(MylistSortKey sortKey, int page);


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
