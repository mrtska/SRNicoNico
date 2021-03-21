using System.Threading.Tasks;
using FastEnumUtility;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコレポを操作する処理を提供するサービス
    /// </summary>
    public interface INicoRepoService {

        /// <summary>
        /// 指定した条件のニコレポを取得する
        /// </summary>
        /// <param name="type">ニコレポの種類</param>
        /// <param name="filter">フィルタ</param>
        /// <param name="untilId">さらに読み込む時のID</param>
        /// <returns>ニコレポのリスト</returns>
        Task<NicoRepoList> GetNicoRepoAsync(NicoRepoType type, NicoRepoFilter filter, string? untilId = null);


    }

    /// <summary>
    /// ニコレポフィルタ
    /// </summary>
    public enum NicoRepoFilter {
        /// <summary>
        /// すべて
        /// </summary>
        All,
        /// <summary>
        /// 動画投稿
        /// </summary>
        [Label("video")]
        VideoUpload,
        /// <summary>
        /// 生放送開始
        /// </summary>
        [Label("program")]
        ProgramOnAir,
        /// <summary>
        /// イラスト投稿
        /// </summary>
        [Label("image")]
        ImageAdd,
        /// <summary>
        /// 漫画投稿
        /// </summary>
        [Label("comicStory")]
        ComicStoryAdd,
        /// <summary>
        /// 記事投稿
        /// </summary>
        [Label("article")]
        ArticleAdd,
        /// <summary>
        /// ゲーム投稿
        /// </summary>
        [Label("game")]
        GameAdd
    }

    /// <summary>
    /// ニコレポ表示対象
    /// </summary>
    public enum NicoRepoType {
        /// <summary>
        /// すべて
        /// </summary>
        All,
        /// <summary>
        /// 自分
        /// </summary>
        [Label("self")]
        Me,
        /// <summary>
        /// ユーザー
        /// </summary>
        [Label("followingUser")]
        User,
        /// <summary>
        ///  チャンネル
        /// </summary>
        [Label("followingChannel")]
        Channel,
        /// <summary>
        /// コミュニティ
        /// </summary>
        [Label("followingCommunity")]
        Community,
        /// <summary>
        /// マイリスト
        /// </summary>
        [Label("followingMylist")]
        Mylist
    }
}
