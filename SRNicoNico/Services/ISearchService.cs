using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastEnumUtility;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// 検索処理を提供するサービス
    /// </summary>
    public interface ISearchService {
        /// <summary>
        /// 検索API
        /// </summary>
        /// <param name="sortKey">ソート設定</param>
        /// <param name="type">検索タイプ</param>
        /// <param name="value">検索ワード</param>
        /// <param name="page">ページ</param>
        /// <param name="genre">ジャンルフィルター</param>
        /// <param name="pageSize">1ページ当たりの項目数</param>
        /// <returns></returns>
        Task<SearchResult> SearchAsync(SearchSortKey sortKey, SearchType type, string value, int page = 1, string? genre = null, int pageSize = 30);
        /// <summary>
        /// 指定した検索値のジャンル情報を取得する
        /// </summary>
        /// <param name="searchType">キーワードかタグか</param>
        /// <param name="value">値</param>
        /// <returns>ジャンル情報</returns>
        Task<IEnumerable<SearchGenreFacet>> GetGenreFacetAsync(SearchType searchType, string value);

        /// <summary>
        /// お気に入り登録したタグを返す
        /// </summary>
        /// <returns>お気に入り登録したタグのリスト</returns>
        Task<IEnumerable<string>> GetFavoriteTagsAsync();

        /// <summary>
        /// 検索ワードからタグをサジェストする
        /// </summary>
        /// <param name="tag">タグ</param>
        /// <returns>サジェストされたタグ</returns>
        Task<IEnumerable<string>> GetTagSuggestionAsync(string tag);

        /// <summary>
        /// 保存されている検索履歴を取得する
        /// </summary>
        /// <returns>検索履歴</returns>
        Task<IEnumerable<string>> GetSearchHistoriesAsync();

        /// <summary>
        /// 検索履歴を保存する
        /// </summary>
        /// <param name="searchHistories">検索履歴</param>
        Task SaveSearchHistoriesAsync(IEnumerable<string> searchHistories);
    }

    public enum SearchType {
        /// <summary>
        /// キーワード
        /// </summary>
        Keyword,
        /// <summary>
        /// タグ
        /// </summary>
        Tag
    }

    public enum SearchSortKey {
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
        /// いいね！数が多い順
        /// </summary>
        [Display(Name = "いいね！数が多い順")]
        [Label("sortKey=likeCount&sortOrder=desc")]
        LikeCountDesc,
        /// <summary>
        /// いいね！数が少ない順
        /// </summary>
        [Display(Name = "いいね！数が少ない順")]
        [Label("sortKey=likeCount&sortOrder=asc")]
        LikeCountAsc,
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
