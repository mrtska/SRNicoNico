using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace SRNicoNico.ViewModels {
    public class VideoHtml5Handler : IDisposable {


        public WebView2? WebView { get; private set; }



        public VideoHtml5Handler() {

            WebView = new WebView2 { Source = new Uri(GetHtml5PlayerPath()) };
        }

        /// <summary>
        /// 指定した初期化する
        /// </summary>
        /// <param name="vm">JavaScript環境に露出するViewModel</param>
        public async Task InitializeAsync(VideoViewModel vm) {

            if (WebView == null) {
                return;
            }
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.AddHostObjectToScript("vm", vm);
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
