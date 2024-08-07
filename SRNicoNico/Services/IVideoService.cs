using System.Collections.Generic;
using System.Threading.Tasks;
using SRNicoNico.Entities;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// 動画関連の処理を提供するサービス
    /// </summary>
    public interface IVideoService {

        /// <summary>
        /// 動画を視聴するために必要な情報を取得する
        /// </summary>
        /// <param name="videoId">動画ID sm9など</param>
        /// <param name="withoutHistory">視聴履歴に追加したくない場合はtrueにする</param>
        /// <returns>動画情報</returns>
        Task<WatchApiData> WatchAsync(string videoId, bool withoutHistory = false);

        /// <summary>
        /// 動画再生中に再度動画情報を取得する
        /// </summary>
        /// <param name="videoId">動画ID</param>
        /// <returns>動画情報</returns>
        Task<WatchApiData> WatchContinueAsync(string videoId);

        /// <summary>
        /// トラッキングIDを送信する
        /// </summary>
        /// <param name="trackingId">トラッキングID</param>
        /// <returns>Trueなら成功</returns>
        Task<bool> SendTrackingAsync(string trackingId);

        /// <summary>
        /// 指定したメディア情報からDMCのセッションを作成する
        /// 作成したセッションは一定時間ハートビートを送らないと自動的に破棄される
        /// 明示的に破棄する必要は無い
        /// </summary>
        /// <param name="movie">パラメータ</param>
        /// <param name="encryption">暗号化情報</param>
        /// <param name="videoId">動画の画質ID 指定しない場合は一番良い画質のものが選ばれる</param>
        /// <param name="audioId">動画の音質ID 指定しない場合は一番良い音質のものが選ばれる</param>
        /// <returns>DMCセッション</returns>
        Task<DmcSession> CreateSessionAsync(MediaMovie movie, MediaEncryption? encryption = null, string? videoId = null, string? audioId = null);

        /// <summary>
        /// 指定したセッションを延命する
        /// </summary>
        /// <param name="dmcSession">セッション</param>
        /// <returns>延命されたセッション</returns>
        Task<DmcSession> HeartbeatAsync(DmcSession dmcSession);

        /// <summary>
        /// 指定したセッションを破棄する
        /// </summary>
        /// <param name="dmcSession">セッション</param>
        Task DeleteSessionAsync(DmcSession dmcSession);

        /// <summary>
        /// コメントデータを取得する
        /// </summary>
        /// <param name="comment">コメント情報</param>
        /// <returns>コメント取得結果</returns>
        Task<IEnumerable<VideoCommentThread>> GetCommentAsync(WatchApiDataComment comment);

        /// <summary>
        /// かんたんコメントを投稿する
        /// </summary>
        /// <param name="videoId">動画ID</param>
        /// <param name="phrase">コメントフレーズ</param>
        /// <param name="threadId">スレッドID</param>
        /// <param name="vpos">コメント位置</param>
        /// <returns>コメント番号 失敗時はnull</returns>
        Task<int?> PostEasyCommentAsync(string videoId, EasyCommentPhrase phrase, string threadId, int vpos);

        /// <summary>
        /// コメントを投稿する
        /// </summary>
        /// <param name="comment">コメント本文</param>
        /// <param name="threadId">スレッドID</param>
        /// <param name="ticket">チケット</param>
        /// <param name="vpos">コメント位置</param>
        /// <returns></returns>
        Task PostCommentAsync(string comment, string threadId, string ticket, int vpos);

        /// <summary>
        /// コメントを投稿するためのポストキーを取得する
        /// </summary>
        /// <param name="threadId">対象のスレッドID</param>
        /// <param name="blockNo">ブロック番号</param>
        /// <returns>ポストキー</returns>
        Task<string> GetPostKeyAsync(string threadId, int blockNo);

        /// <summary>
        /// コメントを取得するためのユーザーキーを取得する
        /// </summary>
        /// <returns>ユーザーキー</returns>
        Task<string> GetUserKeyAsync();

        /// <summary>
        /// スレッドキーを取得する
        /// </summary>
        /// <param name="threadId">対象のスレッドID</param>
        /// <returns>スレッドキー</returns>
        Task<string?> GetThreadKeyAsync(string threadId);

        /// <summary>
        /// ストーリーボードを取得する
        /// </summary>
        /// <param name="storyBoard">パラメータ</param>
        /// <returns>ストーリーボード情報</returns>
        Task<VideoStoryBoard> GetStoryBoardAsync(MediaStoryBoard storyBoard);

        /// <summary>
        /// 視聴した再生位置を保存する
        /// アカウントの視聴履歴に該当の動画がないと失敗する
        /// </summary>
        /// <param name="watchId">動画ID</param>
        /// <param name="position">視聴位置 秒</param>
        /// <returns>成功したらTrue</returns>
        Task<bool> SavePlaybackPositionAsync(string watchId, double position);

        /// <summary>
        /// 指定した動画をいいね！する
        /// </summary>
        /// <param name="videoId">動画ID</param>
        /// <returns>投稿者メッセージ 失敗した場合はnull</returns>
        Task<string?> LikeAsync(string videoId);

        /// <summary>
        /// 指定した動画のいいね！を解除する
        /// </summary>
        /// <param name="videoId">動画ID</param>
        /// <returns>成功したらTrue</returns>
        Task<bool> UnlikeAsync(string videoId);

        /// <summary>
        /// 指定した動画IDのABリピートポジションをローカルデータベースからあれば取得する
        /// 無ければnull
        /// </summary>
        /// <param name="videoId">動画ID</param>
        /// <returns>リピートポジション</returns>
        Task<ABRepeatPosition?> GetABRepeatPositionAsync(string videoId);

        /// <summary>
        /// 指定した動画IDのABリピートポジションをローカルデータベースに保存する
        /// </summary>
        /// <param name="videoId">動画ID</param>
        /// <param name="repeatA">A地点</param>
        /// <param name="repeatB">B地点</param>
        Task SaveABRepeatPositionAsync(string videoId, double repeatA, double repeatB);
    }
}
