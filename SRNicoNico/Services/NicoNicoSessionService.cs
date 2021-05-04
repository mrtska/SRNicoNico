using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SRNicoNico.Models;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコニコにHTTPアクセスを提供するサービス
    /// </summary>
    public sealed class NicoNicoSessionService : ISessionService {

        /// <summary>
        /// NicoNicoViewerのUserAgent
        /// フォークして改造する人は書き換えてください
        /// </summary>
        private const string UserAgent = "SRNicoNico/{0:F1} (@m__gl user/23425727)";

        /// <summary>
        /// ニコニコのトップページURL
        /// </summary>
        private const string NicoNicoTop = "https://www.nicovideo.jp/";

        /// <summary>
        /// 一部のAPIで必要になるHTTPヘッダ
        /// </summary>
        public static readonly Dictionary<string, string> ApiHeaders = new Dictionary<string, string> {
            ["X-Frontend-Id"] = "6",
            ["X-Frontend-Version"] = "0",
            ["X-Niconico-Language"] = "ja-jp"
        };
        /// <summary>
        /// 一部のAPIで必要になるHTTPヘッダ ajax版
        /// </summary>
        public static readonly Dictionary<string, string> AjaxApiHeaders = new Dictionary<string, string> {
            ["X-Frontend-Id"] = "6",
            ["X-Frontend-Version"] = "0",
            ["X-Niconico-Language"] = "ja-jp",
            ["X-Request-With"] = "https://www.nicovideo.jp"
        };

        private readonly ISettings Settings;

        // メインで使うHTTPクライアント
        private readonly HttpClient HttpClient;
        private readonly HttpClientHandler HttpClientHandler;

        /// <summary>
        /// サインインダイアログを表示する関数
        /// </summary>
        public Func<Task> SignInDialogHandler { get; set; }

        public NicoNicoSessionService(INicoNicoViewer nicoNicoViewer, ISettings settings) {

            Settings = settings;

            // Cookieを使用するように
            // リダイレクトはしないように
            // 圧縮されているペイロードはHttpClientHandler側で自動で解凍するように
            HttpClientHandler = new HttpClientHandler {
                UseCookies = true,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All
            };

            HttpClient = new HttpClient(HttpClientHandler, true);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", string.Format(UserAgent, nicoNicoViewer.CurrentVersion));
            HttpClient.Timeout = TimeSpan.FromSeconds(30);

            SignInDialogHandler = () => { return Task.CompletedTask; };
        }

        /// <summary>
        /// ニコニコのTOPページにHEADリクエストを送信してCookieが生きているか確認する
        /// </summary>
        /// <returns>trueならログイン成功</returns>
        private async ValueTask<bool> VerifyAuthAsync() {

            var verifyRequest = new HttpRequestMessage(HttpMethod.Head, NicoNicoTop);
            var result = await HttpClient.SendAsync(verifyRequest).ConfigureAwait(false);

            var flags = result.Headers.GetValues("x-niconico-authflag");

            foreach (var flag in flags) {

                // 値が0ではなかったらログイン成功判定
                if (flag != "0") {

                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public async ValueTask<bool> VerifyAsync() {

            var session = Settings.UserSession;

            if (string.IsNullOrEmpty(session)) {

                // サインインダイアログを表示させる
                await SignInDialogHandler().ConfigureAwait(false);
            }

            // Cookieを保存する
            HttpClientHandler.CookieContainer.Add(new Cookie("user_session", session, "/", ".nicovideo.jp"));

            var result = await VerifyAuthAsync().ConfigureAwait(false);
            if (result) {

                return true;
            }

            // トークンが使えなかったのでサインインダイアログを表示させる
            await SignInDialogHandler().ConfigureAwait(false);

            // もう一度確認する
            return await VerifyAuthAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void StoreSession(string userSession) {

            Settings.UserSession = userSession;
            HttpClientHandler.CookieContainer.Add(new Cookie("user_session", userSession, "/", ".nicovideo.jp"));
        }

        /// <summary>
        /// 指定したHTTPリクエストにHTTPヘッダを追加する
        /// </summary>
        /// <param name="request">HTTPリクエスト</param>
        /// <param name="headers">追加したいHTTPヘッダのリスト nullの場合は何もしない</param>
        /// <returns>HTTPリクエスト</returns>
        private HttpRequestMessage WithHttpHeaders(HttpRequestMessage request, IDictionary<string, string>? headers) {

            if (headers != null) {
                foreach (var (key, value) in headers) {

                    request.Headers.Add(key, value);
                }
            }
            return request;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> GetAsync(string url) {

            return GetAsync(url, null);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string>? addtionalHeaders) {

            var result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Get, url), addtionalHeaders)).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.Unauthorized) {
                // サインインダイアログを表示して再ログインさせる
                await SignInDialogHandler().ConfigureAwait(false);
                result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Get, url), addtionalHeaders)).ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> PostAsync(string url, string json) {

            return PostAsync(url, json, null);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> PostAsync(string url, string json, IDictionary<string, string>? additionalHeaders) {

            var result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }, additionalHeaders)).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.Unauthorized) {
                // サインインダイアログを表示して再ログインさせる
                await SignInDialogHandler().ConfigureAwait(false);
                result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Post, url) {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                }, additionalHeaders)).ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string>? formData) {

            return PostAsync(url, formData, null);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string>? formData, IDictionary<string, string>? additionalHeaders) {

            var result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Post, url) {
                Content = formData != null ? new FormUrlEncodedContent(formData) : null
            }, additionalHeaders)).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.Unauthorized) {
                // サインインダイアログを表示して再ログインさせる
                await SignInDialogHandler().ConfigureAwait(false);
                result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Post, url) {
                    Content = formData != null ? new FormUrlEncodedContent(formData) : null
                }, additionalHeaders)).ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> formData) {

            return PutAsync(url, formData, null);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> PutAsync(string url, IDictionary<string, string> formData, IDictionary<string, string>? additionalHeaders) {

            var result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Put, url) {
                Content = new FormUrlEncodedContent(formData)
            }, additionalHeaders)).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.Unauthorized) {
                // サインインダイアログを表示して再ログインさせる
                await SignInDialogHandler().ConfigureAwait(false);
                result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Put, url) {
                    Content = new FormUrlEncodedContent(formData)
                }, additionalHeaders)).ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> DeleteAsync(string url) {

            return DeleteAsync(url, null);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> DeleteAsync(string url, IDictionary<string, string>? addtionalHeaders) {

            var result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Delete, url), addtionalHeaders)).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.Unauthorized) {
                // サインインダイアログを表示して再ログインさせる
                await SignInDialogHandler().ConfigureAwait(false);
                result = await HttpClient.SendAsync(WithHttpHeaders(new HttpRequestMessage(HttpMethod.Delete, url), addtionalHeaders)).ConfigureAwait(false);
            }
            return result;
        }
    }
}
