using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DynaJson;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace SRNicoNico.ViewModels {
    public class VideoHtml5Handler : IDisposable {

        /// <summary>
        /// WebView
        /// ライフタイムはこのクラスと同じ
        /// </summary>
        public WebView2? WebView { get; private set; }

        private bool Initialized = false;

        public VideoHtml5Handler() {

            WebView = new WebView2 { Source = new Uri(GetHtml5PlayerPath()) };
        }

        /// <summary>
        /// 指定したViewModelでWebViewを初期化する
        /// </summary>
        /// <param name="vm">JavaScript環境に露出するViewModel</param>
        public async Task InitializeAsync(VideoViewModel vm, string contentUri) {

            if (WebView == null || Initialized) {
                return;
            }
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.AddHostObjectToScript("vm", vm);

            WebView.CoreWebView2.WebMessageReceived += (o, e) => {

                var json = JsonObject.Parse(e.WebMessageAsJson);
                var type = json.type as string;

                switch (type) {
                    case "initialized": // ブラウザ側の初期化が終わったので動画URLをブラウザに送信する
                        WebView?.CoreWebView2.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setContent", value = contentUri }));
                        break;
                        
                }


            };

            Initialized = true;
        }

        /// <summary>
        /// 動画プレイヤーのパスを返す
        /// </summary>
        /// <returns>ファイルパス</returns>
        private string GetHtml5PlayerPath() {

            return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!, "Html/player.html");
        }

        public void Dispose() {

            WebView?.Dispose();
            WebView = null;
        }

    }
}
