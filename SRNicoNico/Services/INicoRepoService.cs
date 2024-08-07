using System.ComponentModel.DataAnnotations;
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

        /// <summary>
        /// 指定したユーザーと条件のニコレポを取得する
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="filter">フィルタ</param>
        /// <param name="untilId">さらに読み込む時のID</param>
        /// <returns>ニコレポのリスト</returns>
        Task<NicoRepoList> GetUserNicoRepoAsync(string userId, NicoRepoFilter filter, string? untilId = null);
    }

    /// <summary>
    /// ニコレポフィルタ
    /// </summary>
    public enum NicoRepoFilter {
        /// <summary>
        /// すべて
        /// </summary>
        [Display(Name = "すべて")]
        All,
        /// <summary>
        /// 動画関連
        /// </summary>
        [Display(Name = "動画関連のみ")]
        [Label("object%5Btype%5D=video")]
        Video,
        /// <summary>
        /// 動画投稿
        /// </summary>
        [Display(Name = "動画投稿のみ")]
        [Label("object%5Btype%5D=video&type=upload")]
        VideoUpload,
        /// <summary>
        /// 生放送開始
        /// </summary>
        [Display(Name = "生放送関連のみ")]
        [Label("object%5Btype%5D=program")]
        Program,
        /// <summary>
        /// 生放送開始
        /// </summary>
        [Display(Name = "生放送開始のみ")]
        [Label("object%5Btype%5D=program&type=onair")]
        ProgramOnAir,
        /// <summary>
        /// イラスト関連
        /// </summary>
        [Display(Name = "イラスト関連のみ")]
        [Label("object%5Btype%5D=image")]
        ImageAdd,
        /// <summary>
        /// 漫画関連
        /// </summary>
        [Display(Name = "漫画関連のみ")]
        [Label("object%5Btype%5D=comicStory")]
        ComicStoryAdd,
        /// <summary>
        /// 記事関連
        /// </summary>
        [Display(Name = "記事関連のみ")]
        [Label("object%5Btype%5D=article")]
        ArticleAdd,
        /// <summary>
        /// ゲーム関連
        /// </summary>
        [Display(Name = "ゲーム関連のみ")]
        [Label("object%5Btype%5D=game")]
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
        Self,
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
