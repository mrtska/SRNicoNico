using System.Threading.Tasks;
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
        /// 指定したメディア情報からDMCのセッションを作成する
        /// 作成したセッションは一定時間ハートビートを送らないと自動的に破棄される
        /// 明示的に破棄する必要は無い
        /// </summary>
        /// <param name="movieSession">パラメータ</param>
        /// <param name="videoId">動画の画質ID 指定しない場合は一番良い画質のものが選ばれる</param>
        /// <param name="audioId">動画の音質ID 指定しない場合は一番良い音質のものが選ばれる</param>
        /// <returns>DMCセッション</returns>
        Task<DmcSession> CreateSessionAsync(MediaSession movieSession, string? videoId = null, string? audioId = null);

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
    }
}
