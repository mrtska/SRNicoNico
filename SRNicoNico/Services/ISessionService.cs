using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRNicoNico.Services {
    /// <summary>
    /// セッションやHTTPリクエストの処理を提供するサービス
    /// </summary>
    public interface ISessionService {

        /// <summary>
        /// GETリクエストを送信する
        /// </summary>
        /// <param name="url">リクエストを送信するURL GETクエリも必要であれば付ける</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> GetAsync(string url);

        /// <summary>
        /// GETリクエストを指定したHTTPヘッダと一緒に送信する
        /// </summary>
        /// <param name="url">リクエストを送信するURL GETクエリも必要であれば付ける</param>
        /// <param name="additionalHeaders">HTTPヘッダのリスト</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string>? additionalHeaders);

        /// <summary>
        /// POSTリクエストを送信する
        /// json版
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <param name="json">POSTするJson</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> PostAsync(string url, string json);

        /// <summary>
        /// POSTリクエストを送信する
        /// x-www-form-urlencoded版
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <param name="formData">POSTするフォームデータ</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> formData);

        /// <summary>
        /// DELETEリクエストを送信する
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> DeleteAsync(string url);
    }
}
