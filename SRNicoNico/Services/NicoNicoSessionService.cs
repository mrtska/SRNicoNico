using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Services {
    /// <summary>
    /// ニコニコにHTTPアクセスを提供するサービス
    /// </summary>
    public sealed class NicoNicoSessionService {

        /// <summary>
        /// NicoNicoViewerのUserAgent
        /// フォークして改造する人は書き換えてください
        /// </summary>
        private const string UserAgent = "SRNicoNico/{0:F1} (@m__gl user/23425727)";

        /// <summary>
        /// ニコニコのトップページURL
        /// </summary>
        private const string NicoNicoTop = "https://www.nicovideo.jp/";

        private readonly HttpClient HttpClient;
        private readonly HttpClientHandler HttpClientHandler;

        /// <summary>
        /// サインインダイアログを表示する関数
        /// </summary>
        public Func<Task>? SignInDialogHandler { get; set; }

        public NicoNicoSessionService(INicoNicoViewer nicoNicoViewer) {

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
        }

        /// <summary>
        /// 既にサインインしているか確認する
        /// このメソッドを呼ぶ前にSignInDialogHandlerに値を設定すること
        /// </summary>
        /// <returns>サインインしている場合はTrue</returns>
        public async ValueTask<bool> VerifyAsync() {

            if (SignInDialogHandler == null) {

                // SignInDialogHandlerを設定しなされ
                throw new InvalidOperationException();
            }

            var session = Settings.Instance.UserSession;

            if (string.IsNullOrEmpty(session)) {

                // サインインダイアログを表示させる
                await SignInDialogHandler();
                // サインイン後は値が更新されているので再代入する
                session = Settings.Instance.UserSession;
            }

            HttpClientHandler.CookieContainer.Add(new Cookie("user_session", session, "/", ".nicovideo.jp"));

            var verifyRequest = new HttpRequestMessage(HttpMethod.Head, NicoNicoTop);
            var result = await HttpClient.SendAsync(verifyRequest);

            var flags = result.Headers.GetValues("x-niconico-authflag");

            foreach (var flag in flags) {

                // 値が0ではなかったらログイン成功判定
                if (flag != "0") {

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// セッションを保存する
        /// </summary>
        /// <param name="userSession">セッションCookieの値</param>
        public void StoreSession(string userSession) {

            Settings.Instance.UserSession = userSession;
        }


        public async Task<HttpResponseMessage> SendAsync() {

            throw new NotImplementedException();
        }






    }
}
