using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRNicoNico.Services {
    /// <summary>
    /// セッションやHTTPリクエストの処理を提供するサービス
    /// </summary>
    public interface ISessionService {

        /// <summary>
        /// サインインダイアログを表示する関数
        /// </summary>
        Func<Task> SignInDialogHandler { get; set; }

        /// <summary>
        /// 既にサインインしているか確認する
        /// このメソッドを呼ぶ前にSignInDialogHandlerに値を設定すること
        /// </summary>
        /// <returns>サインインしている場合はTrue</returns>
        ValueTask<bool> VerifyAsync();

        /// <summary>
        /// セッションを保存する
        /// </summary>
        /// <param name="userSession">セッションCookieの値</param>
        void StoreSession(string value);

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
        /// POSTリクエストを指定したHTTPヘッダと一緒に送信する
        /// json版
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <param name="json">POSTするJson</param>
        /// <param name="additionalHeaders">HTTPヘッダのリスト</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> PostAsync(string url, string json, IDictionary<string, string>? additionalHeaders);

        /// <summary>
        /// POSTリクエストを送信する
        /// x-www-form-urlencoded版
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <param name="formData">POSTするフォームデータ</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> formData);

        /// <summary>
        /// POSTリクエストを指定したHTTPヘッダと一緒に送信する
        /// x-www-form-urlencoded版
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <param name="formData">POSTするフォームデータ</param>
        /// <param name="additionalHeaders">HTTPヘッダのリスト</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> formData, IDictionary<string, string>? additionalHeaders);

        /// <summary>
        /// PUTリクエストを送信する
        /// x-www-form-urlencoded版
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <param name="formData">フォームデータ</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> formData);

        /// <summary>
        /// PUTリクエストを指定したHTTPヘッダと一緒に送信する
        /// x-www-form-urlencoded版
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <param name="formData">フォームデータ</param>
        /// <param name="additionalHeaders">HTTPヘッダのリスト</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> formData, IDictionary<string, string>? additionalHeaders);

        /// <summary>
        /// DELETEリクエストを送信する
        /// </summary>
        /// <param name="url">リクエストを送信するURL</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> DeleteAsync(string url);

        /// <summary>
        /// DELETEリクエストを指定したHTTPヘッダと一緒に送信する
        /// </summary>
        /// <param name="url">リクエストを送信するURL GETクエリも必要であれば付ける</param>
        /// <param name="additionalHeaders">HTTPヘッダのリスト</param>
        /// <returns>HTTPレスポンス</returns>
        Task<HttpResponseMessage> DeleteAsync(string url, IDictionary<string, string>? additionalHeaders);
    }
}
