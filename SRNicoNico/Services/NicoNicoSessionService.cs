using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
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
        private const string UserAgent = "SRNicoNico/2.0 (@m__gl user/23425727)";

        /// <summary>
        /// ニコニコのトップページURL
        /// </summary>
        private const string NicoNicoTop = "https://www.nicovideo.jp/";

        private readonly HttpClient HttpClient;
        private readonly HttpClientHandler HttpClientHandler;

        /// <summary>
        /// サインインダイアログを表示する関数
        /// </summary>
        public Func<Task> SignInDialogHandler { get; set; }

        public NicoNicoSessionService() {

            // Cookieを使用するように
            // リダイレクトはしないように
            // 圧縮されているペイロードはHttpClientHandler側で自動で解凍するように
            HttpClientHandler = new HttpClientHandler {
                UseCookies = true,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All
            };

            HttpClient = new HttpClient(HttpClientHandler, true);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            HttpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// 既にサインインしているか確認する
        /// このメソッドを呼ぶ前にSignInDialogHandlerに値を設定すること
        /// </summary>
        /// <returns>サインインしている場合はTrue</returns>
        public async ValueTask<bool> VerifyAsync() {

            var session = Settings.Instance.UserSession;

            if (string.IsNullOrEmpty(session)) {

                await SignInDialogHandler();
            }




            return true;
        }



        public async Task<HttpResponseMessage> SendAsync() {

            throw new NotImplementedException();
        }






    }
}
