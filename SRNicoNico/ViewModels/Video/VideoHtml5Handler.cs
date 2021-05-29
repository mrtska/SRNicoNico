using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using DynaJson;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using SRNicoNico.Views.Controls;

namespace SRNicoNico.ViewModels {
    public class VideoHtml5Handler : IDisposable {

        /// <summary>
        /// WebView
        /// ライフタイムはこのクラスと同じ
        /// </summary>
        public WebView2? WebView { get; private set; }

        private bool Initialized = false;

        public VideoHtml5Handler() {

            WebView = new WebView2 { DefaultBackgroundColor = Color.Black };
        }

        /// <summary>
        /// 指定したViewModelでWebViewを初期化する
        /// </summary>
        /// <param name="vm">JavaScript環境に露出するViewModel</param>
        /// <param name="contentUri">動画URL</param>
        public async Task InitializeAsync(VideoViewModel vm, string contentUri) {

            if (WebView == null || Initialized) {
                return;
            }
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("srniconico", GetHtml5PlayerPath(), CoreWebView2HostResourceAccessKind.Allow);
            WebView.Source = new Uri("http://srniconico/player.html");
            WebView.CoreWebView2.AddHostObjectToScript("vm", vm);

            WebView.CoreWebView2.WebMessageReceived += (o, e) => {

                var json = JsonObject.Parse(e.WebMessageAsJson);
                var type = json.type as string;

                switch (type) {
                    case "initialized": // ブラウザ側の初期化が終わったので動画URLをブラウザに送信する
                        WebView?.CoreWebView2.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setContent", value = new { contentUri, volume = vm.Volume, autoplay = true } }));
                        break;
                    case "clicked":
                        vm.TogglePlay();
                        break;
                    case "info":
                        vm.ActualVideoWidth = (int)json.value.width;
                        vm.ActualVideoHeight = (int)json.value.height;
                        vm.ActualVideoDuration = json.value.duration;
                        break;
                    case "loop":
                        // 再生済みの位置やバッファ済みの位置をWebViewから取得する
                        vm.PlayedRange.Clear();
                        foreach (var played in json.value.played) {

                            if (played == null) {
                                continue;
                            }
                            vm.PlayedRange.Add(new TimeRange((float)played.start, (float)played.end));
                        }
                        vm.BufferedRange.Clear();
                        foreach (var buffered in json.value.buffered) {

                            if (buffered == null) {
                                continue;
                            }
                            vm.BufferedRange.Add(new TimeRange((float)buffered.start, (float)buffered.end));
                        }
                        break;
                }
            };

            Initialized = true;
        }

        public void Seek(int position) {

            WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "seek", value = position }));
        }

        public void SetVolume(float volume) {

            WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setVolume", value = volume }));
        }


        /// <summary>
        /// 動画プレイヤーのパスを返す
        /// </summary>
        /// <returns>ファイルパス</returns>
        private string GetHtml5PlayerPath() {
#if DEBUG
            return Environment.GetEnvironmentVariable("SRNICONICO_HTML_PATH") ?? Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!, "Html");
#else
            return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!, "Html");
#endif
        }

        public void Dispose() {

            WebView?.Dispose();
            WebView = null;
        }

    }
}
